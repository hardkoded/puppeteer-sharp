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
        static int _port = 21221;

        public OOPIFTests(ITestOutputHelper output) : base(output)
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.Args = new[]
            {
                "--site-per-process",
                $"--remote-debugging-port={++_port}",
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
        [PuppeteerFact(Skip = "See why this is so brittle")]
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
            var nav = frame.WaitForNavigationAsync();
            await FrameUtils.NavigateFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/assets/frame.html"
            );
            await nav.WithTimeout();
            Assert.Contains("/assets/frame.html", frame.Url);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should support OOP iframes becoming normal iframes again")]
        [PuppeteerFact(Skip = "See why this is so brittle")]
        public async Task ShouldSupportOopIframesBecomingNormalIframesAgain()
        {
            await Page.GoToAsync(TestConstants.EmptyPage).WithTimeout();
            var frameTask = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage).WithTimeout();
            var frame = await frameTask.WithTimeout();
            Assert.False(frame.IsOopFrame);
            var nav = frame.WaitForNavigationAsync();
            await FrameUtils.NavigateFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            ).WithTimeout();
            Assert.True(frame.IsOopFrame);
            await nav.WithTimeout();
            nav = frame.WaitForNavigationAsync();
            await FrameUtils.NavigateFrameAsync(Page, "frame1", TestConstants.EmptyPage).WithTimeout();
            await nav.WithTimeout();
            Assert.False(frame.IsOopFrame);
            Assert.Equal(2, Page.Frames.Length);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should support frames within OOP frames")]
        [PuppeteerFact(Skip = "See why this is so brittle")]
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
        [PuppeteerFact(Skip = "See why this is so brittle")]
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
        [PuppeteerFact(Skip = "See why this is so brittle")]
        public async Task ShouldSupportWaitForNavigationForTransitionsFromLocalToOopif()
        {
            await Page.GoToAsync(TestConstants.EmptyPage).WithTimeout();
            var frameTask = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage).WithTimeout();
            var frame = await frameTask.WithTimeout();
            Assert.False(frame.IsOopFrame);
            var nav = frame.WaitForNavigationAsync();
            await FrameUtils.NavigateFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            ).WithTimeout();
            await nav.WithTimeout();
            Assert.True(frame.IsOopFrame);
            var detachedTcs = new TaskCompletionSource<bool>();
            Page.FrameManager.FrameDetached += (sender, e) => detachedTcs.TrySetResult(true);
            await FrameUtils.DetachFrameAsync(Page, "frame1").WithTimeout();
            await detachedTcs.Task.WithTimeout();
            Assert.Single(Page.Frames);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should keep track of a frames OOP state")]
        [PuppeteerFact(Skip = "See why this is so brittle")]
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
        [PuppeteerFact(Skip = "See why this is so brittle")]
        public async Task ShouldReportOopifFrames()
        {
            var frameTask = Page.WaitForFrameAsync((frame) => frame.Url.EndsWith("inner-frame2.html"));
            await Page.GoToAsync($"http://mainframe:{TestConstants.Port}/main-frame.html");
            var frame = await frameTask.WithTimeout();
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

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should support frames within OOP iframes")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSupportFramesWithinOopIframes()
        {
            var oopifFrameTask = Page.WaitForFrameAsync((frame) => frame.Url.EndsWith("/oopif.html"));
            await Page.GoToAsync(TestConstants.ServerUrl + "/dynamic-oopif.html");
            var oopIframe = await oopifFrameTask.WithTimeout();
            await FrameUtils.AttachFrameAsync(
              oopIframe,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            ).WithTimeout();
            var frame1 = oopIframe.ChildFrames[0];
            Assert.Contains("empty.html", frame1.Url);
            await FrameUtils.NavigateFrameAsync(
              oopIframe,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/oopif.html"
            ).WithTimeout();
            Assert.Contains("oopif.html", frame1.Url);
            await frame1.GoToAsync(
                TestConstants.CrossProcessHttpPrefix + "/oopif.html#navigate-within-document",
                new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Load } }).WithTimeout();
            Assert.Contains("/oopif.html#navigate-within-document", frame1.Url);
            var detachedTcs = new TaskCompletionSource<bool>();
            Page.FrameManager.FrameDetached += (sender, e) => detachedTcs.TrySetResult(true);
            await FrameUtils.DetachFrameAsync(oopIframe, "frame1").WithTimeout();
            await detachedTcs.Task.WithTimeout();
            Assert.Empty(oopIframe.ChildFrames);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "clickablePoint, boundingBox, boxModel should work for elements inside OOPIFs")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ClickablePointBoundingBoxBoxModelShouldWorkForElementsInsideOopifs()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameTask = Page.WaitForFrameAsync((frame) => frame != Page.MainFrame);
            await FrameUtils.AttachFrameAsync(
              Page,
              "frame1",
              TestConstants.CrossProcessHttpPrefix + "/empty.html"
            ).WithTimeout();
            var frame = await frameTask.WithTimeout();
            await Page.EvaluateFunctionAsync(@"() => {
                document.body.style.border = '50px solid black';
                document.body.style.margin = '50px';
                document.body.style.padding = '50px';
            }");
            await frame.EvaluateFunctionAsync(@"() => {
                const button = document.createElement('button');
                button.id = 'test-button';
                button.innerText = 'click';
                document.body.appendChild(button);
            }");

            var button = await frame.WaitForSelectorAsync("#test-button", new WaitForSelectorOptions { Visible = true });
            var result = await button.ClickablePointAsync();
            Assert.True(result.X > 150); // padding + margin + border left
            Assert.True(result.Y > 150); // padding + margin + border top
            var resultBoxModel = await button.BoxModelAsync();
            foreach (var quad in new[] {
              resultBoxModel.Content,
              resultBoxModel.Border,
              resultBoxModel.Margin,
              resultBoxModel.Padding})
            {
                foreach (var part in quad)
                {
                    Assert.True(part.X > 150); // padding + margin + border left
                    Assert.True(part.Y > 150); // padding + margin + border top
                }
            }
            var resultBoundingBox = await button.BoundingBoxAsync();
            Assert.True(resultBoundingBox.X > 150); // padding + margin + border left
            Assert.True(resultBoundingBox.Y > 150); // padding + margin + border top
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should detect existing OOPIFs when Puppeteer connects to an existing page")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldDetectExistingOopifsWhenPuppeteerConnectsToAnExistingPage()
        {
            var frameTask = Page.WaitForFrameAsync((frame) => frame.Url.EndsWith("/oopif.html"));
            await Page.GoToAsync(TestConstants.ServerUrl + "/dynamic-oopif.html");
            await frameTask.WithTimeout();
            Assert.Single(Oopifs);
            Assert.Equal(2, Page.Frames.Length);

            var browserURL = $"http://127.0.0.1:{_port}";
            using var browser1 = await Puppeteer.ConnectAsync(new (){ BrowserURL = browserURL });
            var target = await browser1.WaitForTargetAsync((target) =>
              target.Url.EndsWith("dynamic-oopif.html")
            ).WithTimeout();
            await target.PageAsync();
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should support lazy OOP frames")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSupportLazyOopframes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/lazy-oopif-frame.html");
            await Page.SetViewportAsync(new ViewPortOptions() { Width = 1000, Height = 1000 });
            Assert.Single(Page.Frames.Where(frame => !frame.HasStartedLoading));
        }

        private IEnumerable<Target> Oopifs => Context.Targets().Where(target => target.TargetInfo.Type == TargetType.IFrame);
    }
}
