using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class PageQuerySelectorAllWithoutIsolationTests : PuppeteerPageBaseTest
    {
        public PageQuerySelectorAllWithoutIsolationTests() : base()
        {
        }

        [Test, PuppeteerTest("queryselector.spec", "Page.$$", "should query existing elements without isolation")]
        public async Task ShouldQueryExistingElementsWithoutIsolation()
        {
            await Page.SetContentAsync(TestUtils.Html(@"<div>A</div>
          <br />
          <div>B</div>"));
            var elements = await Page.QuerySelectorAllAsync("div", new QueryOptions { Isolate = false });
            Assert.That(elements, Has.Length.EqualTo(2));
            var tasks = elements.Select(element => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.That(await Task.WhenAll(tasks), Is.EqualTo(new[] { "A", "B" }));
        }
    }
}
