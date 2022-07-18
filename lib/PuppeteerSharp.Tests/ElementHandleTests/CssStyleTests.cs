using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CssStyleTests : DevToolsContextBaseTest
    {
        public CssStyleTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            Assert.NotNull(style);
        }

        [PuppeteerFact]
        public async Task ShouldGetPriorityFalse()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            var priority = await style.GetPropertyPriorityAsync("border");

            Assert.False(priority);
        }

        [PuppeteerFact]
        public async Task ShouldGetPriorityTrue()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            await style.SetPropertyAsync("border", "1px solid red", true);

            var val = await style.GetPropertyValueAsync<string>("border");

            var priority = await style.GetPropertyPriorityAsync("border");

            Assert.True(priority);
        }

        [PuppeteerFact]
        public async Task ShouldGetPropertyNameByIndex()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            await style.SetPropertyAsync("border", "1px solid red", true);

            var propertyName = await style.ItemAsync(0);

            Assert.Equal("border-top-width", propertyName);
        }

        [PuppeteerFact]
        public async Task ShouldRemovePropertyByName()
        {
            const string expectedValue = "1px";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            await style.SetPropertyAsync("border-top-width", expectedValue, true);
            
            var actualValue = await style.RemovePropertyAsync("border-top-width");

            Assert.Equal(expectedValue, actualValue);
        }
    }
}
