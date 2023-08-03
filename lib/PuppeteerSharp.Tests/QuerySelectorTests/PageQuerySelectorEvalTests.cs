using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class PageQuerySelectorEvalTests : PuppeteerPageBaseTest
    {
        public PageQuerySelectorEvalTests(): base()
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$eval", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<section id='testAttribute'>43543</section>");
            var idAttribute = await Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("e => e.id");
            Assert.AreEqual("testAttribute", idAttribute);
        }

        [PuppeteerTimeout]
        public async Task ShouldWorkWithAwaitedElements()
        {
            await Page.SetContentAsync("<section id='testAttribute'>43543</section>");
            var section = await Page.QuerySelectorAsync("section");
            var idAttribute = await section.EvaluateFunctionAsync<string>("e => e.id");
            Assert.AreEqual("testAttribute", idAttribute);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$eval", "should accept arguments")]
        [PuppeteerTimeout]
        public async Task ShouldAcceptArguments()
        {
            await Page.SetContentAsync("<section>hello</section>");
            var text = await Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("(e, suffix) => e.textContent + suffix", " world!");
            Assert.AreEqual("hello world!", text);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$eval", "should accept ElementHandles as arguments")]
        [PuppeteerTimeout]
        public async Task ShouldAcceptElementHandlesAsArguments()
        {
            await Page.SetContentAsync("<section>hello</section><div> world</div>");
            var divHandle = await Page.QuerySelectorAsync("div");
            var text = await Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("(e, div) => e.textContent + div.textContent", divHandle);
            Assert.AreEqual("hello world", text);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$eval", "should throw error if no element is found")]
        [PuppeteerTimeout]
        public void ShouldThrowErrorIfNoElementIsFound()
        {
            var exception = Assert.ThrowsAsync<SelectorException>(()
                => Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("e => e.id"));
            StringAssert.Contains("failed to find element matching selector", exception.Message);
        }
    }
}
