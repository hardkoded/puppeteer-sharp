using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkRestrictionTests;

public class NetworkRestrictionsTests : PuppeteerBaseTest
{
    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should block page.goto when the destination is in the blocklist")]
    public async Task ShouldBlockPageGotoWhenDestinationIsInBlocklist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/empty.html"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        var allowedUrl = TestConstants.ServerUrl + "/title.html";
        var blockedUrl = TestConstants.ServerUrl + "/empty.html";

        await page.GoToAsync(allowedUrl);

        Exception error = null;
        await page.GoToAsync(blockedUrl).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                error = t.Exception?.InnerException ?? t.Exception;
            }

            return t;
        });

        Assert.That(error, Is.Not.Null);
        Assert.That(error.Message, Does.Contain("net::ERR_INTERNET_DISCONNECTED"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should block window.location.href navigation to URLs in the blocklist")]
    public async Task ShouldBlockWindowLocationHrefNavigationToUrlsInBlocklist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/empty.html"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        var allowedUrl = TestConstants.ServerUrl + "/title.html";
        var blockedUrl = TestConstants.ServerUrl + "/empty.html";

        await page.GoToAsync(allowedUrl);

        var navTask = page.WaitForNavigationAsync(new NavigationOptions { Timeout = 2000 }).ContinueWith(t => t.IsFaulted ? null : t.Result);
        await page.EvaluateFunctionAsync("url => { window.location.href = url; }", blockedUrl);
        await navTask;

        var finalUrl = page.Url;
        Assert.That(finalUrl, Is.Not.EqualTo(blockedUrl));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should fail fetch requests to URLs in the blocklist")]
    public async Task ShouldFailFetchRequestsToUrlsInBlocklist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/empty.html"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        var allowedUrl = TestConstants.ServerUrl + "/title.html";
        var blockedUrl = TestConstants.ServerUrl + "/empty.html";

        await page.GoToAsync(allowedUrl);

        var fetchError = await page.EvaluateFunctionAsync<string>(
            @"async (url) => {
                try {
                    await fetch(url);
                    return null;
                } catch (e) {
                    return e.message;
                }
            }",
            blockedUrl);

        Assert.That(fetchError, Is.Not.Null.And.Not.Empty);
        Assert.That(fetchError, Does.Contain("Failed to fetch"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should prevent loading of blocklisted subresources (e.g., images)")]
    public async Task ShouldPreventLoadingOfBlocklistedSubresources()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/pptr.png"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        var allowedUrl = TestConstants.ServerUrl + "/one-style.css";
        var blockedUrl = TestConstants.ServerUrl + "/pptr.png";

        var failedRequests = new Dictionary<string, string>();
        var finishedRequests = new HashSet<string>();

        page.RequestFailed += (_, e) =>
        {
            failedRequests[e.Request.Url] = e.Request.FailureText;
        };
        page.RequestFinished += (_, e) =>
        {
            finishedRequests.Add(e.Request.Url);
        };

        await page.GoToAsync(TestConstants.EmptyPage);

        await page.SetContentAsync(
            $@"<img src=""{blockedUrl}"" />
               <link rel=""stylesheet"" href=""{allowedUrl}"" />",
            new NavigationOptions { WaitUntil = [WaitUntilNavigation.Networkidle0] });

        Assert.That(failedRequests.ContainsKey(blockedUrl), Is.True);
        Assert.That(failedRequests[blockedUrl], Does.Contain("net::ERR_INTERNET_DISCONNECTED"));
        Assert.That(finishedRequests.Contains(allowedUrl), Is.True);
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should detach from targets violating blocklist when connecting to a running browser")]
    public async Task ShouldDetachFromTargetsViolatingBlocklistWhenConnectingToRunningBrowser()
    {
        await using var originalBrowser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.LoggerFactory);
        var blockedUrl = TestConstants.ServerUrl + "/empty.html";

        var page = await originalBrowser.NewPageAsync();
        await page.GoToAsync(blockedUrl);

        var wsEndpoint = originalBrowser.WebSocketEndpoint;

        IBrowser connectedBrowser = null;
        try
        {
            connectedBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = wsEndpoint,
                BlockList = ["*://*:*/empty.html"],
            });

            var targets = connectedBrowser.Targets();
            var blockedTarget = Array.Find(targets, t => t.Url == blockedUrl);

            Assert.That(blockedTarget, Is.Null);
        }
        finally
        {
            connectedBrowser?.Disconnect();
            await page.CloseAsync();
        }
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should not block chrome://version/ even if it matches blocklist")]
    public async Task ShouldNotBlockChromeVersionEvenIfItMatchesBlocklist()
    {
        const string chromeUrl = "chrome://version/";
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = [chromeUrl];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        await page.GoToAsync(chromeUrl);

        // Navigation should succeed as chrome:// URLs usually bypass the network
        Assert.That(page.Url, Is.EqualTo(chromeUrl));
    }
}
