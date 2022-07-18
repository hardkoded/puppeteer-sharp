using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using CefSharp.DevTools.Dom;
using CefSharp.DevTools.Dom.Helpers;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.WaitTaskTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FrameWaitForXPathTests : DevToolsContextBaseTest
    {
        const string addElement = "tag => document.body.appendChild(document.createElement(tag))";

        public FrameWaitForXPathTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should support some fancy xpath")]
        [PuppeteerFact]
        public async Task ShouldSupportSomeFancyXpath()
        {
            await DevToolsContext.SetContentAsync("<p>red herring</p><p>hello  world  </p>");
            var waitForXPath = DevToolsContext.WaitForXPathAsync("//p[normalize-space(.)=\"hello world\"]");
            Assert.Equal("hello  world  ", await DevToolsContext.EvaluateFunctionAsync<string>("x => x.textContent", await waitForXPath));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should run in specified frame")]
        [PuppeteerFact(Skip = "BUG: OOPIFs aren't working correct")]
        public async Task ShouldRunInSpecifiedFrame()
        {
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame2", TestConstants.EmptyPage);
            var frame1 = DevToolsContext.Frames.First(f => f.Name == "frame1");
            var frame2 = DevToolsContext.Frames.First(f => f.Name == "frame2");
            var waitForXPathPromise = frame2.WaitForXPathAsync("//div");
            await frame1.EvaluateFunctionAsync(addElement, "div");
            await frame2.EvaluateFunctionAsync(addElement, "div");
            var eHandle = await waitForXPathPromise;
            Assert.Equal(frame2, eHandle.ExecutionContext.Frame);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should throw when frame is detached")]
        [PuppeteerFact(Skip = "BUG: OOPIFs aren't working correct")]
        public async Task ShouldThrowWhenFrameIsDetached()
        {
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            var frame = DevToolsContext.FirstChildFrame();
            var waitPromise = frame.WaitForXPathAsync("//*[@class=\"box\"]");
            await FrameUtils.DetachFrameAsync(DevToolsContext, "frame1");
            var exception = await Assert.ThrowsAnyAsync<Exception>(() => waitPromise);
            Assert.Contains("waitForFunction failed: frame got detached.", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "hidden should wait for display: none")]
        [PuppeteerFact]
        public async Task HiddenShouldWaitForDisplayNone()
        {
            var divHidden = false;
            await DevToolsContext.SetContentAsync("<div style='display: block;'></div>");
            var waitForXPath = DevToolsContext.WaitForXPathAsync("//div", new WaitForSelectorOptions { Hidden = true })
                .ContinueWith(_ => divHidden = true);
            await DevToolsContext.WaitForXPathAsync("//div"); // do a round trip
            Assert.False(divHidden);
            await DevToolsContext.EvaluateExpressionAsync("document.querySelector('div').style.setProperty('display', 'none')");
            Assert.True(await waitForXPath.WithTimeout());
            Assert.True(divHidden);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should return the element handle")]
        [PuppeteerFact]
        public async Task ShouldReturnTheElementHandle()
        {
            var waitForXPath = DevToolsContext.WaitForXPathAsync("//*[@class=\"zombo\"]");
            await DevToolsContext.SetContentAsync("<div class='zombo'>anything</div>");
            Assert.Equal("anything", await DevToolsContext.EvaluateFunctionAsync<string>("x => x.textContent", await waitForXPath));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should allow you to select a text node")]
        [PuppeteerFact]
        public async Task ShouldAllowYouToSelectATextNode()
        {
            await DevToolsContext.SetContentAsync("<div>some text</div>");
            var text = await DevToolsContext.WaitForXPathAsync("//div/text()");
            Assert.Equal(3 /* Node.TEXT_NODE */, await (await text.GetPropertyAsync("nodeType")).JsonValueAsync<int>());
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should allow you to select an element with single slash")]
        [PuppeteerFact]
        public async Task ShouldAllowYouToSelectAnElementWithSingleSlash()
        {
            await DevToolsContext.SetContentAsync("<div>some text</div>");
            var waitForXPath = DevToolsContext.WaitForXPathAsync("/html/body/div");
            Assert.Equal("some text", await DevToolsContext.EvaluateFunctionAsync<string>("x => x.textContent", await waitForXPath));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should respect timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectTimeout()
        {
            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(()
                    => DevToolsContext.WaitForXPathAsync("//div", new WaitForSelectorOptions { Timeout = 10 }));

            Assert.Contains("waiting for XPath '//div' failed: timeout", exception.Message);
        }
    }
}
