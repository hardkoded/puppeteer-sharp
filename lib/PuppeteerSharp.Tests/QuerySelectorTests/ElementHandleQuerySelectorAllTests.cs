using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class ElementHandleQuerySelectorAllTests : PuppeteerPageBaseTest
    {
        public ElementHandleQuerySelectorAllTests() : base()
        {
        }

        [Test, PuppeteerTest("queryselector.spec", "ElementHandle.$$", "should query existing elements")]
        public async Task ShouldQueryExistingElements()
        {
            await Page.SetContentAsync("<html><body><div>A</div><br/><div>B</div></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("div");
            Assert.That(elements, Has.Length.EqualTo(2));
            var tasks = elements.Select(element => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.That(await Task.WhenAll(tasks), Is.EqualTo(new[] { "A", "B" }));
        }

        [Test, PuppeteerTest("queryselector.spec", "ElementHandle.$$", "should return empty array for non-existing elements")]
        public async Task ShouldReturnEmptyArrayForNonExistingElements()
        {
            await Page.SetContentAsync("<html><body><span>A</span><br/><span>B</span></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("div");
            Assert.That(elements, Is.Empty);
        }
    }
}
