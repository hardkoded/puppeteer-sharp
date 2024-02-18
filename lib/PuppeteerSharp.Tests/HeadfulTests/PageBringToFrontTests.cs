using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.HeadfulTests
{
    public class PageBringToFrontTests : PuppeteerBaseTest
    {
        [Test,  Retry(2), PuppeteerTest("headful.spec", "Page.bringToFront", "should work")]
        public async Task ShouldWork()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            await using var page = await browserWithExtension.NewPageAsync();
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
