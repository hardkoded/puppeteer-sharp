using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.DevtoolsTests
{
    public class DevtoolsTests : PuppeteerBaseTest
    {
        public DevtoolsTests()
        {
        }

        [Test, PuppeteerTest("devtools.spec", "DevTools", "should open devtools when \"devtools: true\" option is given")]
        public async Task ShouldOpenDevtoolsWhenDevtoolsTrueOptionIsGiven()
        {
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Devtools = true;
            await using var browser = await Puppeteer.LaunchAsync(headfulOptions);
            var context = await browser.CreateIncognitoBrowserContextAsync();
            await Task.WhenAll(
                context.NewPageAsync(),
                browser.WaitForTargetAsync(target => target.Url.Contains("devtools://")));
        }

        [Test, PuppeteerTest("devtools.spec", "DevTools", "should expose DevTools as a page")]
        public async Task ShouldExposeDevToolsAsAPage()
        {
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Devtools = true;
            await using var browser = await Puppeteer.LaunchAsync(headfulOptions);
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
