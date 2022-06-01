using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageQuerySelectorAllTests : DevToolsContextBaseTest
    {
        public PageQuerySelectorAllTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$$", "should query existing elements")]
        [PuppeteerFact]
        public async Task ShouldQueryExistingElements()
        {
            await DevToolsContext.SetContentAsync("<div>A</div><br/><div>B</div>");
            var elements = await DevToolsContext.QuerySelectorAllAsync("div");
            Assert.Equal(2, elements.Length);
            var tasks = elements.Select(element => DevToolsContext.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.Equal(new[] { "A", "B" }, await Task.WhenAll(tasks));
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$$", "should return empty array if nothing is found")]
        [PuppeteerFact]
        public async Task ShouldReturnEmptyArrayIfNothingIsFound()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var elements = await DevToolsContext.QuerySelectorAllAsync("div");
            Assert.Empty(elements);
        }
    }
}
