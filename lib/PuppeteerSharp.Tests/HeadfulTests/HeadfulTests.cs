using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.HeadfulTests
{
    public class HeadfulTests : PuppeteerBaseTest
    {
        [Test, Retry(2), PuppeteerTest("headful.spec", "HEADFUL", "should have default url when launching browser")]
        public async Task ShouldHaveDefaultUrlWhenLaunchingBrowser()
        {
            await using var browser = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            var pages = (await browser.PagesAsync()).Select(page => page.Url).ToArray();
            Assert.AreEqual(new[] { "about:blank" }, pages);
        }

        [Test, Retry(2), PuppeteerTest("headful.spec", "HEADFUL", "headless should be able to read cookies written by headful")]
        [Ignore("Puppeteer ignores this in windows we do not have a platform filter yet")]
        public async Task HeadlessShouldBeAbleToReadCookiesWrittenByHeadful()
        {
            using var userDataDir = new TempDirectory();
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
                Assert.AreEqual("foo=true", await page2.EvaluateExpressionAsync<string>("document.cookie"));
            }
        }

        [Test, Retry(2), PuppeteerTest("headful.spec", "HEADFUL", "OOPIF: should report google.com frame")]
        [Ignore("TODO: Support OOOPIF. @see https://github.com/GoogleChrome/puppeteer/issues/2548")]
        public async Task OOPIFShouldReportGoogleComFrame()
        {
            // https://google.com is isolated by default in Chromium embedder.
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Headless = false;
            await using var browser = await Puppeteer.LaunchAsync(headfulOptions);
            await using var page = await browser.NewPageAsync();
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
            Array.Sort((Array)urls);
            Assert.AreEqual(new[] { TestConstants.EmptyPage, "https://google.com/" }, urls);
        }

        [Test, Retry(2), PuppeteerTest("headful.spec", "HEADFUL", "should close browser with beforeunload page")]
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
    }
}
