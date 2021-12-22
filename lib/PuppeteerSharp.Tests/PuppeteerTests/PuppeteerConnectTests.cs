using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using PuppeteerSharp.Transport;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PuppeteerConnectTests : PuppeteerBrowserBaseTest
    {
        public PuppeteerConnectTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldBeAbleToConnectMultipleTimesToSameBrowser()
        {
            var options = new ConnectOptions()
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint
            };
            var browser = await Puppeteer.ConnectAsync(options, TestConstants.LoggerFactory);
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.EvaluateExpressionAsync<int>("7 * 8");
                Assert.Equal(56, response);
            }

            using (var originalPage = await Browser.NewPageAsync())
            {
                var response = await originalPage.EvaluateExpressionAsync<int>("7 * 6");
                Assert.Equal(42, response);
            }
        }

        [Fact]
        public async Task ShouldBeAbleToCloseRemoteBrowser()
        {
            var originalBrowser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var remoteBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = originalBrowser.WebSocketEndpoint
            });
            var tcsDisconnected = new TaskCompletionSource<bool>();

            originalBrowser.Disconnected += (sender, e) => tcsDisconnected.TrySetResult(true);
            await Task.WhenAll(
              tcsDisconnected.Task,
              remoteBrowser.CloseAsync());
        }

        [Fact]
        public async Task ShouldSupportIgnoreHTTPSErrorsOption()
        {
            using (var originalBrowser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions()))
            using (var browser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = originalBrowser.WebSocketEndpoint,
                IgnoreHTTPSErrors = true
            }))
            using (var page = await browser.NewPageAsync())
            {
                var requestTask = HttpsServer.WaitForRequest(
                    "/empty.html",
                    request => request.HttpContext.Features.Get<ITlsHandshakeFeature>().Protocol);
                var responseTask = page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

                await Task.WhenAll(
                    requestTask,
                    responseTask);

                var response = responseTask.Result;
                Assert.True(response.Ok);
                Assert.NotNull(response.SecurityDetails);
                Assert.Equal(
                    TestUtils.CurateProtocol(requestTask.Result.ToString()),
                    TestUtils.CurateProtocol(response.SecurityDetails.Protocol));
            }
        }

        [Fact]
        public async Task ShouldBeAbleToReconnectToADisconnectedBrowser()
        {
            var options = new ConnectOptions()
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint
            };

            var url = TestConstants.ServerUrl + "/frames/nested-frames.html";
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(url);

            Browser.Disconnect();

            using (var browser = await Puppeteer.ConnectAsync(options, TestConstants.LoggerFactory))
            {
                var pages = (await browser.PagesAsync()).ToList();
                var restoredPage = pages.FirstOrDefault(x => x.Url == url);
                Assert.NotNull(restoredPage);
                var frameDump = FrameUtils.DumpFrames(restoredPage.MainFrame);
                Assert.Equal(TestConstants.NestedFramesDumpResult, frameDump);
                var response = await restoredPage.EvaluateExpressionAsync<int>("7 * 8");
                Assert.Equal(56, response);
            }
        }

        [Fact]
        public async Task ShouldBeAbleToConnectToTheSamePageSimultaneously()
        {
            var browserOne = await Puppeteer.LaunchAsync(new LaunchOptions());
            var browserTwo = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = browserOne.WebSocketEndpoint
            });
            var tcs = new TaskCompletionSource<Page>();
            async void TargetCreated(object sender, TargetChangedArgs e)
            {
                tcs.TrySetResult(await e.Target.PageAsync());
                browserOne.TargetCreated -= TargetCreated;
            }
            browserOne.TargetCreated += TargetCreated;
            var page2Task = browserOne.NewPageAsync();

            await Task.WhenAll(tcs.Task, page2Task);
            var page1 = tcs.Task.Result;
            var page2 = page2Task.Result;

            Assert.Equal(56, await page1.EvaluateExpressionAsync<int>("7 * 8"));
            Assert.Equal(42, await page1.EvaluateExpressionAsync<int>("7 * 6"));
            await browserOne.CloseAsync();
        }
        [Fact]
        public async Task ShouldSupportCustomWebSocket()
        {
            var customSocketCreated = false;
            var options = new ConnectOptions()
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint,
                WebSocketFactory = (uri, socketOptions, cancellationToken) =>
                {
                    customSocketCreated = true;
                    return WebSocketTransport.DefaultWebSocketFactory(uri, socketOptions, cancellationToken);
                }
            };

            using (await Puppeteer.ConnectAsync(options, TestConstants.LoggerFactory))
            {
                Assert.True(customSocketCreated);
            }
        }

        [Fact]
        public async Task ShouldSupportCustomTransport()
        {
            var customTransportCreated = false;
            var options = new ConnectOptions()
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint,
                TransportFactory = (url, opt, cancellationToken) =>
                {
                    customTransportCreated = true;
                    return WebSocketTransport.DefaultTransportFactory(url, opt, cancellationToken);
                }
            };

            using (await Puppeteer.ConnectAsync(options, TestConstants.LoggerFactory))
            {
                Assert.True(customTransportCreated);
            }
        }
    }
}
