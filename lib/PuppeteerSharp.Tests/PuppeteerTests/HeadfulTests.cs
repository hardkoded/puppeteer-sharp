using System;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HeadfulTests : PuppeteerBaseTest
    {
        public HeadfulTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task BackgroundPageTargetTypeShouldBeAvailable()
        {
            using (var browserWithExtension = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory))
            using (var page = await browserWithExtension.NewPageAsync())
            {
                var backgroundPageTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.BackgroundPage);
                Assert.NotNull(backgroundPageTarget);
            }
        }

        [Fact]
        public async Task TargetPageShouldReturnABackgroundPage()
        {
            using (var browserWithExtension = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory))
            {
                var backgroundPageTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.BackgroundPage);
                using (var page = await backgroundPageTarget.PageAsync())
                {
                    Assert.Equal(6, await page.EvaluateFunctionAsync<int>("() => 2 * 3"));
                    Assert.Equal(42, await page.EvaluateFunctionAsync<int>("() => window.MAGIC"));
                }
            }
        }

        [Fact]
        public async Task ShouldHaveDefaultUrlWhenLaunchingBrowser()
        {
            using (var browser = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory))
            {
                var pages = (await browser.PagesAsync()).Select(page => page.Url).ToArray();
                Assert.Equal(new[] { "about:blank" }, pages);
            }
        }

        [Fact]
        public async Task HeadlessShouldBeAbleToReadCookiesWrittenByHeadful()
        {
            using (var userDataDir = new TempDirectory())
            {
                var launcher = new Launcher(TestConstants.LoggerFactory);
                var options = TestConstants.DefaultBrowserOptions();
                options.Args = options.Args.Concat(new[] { $"--user-data-dir=\"{userDataDir}\"" }).ToArray();
                options.Headless = false;
                using (var browser = await launcher.LaunchAsync(options))
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(TestConstants.EmptyPage);
                    await page.EvaluateExpressionAsync(
                        "document.cookie = 'foo=true; expires=Fri, 31 Dec 9999 23:59:59 GMT'");
                }

                await TestUtils.WaitForCookieInChromiumFileAsync(userDataDir.Path, "foo");

                options.Headless = true;
                using (var browser2 = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
                {
                    var page2 = await browser2.NewPageAsync();
                    await page2.GoToAsync(TestConstants.EmptyPage);
                    Assert.Equal("foo=true", await page2.EvaluateExpressionAsync<string>("document.cookie"));
                }
            }
        }

        [Fact(Skip = "TODO: Support OOOPIF. @see https://github.com/GoogleChrome/puppeteer/issues/2548")]
        public async Task OOPIFShouldReportGoogleComFrame()
        {
            // https://google.com is isolated by default in Chromium embedder.
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Headless = false;
            using (var browser = await Puppeteer.LaunchAsync(headfulOptions))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync(TestConstants.EmptyPage);
                await page.SetRequestInterceptionAsync(true);
                page.Request += async (sender, e) => await e.Request.RespondAsync(
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

        [Fact]
        public async Task ShouldCloseBrowserWithBeforeunloadPage()
        {
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Headless = false;
            using (var browser = await Puppeteer.LaunchAsync(headfulOptions))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");
                // We have to interact with a page so that 'beforeunload' handlers fire.
                await page.ClickAsync("body");
            }
        }

        [Fact]
        public async Task ShouldOpenDevtoolsWhenDevtoolsTrueOptionIsGiven()
        {
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Devtools = true;
            using (var browser = await Puppeteer.LaunchAsync(headfulOptions))
            {
                var context = await browser.CreateIncognitoBrowserContextAsync();
                await Task.WhenAll(
                    context.NewPageAsync(),
                    context.WaitForTargetAsync(target => target.Url.Contains("devtools://")));
            }
        }

        [Fact]
        public async Task BringToFrontShouldWork()
        {
            using (var browserWithExtension = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory))
            using (var page = await browserWithExtension.NewPageAsync())
            {
                await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal("visible", await page.EvaluateExpressionAsync<string>("document.visibilityState"));

                var newPage = await browserWithExtension.NewPageAsync();
                await newPage.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal("hidden", await page.EvaluateExpressionAsync<string>("document.visibilityState"));
                Assert.Equal("visible", await newPage.EvaluateExpressionAsync<string>("document.visibilityState"));

                await page.BringToFrontAsync();
                Assert.Equal("visible", await page.EvaluateExpressionAsync<string>("document.visibilityState"));
                Assert.Equal("hidden", await newPage.EvaluateExpressionAsync<string>("document.visibilityState"));

                await newPage.CloseAsync();
            }
        }
    }
}