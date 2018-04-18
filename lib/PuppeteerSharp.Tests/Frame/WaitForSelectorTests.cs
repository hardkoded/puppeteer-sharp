using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Frame
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class WaitForSelectorTests : PuppeteerPageBaseTest
    {
        const string AddElement = "tag => document.body.appendChild(document.createElement(tag))";

        [Fact]
        public async Task ShouldImmediatelyResolveTaskIfNodeExists()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frame = Page.MainFrame;
            var added = false;
            await frame.WaitForSelectorAsync("*").ContinueWith(_ => added = true);
            Assert.True(added);

            added = false;
            await frame.EvaluateFunctionAsync(AddElement, "div");
            await frame.WaitForSelectorAsync("div").ContinueWith(_ => added = true);
            Assert.True(added);
        }

        [Fact]
        public async Task ShouldResolveTaskWhenNodeIsAdded()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frame = Page.MainFrame;
            var added = false;
            var watchdog = frame.WaitForSelectorAsync("div").ContinueWith(_ => added = true);
            // run nop function..
            await frame.EvaluateExpressionAsync("42");
            // .. to be sure that waitForSelector promise is not resolved yet.
            Assert.False(added);

            await frame.EvaluateFunctionAsync(AddElement, "br");
            Assert.False(added);

            await frame.EvaluateFunctionAsync(AddElement, "div");
            await watchdog;
            Assert.True(added);
        }

        [Fact]
        public async Task ShouldWorkWhenNodeIsAddedThroughInnerHTML()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var watchdog = Page.WaitForSelectorAsync("h3 div");
            await Page.EvaluateFunctionAsync(AddElement, "span");
            await Page.EvaluateExpressionAsync("document.querySelector('span').innerHTML = '<h3><div></div></h3>'");
            await watchdog;
        }

        [Fact]
        public async Task PageWaitForSelectorAsyncIsShortcutForMainFrame()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrame(Page, "frame1", TestConstants.EmptyPage);
            var otherFrame = Page.Frames.ElementAt(1);
            var added = false;
            var watchdog = Page.WaitForSelectorAsync("div").ContinueWith(_ => added = true);
            await otherFrame.EvaluateFunctionAsync(AddElement, "div");
            Assert.False(added);

            await Page.EvaluateFunctionAsync(AddElement, "div");
            Assert.True(added);

            await watchdog;
        }

        [Fact]
        public async Task ShouldRunInSpecifiedFrame()
        {
            await FrameUtils.AttachFrame(Page, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrame(Page, "frame2", TestConstants.EmptyPage);
            var frame1 = Page.Frames.ElementAt(1);
            var frame2 = Page.Frames.ElementAt(2);
            var added = false;
            var watchdog = frame2.WaitForSelectorAsync("div").ContinueWith(_ => added = true);
            Assert.False(added);

            await frame1.EvaluateFunctionAsync(AddElement, "div");
            Assert.False(added);

            await frame2.EvaluateFunctionAsync(AddElement, "div");
            Assert.True(added);

            await watchdog;
        }

        [Fact]
        public async Task ShouldThrowIfEvaluationFailed()
        {
            await Page.EvaluateOnNewDocumentAsync(@"function() {
                document.querySelector = null;
            }");
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAnyAsync<PuppeteerException>(() => Page.WaitForSelectorAsync("*"));
            Assert.Contains("document.querySelector is not a function", exception.Message);
        }

        [Fact(Skip = "FrameUtils.DetachFrame hangs :(")]
        public async Task ShouldThrowWhenFrameIsDetached()
        {
            await FrameUtils.AttachFrame(Page, "frame1", TestConstants.EmptyPage);
            var frame = Page.Frames.ElementAt(1);            
            var waitTask = frame.WaitForSelectorAsync(".box").ContinueWith(task => task?.Exception?.InnerException);
            await FrameUtils.DetachFrame(Page, "frame1");
            var waitException = await waitTask;
            Assert.NotNull(waitException);
            Assert.Contains("waitForSelector failed: frame got detached", waitException.Message);
        }
    }
}
