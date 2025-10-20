using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NavigationTests
{
    public class FrameWaitForNavigationTests : PuppeteerPageBaseTest
    {
        public FrameWaitForNavigationTests() : base()
        {
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Frame.waitForNavigation", "should work")]
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
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Url, Does.Contain("grid.html"));
            Assert.That(response.Frame, Is.SameAs(frame));
            Assert.That(Page.Url, Does.Contain("/frames/one-frame.html"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Frame.waitForNavigation", "should fail when frame detaches")]
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
            var exception = Assert.ThrowsAsync<PuppeteerException>(() => waitForNavigationResult);
            Assert.That(exception.Message, Is.EqualTo("Navigating frame was detached"));
        }
    }
}
