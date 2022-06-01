using System.Threading.Tasks;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ClickTests : DevToolsContextBaseTest
    {
        public ClickTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync("button");
            await button.ClickAsync();
            Assert.Equal("Clicked", await DevToolsContext.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should work for Shadow DOM v1")]
        [PuppeteerFact]
        public async Task ShouldWorkForShadowDomV1()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/shadow.html");
            var buttonHandle = (ElementHandle)await DevToolsContext.EvaluateExpressionHandleAsync("button");
            await buttonHandle.ClickAsync();
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("clicked"));
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should work for TextNodes")]
        [PuppeteerFact]
        public async Task ShouldWorkForTextNodes()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var buttonTextNode = (ElementHandle)await DevToolsContext.EvaluateExpressionHandleAsync(
                "document.querySelector('button').firstChild");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await buttonTextNode.ClickAsync());
            Assert.Equal("Node is not of type HTMLElement", exception.Message);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for detached nodes")]
        [PuppeteerFact]
        public async Task ShouldThrowForDetachedNodes()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync("button");
            await DevToolsContext.EvaluateFunctionAsync("button => button.remove()", button);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is detached from document", exception.Message);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for hidden nodes")]
        [PuppeteerFact]
        public async Task ShouldThrowForHiddenNodes()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync("button");
            await DevToolsContext.EvaluateFunctionAsync("button => button.style.display = 'none'", button);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is either not visible or not an HTMLElement", exception.Message);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for recursively hidden nodes")]
        [PuppeteerFact]
        public async Task ShouldThrowForRecursivelyHiddenNodes()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync("button");
            await DevToolsContext.EvaluateFunctionAsync("button => button.parentElement.style.display = 'none'", button);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is either not visible or not an HTMLElement", exception.Message);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for <br> elements")]
        [PuppeteerFact]
        public async Task ShouldThrowForBrElements()
        {
            await DevToolsContext.SetContentAsync("hello<br>goodbye");
            var br = await DevToolsContext.QuerySelectorAsync("br");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await br.ClickAsync());
            Assert.Equal("Node is either not visible or not an HTMLElement", exception.Message);
        }
    }
}
