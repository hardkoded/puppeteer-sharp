using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.DevtoolsTests
{
    public class DevtoolsTests : PuppeteerBaseTest
    {
        private readonly LaunchOptions _optionsWithDevtools;

        public DevtoolsTests()
        {
            _optionsWithDevtools = TestConstants.DefaultBrowserOptions();
            _optionsWithDevtools.Devtools = true;
        }

        [PuppeteerTest("devtools.spec.ts", "DevTools", "target.page() should return a DevTools page if asPage is used")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task TargetPageShouldReturnADevToolsPageIfAsPageIsUsed()
        {
            await using var originalBrowser = await Puppeteer.LaunchAsync(
                _optionsWithDevtools,
                TestConstants.LoggerFactory);
            var browserWSEndpoint = originalBrowser.WebSocketEndpoint;
            await using var browser = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserWSEndpoint = browserWSEndpoint }, TestConstants.LoggerFactory);
            var devtoolsTargetTask = browser.WaitForTargetAsync(t => t.Type == TargetType.Other);
            await browser.NewPageAsync();
            var devtoolsTarget = await devtoolsTargetTask;
            await using var page = await devtoolsTarget.AsPageAsync();
            Assert.AreEqual(6, await page.EvaluateFunctionAsync<int>("() => 2 * 3"));
            Assert.True((await browser.PagesAsync()).Contains(page));
        }

        [PuppeteerTest("devtools.spec.ts", "DevTools", "should open devtools when \"devtools: true\" option is given")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldOpenDevtoolsWhenDevtoolsTrueOptionIsGiven()
        {
            await using var browser = await Puppeteer.LaunchAsync(_optionsWithDevtools);
            var context = await browser.CreateIncognitoBrowserContextAsync();
            await Task.WhenAll(
                context.NewPageAsync(),
                browser.WaitForTargetAsync(target => target.Url.Contains("devtools://")));
        }

        [PuppeteerTest("devtools.spec.ts", "DevTools", "should expose DevTools as a page")]
        [Skip(SkipAttribute.Targets.Firefox, SkipAttribute.Targets.Windows)]
        public async Task ShouldExposeDevToolsAsAPage()
        {
            await using var browser = await Puppeteer.LaunchAsync(_optionsWithDevtools);
            var context = await browser.CreateIncognitoBrowserContextAsync();
            var targetTask = browser.WaitForTargetAsync(target => target.Url.Contains("devtools://"));
            await Task.WhenAll(
                context.NewPageAsync(),
                targetTask);

            var target = await targetTask;
            var page = await target.PageAsync();
            Assert.True(await page.EvaluateExpressionAsync<bool>("Boolean(DevToolsAPI)"));
        }
    }
}
