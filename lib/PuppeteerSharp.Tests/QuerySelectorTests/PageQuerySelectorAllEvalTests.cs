using System.Threading.Tasks;
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

        [PuppeteerTest("queryselector.spec.ts", "Page.$$eval", "should accept extra arguments")]
        [PuppeteerFact]
        public async Task ShouldAcceptExtraArguments()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divsCount = await Page
                .QuerySelectorAllHandleAsync("div")
                .EvaluateFunctionAsync<int>("(divs, two, three) => divs.length + two + three", 2, 3);
            Assert.Equal(8, divsCount);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$$eval", "should accept ElementHandles as arguments")]
        [PuppeteerFact]
        public async Task ShouldAcceptElementHandlesAsArguments()
        {
            await Page.SetContentAsync("<section>2</section><section>2</section><section>1</section><div>3</div>");
            var divHandle = await Page.QuerySelectorAsync("div");
            var divsCount = await Page
                .QuerySelectorAllHandleAsync("section")
                .EvaluateFunctionAsync<int>(@"(sections, div) =>
                sections.reduce(
                    (acc, section) => acc + Number(section.textContent),
                    0
                ) + Number(div.textContent)", divHandle);
            Assert.Equal(8, divsCount);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$$eval", "should handle many elements")]
        [PuppeteerFact]
        public async Task ShouldHandleManyElements()
        {
            await Page.EvaluateExpressionAsync(@"
                for (var i = 0; i <= 1000; i++) {
                    const section = document.createElement('section');
                    section.textContent = i;
                    document.body.appendChild(section);
                }
            ");

            var sum = await Page
                .QuerySelectorAllHandleAsync("section")
                .EvaluateFunctionAsync<int>(@"(sections, div) =>
                sections.reduce((acc, section) => acc + Number(section.textContent), 0)");
            Assert.Equal(500500, sum);
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
