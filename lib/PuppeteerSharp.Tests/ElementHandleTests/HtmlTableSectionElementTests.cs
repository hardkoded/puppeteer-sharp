using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlTableSectionElementTests : DevToolsContextBaseTest
    {
        public HtmlTableSectionElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            Assert.NotNull(element);
        }

        [PuppeteerFact]
        public async Task ShouldReturnNull()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableElement>("#table2").GetBodyAsync();

            Assert.Null(element);
        }

        [PuppeteerFact]
        public async Task ShouldInsertRow()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            var row = await element.InsertRowAsync(-1);

            Assert.NotNull(row);
        }

        [PuppeteerFact]
        public async Task ShouldDeleteRow()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            var initialLength = await element.GetRowsAsync().GetLengthAsync();
            var expected = initialLength - 1;

            await element.DeleteRowAsync(0);

            var actual = await element.GetRowsAsync().GetLengthAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldInsertThenDeleteRow()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            var expected = await element.GetRowsAsync().GetLengthAsync();

            var row = await element.InsertRowAsync(-1);

            var rowIndex = await row.GetSectionRowIndexAsync();

            await element.DeleteRowAsync(rowIndex);

            var actual = await element.GetRowsAsync().GetLengthAsync();

            Assert.Equal(expected, actual);
        }
    }
}
