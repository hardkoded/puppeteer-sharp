using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DevtoolsTests
{
    public class DevtoolsTests : PuppeteerBaseTest
    {
        [Test, Retry(2), PuppeteerTest("devtools.spec", "DevTools", "should open devtools when \"devtools: true\" option is given")]
        public async Task ShouldOpenDevtoolsWhenDevtoolsTrueOptionIsGiven()
        {
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Devtools = true;
            await using var browser = await Puppeteer.LaunchAsync(headfulOptions);
            var context = await browser.CreateBrowserContextAsync();
            await Task.WhenAll(
                context.NewPageAsync(),
                browser.WaitForTargetAsync(target => target.Url.Contains("devtools://")));
        }

        [Test, Retry(2), PuppeteerTest("devtools.spec", "DevTools", "should expose DevTools as a page")]
        public async Task ShouldExposeDevToolsAsAPage()
        {
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Devtools = true;
            await using var browser = await Puppeteer.LaunchAsync(headfulOptions);
            var context = await browser.CreateBrowserContextAsync();
            var targetTask = browser.WaitForTargetAsync(target => target.Url.Contains("devtools://"));
            await Task.WhenAll(
                context.NewPageAsync(),
                targetTask);

            var target = await targetTask;
            var page = await target.PageAsync();
            Assert.True(await page.EvaluateExpressionAsync<bool>("Boolean(DevToolsAPI)"));
        }

        [Test, Retry(2),
         PuppeteerTest("devtools.spec", "DevTools",
             "target.page() should return a DevTools page if custom isPageTarget is provided")]
        public async Task TargetPageShouldReturnADevToolsPageIfCustomIsPageTargetIsProvided()
        {
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Devtools = true;

            await using var originalBrowser = await Puppeteer.LaunchAsync(
                headfulOptions,
                TestConstants.LoggerFactory);
            var browserWSEndpoint = originalBrowser.WebSocketEndpoint;
            await using var browser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = browserWSEndpoint,
                IsPageTarget = target => target.Type == TargetType.Other && target.Url.StartsWith("devtools://")
            }, TestConstants.LoggerFactory);
            var devtoolsTargetTask = browser.WaitForTargetAsync(t => t.Type == TargetType.Other);
            var devtoolsTarget = await devtoolsTargetTask;
            await using var page = await devtoolsTarget.PageAsync();
            Assert.AreEqual(6, await page.EvaluateFunctionAsync<int>("() => 2 * 3"));
            var pages = await browser.PagesAsync();
            Assert.True(pages.Contains(page));
        }

        [Test, Retry(2), PuppeteerTest("devtools.spec", "DevTools", "target.page() should return Page when calling asPage on DevTools target")]
        public async Task TargetPageShouldReturnADevToolsPageIfAsPageIsUsed()
        {
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Devtools = true;
            await using var originalBrowser = await Puppeteer.LaunchAsync(
                headfulOptions,
                TestConstants.LoggerFactory);
            var browserWSEndpoint = originalBrowser.WebSocketEndpoint;
            await using var browser = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserWSEndpoint = browserWSEndpoint }, TestConstants.LoggerFactory);
            var devtoolsTargetTask = browser.WaitForTargetAsync(t => t.Type == TargetType.Other);
            await browser.NewPageAsync();
            var devtoolsTarget = await devtoolsTargetTask;
            await using var page = await devtoolsTarget.AsPageAsync();
            Assert.AreEqual(6, await page.EvaluateFunctionAsync<int>("() => 2 * 3"));
            Assert.False((await browser.PagesAsync()).Contains(page));
        }

    }
}
