using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class ClickTests : PuppeteerPageBaseTest
    {
        public ClickTests() : base()
        {
        }

        [Test, PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await button.ClickAsync();
            Assert.AreEqual("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Test, PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should work for Shadow DOM v1")]
        [PuppeteerTimeout]
        public async Task ShouldWorkForShadowDomV1()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/shadow.html");
            var buttonHandle = (IElementHandle)await Page.EvaluateExpressionHandleAsync("button");
            await buttonHandle.ClickAsync();
            Assert.True(await Page.EvaluateExpressionAsync<bool>("clicked"));
        }

        [Test, PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should not work for TextNodes")]
        [PuppeteerTimeout]
        public async Task ShouldNotWorkForTextNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var buttonTextNode = (IElementHandle)await Page.EvaluateExpressionHandleAsync(
                "document.querySelector('button').firstChild");
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(async () => await buttonTextNode.ClickAsync());

            if (TestConstants.IsChrome)
            {
                Assert.That(exception.Message, Does.Contain("is not of type 'Element'"));
            }
            else
            {
                Assert.That(exception.Message, Does.Contain("implement interface Element"));
            }
        }

        [Test, PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for detached nodes")]
        [PuppeteerTimeout]
        public async Task ShouldThrowForDetachedNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.remove()", button);
            var exception = Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.AreEqual("Node is either not visible or not an HTMLElement", exception.Message);
        }

        [Test, PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for hidden nodes")]
        [PuppeteerTimeout]
        public async Task ShouldThrowForHiddenNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.style.display = 'none'", button);
            var exception = Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.AreEqual("Node is either not visible or not an HTMLElement", exception.Message);
        }

        [Test, PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for recursively hidden nodes")]
        [PuppeteerTimeout]
        public async Task ShouldThrowForRecursivelyHiddenNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.parentElement.style.display = 'none'", button);
            var exception = Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.AreEqual("Node is either not visible or not an HTMLElement", exception.Message);
        }

        [Test, PuppeteerTest("elementhandle.spec.ts", "ElementHandle.click", "should throw for <br> elements")]
        [PuppeteerTimeout]
        public async Task ShouldThrowForBrElements()
        {
            await Page.SetContentAsync("hello<br>goodbye");
            var br = await Page.QuerySelectorAsync("br");
            var exception = Assert.ThrowsAsync<PuppeteerException>(async () => await br.ClickAsync());
            Assert.AreEqual("Node is either not visible or not an HTMLElement", exception.Message);
        }
    }
}
