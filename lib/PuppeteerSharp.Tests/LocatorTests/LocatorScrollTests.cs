using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Locators;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LocatorTests
{
    public class LocatorScrollTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("locator.spec", "Locator Locator.scroll", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(@"
                <div style=""height: 500px; width: 500px; overflow: scroll;"">
                    <div style=""height: 1000px; width: 1000px;"">test</div>
                </div>");

            await Page.Locator("div").ScrollAsync(new LocatorScrollOptions
            {
                ScrollTop = 500,
                ScrollLeft = 500,
            });

            var scrollable = await Page.QuerySelectorAsync("div");
            var scroll = await scrollable.EvaluateFunctionAsync<string>("el => el.scrollTop + ' ' + el.scrollLeft");
            Assert.That(scroll, Is.EqualTo("500 500"));
        }
    }
}
