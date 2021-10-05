using System.Threading.Tasks;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageQuerySelectorAllEvalTests : PuppeteerPageBaseTest
    {
        public PageQuerySelectorAllEvalTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$$eval", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divsCount = await Page.QuerySelectorAllHandleAsync("div").EvaluateFunctionAsync<int>("divs => divs.length");
            Assert.Equal(3, divsCount);
        }

        [PuppeteerFact]
        public async Task ShouldWorkWithAwaitedElements()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divs = await Page.QuerySelectorAllHandleAsync("div");
            var divsCount = await divs.EvaluateFunctionAsync<int>("divs => divs.length");
            Assert.Equal(3, divsCount);
        }
    }
}
