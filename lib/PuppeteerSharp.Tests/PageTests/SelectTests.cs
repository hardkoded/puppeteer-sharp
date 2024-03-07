using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class SelectTests : PuppeteerPageBaseTest
    {
        public SelectTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should select single option")]
        public async Task ShouldSelectSingleOption()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.SelectAsync("select", "blue");
            Assert.AreEqual(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.AreEqual(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onChange"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should select only first option")]
        public async Task ShouldSelectOnlyFirstOption()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.SelectAsync("select", "blue", "green", "red");
            Assert.AreEqual(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.AreEqual(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onChange"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should not throw when select causes navigation")]
        public async Task ShouldNotThrowWhenSelectCausesNavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.QuerySelectorAsync("select").EvaluateFunctionAsync("select => select.addEventListener('input', () => window.location = '/empty.html')");
            await Task.WhenAll(
              Page.SelectAsync("select", "blue"),
              Page.WaitForNavigationAsync()
            );
            StringAssert.Contains("empty.html", Page.Url);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should select multiple options")]
        public async Task ShouldSelectMultipleOptions()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.EvaluateExpressionAsync("makeMultiple()");
            await Page.SelectAsync("select", "blue", "green", "red");
            Assert.AreEqual(new string[] { "blue", "green", "red" },
                         await Page.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.AreEqual(new string[] { "blue", "green", "red" },
                         await Page.EvaluateExpressionAsync<string[]>("result.onChange"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should respect event bubbling")]
        public async Task ShouldRespectEventBubbling()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.SelectAsync("select", "blue");
            Assert.AreEqual(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onBubblingInput"));
            Assert.AreEqual(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onBubblingChange"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should throw when element is not a <select>")]
        public async Task ShouldThrowWhenElementIsNotASelect()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(async () => await Page.SelectAsync("body", ""));
            StringAssert.Contains("Element is not a <select> element.", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should return [] on no matched values")]
        public async Task ShouldReturnEmptyArrayOnNoMatchedValues()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var result = await Page.SelectAsync("select", "42", "abc");
            Assert.IsEmpty(result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should return an array of matched values")]
        public async Task ShouldReturnAnArrayOfMatchedValues()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.EvaluateExpressionAsync("makeMultiple()");
            var result = await Page.SelectAsync("select", "blue", "black", "magenta");
            Array.Sort(result);
            Assert.AreEqual(new string[] { "black", "blue", "magenta" }, result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should return an array of one element when multiple is not set")]
        public async Task ShouldReturnAnArrayOfOneElementWhenMultipleIsNotSet()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            Assert.That(await Page.SelectAsync("select", "42", "blue", "black", "magenta"), Has.Exactly(1).Items);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should return [] on no values")]
        public async Task ShouldReturnEmptyArrayOnNoValues()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            Assert.IsEmpty(await Page.SelectAsync("select"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should deselect all options when passed no values for a multiple select")]
        public async Task ShouldDeselectAllOptionsWhenPassedNoValuesForAMultipleSelect()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.EvaluateExpressionAsync("makeMultiple()");
            await Page.SelectAsync("select", "blue", "black", "magenta");
            await Page.SelectAsync("select");
            Assert.True(await Page.QuerySelectorAsync("select").EvaluateFunctionAsync<bool>(
                "select => Array.from(select.options).every(option => !option.selected)"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.select", "should deselect all options when passed no values for a select without multiple")]
        public async Task ShouldDeselectAllOptionsWhenPassedNoValuesForASelectWithoutMultiple()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.SelectAsync("select", "blue", "black", "magenta");
            await Page.SelectAsync("select");
            Assert.True(await Page.QuerySelectorAsync("select").EvaluateFunctionAsync<bool>(
                "select => Array.from(select.options).every(option => !option.selected)"));
        }
    }
}
