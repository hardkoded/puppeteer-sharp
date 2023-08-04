using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using PuppeteerSharp.Transport;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.LauncherTests
{
    public class PuppeteerConnectTests : PuppeteerBrowserBaseTest
    {
        public PuppeteerConnectTests(): base()
        {
        }

        [PuppeteerTest("launcher.spec.ts", "Puppeteer.connect", "should be able to connect multiple times to the same browser")]
        [PuppeteerTimeout]
        public async Task ShouldBeAbleToConnectMultipleTimesToSameBrowser()
        {
            var options = new ConnectOptions()
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint
            };
            var browser = await Puppeteer.ConnectAsync(options, TestConstants.LoggerFactory);
            await using (var page = await browser.NewPageAsync())
            {
                var response = await page.EvaluateExpressionAsync<int>("7 * 8");
                Assert.AreEqual(56, response);
            }

            await using (var originalPage = await Browser.NewPageAsync())
            {
                var response = await originalPage.EvaluateExpressionAsync<int>("7 * 6");
                Assert.AreEqual(42, response);
            }
        }

        [PuppeteerTest("launcher.spec.ts", "Puppeteer.connect", "should be able to close remote browser")]
        [PuppeteerTimeout]
        public async Task ShouldBeAbleToCloseRemoteBrowser()
        {
            var originalBrowser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var remoteBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = originalBrowser.WebSocketEndpoint
            });
            var tcsDisconnected = new TaskCompletionSource<bool>();

            originalBrowser.Disconnected += (_, _) => tcsDisconnected.TrySetResult(true);
            await Task.WhenAll(
              tcsDisconnected.Task,
              remoteBrowser.CloseAsync());
        }

        [PuppeteerTest("launcher.spec.ts", "Puppeteer.connect", "should support ignoreHTTPSErrors option")]
        [PuppeteerTimeout]
        public async Task ShouldSupportIgnoreHTTPSErrorsOption()
        {
            await using (var originalBrowser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions()))
            await using (var browser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = originalBrowser.WebSocketEndpoint,
                IgnoreHTTPSErrors = true
            }))
            await using (var page = await browser.NewPageAsync())
            {
                var requestTask = HttpsServer.WaitForRequest(
                    "/empty.html",
                    request => request?.HttpContext?.Features?.Get<ITlsHandshakeFeature>()?.Protocol);
                var responseTask = page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

                await Task.WhenAll(
                    requestTask,
                    responseTask).WithTimeout(Puppeteer.DefaultTimeout);

                var response = responseTask.Result;
                Assert.True(response.Ok);
                Assert.NotNull(response.SecurityDetails);
                Assert.AreEqual(
                    TestUtils.CurateProtocol(requestTask.Result.ToString()),
                    TestUtils.CurateProtocol(response.SecurityDetails.Protocol));
            }
        }

        [PuppeteerTest("launcher.spec.ts", "Puppeteer.connect", "should support targetFilter option")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldSupportTargetFilter()
        {
            await using (var originalBrowser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.LoggerFactory))
            {
                var page1 = await originalBrowser.NewPageAsync();
                await page1.GoToAsync(TestConstants.EmptyPage);

                var page2 = await originalBrowser.NewPageAsync();
                await page2.GoToAsync(TestConstants.EmptyPage + "?should-be-ignored");

                var browser = await Puppeteer.ConnectAsync(new ConnectOptions {
                    BrowserWSEndpoint = originalBrowser.WebSocketEndpoint,
                    TargetFilter = (TargetInfo targetInfo) => !targetInfo.Url.Contains("should-be-ignored"),
                }, TestConstants.LoggerFactory);

                var pages = await browser.PagesAsync();

                await page2.CloseAsync();
                await page1.CloseAsync();
                await browser.CloseAsync();

                Assert.AreEqual(
                    new string[]
                    {
                        "about:blank",
                        TestConstants.EmptyPage
                    },
                    pages.Select((IPage p) => p.Url).OrderBy(t => t));
            }
        }

        [PuppeteerTimeout]
        public async Task ShouldBeAbleToSetBrowserPropertiesUsingConnectOptions()
        {
            var initActionExecuted = false;
            var options = new ConnectOptions
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint,
                InitAction = brw =>
                {
                    initActionExecuted = true;
                }
            };
            var browser = await Puppeteer.ConnectAsync(options, TestConstants.LoggerFactory);

            Assert.True(initActionExecuted);

            await browser.CloseAsync();
        }

        [PuppeteerTest("launcher.spec.ts", "Puppeteer.connect", "should be able to reconnect to a disconnected browser")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

            await using (var browser = await Puppeteer.ConnectAsync(options, TestConstants.LoggerFactory))
            {
                var pages = (await browser.PagesAsync()).ToList();
                var restoredPage = pages.FirstOrDefault(x => x.Url == url);
                Assert.NotNull(restoredPage);
                var frameDump = FrameUtils.DumpFrames(restoredPage.MainFrame);
                Assert.AreEqual(TestConstants.NestedFramesDumpResult, frameDump);
                var response = await restoredPage.EvaluateExpressionAsync<int>("7 * 8");
                Assert.AreEqual(56, response);
            }
        }

        [PuppeteerTest("launcher.spec.ts", "Puppeteer.connect", "should be able to connect to the same page simultaneously")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldBeAbleToConnectToTheSamePageSimultaneously()
        {
            var browserOne = await Puppeteer.LaunchAsync(new LaunchOptions());
            var browserTwo = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = browserOne.WebSocketEndpoint
            });
            var tcs = new TaskCompletionSource<IPage>();
            async void TargetCreated(object sender, TargetChangedArgs e)
            {
                tcs.TrySetResult(await e.Target.PageAsync());
                browserOne.TargetCreated -= TargetCreated;
            }
            browserOne.TargetCreated += TargetCreated;
            var page2Task = browserTwo.NewPageAsync();

            await Task.WhenAll(tcs.Task, page2Task);
            var page1 = tcs.Task.Result;
            var page2 = page2Task.Result;

            Assert.AreEqual(56, await page1.EvaluateExpressionAsync<int>("7 * 8"));
            Assert.AreEqual(42, await page2.EvaluateExpressionAsync<int>("7 * 6"));
            await browserOne.CloseAsync();
        }

        [PuppeteerTest("launcher.spec.ts", "Puppeteer.connect", "should be able to reconnect")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldBeAbleToReconnect()
        {
            var browserOne = await Puppeteer.LaunchAsync(new LaunchOptions());
            var browserWSEndpoint = browserOne.WebSocketEndpoint;
            var page1 = await browserOne.NewPageAsync();
            await page1.GoToAsync(TestConstants.EmptyPage);
            browserOne.Disconnect();

            var browserTwo = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = browserWSEndpoint
            });

            var pages = await browserTwo.PagesAsync();
            var pageTwo = pages.First(page => page.Url == TestConstants.EmptyPage);
            await pageTwo.ReloadAsync();
            var bodyHandle = await pageTwo.WaitForSelectorAsync("body", new WaitForSelectorOptions { Timeout = 10000 });
            await bodyHandle.DisposeAsync();
            await browserTwo.CloseAsync();
        }

        [PuppeteerTimeout]
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

            await using (await Puppeteer.ConnectAsync(options, TestConstants.LoggerFactory))
            {
                Assert.True(customSocketCreated);
            }
        }

        [PuppeteerTimeout]
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

            await using (await Puppeteer.ConnectAsync(options, TestConstants.LoggerFactory))
            {
                Assert.True(customTransportCreated);
            }
        }
    }
}
