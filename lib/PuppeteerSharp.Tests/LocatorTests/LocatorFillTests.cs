using System;
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

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should fill readonly color inputs")]
        public async Task ShouldFillReadonlyColorInputs()
        {
            await Page.SetContentAsync("<input type=\"color\" readonly value=\"#000000\" />");

            await Page.Locator("input").SetTimeout(500).FillAsync("#ff0000");

            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector(\'input\').value"), Is.EqualTo("#ff0000"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should fill readonly range inputs")]
        public async Task ShouldFillReadonlyRangeInputs()
        {
            await Page.SetContentAsync("<input type=\"range\" readonly value=\"25\" />");

            await Page.Locator("input").SetTimeout(500).FillAsync("75");

            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector(\'input\').value"), Is.EqualTo("75"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should reject readonly elements")]
        public async Task ShouldRejectReadonlyElements()
        {
            var cases = new[]
            {
                ("<input readonly value=\"fixed\" />", "input", "fixed", "changed"),
                ("<textarea readonly>fixed</textarea>", "textarea", "fixed", "changed"),
                ("<input type=\"number\" readonly value=\"7\" />", "input", "7", "9"),
                ("<input type=\"date\" readonly value=\"2020-01-01\" />", "input", "2020-01-01", "2021-02-03"),
            };

            foreach (var (markup, selector, value, newValue) in cases)
            {
                await Page.SetContentAsync(markup);

                Assert.ThrowsAsync<TimeoutException>(() => Page.Locator(selector).SetTimeout(500).FillAsync(newValue));
                Assert.That(await Page.EvaluateExpressionAsync<string>($"document.querySelector('{selector}').value"), Is.EqualTo(value), markup);
            }
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should preserve inputs on rejected values")]
        public async Task ShouldPreserveInputsOnRejectedValues()
        {
            var cases = new[]
            {
                ("number", "7", "abc", "12"),
                ("date", "2020-01-01", "next tuesday", "2021-02-03"),
                ("time", "10:30", "noon", "12:30"),
            };

            foreach (var (type, value, newValue, acceptedValue) in cases)
            {
                await Page.SetContentAsync($"<input type=\"{type}\" value=\"{value}\" />");

                Assert.ThrowsAsync<TimeoutException>(() => Page.Locator("input").SetTimeout(500).FillAsync(newValue));
                Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector(\'input\').value"), Is.EqualTo(value), type);

                await Page.Locator("input").FillAsync(acceptedValue);
                Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector(\'input\').value"), Is.EqualTo(acceptedValue), type);
            }
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should work if the input becomes writable later")]
        public async Task ShouldWorkIfTheInputBecomesWritableLater()
        {
            await Page.SetContentAsync("<input readonly value=\"fixed\" />");

            var input = await Page.QuerySelectorAsync("input");
            var resultTask = Page.Locator("input").FillAsync("test");

            Assert.That(await input.EvaluateFunctionAsync<string>("el => el.value"), Is.EqualTo("fixed"));

            await input.EvaluateFunctionAsync("el => el.readOnly = false");
            await resultTask;

            Assert.That(await input.EvaluateFunctionAsync<string>("el => el.value"), Is.EqualTo("test"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.fill", "should preserve the selection if no option matches the value")]
        public async Task ShouldPreserveTheSelectionIfNoOptionMatchesTheValue()
        {
            await Page.SetContentAsync(@"
                <select>
                    <option value=""a"">Option A</option>
                    <option value=""b"" selected>Option B</option>
                </select>");

            Assert.ThrowsAsync<TimeoutException>(() => Page.Locator("select").SetTimeout(500).FillAsync("zzz"));
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector(\'select\').value"), Is.EqualTo("b"));
        }
    }
}
