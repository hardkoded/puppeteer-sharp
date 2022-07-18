using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AsElementTests : DevToolsContextBaseTest
    {
        public AsElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.asElement", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var element = await DevToolsContext.EvaluateExpressionHandleAsync<HtmlElement>("document.body");
            
            Assert.NotNull(element);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.asElement", "should return null for non-elements")]
        [PuppeteerFact]
        public async Task ShouldReturnNullForNonElements()
        {
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync<HtmlElement>("2");

            Assert.Null(aHandle);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.asElement", "should return ElementHandle for TextNodes")]
        [PuppeteerFact]
        public async Task ShouldReturnNullForNull()
        {
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync<HtmlElement>("null");

            Assert.Null(aHandle);
        }

        [PuppeteerFact]
        public async Task ShouldReturnElementHandleForTextNodes()
        {
            await DevToolsContext.SetContentAsync("<div>ee!</div>");

            var element = await DevToolsContext.EvaluateExpressionHandleAsync<Text>("document.querySelector('div').firstChild");

            Assert.NotNull(element);

            var nodeType = await element.GetNodeTypeAsync();

            Assert.Equal(NodeType.Text, nodeType);
        }
    }
}
