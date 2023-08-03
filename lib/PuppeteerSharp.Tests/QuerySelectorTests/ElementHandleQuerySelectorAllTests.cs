using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class ElementHandleQuerySelectorAllTests : PuppeteerPageBaseTest
    {
        public ElementHandleQuerySelectorAllTests(): base()
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$$", "should query existing elements")]
        [PuppeteerTimeout]
        public async Task ShouldQueryExistingElements()
        {
            await Page.SetContentAsync("<html><body><div>A</div><br/><div>B</div></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("div");
            Assert.AreEqual(2, elements.Length);
            var tasks = elements.Select(element => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.AreEqual(new[] { "A", "B" }, await Task.WhenAll(tasks));
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$$", "should return empty array for non-existing elements")]
        [PuppeteerTimeout]
        public async Task ShouldReturnEmptyArrayForNonExistingElements()
        {
            await Page.SetContentAsync("<html><body><span>A</span><br/><span>B</span></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("div");
            Assert.IsEmpty(elements);
        }
    }
}