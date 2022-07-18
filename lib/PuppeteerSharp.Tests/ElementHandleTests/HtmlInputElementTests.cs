using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlInputElementTests : DevToolsContextBaseTest
    {
        public HtmlInputElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlInputElement>("#agree");

            Assert.NotNull(element);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetChecked()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlInputElement>("#agree");

            await element.SetCheckedAsync(true);

            var actual = await element.GetCheckedAsync();

            Assert.True(actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetIndeterminate()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlInputElement>("#agree");

            await element.SetIndeterminateAsync(true);

            var actual = await element.GetIndeterminateAsync();

            Assert.True(actual);
        }
    }
}
