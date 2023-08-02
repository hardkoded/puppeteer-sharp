using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class PageQuerySelectorAllTests : PuppeteerPageBaseTest
    {
        public PageQuerySelectorAllTests(): base()
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$$", "should query existing elements")]
        [PuppeteerFact]
        public async Task ShouldQueryExistingElements()
        {
            await Page.SetContentAsync("<div>A</div><br/><div>B</div>");
            var elements = await Page.QuerySelectorAllAsync("div");
            Assert.Equal(2, elements.Length);
            var tasks = elements.Select(element => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.Equal(new[] { "A", "B" }, await Task.WhenAll(tasks));
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$$", "should return empty array if nothing is found")]
        [PuppeteerFact]
        public async Task ShouldReturnEmptyArrayIfNothingIsFound()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var elements = await Page.QuerySelectorAllAsync("div");
            Assert.Empty(elements);
        }
    }
}
