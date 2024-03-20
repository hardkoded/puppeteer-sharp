using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageBringToFrontTests : PuppeteerBrowserContextBaseTest
    {
        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.bringToFront", "should work")]
        public async Task ShouldWork()
        {
            await using var page = await Context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual("visible", await page.EvaluateExpressionAsync<string>("document.visibilityState"));

            await using var newPage = await Context.NewPageAsync();
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
