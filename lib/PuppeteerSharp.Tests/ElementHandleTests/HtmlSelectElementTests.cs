using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlSelectElementTests : DevToolsContextBaseTest
    {
        public HtmlSelectElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlSelectElement>("select");

            Assert.NotNull(button);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetDisabled()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlSelectElement>("select");

            await element.SetDisabledAsync(true);

            var actual = await element.GetDisabledAsync();

            Assert.True(actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetThenGetName()
        {
            const string expected = "selectName";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlSelectElement>("select");

            await element.SetNameAsync(expected);

            var actual = await element.GetNameAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldGetTypeSingle()
        {
            const string expected = "select-one";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlSelectElement>("select");

            var actual = await element.GetTypeAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldGetTypeMultiple()
        {
            const string expected = "select-multiple";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlSelectElement>("select");

            await element.SetMultipleAsync(true);

            var actual = await element.GetTypeAsync();

            Assert.Equal(expected, actual);
        }


        [PuppeteerFact]
        public async Task ShouldSetThenGetValue()
        {
            const string expected = "indigo";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlSelectElement>("select");

            await element.SetValueAsync(expected);

            var actual = await element.GetValueAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldMoveOption()
        {
            const string expected = "green";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlSelectElement>("select");

            var itemSixth = await element.ItemAsync(5);

            await element.AddAsync(itemSixth, 0);

            var itemFirst = await element.ItemAsync(0);

            var actual = await itemFirst.GetValueAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldAddOption()
        {
            const string expected = "darkBlue";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlSelectElement>("select");

            var newOption = await element.AddAsync(expected, "Dark Blue");

            Assert.NotNull(newOption);

            var length = await element.GetLengthAsync();

            var lastElement = await element.ItemAsync(length - 1);

            var actual1 = await lastElement.GetValueAsync();
            var actual2 = await newOption.GetValueAsync();

            Assert.Equal(expected, actual1);
            Assert.Equal(expected, actual2);
        }

        [PuppeteerFact]
        public async Task ShouldRemoveLastOption()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlSelectElement>("select");

            var length = await element.GetLengthAsync();

            var expected = length - 1;

            await element.RemoveAsync(length - 1);

            var actual = await element.GetLengthAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldGetOptionByName()
        {
            const string expected = "red";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await DevToolsContext.QuerySelectorAsync<HtmlSelectElement>("select");

            var option = await element.NamedItemAsync(expected);

            var actual = await option.GetValueAsync();

            Assert.Equal(expected, actual);
        }
    }
}
