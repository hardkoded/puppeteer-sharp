using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class PageQuerySelectorAllTests : PuppeteerPageBaseTest
    {
        public PageQuerySelectorAllTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "Page.$$", "should query existing elements")]
        public async Task ShouldQueryExistingElements()
        {
            await Page.SetContentAsync("<div>A</div><br/><div>B</div>");
            var elements = await Page.QuerySelectorAllAsync("div");
            Assert.AreEqual(2, elements.Length);
            var tasks = elements.Select(element => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.AreEqual(new[] { "A", "B" }, await Task.WhenAll(tasks));
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "Page.$$", "should return empty array if nothing is found")]
        public async Task ShouldReturnEmptyArrayIfNothingIsFound()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var elements = await Page.QuerySelectorAllAsync("div");
            Assert.IsEmpty(elements);
        }
    }
}
