#pragma warning disable CS0618 // WaitForXPathAsync is obsolete but we test the funcionatlity anyway
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.WaitTaskTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FrameWaitForXPathTests : PuppeteerPageBaseTest
    {
        const string addElement = "tag => document.body.appendChild(document.createElement(tag))";

        public FrameWaitForXPathTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should support some fancy xpath")]
        [PuppeteerFact]
        public async Task ShouldSupportSomeFancyXpath()
        {
            await Page.SetContentAsync("<p>red herring</p><p>hello  world  </p>");
            var waitForXPath = Page.WaitForXPathAsync("//p[normalize-space(.)=\"hello world\"]");
            Assert.Equal("hello  world  ", await Page.EvaluateFunctionAsync<string>("x => x.textContent", await waitForXPath));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should run in specified frame")]
        [PuppeteerFact]
        public async Task ShouldRunInSpecifiedFrame()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.EmptyPage);
            var frame1 = Page.Frames.First(f => f.Name == "frame1");
            var frame2 = Page.Frames.First(f => f.Name == "frame2");
            var waitForXPathPromise = frame2.WaitForXPathAsync("//div");
            await frame1.EvaluateFunctionAsync(addElement, "div");
            await frame2.EvaluateFunctionAsync(addElement, "div");
            var eHandle = await waitForXPathPromise;
            Assert.Equal(frame2, eHandle.ExecutionContext.Frame);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should throw when frame is detached")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldThrowWhenFrameIsDetached()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = Page.FirstChildFrame();
            var waitPromise = frame.WaitForXPathAsync("//*[@class=\"box\"]");
            await FrameUtils.DetachFrameAsync(Page, "frame1");
            var exception = await Assert.ThrowsAnyAsync<Exception>(() => waitPromise);
            Assert.Contains("waitForFunction failed: frame got detached.", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "hidden should wait for display: none")]
        [PuppeteerFact]
        public async Task HiddenShouldWaitForDisplayNone()
        {
            var divHidden = false;
            await Page.SetContentAsync("<div style='display: block;'></div>");
            var waitForXPath = Page.WaitForXPathAsync("//div", new WaitForSelectorOptions { Hidden = true })
                .ContinueWith(_ => divHidden = true);
            await Page.WaitForXPathAsync("//div"); // do a round trip
            Assert.False(divHidden);
            await Page.EvaluateExpressionAsync("document.querySelector('div').style.setProperty('display', 'none')");
            Assert.True(await waitForXPath.WithTimeout());
            Assert.True(divHidden);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should return the element handle")]
        [PuppeteerFact]
        public async Task ShouldReturnTheElementHandle()
        {
            var waitForXPath = Page.WaitForXPathAsync("//*[@class=\"zombo\"]");
            await Page.SetContentAsync("<div class='zombo'>anything</div>");
            Assert.Equal("anything", await Page.EvaluateFunctionAsync<string>("x => x.textContent", await waitForXPath));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should allow you to select a text node")]
        [PuppeteerFact]
        public async Task ShouldAllowYouToSelectATextNode()
        {
            await Page.SetContentAsync("<div>some text</div>");
            var text = await Page.WaitForXPathAsync("//div/text()");
            Assert.Equal(3 /* Node.TEXT_NODE */, await (await text.GetPropertyAsync("nodeType")).JsonValueAsync<int>());
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should allow you to select an element with single slash")]
        [PuppeteerFact]
        public async Task ShouldAllowYouToSelectAnElementWithSingleSlash()
        {
            await Page.SetContentAsync("<div>some text</div>");
            var waitForXPath = Page.WaitForXPathAsync("/html/body/div");
            Assert.Equal("some text", await Page.EvaluateFunctionAsync<string>("x => x.textContent", await waitForXPath));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForXPath", "should respect timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectTimeout()
        {
            const int timeout = 10;

            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(()
                    => Page.WaitForXPathAsync("//div", new WaitForSelectorOptions { Timeout = timeout }));

            Assert.Contains($"Waiting failed: {timeout}ms exceeded", exception.Message);
        }
    }
}
#pragma warning restore CS0618