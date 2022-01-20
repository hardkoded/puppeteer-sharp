using System.Threading.Tasks;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AsElementTests : PuppeteerPageBaseTest
    {
        public AsElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.asElement", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync("document.body");
            var element = aHandle as ElementHandle;
            Assert.NotNull(element);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.asElement", "should return null for non-elements")]
        [PuppeteerFact]
        public async Task ShouldReturnNullForNonElements()
        {
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync("2");
            var element = aHandle as ElementHandle;
            Assert.Null(element);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.asElement", "should return ElementHandle for TextNodes")]
        [PuppeteerFact]
        public async Task ShouldReturnElementHandleForTextNodes()
        {
            await DevToolsContext.SetContentAsync("<div>ee!</div>");
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync("document.querySelector('div').firstChild");
            var element = aHandle as ElementHandle;
            Assert.NotNull(element);
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("e => e.nodeType === HTMLElement.TEXT_NODE", element));
        }
    }
}
