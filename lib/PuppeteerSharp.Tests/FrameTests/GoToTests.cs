using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class GoToTests : PuppeteerPageBaseTest
    {
        public GoToTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldNavigateSubFrames()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            Assert.Single(Page.Frames.Where(f => f.Url.Contains("/frames/one-frame.html")));
            Assert.Single(Page.Frames.Where(f => f.Url.Contains("/frames/frame.html")));
            var childFrame = Page.FirstChildFrame();
            var response = await childFrame.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Same(response.Frame, childFrame);
        }

        [Fact]
        public async Task ShouldRejectWhenFrameDetaches()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            Server.SetRoute("/empty.html", context => Task.Delay(10000));
            var waitForRequestTask = Server.WaitForRequest("/empty.html");
            var navigationTask = Page.FirstChildFrame().GoToAsync(TestConstants.EmptyPage);
            await waitForRequestTask;
            await Page.QuerySelectorAsync("iframe").EvaluateFunctionAsync("frame => frame.remove()");
            var exception = await Assert.ThrowsAsync<NavigationException>(async () => await navigationTask);
            Assert.Equal("Navigating frame was detached", exception.Message);
        }

        [Fact]
        public async Task ShouldReturnMatchingResponses()
        {
            // Disable cache: otherwise, chromium will cache similar requests.
            await Page.SetCacheEnabledAsync(false);
            await Page.GoToAsync(TestConstants.EmptyPage);
            // Attach three frames.
            var matchingData = new MatchingResponseData[]
            {
                new MatchingResponseData{ FrameTask =  FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage)},
                new MatchingResponseData{ FrameTask =  FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.EmptyPage)},
                new MatchingResponseData{ FrameTask =  FrameUtils.AttachFrameAsync(Page, "frame3", TestConstants.EmptyPage)}
            };

            await Task.WhenAll(matchingData.Select(m => m.FrameTask));

            // Navigate all frames to the same URL.
            var requestHandler = new RequestDelegate(async (context) =>
            {
                if (int.TryParse(context.Request.Query["index"], out var index))
                {
                    await context.Response.WriteAsync(await matchingData[index].ServerResponseTcs.Task);
                }
            });

            Server.SetRoute("/one-style.html?index=0", requestHandler);
            Server.SetRoute("/one-style.html?index=1", requestHandler);
            Server.SetRoute("/one-style.html?index=2", requestHandler);

            for (var i = 0; i < 3; ++i)
            {
                var waitRequestTask = Server.WaitForRequest("/one-style.html");
                matchingData[i].NavigationTask = matchingData[i].FrameTask.Result.GoToAsync($"{TestConstants.ServerUrl}/one-style.html?index={i}");
                await waitRequestTask;
            }
            // Respond from server out-of-order.
            var serverResponseTexts = new string[] { "AAA", "BBB", "CCC" };
            for (var i = 0; i < 3; ++i)
            {
                matchingData[i].ServerResponseTcs.TrySetResult(serverResponseTexts[i]);
                var response = await matchingData[i].NavigationTask;
                Assert.Same(matchingData[i].FrameTask.Result, response.Frame);
                Assert.Equal(serverResponseTexts[i], await response.TextAsync());
            }
        }

        private class MatchingResponseData
        {
            public Task<Frame> FrameTask { get; internal set; }
            public TaskCompletionSource<string> ServerResponseTcs { get; internal set; } = new TaskCompletionSource<string>();
            public Task<Response> NavigationTask { get; internal set; }
        }
    }
}
