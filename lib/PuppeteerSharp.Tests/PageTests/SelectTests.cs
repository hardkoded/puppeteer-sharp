using System;
using System.Threading.Tasks;
using CefSharp.Puppeteer;
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
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await DevToolsContext.SelectAsync("select", "blue");
            Assert.Equal(new string[] { "blue" }, await DevToolsContext.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.Equal(new string[] { "blue" }, await DevToolsContext.EvaluateExpressionAsync<string[]>("result.onChange"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should select only first option")]
        [PuppeteerFact]
        public async Task ShouldSelectOnlyFirstOption()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await DevToolsContext.SelectAsync("select", "blue", "green", "red");
            Assert.Equal(new string[] { "blue" }, await DevToolsContext.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.Equal(new string[] { "blue" }, await DevToolsContext.EvaluateExpressionAsync<string[]>("result.onChange"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should not throw when select causes navigation")]
        [PuppeteerFact]
        public async Task ShouldNotThrowWhenSelectCausesNavigation()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await DevToolsContext.QuerySelectorAsync("select").EvaluateFunctionAsync("select => select.addEventListener('input', () => window.location = '/empty.html')");
            await Task.WhenAll(
              DevToolsContext.SelectAsync("select", "blue"),
              DevToolsContext.WaitForNavigationAsync()
            );
            Assert.Contains("empty.html", DevToolsContext.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should select multiple options")]
        [PuppeteerFact]
        public async Task ShouldSelectMultipleOptions()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await DevToolsContext.EvaluateExpressionAsync("makeMultiple()");
            await DevToolsContext.SelectAsync("select", "blue", "green", "red");
            Assert.Equal(new string[] { "blue", "green", "red" },
                         await DevToolsContext.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.Equal(new string[] { "blue", "green", "red" },
                         await DevToolsContext.EvaluateExpressionAsync<string[]>("result.onChange"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should respect event bubbling")]
        [PuppeteerFact]
        public async Task ShouldRespectEventBubbling()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await DevToolsContext.SelectAsync("select", "blue");
            Assert.Equal(new string[] { "blue" }, await DevToolsContext.EvaluateExpressionAsync<string[]>("result.onBubblingInput"));
            Assert.Equal(new string[] { "blue" }, await DevToolsContext.EvaluateExpressionAsync<string[]>("result.onBubblingChange"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "throw when element is not a <select>")]
        [PuppeteerFact]
        public async Task ShouldThrowWhenElementIsNotASelect()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(async () => await DevToolsContext.SelectAsync("body", ""));
            Assert.Contains("Element is not a <select> element.", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should return [] on no matched values")]
        [PuppeteerFact]
        public async Task ShouldReturnEmptyArrayOnNoMatchedValues()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var result = await DevToolsContext.SelectAsync("select", "42", "abc");
            Assert.Empty(result);
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should return an array of matched values")]
        [PuppeteerFact]
        public async Task ShouldReturnAnArrayOfMatchedValues()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await DevToolsContext.EvaluateExpressionAsync("makeMultiple()");
            var result = await DevToolsContext.SelectAsync("select", "blue", "black", "magenta");
            Array.Sort(result);
            Assert.Equal(new string[] { "black", "blue", "magenta" }, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should return an array of one element when multiple is not set")]
        [PuppeteerFact]
        public async Task ShouldReturnAnArrayOfOneElementWhenMultipleIsNotSet()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            Assert.Single(await DevToolsContext.SelectAsync("select", "42", "blue", "black", "magenta"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should return [] on no values")]
        [PuppeteerFact]
        public async Task ShouldReturnEmptyArrayOnNoValues()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            Assert.Empty(await DevToolsContext.SelectAsync("select"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should deselect all options when passed no values for a multiple select")]
        [PuppeteerFact]
        public async Task ShouldDeselectAllOptionsWhenPassedNoValuesForAMultipleSelect()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await DevToolsContext.EvaluateExpressionAsync("makeMultiple()");
            await DevToolsContext.SelectAsync("select", "blue", "black", "magenta");
            await DevToolsContext.SelectAsync("select");
            Assert.True(await DevToolsContext.QuerySelectorAsync("select").EvaluateFunctionAsync<bool>(
                "select => Array.from(select.options).every(option => !option.selected)"));
        }

        [PuppeteerTest("page.spec.ts", "Page.select", "should deselect all options when passed no values for a select without multiple")]
        [PuppeteerFact]
        public async Task ShouldDeselectAllOptionsWhenPassedNoValuesForASelectWithoutMultiple()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await DevToolsContext.SelectAsync("select", "blue", "black", "magenta");
            await DevToolsContext.SelectAsync("select");
            Assert.True(await DevToolsContext.QuerySelectorAsync("select").EvaluateFunctionAsync<bool>(
                "select => Array.from(select.options).every(option => !option.selected)"));
        }
    }
}
