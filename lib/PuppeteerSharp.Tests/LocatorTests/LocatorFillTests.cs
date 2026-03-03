using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LocatorTests
{
    public class LocatorFillTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should work for textarea")]
        public async Task ShouldWorkForTextarea()
        {
            await Page.SetContentAsync("<textarea></textarea>");

            await Page.Locator("textarea").FillAsync("test");

            var result = await Page.EvaluateExpressionAsync<bool>(
                "document.querySelector('textarea')?.value === 'test'");
            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should work for selects")]
        public async Task ShouldWorkForSelects()
        {
            await Page.SetContentAsync(@"
                <select>
                    <option value=""value1"">Option 1</option>
                    <option value=""value2"">Option 2</option>
                </select>");

            await Page.Locator("select").FillAsync("value2");

            var result = await Page.EvaluateExpressionAsync<bool>(
                "document.querySelector('select')?.value === 'value2'");
            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should work for inputs")]
        public async Task ShouldWorkForInputs()
        {
            await Page.SetContentAsync("<input />");

            await Page.Locator("input").FillAsync("test");

            var result = await Page.EvaluateExpressionAsync<bool>(
                "document.querySelector('input')?.value === 'test'");
            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should work if the input becomes enabled later")]
        public async Task ShouldWorkIfTheInputBecomesEnabledLater()
        {
            await Page.SetContentAsync("<input disabled />");

            var input = await Page.QuerySelectorAsync("input");
            var resultTask = Page.Locator("input").FillAsync("test");

            var value = await input.EvaluateFunctionAsync<string>("el => el.value");
            Assert.That(value, Is.EqualTo(string.Empty));

            await input.EvaluateFunctionAsync("el => el.disabled = false");
            await resultTask;

            value = await input.EvaluateFunctionAsync<string>("el => el.value");
            Assert.That(value, Is.EqualTo("test"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should work for contenteditable")]
        public async Task ShouldWorkForContenteditable()
        {
            await Page.SetContentAsync("<div contenteditable=\"true\"></div>");

            await Page.Locator("div").FillAsync("test");

            var result = await Page.EvaluateExpressionAsync<bool>(
                "document.querySelector('div')?.innerText === 'test'");
            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should work for pre-filled inputs")]
        public async Task ShouldWorkForPreFilledInputs()
        {
            await Page.SetContentAsync("<input value=\"te\" />");

            await Page.Locator("input").FillAsync("test");

            var result = await Page.EvaluateExpressionAsync<bool>(
                "document.querySelector('input')?.value === 'test'");
            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should override pre-filled inputs")]
        public async Task ShouldOverridePreFilledInputs()
        {
            await Page.SetContentAsync("<input value=\"wrong prefix\" />");

            await Page.Locator("input").FillAsync("test");

            var result = await Page.EvaluateExpressionAsync<bool>(
                "document.querySelector('input')?.value === 'test'");
            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should work for non-text inputs")]
        public async Task ShouldWorkForNonTextInputs()
        {
            await Page.SetContentAsync("<input type=\"color\" />");

            await Page.Locator("input").FillAsync("#333333");

            var result = await Page.EvaluateExpressionAsync<bool>(
                "document.querySelector('input')?.value === '#333333'");
            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should work for large text")]
        public async Task ShouldWorkForLargeText()
        {
            await Page.SetContentAsync("<textarea></textarea>");

            var largeText = new string('a', 1000);
            await Page.Locator("textarea").FillAsync(largeText);

            var result = await Page.EvaluateExpressionAsync<bool>(
                "document.querySelector('textarea')?.value.length === 1000");
            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should work for large text in contenteditable")]
        public async Task ShouldWorkForLargeTextInContenteditable()
        {
            await Page.SetContentAsync("<div contenteditable=\"true\"></div>");

            var largeText = new string('a', 1000);
            await Page.Locator("div").FillAsync(largeText);

            var result = await Page.EvaluateExpressionAsync<bool>(
                "document.querySelector('div')?.innerText.length === 1000");
            Assert.That(result, Is.True);
        }
    }
}
