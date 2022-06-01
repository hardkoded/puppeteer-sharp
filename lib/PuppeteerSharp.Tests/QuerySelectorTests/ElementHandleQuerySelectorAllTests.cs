using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ElementHandleQuerySelectorAllTests : DevToolsContextBaseTest
    {
        public ElementHandleQuerySelectorAllTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$$", "should query existing elements")]
        [PuppeteerFact]
        public async Task ShouldQueryExistingElements()
        {
            await DevToolsContext.SetContentAsync("<html><body><div>A</div><br/><div>B</div></body></html>");
            var html = await DevToolsContext.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("div");
            Assert.Equal(2, elements.Length);
            var tasks = elements.Select(element => DevToolsContext.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.Equal(new[] { "A", "B" }, await Task.WhenAll(tasks));
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$$", "should return empty array for non-existing elements")]
        [PuppeteerFact]
        public async Task ShouldReturnEmptyArrayForNonExistingElements()
        {
            await DevToolsContext.SetContentAsync("<html><body><span>A</span><br/><span>B</span></body></html>");
            var html = await DevToolsContext.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("div");
            Assert.Empty(elements);
        }
    }
}