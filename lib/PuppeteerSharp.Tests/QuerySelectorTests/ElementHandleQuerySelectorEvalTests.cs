using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ElementHandleQuerySelectorEvalTests : PuppeteerPageBaseTest
    {
        public ElementHandleQuerySelectorEvalTests(ITestOutputHelper output) : base(output)
        {
        }

        async Task Usage(IWebBrowser chromiumWebBrowser)
        {
            #region Evaluate
            await using var page = await chromiumWebBrowser.CreateDevToolsContextAsync();
            var seven = await page.EvaluateExpressionAsync<int>("4 + 3");
            var someObject = await page.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
            Console.WriteLine(someObject.a);
            #endregion
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$eval", "should work")]
        [PuppeteerFact]
        public async Task QuerySelectorShouldWork()
        {
            await DevToolsContext.SetContentAsync("<html><body><div class='tweet'><div class='like'>100</div><div class='retweets'>10</div></div></body></html>");
            var tweet = await DevToolsContext.QuerySelectorAsync(".tweet");
            var content = await tweet.QuerySelectorAsync(".like")
                .EvaluateFunctionAsync<string>("node => node.innerText");
            Assert.Equal("100", content);
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$eval", "should retrieve content from subtree")]
        [PuppeteerFact]
        public async Task QuerySelectorShouldRetrieveContentFromSubtree()
        {
            var htmlContent = "<div class='a'>not-a-child-div</div><div id='myId'><div class='a'>a-child-div</div></div>";
            await DevToolsContext.SetContentAsync(htmlContent);
            var elementHandle = await DevToolsContext.QuerySelectorAsync("#myId");
            var content = await elementHandle.QuerySelectorAsync(".a")
                .EvaluateFunctionAsync<string>("node => node.innerText");
            Assert.Equal("a-child-div", content);
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$eval", "should throw in case of missing selector")]
        [PuppeteerFact]
        public async Task QuerySelectorShouldThrowInCaseOfMissingSelector()
        {
            var htmlContent = "<div class=\"a\">not-a-child-div</div><div id=\"myId\"></div>";
            await DevToolsContext.SetContentAsync(htmlContent);
            var elementHandle = await DevToolsContext.QuerySelectorAsync("#myId");
            var exception = await Assert.ThrowsAsync<SelectorException>(
                () => elementHandle.QuerySelectorAsync(".a").EvaluateFunctionAsync<string>("node => node.innerText")
            );
            Assert.Equal("Error: failed to find element matching selector", exception.Message);
        }
    }
}
