using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Locators;
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

        [Test, PuppeteerTest("locator.spec", "Locator", "should work without preconditions")]
        public async Task ShouldWorkWithoutPreconditions()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button onclick=\"this.innerText = 'clicked';\">test</button>");

            await Page
                .Locator("button")
                .SetEnsureElementIsInTheViewport(false)
                .SetTimeout(0)
                .SetVisibility(null)
                .SetWaitForEnabled(false)
                .SetWaitForStableBoundingBox(false)
                .ClickAsync();

            var button = await Page.QuerySelectorAsync("button");
            var text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("clicked"));
        }

        [TestCase("() => { document.querySelector('#target').style.display = 'none'; }")]
        [TestCase("() => { document.querySelector('#target').remove(); }")]
        public async Task SetVisibilityHiddenShouldWaitForElementToBecomeHidden(string hideElement)
        {
            await Page.SetContentAsync("<div id=\"target\">Visible element</div>");

            var waitTask = Page
                .Locator("#target")
                .SetVisibility(VisibilityOption.Hidden)
                .SetTimeout(5000)
                .WaitAsync();

            await Task.Delay(500);
            Assert.That(waitTask.IsCompleted, Is.False, "Locator should not resolve while the element is still visible.");

            await Page.EvaluateFunctionAsync(hideElement);

            await waitTask;
            Assert.That(waitTask.IsCompletedSuccessfully, Is.True);
        }
    }
}
