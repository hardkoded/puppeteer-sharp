using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkRestrictionTests;

public class WebSocketTests : PuppeteerBaseTest
{
    private const string OpenWebSocketScript =
        @"(url) => {
            return new Promise(resolve => {
                const socket = new WebSocket(url);
                socket.onmessage = () => {
                    resolve('opened');
                    socket.close();
                };
                socket.onerror = () => {
                    resolve('failed');
                };
                socket.onclose = () => {
                    resolve('failed');
                };
            });
        }";

    /// <summary>
    /// The execution contexts that can open WebSocket connections.
    /// </summary>
    private enum WebSocketClient
    {
        Page,
        DedicatedWorker,
        SharedWorker,
        ServiceWorker,
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket blocklist",
            "should fail WebSocket connections from a page to URLs in the blocklist"
        )
    ]
    public async Task ShouldFailWebSocketConnectionsFromAPageToURLsInTheBlocklist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/blocked-socket"];
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.Page, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("failed")
        );
        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/allowed-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket blocklist",
            "should fail WebSocket connections from a dedicated worker to URLs in the blocklist"
        )
    ]
    public async Task ShouldFailWebSocketConnectionsFromADedicatedWorkerToURLsInTheBlocklist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/blocked-socket"];
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.DedicatedWorker, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("failed")
        );
        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/allowed-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket blocklist",
            "should fail WebSocket connections from a shared worker to URLs in the blocklist"
        )
    ]
    public async Task ShouldFailWebSocketConnectionsFromASharedWorkerToURLsInTheBlocklist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/blocked-socket"];
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.SharedWorker, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("failed")
        );
        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/allowed-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket blocklist",
            "should fail WebSocket connections from a service worker to URLs in the blocklist"
        )
    ]
    public async Task ShouldFailWebSocketConnectionsFromAServiceWorkerToURLsInTheBlocklist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/blocked-socket"];
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.ServiceWorker, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("failed")
        );
        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/allowed-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket allowlist",
            "should fail WebSocket connections from a page to URLs not in the allowlist"
        )
    ]
    public async Task ShouldFailWebSocketConnectionsFromAPageToURLsNotInTheAllowlist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.Allowlist =
        [
            "*://*:*/empty.html",
            "*://*:*/worker/worker.html",
            "*://*:*/worker/worker.js",
            "*://*:*/serviceworkers/fetch/sw.html",
            "*://*:*/serviceworkers/fetch/sw.js",
            "*://*:*/allowed-socket",
        ];
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.Page, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("failed")
        );
        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/allowed-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket allowlist",
            "should fail WebSocket connections from a dedicated worker to URLs not in the allowlist"
        )
    ]
    public async Task ShouldFailWebSocketConnectionsFromADedicatedWorkerToURLsNotInTheAllowlist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.Allowlist =
        [
            "*://*:*/empty.html",
            "*://*:*/worker/worker.html",
            "*://*:*/worker/worker.js",
            "*://*:*/serviceworkers/fetch/sw.html",
            "*://*:*/serviceworkers/fetch/sw.js",
            "*://*:*/allowed-socket",
        ];
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.DedicatedWorker, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("failed")
        );
        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/allowed-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket allowlist",
            "should fail WebSocket connections from a shared worker to URLs not in the allowlist"
        )
    ]
    public async Task ShouldFailWebSocketConnectionsFromASharedWorkerToURLsNotInTheAllowlist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.Allowlist =
        [
            "*://*:*/empty.html",
            "*://*:*/worker/worker.html",
            "*://*:*/worker/worker.js",
            "*://*:*/serviceworkers/fetch/sw.html",
            "*://*:*/serviceworkers/fetch/sw.js",
            "*://*:*/allowed-socket",
        ];
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.SharedWorker, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("failed")
        );
        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/allowed-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket allowlist",
            "should fail WebSocket connections from a service worker to URLs not in the allowlist"
        )
    ]
    public async Task ShouldFailWebSocketConnectionsFromAServiceWorkerToURLsNotInTheAllowlist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.Allowlist =
        [
            "*://*:*/empty.html",
            "*://*:*/worker/worker.html",
            "*://*:*/worker/worker.js",
            "*://*:*/serviceworkers/fetch/sw.html",
            "*://*:*/serviceworkers/fetch/sw.js",
            "*://*:*/allowed-socket",
        ];
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.ServiceWorker, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("failed")
        );
        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/allowed-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket no restrictions",
            "should open WebSocket connections from a page"
        )
    ]
    public async Task ShouldOpenWebSocketConnectionsFromAPage()
    {
        await using var browser = await Puppeteer.LaunchAsync(
            TestConstants.DefaultBrowserOptions(),
            TestConstants.LoggerFactory
        );
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.Page, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket no restrictions",
            "should open WebSocket connections from a dedicated worker"
        )
    ]
    public async Task ShouldOpenWebSocketConnectionsFromADedicatedWorker()
    {
        await using var browser = await Puppeteer.LaunchAsync(
            TestConstants.DefaultBrowserOptions(),
            TestConstants.LoggerFactory
        );
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.DedicatedWorker, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket no restrictions",
            "should open WebSocket connections from a shared worker"
        )
    ]
    public async Task ShouldOpenWebSocketConnectionsFromASharedWorker()
    {
        await using var browser = await Puppeteer.LaunchAsync(
            TestConstants.DefaultBrowserOptions(),
            TestConstants.LoggerFactory
        );
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.SharedWorker, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket no restrictions",
            "should open WebSocket connections from a service worker"
        )
    ]
    public async Task ShouldOpenWebSocketConnectionsFromAServiceWorker()
    {
        await using var browser = await Puppeteer.LaunchAsync(
            TestConstants.DefaultBrowserOptions(),
            TestConstants.LoggerFactory
        );
        await using var page = await browser.NewPageAsync();
        var client = await StartAsync(WebSocketClient.ServiceWorker, page);

        Assert.That(
            await OpenWebSocketAsync(client, WebSocketUrl("/blocked-socket")),
            Is.EqualTo("opened")
        );
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket dynamic restrictions",
            "applies and clears restrictions on WebSocket connections from a page"
        )
    ]
    public async Task AppliesAndClearsRestrictionsOnWebSocketConnectionsFromAPage()
    {
        await using var browser = await Puppeteer.LaunchAsync(
            TestConstants.DefaultBrowserOptions(),
            TestConstants.LoggerFactory
        );
        await using var page = await browser.NewPageAsync();
        var url = WebSocketUrl("/blocked-socket");
        var client = await StartAsync(WebSocketClient.Page, page);

        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("opened"));

        await browser.RestrictNetworkAsync(
            new NetworkRestrictions { BlockList = ["*://*:*/blocked-socket"] }
        );
        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("failed"));

        await browser.RestrictNetworkAsync(null);
        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("opened"));
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket dynamic restrictions",
            "applies and clears restrictions on WebSocket connections from a dedicated worker"
        )
    ]
    public async Task AppliesAndClearsRestrictionsOnWebSocketConnectionsFromADedicatedWorker()
    {
        await using var browser = await Puppeteer.LaunchAsync(
            TestConstants.DefaultBrowserOptions(),
            TestConstants.LoggerFactory
        );
        await using var page = await browser.NewPageAsync();
        var url = WebSocketUrl("/blocked-socket");
        var client = await StartAsync(WebSocketClient.DedicatedWorker, page);

        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("opened"));

        await browser.RestrictNetworkAsync(
            new NetworkRestrictions { BlockList = ["*://*:*/blocked-socket"] }
        );
        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("failed"));

        await browser.RestrictNetworkAsync(null);
        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("opened"));
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket dynamic restrictions",
            "applies and clears restrictions on WebSocket connections from a shared worker"
        )
    ]
    public async Task AppliesAndClearsRestrictionsOnWebSocketConnectionsFromASharedWorker()
    {
        await using var browser = await Puppeteer.LaunchAsync(
            TestConstants.DefaultBrowserOptions(),
            TestConstants.LoggerFactory
        );
        await using var page = await browser.NewPageAsync();
        var url = WebSocketUrl("/blocked-socket");
        var client = await StartAsync(WebSocketClient.SharedWorker, page);

        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("opened"));

        await browser.RestrictNetworkAsync(
            new NetworkRestrictions { BlockList = ["*://*:*/blocked-socket"] }
        );
        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("failed"));

        await browser.RestrictNetworkAsync(null);
        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("opened"));
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebSocket dynamic restrictions",
            "applies and clears restrictions on WebSocket connections from a service worker"
        )
    ]
    public async Task AppliesAndClearsRestrictionsOnWebSocketConnectionsFromAServiceWorker()
    {
        await using var browser = await Puppeteer.LaunchAsync(
            TestConstants.DefaultBrowserOptions(),
            TestConstants.LoggerFactory
        );
        await using var page = await browser.NewPageAsync();
        var url = WebSocketUrl("/blocked-socket");
        var client = await StartAsync(WebSocketClient.ServiceWorker, page);

        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("opened"));

        await browser.RestrictNetworkAsync(
            new NetworkRestrictions { BlockList = ["*://*:*/blocked-socket"] }
        );
        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("failed"));

        await browser.RestrictNetworkAsync(null);
        Assert.That(await OpenWebSocketAsync(client, url), Is.EqualTo("opened"));
    }

    /// <summary>
    /// Returns the WebSocket URL of the test server for the given path. The test server accepts
    /// WebSocket connections on all paths.
    /// </summary>
    private static string WebSocketUrl(string path) =>
        TestConstants.ServerUrl.Replace("http", "ws", StringComparison.Ordinal) + path;

    /// <summary>
    /// Opens a WebSocket connection to the given URL from a page or a worker. The test server sends
    /// a message to each WebSocket client. Returns "opened" when the message arrives and "failed"
    /// when the connection fails or closes before the message arrives.
    /// </summary>
    private static Task<string> OpenWebSocketAsync(WebSocketContext client, string url) =>
        client.EvaluateAsync(url);

    /// <summary>
    /// Loads a page from the test server and returns the page or the worker that opens the connections.
    /// </summary>
    private static async Task<WebSocketContext> StartAsync(WebSocketClient client, IPage page)
    {
        switch (client)
        {
            case WebSocketClient.Page:
                await page.GoToAsync(TestConstants.ServerUrl + "/empty.html");
                return new WebSocketContext(page, null);
            case WebSocketClient.DedicatedWorker:
                var created = new TaskCompletionSource<WebWorker>(
                    TaskCreationOptions.RunContinuationsAsynchronously
                );
                page.WorkerCreated += (_, e) => created.TrySetResult(e.Worker);
                await page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html");
                return new WebSocketContext(null, await created.Task);
            case WebSocketClient.SharedWorker:
                await page.GoToAsync(TestConstants.ServerUrl + "/empty.html");
                var workerUrl = await page.EvaluateFunctionAsync<string>(
                    @"() => {
                        const url = URL.createObjectURL(
                            new Blob(['onconnect = () => {};'], {type: 'text/javascript'}),
                        );
                        window.sharedWorker = new SharedWorker(url);
                        return url;
                    }"
                );
                var shared = await page.BrowserContext.WaitForTargetAsync(t =>
                    t.Type == TargetType.SharedWorker && t.Url == workerUrl
                );
                return new WebSocketContext(null, await shared.WorkerAsync());
            default:
                await page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/fetch/sw.html");
                var service = await page.BrowserContext.WaitForTargetAsync(t =>
                    t.Type == TargetType.ServiceWorker
                );
                return new WebSocketContext(null, await service.WorkerAsync());
        }
    }

    private sealed record WebSocketContext(IPage Page, WebWorker Worker)
    {
        public Task<string> EvaluateAsync(string url) =>
            Page is not null
                ? Page.EvaluateFunctionAsync<string>(OpenWebSocketScript, url)
                : Worker.EvaluateFunctionAsync<string>(OpenWebSocketScript, url);
    }
}
