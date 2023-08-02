using System;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class ElementHandleQuerySelectorAllEvalTests : PuppeteerPageBaseTest
    {
        public ElementHandleQuerySelectorAllEvalTests(): base()
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$$eval", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<html><body><div class='tweet'><div class='like'>100</div><div class='like'>10</div></div></body></html>");
            var tweet = await Page.QuerySelectorAsync(".tweet");
            var content = await tweet.QuerySelectorAllHandleAsync(".like")
                .EvaluateFunctionAsync<string[]>("nodes => nodes.map(n => n.innerText)");
            Assert.Equal(new[] { "100", "10" }, content);
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$$eval", "should retrieve content from subtree")]
        [PuppeteerFact]
        public async Task QuerySelectorAllShouldRetrieveContentFromSubtree()
        {
            var htmlContent = "<div class='a'>not-a-child-div</div><div id='myId'><div class='a'>a1-child-div</div><div class='a'>a2-child-div</div></div>";
            await Page.SetContentAsync(htmlContent);
            var elementHandle = await Page.QuerySelectorAsync("#myId");
            var content = await elementHandle.QuerySelectorAllHandleAsync(".a")
                .EvaluateFunctionAsync<string[]>("nodes => nodes.map(n => n.innerText)");
            Assert.Equal(new[] { "a1-child-div", "a2-child-div" }, content);
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$$eval", "should not throw in case of missing selector")]
        [PuppeteerFact]
        public async Task QuerySelectorAllShouldNotThrowInCaseOfMissingSelector()
        {
            var htmlContent = "<div class='a'>not-a-child-div</div><div id='myId'></div>";
            await Page.SetContentAsync(htmlContent);
            var elementHandle = await Page.QuerySelectorAsync("#myId");
            var nodesLength = await elementHandle.QuerySelectorAllHandleAsync(".a")
                .EvaluateFunctionAsync<int>("nodes => nodes.length");
            Assert.Equal(0, nodesLength);
        }
    }
}
