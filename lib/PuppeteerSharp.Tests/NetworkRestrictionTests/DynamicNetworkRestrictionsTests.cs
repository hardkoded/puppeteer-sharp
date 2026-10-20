using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkRestrictionTests;

public class DynamicNetworkRestrictionsTests : PuppeteerPageBaseTest
{
    private const string FetchScript = @"async (url) => {
        try {
            await fetch(url);
            return true;
        } catch {
            return false;
        }
    }";

    [TearDown]
    public async Task ClearRestrictionsAsync()
    {
        if (Browser is not null)
        {
            await Browser.RestrictNetworkAsync(null);
        }
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions dynamic restrictions", "can set blocklist dynamically")]
    public async Task CanSetBlocklistDynamically()
    {
        var blockedUrl = TestConstants.ServerUrl + "/empty.html";

        await Page.GoToAsync(TestConstants.ServerUrl + "/title.html");
        Assert.That(await Page.EvaluateFunctionAsync<bool>(FetchScript, blockedUrl), Is.True);

        await Browser.RestrictNetworkAsync(new NetworkRestrictions { BlockList = ["*://*:*/empty.html"] });
        Assert.That(await Page.EvaluateFunctionAsync<bool>(FetchScript, blockedUrl), Is.False);

        await Browser.RestrictNetworkAsync();
        Assert.That(await Page.EvaluateFunctionAsync<bool>(FetchScript, blockedUrl), Is.True);
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions dynamic restrictions", "can set allowlist dynamically")]
    public async Task CanSetAllowlistDynamically()
    {
        var blockedUrl = TestConstants.ServerUrl + "/empty.html";

        await Page.GoToAsync(TestConstants.ServerUrl + "/title.html");
        Assert.That(await Page.EvaluateFunctionAsync<bool>(FetchScript, blockedUrl), Is.True);

        await Browser.RestrictNetworkAsync(new NetworkRestrictions { Allowlist = ["*://*:*/title.html"] });
        Assert.That(await Page.EvaluateFunctionAsync<bool>(FetchScript, blockedUrl), Is.False);

        await Browser.RestrictNetworkAsync();
        Assert.That(await Page.EvaluateFunctionAsync<bool>(FetchScript, blockedUrl), Is.True);
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions dynamic restrictions", "can switch directly from blocklist to allowlist")]
    public async Task CanSwitchDirectlyFromBlocklistToAllowlist()
    {
        await Browser.RestrictNetworkAsync(new NetworkRestrictions { BlockList = ["*://*:*/empty.html"] });

        Assert.ThrowsAsync<NavigationException>(() => Page.GoToAsync(TestConstants.EmptyPage));

        await Browser.RestrictNetworkAsync(new NetworkRestrictions { Allowlist = ["*://*:*/empty.html"] });

        await Page.GoToAsync(TestConstants.EmptyPage);
        Assert.That(Page.Url, Is.EqualTo(TestConstants.EmptyPage));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions dynamic restrictions", "blocks page.emulateNetworkConditions after restrictNetwork is called")]
    public async Task ShouldBlockPageEmulateNetworkConditionsAfterRestrictNetworkIsCalled()
    {
        await Browser.RestrictNetworkAsync(new NetworkRestrictions { BlockList = ["*://*:*/empty.html"] });

        var exception = Assert.ThrowsAsync<PuppeteerException>(() => Page.EmulateNetworkConditionsAsync(
            new NetworkConditions { Offline = false, Latency = 0, Download = 0, Upload = 0 }));
        Assert.That(exception.Message, Does.Contain("Cannot reset network conditions: rule-based emulation is enabled."));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions dynamic restrictions", "does not mutate state when restrictNetwork fails with an invalid pattern")]
    public async Task ShouldNotMutateStateWhenRestrictNetworkFailsWithAnInvalidPattern()
    {
        var invalid = Assert.ThrowsAsync<PuppeteerException>(() =>
            Browser.RestrictNetworkAsync(new NetworkRestrictions { BlockList = ["(invalid pattern"] }));
        Assert.That(invalid.Message, Does.Contain("URLPattern"));

        await Page.EmulateNetworkConditionsAsync(
            new NetworkConditions { Offline = false, Latency = 0, Download = 0, Upload = 0 });

        await Browser.RestrictNetworkAsync(new NetworkRestrictions { BlockList = ["*://*:*/empty.html"] });

        invalid = Assert.ThrowsAsync<PuppeteerException>(() =>
            Browser.RestrictNetworkAsync(new NetworkRestrictions { Allowlist = ["(invalid pattern"] }));
        Assert.That(invalid.Message, Does.Contain("URLPattern"));

        var blocked = Assert.ThrowsAsync<NavigationException>(() => Page.GoToAsync(TestConstants.EmptyPage));
        Assert.That(blocked.Message, Does.Contain("is blocked by blocklist/allowlist rules"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions dynamic restrictions", "allows empty arrays alongside a populated or empty list")]
    public async Task ShouldAllowEmptyArraysAlongsideAPopulatedOrEmptyList()
    {
        await Browser.RestrictNetworkAsync(new NetworkRestrictions
        {
            BlockList = [],
            Allowlist = ["*://*:*/title.html"],
        });

        var blocked = Assert.ThrowsAsync<NavigationException>(() => Page.GoToAsync(TestConstants.EmptyPage));
        Assert.That(blocked.Message, Does.Contain("is blocked by blocklist/allowlist rules"));
        await Page.GoToAsync(TestConstants.ServerUrl + "/title.html");
        Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/title.html"));

        await Browser.RestrictNetworkAsync(new NetworkRestrictions { BlockList = [], Allowlist = [] });

        await Page.GoToAsync(TestConstants.EmptyPage);
        Assert.That(Page.Url, Is.EqualTo(TestConstants.EmptyPage));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions dynamic restrictions", "applies restrictions to a worker target")]
    public async Task ShouldApplyRestrictionsToAWorkerTarget()
    {
        await Browser.RestrictNetworkAsync(new NetworkRestrictions { BlockList = ["*://*:*/empty.html"] });

        await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/fetch/sw.html");

        var target = await Browser.WaitForTargetAsync(
            t => t.Type == TargetType.ServiceWorker,
            new WaitForOptions { Timeout = 3000 });
        var worker = await target.WorkerAsync();

        var fetchError = await worker.EvaluateFunctionAsync<string>(
            @"async (url) => {
                try {
                    await fetch(url);
                    return null;
                } catch (e) {
                    return e.message;
                }
            }",
            TestConstants.EmptyPage);

        Assert.That(fetchError, Is.Not.Null.And.Contain("Failed to fetch"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions dynamic restrictions", "applies and clears restrictions on an already-running worker")]
    public async Task ShouldApplyAndClearRestrictionsOnAnAlreadyRunningWorker()
    {
        var workerTcs = new TaskCompletionSource<WebWorker>();
        Page.WorkerCreated += (_, e) => workerTcs.TrySetResult(e.Worker);
        await Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html");
        var worker = await workerTcs.Task;

        await AssertWorkerFetchAsync(worker, true);

        await Browser.RestrictNetworkAsync(new NetworkRestrictions { BlockList = ["*://*:*/empty.html"] });
        await AssertWorkerFetchAsync(worker, false);

        await Browser.RestrictNetworkAsync(null);
        await AssertWorkerFetchAsync(worker, true);
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions dynamic restrictions", "applies and clears restrictions on an already-running service worker")]
    public async Task ShouldApplyAndClearRestrictionsOnAnAlreadyRunningServiceWorker()
    {
        await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/fetch/sw.html");

        var target = await Browser.WaitForTargetAsync(
            t => t.Type == TargetType.ServiceWorker,
            new WaitForOptions { Timeout = 3000 });
        var worker = await target.WorkerAsync();

        await AssertWorkerFetchAsync(worker, true);

        await Browser.RestrictNetworkAsync(new NetworkRestrictions { BlockList = ["*://*:*/empty.html"] });
        await AssertWorkerFetchAsync(worker, false);

        await Browser.RestrictNetworkAsync(null);
        await AssertWorkerFetchAsync(worker, true);
    }

    private static async Task AssertWorkerFetchAsync(WebWorker worker, bool expected)
        => Assert.That(await worker.EvaluateFunctionAsync<bool>(FetchScript, TestConstants.EmptyPage), Is.EqualTo(expected));
}
