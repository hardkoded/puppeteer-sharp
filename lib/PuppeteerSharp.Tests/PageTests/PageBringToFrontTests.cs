using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageBringToFrontTests : PuppeteerBrowserContextBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.bringToFront", "should work")]
        public async Task ShouldWork()
        {
            await using var page = await Context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(await page.EvaluateExpressionAsync<string>("document.visibilityState"), Is.EqualTo("visible"));

            await using var newPage = await Context.NewPageAsync();
            await newPage.GoToAsync(TestConstants.EmptyPage);
            Assert.That(await page.EvaluateExpressionAsync<string>("document.visibilityState"), Is.EqualTo("hidden"));
            Assert.That(await newPage.EvaluateExpressionAsync<string>("document.visibilityState"), Is.EqualTo("visible"));

            await page.BringToFrontAsync();
            Assert.That(await page.EvaluateExpressionAsync<string>("document.visibilityState"), Is.EqualTo("visible"));
            Assert.That(await newPage.EvaluateExpressionAsync<string>("document.visibilityState"), Is.EqualTo("hidden"));

            await newPage.CloseAsync();
        }
    }
}
