using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class WaitForNavigationTests : PuppeteerPageBaseTest
    {
        public WaitForNavigationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWaitForNavigate()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            var frame = Page.Frames[1];
            var waitForNavigationResult = frame.WaitForNavigationAsync();

            await Task.WhenAll(
                waitForNavigationResult,
                frame.EvaluateFunctionAsync("url => window.location.href = url", TestConstants.ServerUrl + "/grid.html")
            );
            var response = await waitForNavigationResult;
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Contains("grid.html", response.Url);
            Assert.Equal(frame, response.Frame);
        }

        [Fact]
        public async Task ShouldRejectWhenFrameDetaches()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            var frame = Page.Frames[1];
            Server.SetRoute("/empty.html", context => Task.Delay(10000));
            var waitForNavigationResult = frame.WaitForNavigationAsync();
            await Task.WhenAll(
             Server.WaitForRequest("/empty.html"),
            frame.EvaluateFunctionAsync($"() => window.location = '{TestConstants.EmptyPage}'"));

            //await Page.QuerySelectorAsync("iframe").EvaluateFunctionAsync("frame => frame.remove()");
            var response = await waitForNavigationResult;
            return;
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
