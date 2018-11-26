using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    [Collection("PuppeteerLoaderFixture collection")]
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
                var backgroundPageTarget = await WaitForBackgroundPageTargetAsync(browserWithExtension);
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
                var backgroundPageTarget = await WaitForBackgroundPageTargetAsync(browserWithExtension);
                var page = await backgroundPageTarget.PageAsync();
                Assert.Equal(6, await page.EvaluateFunctionAsync<int>("() => 2 * 3"));
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
                {
                    var page = await browser.NewPageAsync();
                    await page.GoToAsync(TestConstants.EmptyPage);
                    await page.EvaluateExpressionAsync(
                        "document.cookie = 'foo=true; expires=Fri, 31 Dec 9999 23:59:59 GMT'");
                }

                options.Headless = true;
                using (var browser2 = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
                {
                    var page2 = await browser2.NewPageAsync();
                    await page2.GoToAsync(TestConstants.EmptyPage);
                    Assert.Equal("foo=true", await page2.EvaluateExpressionAsync("document.cookie"));
                }
            }
        }

        [Fact]
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

        private Task<Target> WaitForBackgroundPageTargetAsync(Browser browser)
        {
            var target = browser.Targets().FirstOrDefault(t => t.Type == TargetType.BackgroundPage);
            if (target != null)
            {
                return Task.FromResult(target);
            }
            var targetCreatedTcs = new TaskCompletionSource<Target>();
            void targetCreated(object sender, TargetChangedArgs e)
            {
                if (e.Target.Type != TargetType.BackgroundPage)
                {
                    return;
                }
                targetCreatedTcs.TrySetResult(e.Target);
                browser.TargetCreated -= targetCreated;
            }
            browser.TargetCreated += targetCreated;

            return targetCreatedTcs.Task;
        }
    }
}