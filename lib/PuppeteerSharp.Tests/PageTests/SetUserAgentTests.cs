using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class SetUserAgentTests : PuppeteerPageBaseTest
    {
        public SetUserAgentTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setUserAgent", "should work")]
        public async Task ShouldWork()
        {
            StringAssert.Contains("Mozilla", await Page.EvaluateFunctionAsync<string>("() => navigator.userAgent"));
            await Page.SetUserAgentAsync("foobar");

            var userAgentTask = Server.WaitForRequest("/empty.html", request => request.Headers["User-Agent"].ToString());
            await Task.WhenAll(
                userAgentTask,
                Page.GoToAsync(TestConstants.EmptyPage)
            );
            Assert.AreEqual("foobar", userAgentTask.Result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setUserAgent", "should work for subframes")]
        public async Task ShouldWorkForSubframes()
        {
            StringAssert.Contains("Mozilla", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
            await Page.SetUserAgentAsync("foobar");
            var waitForRequestTask = Server.WaitForRequest<string>("/empty.html", (request) => request.Headers["user-agent"]);

            await Task.WhenAll(
              waitForRequestTask,
              FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setUserAgent", "should emulate device user-agent")]
        public async Task ShouldSimulateDeviceUserAgent()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            StringAssert.DoesNotContain("iPhone", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
            await Page.SetUserAgentAsync(TestConstants.IPhone.UserAgent);
            StringAssert.Contains("iPhone", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setUserAgent", "should work with additional userAgentMetdata")]
        public async Task ShouldWorkWithAdditionalUserAgentMetdata()
        {
            await Page.SetUserAgentAsync(
                "MockBrowser",
                new UserAgentMetadata
                {
                    Architecture = "Mock1",
                    Mobile = false,
                    Model = "Mockbook",
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

            Assert.AreEqual("Mock1", uaData["architecture"]);
            Assert.AreEqual("Mockbook", uaData["model"]);
            Assert.AreEqual("MockOS", uaData["platform"]);
            Assert.AreEqual("3.1", uaData["platformVersion"]);
            Assert.AreEqual("MockBrowser", await requestTask);
        }
    }
}
