using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class StringMapTests : DevToolsContextBaseTest
    {
        public StringMapTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/dataattributes.html");
            var dataSet = await DevToolsContext.QuerySelectorAsync<HtmlElement>("h1").AndThen(x => x.GetDatasetAsync());

            Assert.NotNull(dataSet);

            var data = await dataSet.ToArrayAsync();

            Assert.NotNull(data);
            Assert.NotEmpty(data);

            Assert.Equal("testing", data[0].Key);
            Assert.Equal("Test1", data[0].Value);
            Assert.Equal("extra", data[1].Key);
            Assert.Equal("Test2", data[1].Value);
        }
    }
}
