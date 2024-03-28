using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ExtensionsTests
{
    public class ExtensionsTests : PuppeteerBaseTest
    {
        [Test, Retry(2), PuppeteerTest("extensions.spec", "extensions", "background_page target type should be available")]
        public async Task BackgroundPageTargetTypeShouldBeAvailable()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            await using (await browserWithExtension.NewPageAsync())
            {
                var backgroundPageTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.BackgroundPage);
                Assert.NotNull(backgroundPageTarget);
            }
        }

        [Test, Retry(2), PuppeteerTest("extensions.spec", "extensions", "target.page() should return a background_page")]
        public async Task TargetPageShouldReturnABackgroundPage()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            var backgroundPageTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.BackgroundPage);
            await using var page = await backgroundPageTarget.PageAsync();
            Assert.AreEqual(6, await page.EvaluateFunctionAsync<int>("() => 2 * 3"));
            Assert.AreEqual(42, await page.EvaluateFunctionAsync<int>("() => window.MAGIC"));
        }
    }
}
