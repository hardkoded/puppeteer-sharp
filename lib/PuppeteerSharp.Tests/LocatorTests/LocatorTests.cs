using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LocatorTests
{
    public class LocatorTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("locator.spec", "Locator", "should work with a frame")]
        public async Task ShouldWorkWithAFrame()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button onclick=\"this.innerText = 'clicked';\">test</button>");

            await Page
                .MainFrame
                .Locator("button")
                .ClickAsync();

            var button = await Page.QuerySelectorAsync("button");
            var text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("clicked"));
        }
    }
}
