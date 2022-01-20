using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class SetUserAgentTests : PuppeteerPageBaseTest
    {
        public SetUserAgentTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.setUserAgent", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            Assert.Contains("Mozilla", await DevToolsContext.EvaluateFunctionAsync<string>("() => navigator.userAgent"));
            await DevToolsContext.SetUserAgentAsync("foobar");

            var userAgentTask = Server.WaitForRequest("/empty.html", request => request.Headers["User-Agent"].ToString());
            await Task.WhenAll(
                userAgentTask,
                DevToolsContext.GoToAsync(TestConstants.EmptyPage)
            );
            Assert.Equal("foobar", userAgentTask.Result);
        }

        [PuppeteerTest("page.spec.ts", "Page.setUserAgent", "should work for subframes")]
        [PuppeteerFact]
        public async Task ShouldWorkForSubframes()
        {
            Assert.Contains("Mozilla", await DevToolsContext.EvaluateExpressionAsync<string>("navigator.userAgent"));
            await DevToolsContext.SetUserAgentAsync("foobar");
            var waitForRequestTask = Server.WaitForRequest<string>("/empty.html", (request) => request.Headers["user-agent"]);

            await Task.WhenAll(
              waitForRequestTask,
              FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage));
        }

        [PuppeteerTest("page.spec.ts", "Page.setUserAgent", "should emulate device user-agent")]
        [PuppeteerFact]
        public async Task ShouldSimulateDeviceUserAgent()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.DoesNotContain("iPhone", await DevToolsContext.EvaluateExpressionAsync<string>("navigator.userAgent"));
            await DevToolsContext.SetUserAgentAsync(TestConstants.IPhone.UserAgent);
            Assert.Contains("iPhone", await DevToolsContext.EvaluateExpressionAsync<string>("navigator.userAgent"));
        }
    }
}
