using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SelectTests : PuppeteerPageBaseTest
    {
        public SelectTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldSelectSingleOption()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.SelectAsync("select", "blue");
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onChange"));
        }

        [Fact]
        public async Task ShouldSelectOnlyFirstOption()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.SelectAsync("select", "blue", "green", "red");
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onChange"));
        }

        [Fact]
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

        [Fact]
        public async Task ShouldRespectEventBubbling()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.SelectAsync("select", "blue");
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onBubblingInput"));
            Assert.Equal(new string[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onBubblingChange"));
        }

        [Fact]
        public async Task ShouldThrowWhenElementIsNotASelect()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(async () => await Page.SelectAsync("body", ""));
            Assert.Contains("Element is not a <select> element.", exception.Message);
        }

        [Fact]
        public async Task ShouldReturnEmptyArrayOnNoMatchedValues()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var result = await Page.SelectAsync("select", "42", "abc");
            Assert.Empty(result);
        }

        [Fact]
        public async Task ShouldReturnAnArrayOfMatchedValues()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.EvaluateExpressionAsync("makeMultiple()");
            var result = await Page.SelectAsync("select", "blue", "black", "magenta");
            Array.Sort(result);
            Assert.Equal(new string[] { "black", "blue", "magenta" }, result);
        }

        [Fact]
        public async Task ShouldReturnAnArrayOfOneElementWhenMultipleIsNotSet()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            Assert.Single(await Page.SelectAsync("select", "42", "blue", "black", "magenta"));
        }

        [Fact]
        public async Task ShouldReturnEmptyArrayOnNoValues()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            Assert.Empty(await Page.SelectAsync("select"));
        }

        [Fact]
        public async Task ShouldDeselectAllOptionsWhenPassedNoValuesForAMultipleSelect()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.EvaluateExpressionAsync("makeMultiple()");
            await Page.SelectAsync("select", "blue", "black", "magenta");
            await Page.SelectAsync("select");
            Assert.True(await Page.QuerySelectorAsync("select").EvaluateFunctionAsync<bool>(
                "select => Array.from(select.options).every(option => !option.selected)"));
        }

        [Fact]
        public async Task ShouldDeselectAllOptionsWhenPassedNoValuesForASelectWithoutMultiple()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.SelectAsync("select", "blue", "black", "magenta");
            await Page.SelectAsync("select");
            Assert.True(await Page.QuerySelectorAsync("select").EvaluateFunctionAsync<bool>(
                "select => Array.from(select.options).every(option => !option.selected)"));
        }

        [Fact]
        public async Task ShouldWorkWhenRedefiningTopLevelEventClass()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            await Page.EvaluateFunctionAsync("() => window.Event = null");
            await Page.SelectAsync("select", "blue");
            Assert.Equal(new[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onInput"));
            Assert.Equal(new[] { "blue" }, await Page.EvaluateExpressionAsync<string[]>("result.onChange"));
        }
    }
}
