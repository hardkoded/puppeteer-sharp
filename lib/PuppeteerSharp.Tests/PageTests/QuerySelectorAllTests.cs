using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class QuerySelectorAllTests : PuppeteerPageBaseTest
    {
        public QuerySelectorAllTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldQueryExistingElements()
        {
            await Page.SetContentAsync("<div>A</div><br/><div>B</div>");
            var elements = await Page.QuerySelectorAllAsync("div");
            Assert.Equal(2, elements.Length);
            var tasks = elements.Select(element => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.Equal(new[] { "A", "B" }, await Task.WhenAll(tasks));
        }

        [Fact]
        public async Task ShouldReturnEmptyArrayIfNothingIsFound()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var elements = await Page.QuerySelectorAllAsync("div");
            Assert.Empty(elements);
        }
    }
}
