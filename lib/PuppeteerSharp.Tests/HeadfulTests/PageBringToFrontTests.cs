using System;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.HeadfulTests
{
    public class PageBringToFrontTests : PuppeteerBaseTest
    {
        public PageBringToFrontTests(): base()
        {
        }

        [PuppeteerTest("headful.spec.ts", "Page.bringToFront", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            await using (var browserWithExtension = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory))
            await using (var page = await browserWithExtension.NewPageAsync())
            {
                await page.GoToAsync(TestConstants.EmptyPage);
                Assert.AreEqual("visible", await page.EvaluateExpressionAsync<string>("document.visibilityState"));

                var newPage = await browserWithExtension.NewPageAsync();
                await newPage.GoToAsync(TestConstants.EmptyPage);
                Assert.AreEqual("hidden", await page.EvaluateExpressionAsync<string>("document.visibilityState"));
                Assert.AreEqual("visible", await newPage.EvaluateExpressionAsync<string>("document.visibilityState"));

                await page.BringToFrontAsync();
                Assert.AreEqual("visible", await page.EvaluateExpressionAsync<string>("document.visibilityState"));
                Assert.AreEqual("hidden", await newPage.EvaluateExpressionAsync<string>("document.visibilityState"));

                await newPage.CloseAsync();
            }
        }
    }
}
