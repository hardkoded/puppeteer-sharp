using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ElementHandleQuerySelectorEvalTests : DevToolsContextBaseTest
    {
        public ElementHandleQuerySelectorEvalTests(ITestOutputHelper output) : base(output)
        {
        }

        async Task Usage(IWebBrowser chromiumWebBrowser)
        {
            #region Evaluate
            await using var devtoolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();
            var seven = await devtoolsContext.EvaluateExpressionAsync<int>("4 + 3");
            var someObject = await devtoolsContext.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
            Console.WriteLine(someObject.a);
            #endregion
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$eval", "should work")]
        [PuppeteerFact]
        public async Task QuerySelectorShouldWork()
        {
            await DevToolsContext.SetContentAsync("<html><body><div class='tweet'><div class='like'>100</div><div class='retweets'>10</div></div></body></html>");
            var tweet = await DevToolsContext.QuerySelectorAsync<HtmlDivElement>(".tweet");
            var content = await tweet.QuerySelectorAsync(".like")
                .AndThen(x => x.EvaluateFunctionAsync<string>("node => node.innerText"));
            Assert.Equal("100", content);
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$eval", "should retrieve content from subtree")]
        [PuppeteerFact]
        public async Task QuerySelectorShouldRetrieveContentFromSubtree()
        {
            var htmlContent = "<div class='a'>not-a-child-div</div><div id='myId'><div class='a'>a-child-div</div></div>";
            await DevToolsContext.SetContentAsync(htmlContent);
            var elementHandle = await DevToolsContext.QuerySelectorAsync<HtmlDivElement>("#myId");
            var content = await elementHandle.QuerySelectorAsync<HtmlElement>(".a")
                .AndThen(x => x.EvaluateFunctionAsync<string>("node => node.innerText"));
            Assert.Equal("a-child-div", content);
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$eval", "should throw in case of missing selector")]
        [PuppeteerFact]
        public async Task QuerySelectorShouldThrowInCaseOfMissingSelector()
        {
            var htmlContent = "<div class=\"a\">not-a-child-div</div><div id=\"myId\"></div>";
            await DevToolsContext.SetContentAsync(htmlContent);
            var elementHandle = await DevToolsContext.QuerySelectorAsync<HtmlDivElement>("#myId");
            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                () => elementHandle.QuerySelectorAsync<HtmlElement>(".a")
                    .AndThen(x => x.GetInnerTextAsync())
            );
            Assert.Equal("Object reference not set to an instance of an object.", exception.Message);
        }
    }
}
