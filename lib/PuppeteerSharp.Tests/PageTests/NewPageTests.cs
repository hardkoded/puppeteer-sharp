using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class NewPageTests : PuppeteerBrowserContextBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.newPage", "should create a background page")]
        public async Task ShouldCreateABackgroundPage()
        {
            var page = await Context.NewPageAsync(new CreatePageOptions
            {
                Background = true,
            });

            var visibilityState = await page.EvaluateExpressionAsync<string>("document.visibilityState");
            Assert.That(visibilityState, Is.EqualTo("hidden"));

            await page.CloseAsync();
        }

    }
}
