using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class NewPageTests : PuppeteerBrowserBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page", "Page.newPage should create a background page")]
        public async Task ShouldCreateABackgroundPage()
        {
            var context = Browser.DefaultContext;

            var page = await context.NewPageAsync(new CreatePageOptions
            {
                Background = true,
            });

            var visibilityState = await page.EvaluateExpressionAsync<string>("document.visibilityState");
            Assert.That(visibilityState, Is.EqualTo("hidden"));

            await page.CloseAsync();
        }

        [Test, PuppeteerTest("page.spec", "Page", "Page.newPage should create a foreground page by default")]
        public async Task ShouldCreateAForegroundPageByDefault()
        {
            var context = Browser.DefaultContext;

            var page = await context.NewPageAsync();

            var visibilityState = await page.EvaluateExpressionAsync<string>("document.visibilityState");
            Assert.That(visibilityState, Is.EqualTo("visible"));

            await page.CloseAsync();
        }
    }
}
