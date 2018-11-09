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
            var frameTasks = new List<Task<Frame>>
            {
                FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage),
                FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.EmptyPage),
                FrameUtils.AttachFrameAsync(Page, "frame3", TestConstants.EmptyPage)
            };
            await Task.WhenAll(frameTasks);

            // Navigate all frames to the same URL.
            var serverResponses = new List<TaskCompletionSource<string>>();
            Server.SetRoute("/one-style.html", async (context) =>
            {
                var tcs = new TaskCompletionSource<string>();
                serverResponses.Add(tcs);
                await context.Response.WriteAsync(await tcs.Task);
            });

            var navigations = new List<Task<Response>>();
            for (var i = 0; i < 3; ++i)
            {
                var waitRequestTask = Server.WaitForRequest("/one-style.html");
                navigations.Add(frameTasks[i].Result.GoToAsync(TestConstants.ServerUrl + "/one-style.html"));
                await waitRequestTask;
            }
            // Respond from server out-of-order.
            var serverResponseTexts = new string[] { "AAA", "BBB", "CCC" };
            for (var i = 0; i < 3; ++i)
            {
                serverResponses[i].TrySetResult(serverResponseTexts[i]);
                var response = await navigations[i];
                Assert.Same(frameTasks[i].Result, response.Frame);
                Assert.Equal(serverResponseTexts[i], await response.TextAsync());
            }
        }
    }
}
