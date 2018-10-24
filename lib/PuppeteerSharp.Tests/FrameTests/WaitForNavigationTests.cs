using System.Net;
using System.Threading.Tasks;
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
            var requestTask = Server.WaitForRequest("/empty.html");

            await frame.EvaluateExpressionAsync($"() => window.location = '{TestConstants.EmptyPage}'");
            await requestTask;
            await Page.QuerySelectorAsync("iframe").EvaluateFunctionAsync("frame => frame.remove()");
            var exception = await Assert.ThrowsAsync<NavigationException>(async () => await waitForNavigationResult);
            Assert.Equal("Navigating frame was detached", exception.Message);
        }
    }
}
