using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class FrameManagementTests : PuppeteerPageBaseTest
    {
        public FrameManagementTests(): base()
        {
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should handle nested frames")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldHandleNestedFrames()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            Assert.AreEqual(
                TestConstants.NestedFramesDumpResult,
                FrameUtils.DumpFrames(Page.MainFrame));
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should send events when frames are manipulated dynamically")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldSendEventsWhenFramesAreManipulatedDynamically()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            // validate frameattached events
            var attachedFrames = new List<IFrame>();

            Page.FrameAttached += (_, e) => attachedFrames.Add(e.Frame);

            await FrameUtils.AttachFrameAsync(Page, "frame1", "./Assets/frame.html");

            Assert.Single(attachedFrames);
            Assert.Contains("/Assets/frame.html", attachedFrames[0].Url);

            // validate framenavigated events
            var navigatedFrames = new List<IFrame>();
            Page.FrameNavigated += (_, e) => navigatedFrames.Add(e.Frame);

            await FrameUtils.NavigateFrameAsync(Page, "frame1", "./empty.html");
            Assert.Single(navigatedFrames);
            Assert.AreEqual(TestConstants.EmptyPage, navigatedFrames[0].Url);

            // validate framedetached events
            var detachedFrames = new List<IFrame>();
            Page.FrameDetached += (_, e) => detachedFrames.Add(e.Frame);

            await FrameUtils.DetachFrameAsync(Page, "frame1");
            Assert.Single(navigatedFrames);
            Assert.True(navigatedFrames[0].Detached);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should send \"framenavigated\" when navigating on anchor URLs")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldSendFrameNavigatedWhenNavigatingOnAnchorURLs()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameNavigated = new TaskCompletionSource<bool>();
            Page.FrameNavigated += (_, _) => frameNavigated.TrySetResult(true);
            await Task.WhenAll(
                Page.GoToAsync(TestConstants.EmptyPage + "#foo"),
                frameNavigated.Task
            );
            Assert.AreEqual(TestConstants.EmptyPage + "#foo", Page.Url);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should support url fragment")]
        [PuppeteerTimeout]
        public async Task ShouldReturnUrlFragmentAsPartOfUrl()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame-url-fragment.html");
            Assert.AreEqual(2, Page.Frames.Length);
            Assert.AreEqual(TestConstants.ServerUrl + "/frames/frame.html?param=value#fragment", Page.FirstChildFrame().Url);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should persist mainFrame on cross-process navigation")]
        [PuppeteerTimeout]
        public async Task ShouldPersistMainFrameOnCrossProcessNavigation()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var mainFrame = Page.MainFrame;
            await Page.GoToAsync(TestConstants.CrossProcessUrl + "/empty.html");
            Assert.AreEqual(mainFrame, Page.MainFrame);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should not send attach/detach events for main frame")]
        [PuppeteerTimeout]
        public async Task ShouldNotSendAttachDetachEventsForMainFrame()
        {
            var hasEvents = false;
            Page.FrameAttached += (_, _) => hasEvents = true;
            Page.FrameDetached += (_, _) => hasEvents = true;

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.False(hasEvents);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should detach child frames on navigation")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldDetachChildFramesOnNavigation()
        {
            var attachedFrames = new List<IFrame>();
            var detachedFrames = new List<IFrame>();
            var navigatedFrames = new List<IFrame>();

            Page.FrameAttached += (_, e) => attachedFrames.Add(e.Frame);
            Page.FrameDetached += (_, e) => detachedFrames.Add(e.Frame);
            Page.FrameNavigated += (_, e) => navigatedFrames.Add(e.Frame);

            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            Assert.AreEqual(4, attachedFrames.Count);
            Assert.Empty(detachedFrames);
            Assert.AreEqual(5, navigatedFrames.Count);

            attachedFrames.Clear();
            detachedFrames.Clear();
            navigatedFrames.Clear();

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Empty(attachedFrames);
            Assert.AreEqual(4, detachedFrames.Count);
            Assert.Single(navigatedFrames);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should report frame from-inside shadow DOM")]
        [PuppeteerTimeout]
        public async Task ShouldReportFrameFromInsideShadowDOM()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/shadow.html");
            await Page.EvaluateFunctionAsync(@"async url =>
            {
                const frame = document.createElement('iframe');
                frame.src = url;
                document.body.shadowRoot.appendChild(frame);
                await new Promise(x => frame.onload = x);
            }", TestConstants.EmptyPage);
            Assert.AreEqual(2, Page.Frames.Length);
            Assert.Single(Page.Frames, frame => frame.Url == TestConstants.EmptyPage);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should report frame.name()")]
        [PuppeteerTimeout]
        public async Task ShouldReportFrameName()
        {
            await FrameUtils.AttachFrameAsync(Page, "theFrameId", TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"url => {
                const frame = document.createElement('iframe');
                frame.name = 'theFrameName';
                frame.src = url;
                document.body.appendChild(frame);
                return new Promise(x => frame.onload = x);
            }", TestConstants.EmptyPage);

            Assert.Single(Page.Frames, frame => frame.Name == string.Empty);
            Assert.Single(Page.Frames, frame => frame.Name == "theFrameId");
            Assert.Single(Page.Frames, frame => frame.Name == "theFrameName");
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should report frame.parent()")]
        [PuppeteerTimeout]
        public async Task ShouldReportFrameParent()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.EmptyPage);

            Assert.Single(Page.Frames, frame => frame.ParentFrame == null);
            Assert.AreEqual(2, Page.Frames.Count(f => f.ParentFrame == Page.MainFrame));
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should report different frame instance when frame re-attaches")]
        [PuppeteerTimeout]
        public async Task ShouldReportDifferentFrameInstanceWhenFrameReAttaches()
        {
            var frame1 = await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"() => {
                window.frame = document.querySelector('#frame1');
                window.frame.remove();
            }");
            Assert.True(frame1.Detached);
            var frame2tsc = new TaskCompletionSource<IFrame>();
            Page.FrameAttached += (_, e) => frame2tsc.TrySetResult(e.Frame);
            await Page.EvaluateExpressionAsync("document.body.appendChild(window.frame)");
            var frame2 = await frame2tsc.Task;
            Assert.False(frame2.Detached);
            Assert.NotSame(frame1, frame2);
        }

        [PuppeteerTest("frame.spec.ts", "Frame Management", "should support framesets")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldSupportFramesets()
        {
            var attachedFrames = new List<IFrame>();
            var detachedFrames = new List<IFrame>();
            var navigatedFrames = new List<IFrame>();

            Page.FrameAttached += (_, e) => attachedFrames.Add(e.Frame);
            Page.FrameDetached += (_, e) => detachedFrames.Add(e.Frame);
            Page.FrameNavigated+= (_, e) => navigatedFrames.Add(e.Frame);

            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/frameset.html");

            Assert.AreEqual(4, attachedFrames.Count);
            Assert.Empty(detachedFrames);
            Assert.AreEqual(5, navigatedFrames.Count);

            attachedFrames.Clear();
            detachedFrames.Clear();
            navigatedFrames.Clear();

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Empty(attachedFrames);
            Assert.AreEqual(4, detachedFrames.Count);
            Assert.Single(navigatedFrames);
        }
    }
}
