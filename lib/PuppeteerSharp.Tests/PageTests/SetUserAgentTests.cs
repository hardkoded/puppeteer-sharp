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
            Assert.That(await Page.EvaluateFunctionAsync<string>("() => navigator.userAgent"), Does.Contain("Mozilla"));
            await Page.SetUserAgentAsync("foobar");

            var userAgentTask = Server.WaitForRequest("/empty.html", request => request.Headers["User-Agent"].ToString());
            await Task.WhenAll(
                userAgentTask,
                Page.GoToAsync(TestConstants.EmptyPage)
            );
            Assert.That(userAgentTask.Result, Is.EqualTo("foobar"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setUserAgent", "should work for subframes")]
        public async Task ShouldWorkForSubframes()
        {
            Assert.That(await Page.EvaluateExpressionAsync<string>("navigator.userAgent"), Does.Contain("Mozilla"));
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
            Assert.That(await Page.EvaluateExpressionAsync<string>("navigator.userAgent"), Does.Not.Contain("iPhone"));
            await Page.SetUserAgentAsync(TestConstants.IPhone.UserAgent);
            Assert.That(await Page.EvaluateExpressionAsync<string>("navigator.userAgent"), Does.Contain("iPhone"));
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

            Assert.That(
              await Page.EvaluateFunctionAsync<bool>(@"() => {
                return navigator.userAgentData.mobile;
              }")
, Is.False);

            var uaData = await Page.EvaluateFunctionAsync<Dictionary<string, object>>(@"() => {
                return navigator.userAgentData.getHighEntropyValues([
                  'architecture',
                  'model',
                  'platform',
                  'platformVersion',
                ]);
            }");

            Assert.That(uaData["architecture"], Is.EqualTo("Mock1"));
            Assert.That(uaData["model"], Is.EqualTo("Mockbook"));
            Assert.That(uaData["platform"], Is.EqualTo("MockOS"));
            Assert.That(uaData["platformVersion"], Is.EqualTo("3.1"));
            Assert.That(await requestTask, Is.EqualTo("MockBrowser"));
        }
    }
}
