using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BFCacheTests;

public class BFCacheTests : PuppeteerPageBaseTest
{
    public BFCacheTests()
    {
        DefaultOptions = TestConstants.DefaultBrowserOptions();
        DefaultOptions.AcceptInsecureCerts = true;
    }

    [Test, PuppeteerTest("bfcache.spec", "BFCache", "can navigate to a BFCached page")]
    public async Task CanNavigateToABFCachedPage()
    {
        Page.DefaultTimeout = 30_000;
        await Page.GoToAsync(TestConstants.ServerUrl + "/cached/bfcache/index.html");
        await Task.WhenAll(Page.WaitForNavigationAsync(), Page.ClickAsync("a"));

        Assert.That(Page.Url, Does.Contain("target"));
        await Task.WhenAll(Page.WaitForNavigationAsync(), Page.GoBackAsync());
        Assert.That(await Page.EvaluateExpressionAsync<string>("document.body.innerText"), Is.EqualTo("BFCachednext"));
    }

    [Test, PuppeteerTest("bfcache.spec", "BFCache", "can navigate to a BFCached page containing an OOPIF and a worker")]
    public async Task CanNavigateToABFCachedPageContainingAnOOPIFAndAWorker()
    {
        Page.DefaultTimeout = 30_000;
        var workerTcs = new TaskCompletionSource<WebWorker>();
        Page.WorkerCreated += (_, e) => workerTcs.TrySetResult(e.Worker);
        await Page.GoToAsync(TestConstants.ServerUrl + "/cached/bfcache/worker-iframe-container.html");
        var worker1 = await workerTcs.Task.WithTimeout();
        Assert.That(await worker1.EvaluateExpressionAsync<int>("1 + 1"), Is.EqualTo(2));
        await Task.WhenAll(
            Page.WaitForNavigationAsync(),
            Page.ClickAsync("a"));

        workerTcs = new TaskCompletionSource<WebWorker>();
        await Task.WhenAll(
            Page.WaitForNavigationAsync(),
            Page.GoBackAsync()).WithTimeout();

        var worker2 = await workerTcs.Task.WithTimeout();
        Assert.That(await worker2.EvaluateExpressionAsync<int>("1 + 1"), Is.EqualTo(2));
    }
}
