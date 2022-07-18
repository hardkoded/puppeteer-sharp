using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using Xunit.Abstractions;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FrameManagementTests : DevToolsContextBaseTest
    {
        public FrameManagementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should handle nested frames")]
        [PuppeteerFact]
        public async Task ShouldHandleNestedFrames()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            Assert.Equal(
                TestConstants.NestedFramesDumpResult,
                FrameUtils.DumpFrames(DevToolsContext.MainFrame));
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should send events when frames are manipulated dynamically")]
        [PuppeteerFact]
        public async Task ShouldSendEventsWhenFramesAreManipulatedDynamically()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            // validate frameattached events
            var attachedFrames = new List<Frame>();

            DevToolsContext.FrameAttached += (_, e) => attachedFrames.Add(e.Frame);

            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", "./Assets/frame.html");

            Assert.Single(attachedFrames);
            Assert.Contains("/Assets/frame.html", attachedFrames[0].Url);

            // validate framenavigated events
            var navigatedFrames = new List<Frame>();
            DevToolsContext.FrameNavigated += (_, e) => navigatedFrames.Add(e.Frame);

            await FrameUtils.NavigateFrameAsync(DevToolsContext, "frame1", "./empty.html");
            Assert.Single(navigatedFrames);
            Assert.Equal(TestConstants.EmptyPage, navigatedFrames[0].Url);

            // validate framedetached events
            var detachedFrames = new List<Frame>();
            DevToolsContext.FrameDetached += (_, e) => detachedFrames.Add(e.Frame);

            await FrameUtils.DetachFrameAsync(DevToolsContext, "frame1");
            Assert.Single(navigatedFrames);
            Assert.True(navigatedFrames[0].Detached);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should send \"framenavigated\" when navigating on anchor URLs")]
        [PuppeteerFact]
        public async Task ShouldSendFrameNavigatedWhenNavigatingOnAnchorURLs()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var frameNavigated = new TaskCompletionSource<bool>();
            DevToolsContext.FrameNavigated += (_, _) => frameNavigated.TrySetResult(true);
            await Task.WhenAll(
                DevToolsContext.GoToAsync(TestConstants.EmptyPage + "#foo"),
                frameNavigated.Task
            );
            Assert.Equal(TestConstants.EmptyPage + "#foo", DevToolsContext.Url);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should support url fragment")]
        [PuppeteerFact]
        public async Task ShouldReturnUrlFragmentAsPartOfUrl()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame-url-fragment.html");
            Assert.Equal(2, DevToolsContext.Frames.Length);
            Assert.Equal(TestConstants.ServerUrl + "/frames/frame.html?param=value#fragment", DevToolsContext.FirstChildFrame().Url);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should persist mainFrame on cross-process navigation")]
        [PuppeteerFact]
        public async Task ShouldPersistMainFrameOnCrossProcessNavigation()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var mainFrame = DevToolsContext.MainFrame;
            await DevToolsContext.GoToAsync(TestConstants.CrossProcessUrl + "/empty.html");
            Assert.Equal(mainFrame, DevToolsContext.MainFrame);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should not send attach/detach events for main frame")]
        [PuppeteerFact]
        public async Task ShouldNotSendAttachDetachEventsForMainFrame()
        {
            var hasEvents = false;
            DevToolsContext.FrameAttached += (_, _) => hasEvents = true;
            DevToolsContext.FrameDetached += (_, _) => hasEvents = true;

            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.False(hasEvents);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should detach child frames on navigation")]
        [PuppeteerFact]
        public async Task ShouldDetachChildFramesOnNavigation()
        {
            var attachedFrames = new List<Frame>();
            var detachedFrames = new List<Frame>();
            var navigatedFrames = new List<Frame>();

            DevToolsContext.FrameAttached += (_, e) => attachedFrames.Add(e.Frame);
            DevToolsContext.FrameDetached += (_, e) => detachedFrames.Add(e.Frame);
            DevToolsContext.FrameNavigated += (_, e) => navigatedFrames.Add(e.Frame);

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            Assert.Equal(4, attachedFrames.Count);
            Assert.Empty(detachedFrames);
            Assert.Equal(5, navigatedFrames.Count);

            attachedFrames.Clear();
            detachedFrames.Clear();
            navigatedFrames.Clear();

            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Empty(attachedFrames);
            Assert.Equal(4, detachedFrames.Count);
            Assert.Single(navigatedFrames);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should report frame from-inside shadow DOM")]
        [PuppeteerFact]
        public async Task ShouldReportFrameFromInsideShadowDOM()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/shadow.html");
            await DevToolsContext.EvaluateFunctionAsync(@"async url =>
            {
                const frame = document.createElement('iframe');
                frame.src = url;
                document.body.shadowRoot.appendChild(frame);
                await new Promise(x => frame.onload = x);
            }", TestConstants.EmptyPage);
            Assert.Equal(2, DevToolsContext.Frames.Length);
            Assert.Single(DevToolsContext.Frames, frame => frame.Url == TestConstants.EmptyPage);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should report frame.name()")]
        [PuppeteerFact(Skip = "BUG: OOPIFs aren't working correct")]
        public async Task ShouldReportFrameName()
        {
            await FrameUtils.AttachFrameAsync(DevToolsContext, "theFrameId", TestConstants.EmptyPage);
            await DevToolsContext.EvaluateFunctionAsync(@"url => {
                const frame = document.createElement('iframe');
                frame.name = 'theFrameName';
                frame.src = url;
                document.body.appendChild(frame);
                return new Promise(x => frame.onload = x);
            }", TestConstants.EmptyPage);

            Assert.Single(DevToolsContext.Frames, frame => frame.Name == string.Empty);
            Assert.Single(DevToolsContext.Frames, frame => frame.Name == "theFrameId");
            Assert.Single(DevToolsContext.Frames, frame => frame.Name == "theFrameName");
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should report frame.name()")]
        [PuppeteerFact]
        public async Task ShouldReportFrameParent()
        {
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame2", TestConstants.EmptyPage);

            Assert.Single(DevToolsContext.Frames, frame => frame.ParentFrame == null);
            Assert.Equal(2, DevToolsContext.Frames.Count(f => f.ParentFrame == DevToolsContext.MainFrame));
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should report different frame instance when frame re-attaches")]
        [PuppeteerFact(Skip = "BUG: OOPIFs aren't working correct")]
        public async Task ShouldReportDifferentFrameInstanceWhenFrameReAttaches()
        {
            var frame1 = await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            await DevToolsContext.EvaluateFunctionAsync(@"() => {
                window.frame = document.querySelector('#frame1');
                window.frame.remove();
            }");
            Assert.True(frame1.Detached);
            var frame2tsc = new TaskCompletionSource<Frame>();
            DevToolsContext.FrameAttached += (_, e) => frame2tsc.TrySetResult(e.Frame);
            await DevToolsContext.EvaluateExpressionAsync("document.body.appendChild(window.frame)");
            var frame2 = await frame2tsc.Task;
            Assert.False(frame2.Detached);
            Assert.NotSame(frame1, frame2);
        }
    }
}
