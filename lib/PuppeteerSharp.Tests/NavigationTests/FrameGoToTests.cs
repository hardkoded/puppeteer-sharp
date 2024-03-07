using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NavigationTests
{
    public class FrameGoToTests : PuppeteerPageBaseTest
    {
        public FrameGoToTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Frame.goto", "should navigate subframes")]
        public async Task ShouldNavigateSubFrames()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            Assert.That(Page.Frames.Where(f => f.Url.Contains("/frames/one-frame.html")), Has.Exactly(1).Items);
            Assert.That(Page.Frames.Where(f => f.Url.Contains("/frames/frame.html")), Has.Exactly(1).Items);
            var childFrame = Page.FirstChildFrame();
            var response = await childFrame.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.AreSame(response.Frame, childFrame);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Frame.goto", "should reject when frame detaches")]
        public async Task ShouldRejectWhenFrameDetaches()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            Server.SetRoute("/empty.html", _ => Task.Delay(10000));
            var waitForRequestTask = Server.WaitForRequest("/empty.html");
            var navigationTask = Page.FirstChildFrame().GoToAsync(TestConstants.EmptyPage);
            await waitForRequestTask;
            await Page.QuerySelectorAsync("iframe").EvaluateFunctionAsync("frame => frame.remove()");
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await navigationTask);
            Assert.True(
                new[]
                {
                    "Navigating frame was detached",
                    "Error: NS_BINDING_ABORTED",
                    "net::ERR_ABORTED"
                }.Any(error => exception.Message.Contains(error)));
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Frame.goto", "should return matching responses")]
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
                Assert.AreSame(matchingData[i].FrameTask.Result, response.Frame);
                Assert.AreEqual(serverResponseTexts[i], await response.TextAsync());
            }
        }

        private class MatchingResponseData
        {
            public Task<IFrame> FrameTask { get; internal set; }
            public TaskCompletionSource<string> ServerResponseTcs { get; internal set; } = new TaskCompletionSource<string>();
            public Task<IResponse> NavigationTask { get; internal set; }
        }
    }
}
