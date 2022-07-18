using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlAnchorElementTests : DevToolsContextBaseTest
    {
        public HtmlAnchorElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlAnchorElement>("a");

            Assert.NotNull(element);
        }

        [PuppeteerFact]
        public async Task ShouldReturnNull()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlAnchorElement>("a");

            Assert.Null(element);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetDisabled()
        {
            const string expected = "_blank";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlAnchorElement>("a");

            await element.SetTargetAsync(expected);

            var actual = await element.GetTargetAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetHref()
        {
            const string expected = "https://microsoft.com/";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlAnchorElement>("a");

            await element.SetHrefAsync(expected);

            var actual = await element.GetHrefAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetType()
        {
            const string expected = "text/html";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlAnchorElement>("a");

            await element.SetTypeAsync(expected);

            var actual = await element.GetTypeAsync();

            Assert.Equal(expected, actual);
        }
    }
}
