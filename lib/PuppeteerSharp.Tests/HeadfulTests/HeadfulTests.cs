using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.HeadfulTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HeadfulTests : PuppeteerBaseTest
    {
        private readonly LaunchOptions _forcedOopifOptions;
        
        public HeadfulTests(ITestOutputHelper output) : base(output)
        {
            _forcedOopifOptions = TestConstants.DefaultBrowserOptions();
            _forcedOopifOptions.Headless =  false;
            _forcedOopifOptions.Devtools = true;
            _forcedOopifOptions.Args = new[] {
                $"--host-rules=\"MAP oopifdomain 127.0.0.1\"",
                $"--isolate-origins=\"{TestConstants.ServerUrl.Replace("localhost", "oopifdomain")}\""
            };
        }

        [PuppeteerTest("headful.spec.ts", "HEADFUL", "background_page target type should be available")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task BackgroundPageTargetTypeShouldBeAvailable()
        {
            await using (var browserWithExtension = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory))
            await using (await browserWithExtension.NewPageAsync())
            {
                var backgroundPageTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.BackgroundPage);
                Assert.NotNull(backgroundPageTarget);
            }
        }

        [PuppeteerTest("headful.spec.ts", "HEADFUL", "target.page() should return a background_page")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task TargetPageShouldReturnABackgroundPage()
        {
            await using (var browserWithExtension = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory))
            {
                var backgroundPageTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.BackgroundPage);
                await using (var page = await backgroundPageTarget.PageAsync())
                {
                    Assert.Equal(6, await page.EvaluateFunctionAsync<int>("() => 2 * 3"));
                    Assert.Equal(42, await page.EvaluateFunctionAsync<int>("() => window.MAGIC"));
                }
            }
        }

        [PuppeteerTest("headful.spec.ts", "HEADFUL", "should have default url when launching browser")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldHaveDefaultUrlWhenLaunchingBrowser()
        {
            await using (var browser = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory))
            {
                var pages = (await browser.PagesAsync()).Select(page => page.Url).ToArray();
                Assert.Equal(new[] { "about:blank" }, pages);
            }
        }

        [PuppeteerTest("headful.spec.ts", "HEADFUL", "headless should be able to read cookies written by headful")]
        [PuppeteerFact(Skip = "Puppeteer ignores this in windows we do not have a platform filter yet")]
        public async Task HeadlessShouldBeAbleToReadCookiesWrittenByHeadful()
        {
            using (var userDataDir = new TempDirectory())
            {
                var launcher = new Launcher(TestConstants.LoggerFactory);
                var options = TestConstants.DefaultBrowserOptions();
                options.Args = options.Args.Concat(new[] { $"--user-data-dir=\"{userDataDir}\"" }).ToArray();
                options.Headless = false;
                await using (var browser = await launcher.LaunchAsync(options))
                await using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(TestConstants.EmptyPage);
                    await page.EvaluateExpressionAsync(
                        "document.cookie = 'foo=true; expires=Fri, 31 Dec 9999 23:59:59 GMT'");
                }

                await TestUtils.WaitForCookieInChromiumFileAsync(userDataDir.Path, "foo");

                options.Headless = true;
                await using (var browser2 = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
                {
                    var page2 = await browser2.NewPageAsync();
                    await page2.GoToAsync(TestConstants.EmptyPage);
                    Assert.Equal("foo=true", await page2.EvaluateExpressionAsync<string>("document.cookie"));
                }
            }
        }

        [PuppeteerTest("headful.spec.ts", "HEADFUL", "OOPIF: should report google.com frame")]
        [PuppeteerFact(Skip = "TODO: Support OOOPIF. @see https://github.com/GoogleChrome/puppeteer/issues/2548")]
        public async Task OOPIFShouldReportGoogleComFrame()
        {
            // https://google.com is isolated by default in Chromium embedder.
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Headless = false;
            await using (var browser = await Puppeteer.LaunchAsync(headfulOptions))
            await using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync(TestConstants.EmptyPage);
                await page.SetRequestInterceptionAsync(true);
                page.Request += async (_, e) => await e.Request.RespondAsync(
                    new ResponseData { Body = "{ body: 'YO, GOOGLE.COM'}" });
                await page.EvaluateFunctionHandleAsync(@"() => {
                    const frame = document.createElement('iframe');
                    frame.setAttribute('src', 'https://google.com/');
                    document.body.appendChild(frame);
                    return new Promise(x => frame.onload = x);
                }");
                await page.WaitForSelectorAsync("iframe[src=\"https://google.com/\"]");
                var urls = Array.ConvertAll(page.Frames, frame => frame.Url);
                Array.Sort(urls);
                Assert.Equal(new[] { TestConstants.EmptyPage, "https://google.com/" }, urls);
            }
        }

        [PuppeteerTest("headful.spec.ts", "HEADFUL", "OOPIF: should expose events within OOPIFs")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task OOPIFShouldExposeEventsWithinOOPIFs()
        {
            await using (var browser = await Puppeteer.LaunchAsync(_forcedOopifOptions))
            await using (var page = await browser.NewPageAsync())
            {
                // Setup our session listeners to observe OOPIF activity.
                var session = await page.Target.CreateCDPSessionAsync();
                var networkEvents = new List<string>();
                var otherSessions = new List<CDPSession>();

                await session.SendAsync("Target.setAutoAttach", new TargetSetAutoAttachRequest
                {
                    AutoAttach = true,
                    Flatten = true,
                    WaitForDebuggerOnStart = true,
                });

                session.SessionAttached += async (_, e) =>
                {
                    otherSessions.Add(e.Session);

                    await e.Session.SendAsync("Network.enable");
                    await e.Session.SendAsync("Runtime.runIfWaitingForDebugger");

                    e.Session.MessageReceived += (_, e) =>
                    {
                        if (e.MessageID.Equals("Network.requestWillBeSent", StringComparison.CurrentCultureIgnoreCase))
                        {
                            networkEvents.Add(e.MessageData.SelectToken("request").SelectToken("url").ToObject<string>());
                        }
                    };
                };

                // Navigate to the empty page and add an OOPIF iframe with at least one request.
                await page.GoToAsync(TestConstants.EmptyPage);
                await page.EvaluateFunctionAsync(@"(frameUrl) => {
                    const frame = document.createElement('iframe');
                    frame.setAttribute('src', frameUrl);
                    document.body.appendChild(frame);
                    return new Promise((x, y) => {
                        frame.onload = x;
                        frame.onerror = y;
                    });
                }", TestConstants.ServerUrl.Replace("localhost", "oopifdomain") + "/one-style.html");
                await page.WaitForSelectorAsync("iframe");

                // Ensure we found the iframe session.
                Assert.Single(otherSessions);

                // Resume the iframe and trigger another request.
                var iframeSession = otherSessions[0];
                await iframeSession.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest {
                    Expression = "fetch('/fetch')",
                    AwaitPromise = true,
                });
                await browser.CloseAsync();

                Assert.Contains($"http://oopifdomain:{TestConstants.Port}/fetch", networkEvents);
            }
        }

        [PuppeteerTest("headful.spec.ts", "HEADFUL", "should close browser with beforeunload page")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldCloseBrowserWithBeforeunloadPage()
        {
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Headless = false;
            await using (var browser = await Puppeteer.LaunchAsync(headfulOptions))
            await using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");
                // We have to interact with a page so that 'beforeunload' handlers fire.
                await page.ClickAsync("body");
            }
        }

        [PuppeteerTest("headful.spec.ts", "HEADFUL", "should open devtools when \"devtools: true\" option is given")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldOpenDevtoolsWhenDevtoolsTrueOptionIsGiven()
        {
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Devtools = true;
            await using (var browser = await Puppeteer.LaunchAsync(headfulOptions))
            {
                var context = await browser.CreateIncognitoBrowserContextAsync();
                await Task.WhenAll(
                    context.NewPageAsync(),
                    browser.WaitForTargetAsync(target => target.Url.Contains("devtools://")));
            }
        }
    }
}
