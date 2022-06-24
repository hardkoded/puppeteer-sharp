using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.OOPIFTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class OOPIFTests : PuppeteerPageBaseTest
    {
        public OOPIFTests(ITestOutputHelper output) : base(output)
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.Args = new[]
            {
                "--site-per-process",
                "--remote-debugging-port=21222",
                "--host-rules=\"MAP * 127.0.0.1\"",
            };
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should treat OOP iframes and normal iframes the same")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldTreatOopIframesAndNormalIframesTheSame()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameTask = Page.WaitForFrameAsync((frame) => frame.Url?.EndsWith("/empty.html") == true);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(
              Page,
              "frame2",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            );
            await frameTask.WithTimeout();
            Assert.Equal(2, Page.MainFrame.ChildFrames.Count);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should track navigations within OOP iframes")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldTrackNavigationsWithinOopIframes()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameTask = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame);
            await FrameUtils.AttachFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            );
            var frame = await frameTask.WithTimeout();
            Assert.Contains("/empty.html", frame.Url);
            await FrameUtils.NavigateFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/assets/frame.html"
            );
            Assert.Contains("/assets/frame.html", frame.Url);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should support OOP iframes becoming normal iframes again")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSupportOopIframesBecomingNormalIframesAgain()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameTask = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = await frameTask.WithTimeout();
            Assert.False(frame.IsOopFrame);
            var nav = frame.WaitForNavigationAsync();
            await FrameUtils.NavigateFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            );
            Assert.True(frame.IsOopFrame);
            await nav;
            nav = frame.WaitForNavigationAsync();
            await FrameUtils.NavigateFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await nav;
            Assert.False(frame.IsOopFrame);
            Assert.Equal(2, Page.Frames.Length);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should support frames within OOP frames")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSupportFramesWithinOopframes()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frame1Task = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame && frame.ParentFrame == Page.MainFrame);
            var frame2Task = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame && frame.ParentFrame != Page.MainFrame);

            await FrameUtils.AttachFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/frames/one-frame.html"
            );

            var frame1 = await frame1Task;
            var frame2 = await frame2Task;

            Assert.Contains("one-frame", await frame1.EvaluateExpressionAsync<string>("document.location.href"));
            Assert.Contains("frame.html", await frame2.EvaluateExpressionAsync<string>("document.location.href"));
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should support OOP iframes getting detached")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSupportOopIframesGettingDetached()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameTask = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage).WithTimeout();
            var frame = await frameTask.WithTimeout();
            Assert.False(frame.IsOopFrame);
            await FrameUtils.NavigateFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            ).WithTimeout();
            Assert.True(frame.IsOopFrame);
            var detachedTcs = new TaskCompletionSource<bool>();
            Page.FrameManager.FrameDetached += (sender, e) => detachedTcs.TrySetResult(true);
            await FrameUtils.DetachFrameAsync(Page, "frame1").WithTimeout();
            await detachedTcs.Task.WithTimeout();
            Assert.Single(Page.Frames);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should support wait for navigation for transitions from local to OOPIF")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSupportWaitForNavigationForTransitionsFromLocalToOopif()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameTask = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = await frameTask.WithTimeout();
            Assert.False(frame.IsOopFrame);
            var nav = frame.WaitForNavigationAsync();
            await FrameUtils.NavigateFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            );
            await nav;
            Assert.True(frame.IsOopFrame);
            await FrameUtils.DetachFrameAsync(Page, "frame1");
            Assert.Single(Page.Frames);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should keep track of a frames OOP state")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldKeepTrackOfAFramesOopState()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameTask = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame);
            await FrameUtils.AttachFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            );
            var frame = await frameTask.WithTimeout();
            Assert.Contains("/empty.html", frame.Url);
            await FrameUtils.NavigateFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            Assert.Equal(TestConstants.EmptyPage, frame.Url);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should support evaluating in oop iframes")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSupportEvaluatingInOopIframes()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameTask = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame);
            await FrameUtils.AttachFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            );
            var frame = await frameTask.WithTimeout();
            await frame.EvaluateFunctionAsync("() => _test = 'Test 123!'");
            var result = await frame.EvaluateFunctionAsync<string>("() => window._test");
            Assert.Equal("Test 123!", result);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should provide access to elements")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldProvideAccessToElements()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameTask = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame);
            await FrameUtils.AttachFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            );
            var frame = await frameTask.WithTimeout();
            await frame.EvaluateFunctionAsync(@"() => {
                const button = document.createElement('button');
                button.id = 'test-button';
                button.innerText = 'click';
                button.onclick = () => {
                    button.id = 'clicked';
                };
                document.body.appendChild(button);
            }");

            await Page.EvaluateFunctionAsync(@"() => {
                document.body.style.border = '150px solid black';
                document.body.style.margin = '250px';
                document.body.style.padding = '50px';
            }");
            await frame.WaitForSelectorAsync("#test-button", new WaitForSelectorOptions{ Visible = true });
            await frame.ClickAsync("#test-button");
            await frame.WaitForSelectorAsync("#clicked");
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should report oopif frames")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldReportOopifFrames()
        {
            await Page.GoToAsync($"http://mainframe:{TestConstants.Port}/main-frame.html");
            var frame = await Page.WaitForFrameAsync((frame) => frame.Url.EndsWith("inner-frame2.html"));
            Assert.Equal(2, Oopifs.Count());
            Assert.Equal(2, Page.Frames.Count(frame => frame.IsOopFrame));
            Assert.Equal(1, await frame.EvaluateFunctionAsync<int>("() => document.querySelectorAll('button').length"));
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should load oopif iframes with subresources and request interception")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldLoadOopifIframesWithSubresourcesAndRequestInterception()
        {
            var frameTask = Page.WaitForFrameAsync((frame) => frame.Url.EndsWith("/oopif.html"));
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += (sender, e) => _ = e.Request.ContinueAsync();

            await Page.GoToAsync(TestConstants.ServerUrl + "/dynamic-oopif.html");
            await frameTask.WithTimeout();
            Assert.Single(Oopifs);
        }

        private IEnumerable<Target> Oopifs => Context.Targets().Where(target => target.TargetInfo.Type == TargetType.iFrame);
    }
}
