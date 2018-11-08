﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class FrameManagementTests : PuppeteerPageBaseTest
    {
        public FrameManagementTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldHandleNestedFrames()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            Assert.Equal(
                TestUtils.CompressText(TestConstants.NestedFramesDumpResult),
                TestUtils.CompressText(FrameUtils.DumpFrames(Page.MainFrame)));
        }

        [Fact]
        public async Task ShouldSendEventsWhenFramesAreManipulatedDynamically()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            // validate frameattached events
            var attachedFrames = new List<Frame>();

            Page.FrameAttached += (sender, e) => attachedFrames.Add(e.Frame);

            await FrameUtils.AttachFrameAsync(Page, "frame1", "./assets/frame.html");

            Assert.Single(attachedFrames);
            Assert.Contains("/assets/frame.html", attachedFrames[0].Url);

            // validate framenavigated events
            var navigatedFrames = new List<Frame>();
            Page.FrameNavigated += (sender, e) => navigatedFrames.Add(e.Frame);

            await FrameUtils.NavigateFrameAsync(Page, "frame1", "./empty.html");
            Assert.Single(navigatedFrames);
            Assert.Equal(TestConstants.EmptyPage, navigatedFrames[0].Url);

            // validate framedetached events
            var detachedFrames = new List<Frame>();
            Page.FrameDetached += (sender, e) => detachedFrames.Add(e.Frame);

            await FrameUtils.DetachFrameAsync(Page, "frame1");
            Assert.Single(navigatedFrames);
            Assert.True(navigatedFrames[0].Detached);
        }

        [Fact]
        public async Task ShouldSendFrameNavigatedWhenNavigatingOnAnchorURLs()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frameNavigated = new TaskCompletionSource<bool>();
            Page.FrameNavigated += (sender, e) => frameNavigated.TrySetResult(true);
            await Task.WhenAll(
                Page.GoToAsync(TestConstants.EmptyPage + "#foo"),
                frameNavigated.Task
            );
            Assert.Equal(TestConstants.EmptyPage + "#foo", Page.Url);
        }

        [Fact]
        public async Task ShouldPersistMainFrameOnCrossProcessNavigation()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var mainFrame = Page.MainFrame;
            await Page.GoToAsync(TestConstants.CrossProcessUrl + "/empty.html");
            Assert.Equal(mainFrame, Page.MainFrame);
        }

        [Fact]
        public async Task ShouldNotSendAttachDetachEventsForMainFrame()
        {
            var hasEvents = false;
            Page.FrameAttached += (sender, e) => hasEvents = true;
            Page.FrameDetached += (sender, e) => hasEvents = true;

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.False(hasEvents);
        }

        [Fact]
        public async Task ShouldDetachChildFramesOnNavigation()
        {
            var attachedFrames = new List<Frame>();
            var detachedFrames = new List<Frame>();
            var navigatedFrames = new List<Frame>();

            Page.FrameAttached += (sender, e) => attachedFrames.Add(e.Frame);
            Page.FrameDetached += (sender, e) => detachedFrames.Add(e.Frame);
            Page.FrameNavigated += (sender, e) => navigatedFrames.Add(e.Frame);

            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            Assert.Equal(4, attachedFrames.Count);
            Assert.Empty(detachedFrames);
            Assert.Equal(5, navigatedFrames.Count);

            attachedFrames.Clear();
            detachedFrames.Clear();
            navigatedFrames.Clear();

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Empty(attachedFrames);
            Assert.Equal(4, detachedFrames.Count);
            Assert.Single(navigatedFrames);
        }

        [Fact]
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

        [Fact]
        public async Task ShouldReportFrameParent()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.EmptyPage);

            Assert.Single(Page.Frames, frame => frame.ParentFrame == null);
            Assert.Equal(2, Page.Frames.Count(f => f.ParentFrame == Page.MainFrame));
        }
    }
}
