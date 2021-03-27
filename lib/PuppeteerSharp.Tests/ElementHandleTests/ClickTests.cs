using System;
using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ClickTests : PuppeteerPageBaseTest
    {
        public ClickTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should work")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await button.ClickAsync();
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should work for Shadow DOM v1")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkForShadowDomV1()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/shadow.html");
            var buttonHandle = (ElementHandle)await Page.EvaluateExpressionHandleAsync("button");
            await buttonHandle.ClickAsync();
            Assert.True(await Page.EvaluateExpressionAsync<bool>("clicked"));
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should work for TextNodes")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkForTextNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var buttonTextNode = (ElementHandle)await Page.EvaluateExpressionHandleAsync(
                "document.querySelector('button').firstChild");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await buttonTextNode.ClickAsync());
            Assert.Equal("Node is not of type HTMLElement", exception.Message);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for detached nodes")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldThrowForDetachedNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.remove()", button);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is detached from document", exception.Message);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for hidden nodes")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldThrowForHiddenNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.style.display = 'none'", button);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is either not visible or not an HTMLElement", exception.Message);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for recursively hidden nodes")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldThrowForRecursivelyHiddenNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.parentElement.style.display = 'none'", button);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is either not visible or not an HTMLElement", exception.Message);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for <br> elements")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldThrowForBrElements()
        {
            await Page.SetContentAsync("hello<br>goodbye");
            var br = await Page.QuerySelectorAsync("br");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await br.ClickAsync());
            Assert.Equal("Node is either not visible or not an HTMLElement", exception.Message);
        }
    }
}
