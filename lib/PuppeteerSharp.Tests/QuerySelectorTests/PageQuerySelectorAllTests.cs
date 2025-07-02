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

        [Test, PuppeteerTest("queryselector.spec", "Page.$$", "should query existing elements")]
        public async Task ShouldQueryExistingElements()
        {
            await Page.SetContentAsync("<div>A</div><br/><div>B</div>");
            var elements = await Page.QuerySelectorAllAsync("div");
            Assert.That(elements, Has.Length.EqualTo(2));
            var tasks = elements.Select(element => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.That(await Task.WhenAll(tasks), Is.EqualTo(new[] { "A", "B" }));
        }

        [Test, PuppeteerTest("queryselector.spec", "Page.$$", "should return empty array if nothing is found")]
        public async Task ShouldReturnEmptyArrayIfNothingIsFound()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var elements = await Page.QuerySelectorAllAsync("div");
            Assert.That(elements, Is.Empty);
        }
    }
}
