using System;
using System.Threading.Tasks;
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

        async Task Usage(Browser browser)
        {
            #region Evaluate
            await using var page = await browser.NewPageAsync();
            var seven = await page.EvaluateExpressionAsync<int>("4 + 3");
            var someObject = await page.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
            Console.WriteLine(someObject.a);
            #endregion
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$eval", "should work")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task QuerySelectorShouldWork()
        {
            await Page.SetContentAsync("<html><body><div class='tweet'><div class='like'>100</div><div class='retweets'>10</div></div></body></html>");
            var tweet = await Page.QuerySelectorAsync(".tweet");
            var content = await tweet.QuerySelectorAsync(".like")
                .EvaluateFunctionAsync<string>("node => node.innerText");
            Assert.Equal("100", content);
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$eval", "should retrieve content from subtree")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task QuerySelectorShouldRetrieveContentFromSubtree()
        {
            var htmlContent = "<div class='a'>not-a-child-div</div><div id='myId'><div class='a'>a-child-div</div></div>";
            await Page.SetContentAsync(htmlContent);
            var elementHandle = await Page.QuerySelectorAsync("#myId");
            var content = await elementHandle.QuerySelectorAsync(".a")
                .EvaluateFunctionAsync<string>("node => node.innerText");
            Assert.Equal("a-child-div", content);
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$eval", "should throw in case of missing selector")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task QuerySelectorShouldThrowInCaseOfMissingSelector()
        {
            var htmlContent = "<div class=\"a\">not-a-child-div</div><div id=\"myId\"></div>";
            await Page.SetContentAsync(htmlContent);
            var elementHandle = await Page.QuerySelectorAsync("#myId");
            var exception = await Assert.ThrowsAsync<SelectorException>(
                () => elementHandle.QuerySelectorAsync(".a").EvaluateFunctionAsync<string>("node => node.innerText")
            );
            Assert.Equal("Error: failed to find element matching selector", exception.Message);
        }
    }
}
