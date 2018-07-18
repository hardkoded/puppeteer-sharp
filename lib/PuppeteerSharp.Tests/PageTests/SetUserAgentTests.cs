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
        public async Task ShouldSimulateDeviceUserAgent()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.Contains("Chrome", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
            await Page.SetUserAgentAsync(TestConstants.IPhone.UserAgent);
            Assert.Contains("Safari", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
        }
    }
}
