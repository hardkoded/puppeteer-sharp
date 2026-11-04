using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LocatorTests
{
    public class LocatorHoverTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("locator.spec", "Locator Locator.hover", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button onmouseenter=\"this.innerText = 'hovered';\">test</button>");

            await Page.Locator("button").HoverAsync();

            var button = await Page.QuerySelectorAsync("button");
            var text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("hovered"));
        }
    }
}
