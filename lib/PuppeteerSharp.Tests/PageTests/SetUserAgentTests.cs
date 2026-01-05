using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class SetUserAgentTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.setUserAgent", "should work")]
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

        [Test, PuppeteerTest("page.spec", "Page Page.setUserAgent", "should work for subframes")]
        public async Task ShouldWorkForSubframes()
        {
            Assert.That(await Page.EvaluateExpressionAsync<string>("navigator.userAgent"), Does.Contain("Mozilla"));
            await Page.SetUserAgentAsync("foobar");
            var waitForRequestTask = Server.WaitForRequest<string>("/empty.html", (request) => request.Headers["user-agent"]);

            await Task.WhenAll(
              waitForRequestTask,
              FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.setUserAgent", "should emulate device user-agent")]
        public async Task ShouldSimulateDeviceUserAgent()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.That(await Page.EvaluateExpressionAsync<string>("navigator.userAgent"), Does.Not.Contain("iPhone"));
            await Page.SetUserAgentAsync(TestConstants.IPhone.UserAgent);
            Assert.That(await Page.EvaluateExpressionAsync<string>("navigator.userAgent"), Does.Contain("iPhone"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.setUserAgent", "should work with additional userAgentMetdata")]
        public async Task ShouldWorkWithAdditionalUserAgentMetadata()
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

            var uaData = await Page.EvaluateFunctionAsync<JsonElement>(@"() => {
                return navigator.userAgentData.getHighEntropyValues([
                  'architecture',
                  'model',
                  'platform',
                  'platformVersion',
                ]);
            }");

            Assert.That(uaData.GetProperty("architecture").GetString(), Is.EqualTo("Mock1"));
            Assert.That(uaData.GetProperty("model").GetString(), Is.EqualTo("Mockbook"));
            Assert.That(uaData.GetProperty("platform").GetString(), Is.EqualTo("MockOS"));
            Assert.That(uaData.GetProperty("platformVersion").GetString(), Is.EqualTo("3.1"));
            Assert.That(await requestTask, Is.EqualTo("MockBrowser"));
        }

        [Test, PuppeteerTest("puppeteer-sharp", "Page Page.setUserAgent", "should work with bitness and wow64")]
        public async Task ShouldWorkWithBitnessAndWow64()
        {
            await Page.SetUserAgentAsync(
                "MockBrowser",
                new UserAgentMetadata
                {
                    Architecture = "x86",
                    Mobile = false,
                    Model = "Mockbook",
                    Platform = "Windows",
                    PlatformVersion = "10.0.0",
                    Bitness = "64",
                    Wow64 = false,
                });

            await Page.GoToAsync(TestConstants.EmptyPage);

            var uaData = await Page.EvaluateFunctionAsync<JsonElement>(@"() => {
                return navigator.userAgentData.getHighEntropyValues([
                  'architecture',
                  'bitness',
                  'model',
                  'platform',
                  'platformVersion',
                  'wow64',
                ]);
            }");

            Assert.That(uaData.GetProperty("architecture").GetString(), Is.EqualTo("x86"));
            Assert.That(uaData.GetProperty("bitness").GetString(), Is.EqualTo("64"));
            Assert.That(uaData.GetProperty("model").GetString(), Is.EqualTo("Mockbook"));
            Assert.That(uaData.GetProperty("platform").GetString(), Is.EqualTo("Windows"));
            Assert.That(uaData.GetProperty("platformVersion").GetString(), Is.EqualTo("10.0.0"));
            Assert.That(uaData.GetProperty("wow64").GetBoolean(), Is.False);
        }
    }
}
