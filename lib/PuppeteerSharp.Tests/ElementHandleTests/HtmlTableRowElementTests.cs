using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlTableRowElementTests : DevToolsContextBaseTest
    {
        public HtmlTableRowElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableRowElement>("tr");

            Assert.NotNull(element);
        }

        [PuppeteerFact]
        public async Task ShouldReturnNull()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableRowElement>("#table2 tr");

            Assert.Null(element);
        }

        [PuppeteerFact]
        public async Task ShouldGetIndex()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            var rows = await element.GetRowsAsync();
            var row = await rows.ItemAsync(0);

            var index = await row.GetRowIndexAsync();

            Assert.True(index > 0);
        }

        [PuppeteerFact]
        public async Task ShouldGetSelectionIndex()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            var rows = await element.GetRowsAsync();
            var row = await rows.ItemAsync(0);

            var index = await row.GetSectionRowIndexAsync();

            Assert.Equal(0, index);
        }

        [PuppeteerFact]
        public async Task ShouldDeleteCell()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableRowElement>("tr");

            var cells = await element.GetCellsAsync();
            var expected = await cells.GetLengthAsync() - 1;

            await element.DeleteCellAsync(0);

            var actual = await cells.GetLengthAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldInsertThenDeleteCell()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlTableRowElement>("tr");

            var cell = await element.InsertCellAsync(-1, "Testing");

            var expected = await element.GetCellsAsync().GetLengthAsync() - 1;

            await element.DeleteCellAsync(cell);

            var actual = await element.GetCellsAsync().GetLengthAsync();

            Assert.Equal(expected, actual);
        }
    }
}
