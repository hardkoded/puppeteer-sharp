using System.Collections.Generic;
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
            Assert.Contains("Mozilla", await Page.EvaluateFunctionAsync<string>("() => navigator.userAgent"));
            await Page.SetUserAgentAsync("foobar");

            var userAgentTask = Server.WaitForRequest("/empty.html", request => request.Headers["User-Agent"].ToString());
            await Task.WhenAll(
                userAgentTask,
                Page.GoToAsync(TestConstants.EmptyPage)
            );
            Assert.Equal("foobar", userAgentTask.Result);
        }

        [PuppeteerTest("page.spec.ts", "Page.setUserAgent", "should work for subframes")]
        [PuppeteerFact]
        public async Task ShouldWorkForSubframes()
        {
            Assert.Contains("Mozilla", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
            await Page.SetUserAgentAsync("foobar");
            var waitForRequestTask = Server.WaitForRequest<string>("/empty.html", (request) => request.Headers["user-agent"]);

            await Task.WhenAll(
              waitForRequestTask,
              FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage));
        }

        [PuppeteerTest("page.spec.ts", "Page.setUserAgent", "should emulate device user-agent")]
        [PuppeteerFact]
        public async Task ShouldSimulateDeviceUserAgent()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.DoesNotContain("iPhone", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
            await Page.SetUserAgentAsync(TestConstants.IPhone.UserAgent);
            Assert.Contains("iPhone", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
        }

        [PuppeteerTest("page.spec.ts", "Page.setUserAgent", "should work with additional userAgentMetdata")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithAdditionalUserAgentMetdata()
        {
            await Page.SetUserAgentAsync(
                "MockBrowser",
                new UserAgentMetadata
                {
                    Architecture= "Mock1",
                    Mobile= false,
                    Model= "Mockbook",
                    Platform = "MockOS",
                    PlatformVersion = "3.1",
                });

            var requestTask = Server.WaitForRequest("/empty.html", r => r.Headers["user-agent"]);
            await Task.WhenAll(
              requestTask,
              Page.GoToAsync(TestConstants.EmptyPage));

            Assert.False(
              await Page.EvaluateFunctionAsync<bool>(@"() => {
                return navigator.userAgentData.mobile;
              }")
            );

            var uaData = await Page.EvaluateFunctionAsync<Dictionary<string, object>>(@"() => {
                return navigator.userAgentData.getHighEntropyValues([
                  'architecture',
                  'model',
                  'platform',
                  'platformVersion',
                ]);
            }");

            Assert.Equal("Mock1", uaData["architecture"]);
            Assert.Equal("Mockbook", uaData["model"]);
            Assert.Equal("MockOS", uaData["platform"]);
            Assert.Equal("3.1", uaData["platformVersion"]);
            Assert.Equal("MockBrowser", await requestTask);
        }
    }
}
