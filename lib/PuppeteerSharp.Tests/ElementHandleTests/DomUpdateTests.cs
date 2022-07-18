using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DomUpdateTests : DevToolsContextBaseTest
    {
        public DomUpdateTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            const string expected = "Testing123";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");

            var before = await button.GetTextContentAsync();

            Assert.Equal("Click target", before);

            await button.SetTextContentAsync(expected);

            var actual = await button.GetTextContentAsync();

            Assert.Equal(expected, actual);
        }
    }
}
