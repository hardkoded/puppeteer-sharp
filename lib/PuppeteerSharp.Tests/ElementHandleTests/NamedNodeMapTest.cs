using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class NamedNodeMapTest : DevToolsContextBaseTest
    {
        public NamedNodeMapTest(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/dataattributes.html");
            var namedNodeMap = await DevToolsContext.QuerySelectorAsync<HtmlElement>("h1").AndThen(x => x.GetAttributesAsync());

            Assert.NotNull(namedNodeMap);

            var data = await namedNodeMap.ToArrayAsync();

            Assert.NotNull(data);
            Assert.NotEmpty(data);

            Assert.Equal("data-testing", await data[0].GetNameAsync());
            Assert.Equal("Test1", await data[0].GetValueAsync());
        }
    }
}
