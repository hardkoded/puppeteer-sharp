using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class ElementHandleQuerySelectorAllEvalTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("queryselector.spec", "ElementHandle.$$eval", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<html><body><div class='tweet'><div class='like'>100</div><div class='like'>10</div></div></body></html>");
            var tweet = await Page.QuerySelectorAsync(".tweet");
            var content = await tweet.QuerySelectorAllHandleAsync(".like")
                .EvaluateFunctionAsync<string[]>("nodes => nodes.map(n => n.innerText)");
            Assert.AreEqual(new[] { "100", "10" }, content);
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "ElementHandle.$$eval", "should retrieve content from subtree")]
        public async Task QuerySelectorAllShouldRetrieveContentFromSubtree()
        {
            var htmlContent = "<div class='a'>not-a-child-div</div><div id='myId'><div class='a'>a1-child-div</div><div class='a'>a2-child-div</div></div>";
            await Page.SetContentAsync(htmlContent);
            var elementHandle = await Page.QuerySelectorAsync("#myId");
            var content = await elementHandle.QuerySelectorAllHandleAsync(".a")
                .EvaluateFunctionAsync<string[]>("nodes => nodes.map(n => n.innerText)");
            Assert.AreEqual(new[] { "a1-child-div", "a2-child-div" }, content);
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "ElementHandle.$$eval", "should not throw in case of missing selector")]
        public async Task QuerySelectorAllShouldNotThrowInCaseOfMissingSelector()
        {
            var htmlContent = "<div class='a'>not-a-child-div</div><div id='myId'></div>";
            await Page.SetContentAsync(htmlContent);
            var elementHandle = await Page.QuerySelectorAsync("#myId");
            var nodesLength = await elementHandle.QuerySelectorAllHandleAsync(".a")
                .EvaluateFunctionAsync<int>("nodes => nodes.length");
            Assert.AreEqual(0, nodesLength);
        }
    }
}
