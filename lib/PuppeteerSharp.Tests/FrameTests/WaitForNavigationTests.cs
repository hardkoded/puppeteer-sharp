using System.Linq;
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
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            var frame = Page.FirstChildFrame();
            var waitForNavigationResult = frame.WaitForNavigationAsync();

            await Task.WhenAll(
                waitForNavigationResult,
                frame.EvaluateFunctionAsync("url => window.location.href = url", TestConstants.ServerUrl + "/grid.html")
            );
            var response = await waitForNavigationResult;
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Contains("grid.html", response.Url);
            Assert.Same(frame, response.Frame);
            Assert.Contains("/frames/one-frame.html", Page.Url);
        }

        [Fact]
        public async Task ShouldRejectWhenFrameDetaches()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            var frame = Page.FirstChildFrame();
            Server.SetRoute("/empty.html", context => Task.Delay(10000));
            var waitForNavigationResult = frame.WaitForNavigationAsync();
            await Task.WhenAll(
             Server.WaitForRequest("/empty.html"),
            frame.EvaluateFunctionAsync($"() => window.location = '{TestConstants.EmptyPage}'"));

            await Page.QuerySelectorAsync("iframe").EvaluateFunctionAsync("frame => frame.remove()");
            var response = await waitForNavigationResult;
        }
    }
}
