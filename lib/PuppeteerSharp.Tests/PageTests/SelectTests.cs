using System;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class SelectTests : PuppeteerPageBaseTest
    {
        public SelectTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should select single option")]
        [PuppeteerFact]
        public async Task ShouldSelectSingleOption()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.SelectAsync("select", "blue");
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onChange"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should select only first option")]
        [PuppeteerFact]
        public async Task ShouldSelectOnlyFirstOption()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.SelectAsync("select", "blue", "green", "red");
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onChange"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should not throw when select causes navigation")]
        [PuppeteerFact]
        public async Task ShouldNotThrowWhenSelectCausesNavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.QuerySelectorAsync("select").EvaluateFunctionAsync("select => select.addEventListener('input', () => window.location = '/empty.html')");
            await Task.WhenAll(
              Page.SelectAsync("select", "blue"),
              Page.WaitForNavigationAsync()
            );
            Assert.Contains("empty.html", Page.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should select multiple options")]
        [PuppeteerFact]
        public async Task ShouldSelectMultipleOptions()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.EvaluateExpressionAsync("makeMultiple()");
            await Page.SelectAsync("select", "blue", "green", "red");
            Assert.Equal(new string[] { "blue", "green", "red" },
                         await Page.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.Equal(new string[] { "blue", "green", "red" },
                         await Page.EvaluateExpressionAsync<string[]>("result.onChange"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should respect event bubbling")]
        [PuppeteerFact]
        public async Task ShouldRespectEventBubbling()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.SelectAsync("select", "blue");
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onBubblingInput"));
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onBubblingChange"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should throw when element is not a <select>")]
        [PuppeteerFact]
        public async Task ShouldThrowWhenElementIsNotASelect()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(async () => await Page.SelectAsync("body", ""));
            Assert.Contains("Element is not a <select> element.", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should return [] on no matched values")]
        [PuppeteerFact]
        public async Task ShouldReturnEmptyArrayOnNoMatchedValues()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var result = await Page.SelectAsync("select", "42", "abc");
            Assert.Empty(result);
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should return an array of matched values")]
        [PuppeteerFact]
        public async Task ShouldReturnAnArrayOfMatchedValues()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.EvaluateExpressionAsync("makeMultiple()");
            var result = await Page.SelectAsync("select", "blue", "black", "magenta");
            Array.Sort(result);
            Assert.Equal(new string[] { "black", "blue", "magenta" }, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should return an array of one element when multiple is not set")]
        [PuppeteerFact]
        public async Task ShouldReturnAnArrayOfOneElementWhenMultipleIsNotSet()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            Assert.Single(await Page.SelectAsync("select", "42", "blue", "black", "magenta"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should return [] on no values")]
        [PuppeteerFact]
        public async Task ShouldReturnEmptyArrayOnNoValues()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            Assert.Empty(await Page.SelectAsync("select"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should deselect all options when passed no values for a multiple select")]
        [PuppeteerFact]
        public async Task ShouldDeselectAllOptionsWhenPassedNoValuesForAMultipleSelect()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.EvaluateExpressionAsync("makeMultiple()");
            await Page.SelectAsync("select", "blue", "black", "magenta");
            await Page.SelectAsync("select");
            Assert.True(await Page.QuerySelectorAsync("select").EvaluateFunctionAsync<bool>(
                "select => Array.from(select.options).every(option => !option.selected)"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should deselect all options when passed no values for a select without multiple")]
        [PuppeteerFact]
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
