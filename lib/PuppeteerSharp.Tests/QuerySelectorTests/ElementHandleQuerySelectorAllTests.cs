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

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "ElementHandle.$$", "should query existing elements")]
        public async Task ShouldQueryExistingElements()
        {
            await Page.SetContentAsync("<html><body><div>A</div><br/><div>B</div></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("div");
            Assert.AreEqual(2, elements.Length);
            var tasks = elements.Select(element => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.AreEqual(new[] { "A", "B" }, await Task.WhenAll(tasks));
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "ElementHandle.$$", "should return empty array for non-existing elements")]
        public async Task ShouldReturnEmptyArrayForNonExistingElements()
        {
            await Page.SetContentAsync("<html><body><span>A</span><br/><span>B</span></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("div");
            Assert.IsEmpty(elements);
        }
    }
}
