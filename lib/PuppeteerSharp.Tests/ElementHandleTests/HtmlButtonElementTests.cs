using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlButtonElementTests : DevToolsContextBaseTest
    {
        public HtmlButtonElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");

            Assert.NotNull(button);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetDisabled()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");

            await button.SetDisabledAsync(true);

            var actual = await button.GetDisabledAsync();

            Assert.True(actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetName()
        {
            const string expected = "buttonName";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");

            await button.SetNameAsync(expected);

            var actual = await button.GetNameAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetType()
        {
            const HtmlButtonElementType expected = HtmlButtonElementType.Submit;

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");

            await button.SetTypeAsync(expected);

            var actual = await button.GetTypeAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetValue()
        {
            const string expected = "Test Button";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");

            await button.SetValueAsync(expected);

            var actual = await button.GetValueAsync();

            Assert.Equal(expected, actual);
        }
    }
}
