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
        Assert.That(error.Message, Does.Contain("is blocked by blocklist/allowlist rules"));
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

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions blocklist validation", "should fail fetch requests from within a service worker to URLs in the blocklist")]
    public async Task ShouldFailFetchRequestsFromWithinServiceWorkerToUrlsInBlocklist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList =
        [
            "*://*:*/serviceworkers/fetch/style.css",
        ];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var context = await browser.CreateBrowserContextAsync();
        var page = await context.NewPageAsync();

        var allowedUrl = TestConstants.ServerUrl + "/serviceworkers/fetch/sw.html";
        var blockedUrl = TestConstants.ServerUrl + "/serviceworkers/fetch/style.css";

        await page.GoToAsync(allowedUrl);

        var target = await context.WaitForTargetAsync(
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

        var idleTask = page.WaitForNetworkIdleAsync();
        await page.SetContentAsync(
            $@"<img src=""{blockedUrl}"" />
               <link rel=""stylesheet"" href=""{allowedUrl}"" />");
        await idleTask;

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

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should block chrome://version/ when it matches blocklist")]
    public async Task ShouldBlockChromeVersionWhenItMatchesBlocklist()
    {
        const string blockedUrl = "chrome://version/";
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = [blockedUrl];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

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
        Assert.That(error.Message, Does.Contain("is blocked by blocklist/allowlist rules"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should only allow navigation to URLs in the allowlist")]
    public async Task ShouldOnlyAllowNavigationToUrlsInAllowlist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.Allowlist = ["*://*:*/empty.html"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        var allowedUrl = TestConstants.ServerUrl + "/empty.html";
        var blockedUrl = TestConstants.ServerUrl + "/title.html";

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

        Assert.That(page.Url, Is.Not.EqualTo(blockedUrl));
        Assert.That(error, Is.Not.Null);
        Assert.That(error.Message, Does.Contain("is blocked by blocklist/allowlist rules"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should block window.location.href navigation to URLs not in the allowlist")]
    public async Task ShouldBlockWindowLocationHrefNavigationToUrlsNotInAllowlist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.Allowlist = ["*://*:*/empty.html"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        var allowedUrl = TestConstants.ServerUrl + "/empty.html";
        var blockedUrl = TestConstants.ServerUrl + "/title.html";

        await page.GoToAsync(allowedUrl);

        var navTask = page.WaitForNavigationAsync(new NavigationOptions { Timeout = 2000 }).ContinueWith(t => t.IsFaulted ? null : t.Result);
        await page.EvaluateFunctionAsync("url => { window.location.href = url; }", blockedUrl);
        await navTask;

        var finalUrl = page.Url;
        var content = await page.GetContentAsync();
        Assert.That(finalUrl, Is.Not.EqualTo(blockedUrl));
        Assert.That(content, Does.Not.Contain("Woof-Woof"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should fail fetch requests to URLs not in the allowlist")]
    public async Task ShouldFailFetchRequestsToUrlsNotInAllowlist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.Allowlist = ["*://*:*/empty.html"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        var allowedUrl = TestConstants.ServerUrl + "/empty.html";
        var blockedUrl = TestConstants.ServerUrl + "/title.html";

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

        Assert.That(fetchError, Does.Contain("Failed to fetch"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should prevent loading of subresources not in the allowlist (e.g., images)")]
    public async Task ShouldPreventLoadingOfSubresourcesNotInAllowlist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.Allowlist = ["*://*:*/empty.html", "*://*:*/one-style.css"];

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

        var idleTask = page.WaitForNetworkIdleAsync();
        await page.SetContentAsync(
            $@"<img src=""{blockedUrl}"" />
               <link rel=""stylesheet"" href=""{allowedUrl}"" />");
        await idleTask;

        Assert.That(failedRequests.ContainsKey(blockedUrl), Is.True);
        Assert.That(failedRequests[blockedUrl], Does.Contain("net::ERR_INTERNET_DISCONNECTED"));
        Assert.That(finishedRequests.Contains(allowedUrl), Is.True);
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should detach from targets violating allowlist when connecting to a running browser")]
    public async Task ShouldDetachFromTargetsViolatingAllowlistWhenConnectingToRunningBrowser()
    {
        await using var originalBrowser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.LoggerFactory);
        var blockedUrl = TestConstants.ServerUrl + "/title.html";

        var page = await originalBrowser.NewPageAsync();
        await page.GoToAsync(blockedUrl);

        var wsEndpoint = originalBrowser.WebSocketEndpoint;

        IBrowser connectedBrowser = null;
        try
        {
            connectedBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = wsEndpoint,
                Allowlist = ["*://*:*/empty.html"],
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

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should throw an error when both blocklist and allowlist are specified")]
    public async Task ShouldThrowAnErrorWhenBothBlocklistAndAllowlistAreSpecified()
    {
        var launchOptions = TestConstants.DefaultBrowserOptions();
        launchOptions.BlockList = ["*://*:*/empty.html"];
        launchOptions.Allowlist = ["*://*:*/empty.html"];

        Exception launchError = null;
        try
        {
            await using var browser = await Puppeteer.LaunchAsync(launchOptions, TestConstants.LoggerFactory);
        }
        catch (Exception ex)
        {
            launchError = ex;
        }

        Assert.That(launchError, Is.Not.Null);
        Assert.That(launchError.Message, Does.Contain("Cannot specify both blocklist and allowlist"));

        await using var originalBrowser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.LoggerFactory);
        var wsEndpoint = originalBrowser.WebSocketEndpoint;

        Exception connectError = null;
        try
        {
            await using var connectedBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = wsEndpoint,
                BlockList = ["*://*:*/empty.html"],
                Allowlist = ["*://*:*/empty.html"],
            });
        }
        catch (Exception ex)
        {
            connectError = ex;
        }

        Assert.That(connectError, Is.Not.Null);
        Assert.That(connectError.Message, Does.Contain("Cannot specify both blocklist and allowlist"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should throw an error for an invalid pattern")]
    public async Task ShouldThrowAnErrorForAnInvalidPattern()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["(invalid pattern"];

        Exception error = null;
        try
        {
            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        }
        catch (Exception ex)
        {
            error = ex;
        }

        Assert.That(error, Is.Not.Null);
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should block frame.goto when the destination is in the blocklist")]
    public async Task ShouldBlockFrameGotoWhenDestinationIsInBlocklist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/empty.html"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        await page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
        var frame = Array.Find(page.Frames, f => f != page.MainFrame);
        Assert.That(frame, Is.Not.Null);

        var blockedUrl = TestConstants.ServerUrl + "/empty.html";
        Exception error = null;
        await frame.GoToAsync(blockedUrl).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                error = t.Exception?.InnerException ?? t.Exception;
            }

            return t;
        });

        Assert.That(error, Is.Not.Null);
        Assert.That(error.Message, Does.Contain("is blocked by blocklist/allowlist rules"));
    }

    [Test, PuppeteerTest("BrowserConnector.test", "BrowserConnector _connectToBrowser", "should reject blocklist for WebDriver BiDi connections")]
    public void ShouldRejectBlocklistForWebDriverBiDiConnections()
    {
        var connectOptions = new ConnectOptions
        {
            BrowserWSEndpoint = "ws://localhost:1234",
            Protocol = ProtocolType.WebdriverBiDi,
            BlockList = ["https://example.com/*"],
        };

        var error = Assert.ThrowsAsync<PuppeteerException>(async () =>
            await Puppeteer.ConnectAsync(connectOptions));

        Assert.That(error.Message, Does.Contain("blocklist and allowlist are only supported with the CDP protocol"));
    }

    [Test, PuppeteerTest("BrowserConnector.test", "BrowserConnector _connectToBrowser", "should reject allowlist for WebDriver BiDi connections")]
    public void ShouldRejectAllowlistForWebDriverBiDiConnections()
    {
        var connectOptions = new ConnectOptions
        {
            BrowserWSEndpoint = "ws://localhost:1234",
            Protocol = ProtocolType.WebdriverBiDi,
            Allowlist = ["https://example.com/*"],
        };

        var error = Assert.ThrowsAsync<PuppeteerException>(async () =>
            await Puppeteer.ConnectAsync(connectOptions));

        Assert.That(error.Message, Does.Contain("blocklist and allowlist are only supported with the CDP protocol"));
    }

    [Test, PuppeteerTest("FirefoxLauncher.test", "FirefoxLauncher launch", "should reject blocklist for the default Firefox WebDriver BiDi protocol")]
    public void ShouldRejectBlocklistForDefaultFirefoxWebDriverBiDiProtocol()
    {
        var options = new LaunchOptions
        {
            Browser = SupportedBrowser.Firefox,
            BlockList = ["https://example.com/*"],
        };

        var error = Assert.ThrowsAsync<PuppeteerException>(async () =>
            await Puppeteer.LaunchAsync(options));

        Assert.That(error.Message, Does.Contain("blocklist and allowlist are only supported with the CDP protocol"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should block iframe content from loading if the iframe URL is in the blocklist")]
    public async Task ShouldBlockIframeContentFromLoadingIfTheIframeUrlIsInTheBlocklist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/frames/frame.html"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        await page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
        var frame = Array.Find(page.Frames, f => f != page.MainFrame);
        Assert.That(frame, Is.Not.Null);

        var content = await frame.GetContentAsync();
        Assert.That(content, Does.Not.Contain("Hi, I'm frame"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should block out-of-process iframe (OOPIF) content from loading if the iframe URL is in the blocklist")]
    public async Task ShouldBlockOopifContentFromLoadingIfTheIframeUrlIsInTheBlocklist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/frames/frame.html"];
        options.Args = ["--site-per-process"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        await page.GoToAsync(TestConstants.EmptyPage);
        var frame = await FrameUtils.AttachFrameAsync(page, "frame1", TestConstants.CrossProcessHttpPrefix + "/frames/frame.html");
        var content = await frame.GetContentAsync();
        Assert.That(content, Does.Not.Contain("Hi, I'm frame"));
        Assert.That(content, Does.Contain("ERR_INTERNET_DISCONNECTED"));
    }

    [Test, PuppeteerTest("network_restrictions.spec", "Network Restrictions", "should block standard emulation reset when blocklist/allowlist is active")]
    public async Task ShouldBlockStandardEmulationResetWhenBlocklistAllowlistIsActive()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/empty.html"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();

        var session = await page.CreateCDPSessionAsync();

        var sessionError = Assert.ThrowsAsync<PuppeteerException>(async () =>
            await session.SendAsync(
                "Network.emulateNetworkConditions",
                new
                {
                    offline = false,
                    latency = 0,
                    downloadThroughput = 0,
                    uploadThroughput = 0,
                }));

        Assert.That(sessionError.Message, Does.Contain("Cannot reset network conditions: rule-based emulation is enabled."));

        var pageError = Assert.ThrowsAsync<PuppeteerException>(async () =>
            await page.EmulateNetworkConditionsAsync(new NetworkConditions
            {
                Latency = 0,
                Download = 0,
                Upload = 0,
            }));

        Assert.That(pageError.Message, Does.Contain("Cannot reset network conditions: rule-based emulation is enabled."));
    }
}
