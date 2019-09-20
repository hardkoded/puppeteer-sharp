using System;
using System.Threading.Tasks;
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

        [Fact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await button.ClickAsync();
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Fact]
        public async Task ShouldWorkForShadowDomV1()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/shadow.html");
            var buttonHandle = (await Page.EvaluateExpressionHandleAsync("button")) as ElementHandle;
            await buttonHandle.ClickAsync();
            Assert.True(await Page.EvaluateExpressionAsync<bool>("clicked"));
        }

        [Fact]
        public async Task ShouldWorkForTextNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var buttonTextNode = (await Page.EvaluateExpressionHandleAsync(
                "document.querySelector('button').firstChild")) as ElementHandle;
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await buttonTextNode.ClickAsync());
            Assert.Equal("Node is not of type HTMLElement", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowForDetachedNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.remove()", button);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is detached from document", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowForHiddenNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.style.display = 'none'", button);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is either not visible or not an HTMLElement", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowForRecursivelyHiddenNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.parentElement.style.display = 'none'", button);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is either not visible or not an HTMLElement", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowForBrElements()
        {
            await Page.SetContentAsync("hello<br>goodbye");
            var br = await Page.QuerySelectorAsync("br");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await br.ClickAsync());
            Assert.Equal("Node is either not visible or not an HTMLElement", exception.Message);
        }
    }
}
