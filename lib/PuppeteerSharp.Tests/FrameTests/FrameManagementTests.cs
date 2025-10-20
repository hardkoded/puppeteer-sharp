using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class FrameManagementTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should handle nested frames")]
        public async Task ShouldHandleNestedFrames()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            Assert.That(await FrameUtils.DumpFramesAsync(Page.MainFrame),
                Is.EqualTo(TestConstants.NestedFramesDumpResult));
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should send events when frames are manipulated dynamically")]
        public async Task ShouldSendEventsWhenFramesAreManipulatedDynamically()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            // validate frame attached events
            var attachedFrames = new List<IFrame>();

            Page.FrameAttached += (_, e) => attachedFrames.Add(e.Frame);

            await FrameUtils.AttachFrameAsync(Page, "frame1", "./Assets/frame.html");

            Assert.That(attachedFrames, Has.Exactly(1).Items);
            Assert.That(attachedFrames[0].Url, Does.Contain("/Assets/frame.html"));

            // validate frame navigated events
            var navigatedFrames = new List<IFrame>();
            Page.FrameNavigated += (_, e) => navigatedFrames.Add(e.Frame);

            await FrameUtils.NavigateFrameAsync(Page, "frame1", "./empty.html");
            Assert.That(navigatedFrames, Has.Exactly(1).Items);
            Assert.That(navigatedFrames[0].Url, Is.EqualTo(TestConstants.EmptyPage));

            // validate frame detached events
            var detachedFrames = new List<IFrame>();
            Page.FrameDetached += (_, e) => detachedFrames.Add(e.Frame);

            await FrameUtils.DetachFrameAsync(Page, "frame1");
            Assert.That(detachedFrames, Has.Exactly(1).Items);
            Assert.That(detachedFrames[0].Detached, Is.True);
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should send \"framenavigated\" when navigating on anchor URLs")]
        public async Task ShouldSendFrameNavigatedWhenNavigatingOnAnchorURLs()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameNavigated = new TaskCompletionSource<bool>();
            Page.FrameNavigated += (_, _) => frameNavigated.TrySetResult(true);
            await Task.WhenAll(
                Page.GoToAsync(TestConstants.EmptyPage + "#foo"),
                frameNavigated.Task
            );
            Assert.That(Page.Url, Is.EqualTo(TestConstants.EmptyPage + "#foo"));
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should support url fragment")]
        public async Task ShouldReturnUrlFragmentAsPartOfUrl()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame-url-fragment.html");
            Assert.That(Page.Frames, Has.Length.EqualTo(2));
            Assert.That(Page.FirstChildFrame().Url, Is.EqualTo(TestConstants.ServerUrl + "/frames/frame.html?param=value#fragment"));
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should persist mainFrame on cross-process navigation")]
        public async Task ShouldPersistMainFrameOnCrossProcessNavigation()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var mainFrame = Page.MainFrame;
            await Page.GoToAsync(TestConstants.CrossProcessUrl + "/empty.html");
            Assert.That(Page.MainFrame, Is.EqualTo(mainFrame));
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should not send attach/detach events for main frame")]
        public async Task ShouldNotSendAttachDetachEventsForMainFrame()
        {
            var hasEvents = false;
            Page.FrameAttached += (_, _) => hasEvents = true;
            Page.FrameDetached += (_, _) => hasEvents = true;

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(hasEvents, Is.False);
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should detach child frames on navigation")]
        public async Task ShouldDetachChildFramesOnNavigation()
        {
            var attachedFrames = new List<IFrame>();
            var detachedFrames = new List<IFrame>();
            var navigatedFrames = new List<IFrame>();

            Page.FrameAttached += (_, e) => attachedFrames.Add(e.Frame);
            Page.FrameDetached += (_, e) => detachedFrames.Add(e.Frame);
            Page.FrameNavigated += (_, e) => navigatedFrames.Add(e.Frame);

            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            Assert.That(attachedFrames, Has.Count.EqualTo(4));
            Assert.That(detachedFrames, Is.Empty);
            Assert.That(navigatedFrames, Has.Count.EqualTo(5));

            attachedFrames.Clear();
            detachedFrames.Clear();
            navigatedFrames.Clear();

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(attachedFrames, Is.Empty);
            Assert.That(detachedFrames, Has.Count.EqualTo(4));
            Assert.That(navigatedFrames, Has.Exactly(1).Items);
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should click elements in a frameset")]
        public async Task ShouldClickElementsInAFrameset()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/frameset.html");
            var frame = await Page.WaitForFrameAsync(frame => frame.Url.EndsWith("/frames/frame.html"));
            var div = await frame.WaitForSelectorAsync("div");
            Assert.That(div, Is.Not.Null);
            await div.ClickAsync();
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should report frame from-inside shadow DOM")]
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
            Assert.That(Page.Frames, Has.Length.EqualTo(2));
            Assert.That(Page.Frames.Where(frame => frame.Url == TestConstants.EmptyPage), Has.Exactly(1).Items);
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should report frame.parent()")]
        public async Task ShouldReportFrameParent()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.EmptyPage);

            Assert.That(Page.Frames.Where(frame => frame.ParentFrame == null), Has.Exactly(1).Items);
            Assert.That(Page.Frames.Count(f => f.ParentFrame == Page.MainFrame), Is.EqualTo(2));
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should report different frame instance when frame re-attaches")]
        public async Task ShouldReportDifferentFrameInstanceWhenFrameReAttaches()
        {
            var frame1 = await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"() => {
                window.frame = document.querySelector('#frame1');
                window.frame.remove();
            }");
            Assert.That(frame1.Detached, Is.True);
            var frame2Tcs = new TaskCompletionSource<IFrame>();
            Page.FrameAttached += (_, e) => frame2Tcs.TrySetResult(e.Frame);
            await Page.EvaluateExpressionAsync("document.body.appendChild(window.frame)");
            var frame2 = await frame2Tcs.Task;
            Assert.That(frame2.Detached, Is.False);
            Assert.That(frame2, Is.Not.SameAs(frame1));
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame Management", "should support framesets")]
        public async Task ShouldSupportFramesets()
        {
            var attachedFrames = new List<IFrame>();
            var detachedFrames = new List<IFrame>();
            var navigatedFrames = new List<IFrame>();

            Page.FrameAttached += (_, e) => attachedFrames.Add(e.Frame);
            Page.FrameDetached += (_, e) => detachedFrames.Add(e.Frame);
            Page.FrameNavigated += (_, e) => navigatedFrames.Add(e.Frame);

            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/frameset.html");

            Assert.That(attachedFrames, Has.Count.EqualTo(4));
            Assert.That(detachedFrames, Is.Empty);
            Assert.That(navigatedFrames, Has.Count.EqualTo(5));

            attachedFrames.Clear();
            detachedFrames.Clear();
            navigatedFrames.Clear();

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(attachedFrames, Is.Empty);
            Assert.That(detachedFrames, Has.Count.EqualTo(4));
            Assert.That(navigatedFrames, Has.Exactly(1).Items);
        }
    }
}
