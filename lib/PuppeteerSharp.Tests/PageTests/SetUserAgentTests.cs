using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetUserAgentTests : PuppeteerPageBaseTest
    {
        public SetUserAgentTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            Assert.Contains("Mozilla", await Page.EvaluateFunctionAsync<string>("() => navigator.userAgent"));
            await Page.SetUserAgentAsync("foobar");

            var userAgentTask = Server.WaitForRequest("/empty.html", request => request.Headers["User-Agent"].ToString());
            await Task.WhenAll(
                userAgentTask,
                Page.GoToAsync(TestConstants.EmptyPage)
            );
            Assert.Equal("foobar", userAgentTask.Result);
        }

        [Fact]
        public async Task ShouldWorkForSubframes()
        {
            Assert.Contains("Mozilla", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
            await Page.SetUserAgentAsync("foobar");
            var waitForRequestTask = Server.WaitForRequest<string>("/empty.html", (request) => request.Headers["user-agent"]);

            await Task.WhenAll(
              waitForRequestTask,
              FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage));
        }

        [Fact]
        public async Task ShouldSimulateDeviceUserAgent()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.DoesNotContain("iPhone", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
            await Page.SetUserAgentAsync(TestConstants.IPhone.UserAgent);
            Assert.Contains("iPhone", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
        }
    }
}
