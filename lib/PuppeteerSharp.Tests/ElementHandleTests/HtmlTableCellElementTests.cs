using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlTableCellElementTests : DevToolsContextBaseTest
    {
        public HtmlTableCellElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableCellElement>("td");

            Assert.NotNull(element);
        }

        [PuppeteerFact]
        public async Task ShouldReturnNull()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableCellElement>("#table2 td");

            Assert.Null(element);
        }

        [PuppeteerFact]
        public async Task ShouldGetIndex()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableCellElement>("td");

            var index = await element.GetCellIndexAsync();

            Assert.True(index > -1);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetAbbr()
        {
            const string expected = "Testing";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableCellElement>("td");

            await element.SetAbbrAsync(expected);
            var actual = await element.GetAbbrAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldGetScope()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableCellElement>("td");

            var actual = await element.GetScopeAsync();

            Assert.Equal(string.Empty, actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetScope()
        {
            const string expected = "col";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableCellElement>("td");

            await element.SetScopeAsync(expected);
            var actual = await element.GetScopeAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetRowSpan()
        {
            const int expected = 3;

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableCellElement>("td");

            await element.SetRowSpanAsync(expected);
            var actual = await element.GetRowSpanAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetColSpan()
        {
            const int expected = 3;

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableCellElement>("td");

            await element.SetColSpanAsync(expected);
            var actual = await element.GetColSpanAsync();

            Assert.Equal(expected, actual);
        }
    }
}
