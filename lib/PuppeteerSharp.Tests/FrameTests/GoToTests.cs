using System.Net;
using System.Threading.Tasks;
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
            Assert.Contains("/frames/one-frame.html", Page.Frames[0].Url);
            Assert.Contains("/frames/frame.html", Page.Frames[1].Url);
            var response = await Page.Frames[1].GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public async Task ShouldRejectWhenFrameDetaches()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            Server.SetRoute("/empty.html", context => Task.Delay(10000));
            var waitForRequestTask = Server.WaitForRequest("/empty.html");
            var navigationTask = Page.Frames[1].GoToAsync(TestConstants.EmptyPage);
            await waitForRequestTask;
            await Page.QuerySelectorAsync("iframe").EvaluateFunctionAsync("frame => frame.remove()");
            var exception = await Assert.ThrowsAsync<NavigationException>(async () => await navigationTask);
            Assert.Equal("Navigating frame was detached", exception.Message);
        }

        public async Task ShouldReturnMatchingResponses()
        {

        }
    }
}
