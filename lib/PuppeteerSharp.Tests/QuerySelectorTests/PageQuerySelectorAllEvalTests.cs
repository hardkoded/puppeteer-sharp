using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageQuerySelectorAllEvalTests : PuppeteerPageBaseTest
    {
        public PageQuerySelectorAllEvalTests(): base()
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$$eval", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divsCount = await Page.QuerySelectorAllHandleAsync("div").EvaluateFunctionAsync<int>("divs => divs.length");
            Assert.AreEqual(3, divsCount);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$$eval", "should accept extra arguments")]
        [PuppeteerTimeout]
        public async Task ShouldAcceptExtraArguments()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divsCount = await Page
                .QuerySelectorAllHandleAsync("div")
                .EvaluateFunctionAsync<int>("(divs, two, three) => divs.length + two + three", 2, 3);
            Assert.AreEqual(8, divsCount);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$$eval", "should accept ElementHandles as arguments")]
        [PuppeteerTimeout]
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
            Assert.AreEqual(8, divsCount);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$$eval", "should handle many elements")]
        [PuppeteerTimeout]
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
            Assert.AreEqual(500500, sum);
        }

        [PuppeteerTimeout]
        public async Task ShouldWorkWithAwaitedElements()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divs = await Page.QuerySelectorAllHandleAsync("div");
            var divsCount = await divs.EvaluateFunctionAsync<int>("divs => divs.length");
            Assert.AreEqual(3, divsCount);
        }
    }
}
