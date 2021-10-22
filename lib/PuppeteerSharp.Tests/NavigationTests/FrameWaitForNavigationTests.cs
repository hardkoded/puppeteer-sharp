using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NavigationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FrameWaitForNavigationTests : PuppeteerPageBaseTest
    {
        public FrameWaitForNavigationTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("navigation.spec.ts", "Frame.waitForNavigation", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
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

        [PuppeteerTest("navigation.spec.ts", "Frame.waitForNavigation", "should fail when frame detaches")]
        [PuppeteerFact]
        public async Task ShouldFailWhenFrameDetaches()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            var frame = Page.FirstChildFrame();
            Server.SetRoute("/empty.html", _ => Task.Delay(10000));
            var waitForNavigationResult = frame.WaitForNavigationAsync();
            await Task.WhenAll(
                Server.WaitForRequest("/empty.html"),
                frame.EvaluateFunctionAsync($"() => window.location = '{TestConstants.EmptyPage}'"));

            await Page.QuerySelectorAsync("iframe").EvaluateFunctionAsync("frame => frame.remove()");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(() => waitForNavigationResult);
            Assert.Equal("Navigating frame was detached", exception.Message);
        }
    }
}
