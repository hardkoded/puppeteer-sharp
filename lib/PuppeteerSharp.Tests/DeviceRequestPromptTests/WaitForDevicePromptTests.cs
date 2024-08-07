using System;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests
{
    public class WaitForDevicePromptTests : PuppeteerPageBaseTest
    {
        public async Task Usage()
        {
            #region DeviceRequestPromptUsage
            var promptTask = Page.WaitForDevicePromptAsync();
            await Task.WhenAll(
                promptTask,
                Page.ClickAsync("#connect-bluetooth"));

            var devicePrompt = await promptTask;
            await devicePrompt.SelectAsync(
                await devicePrompt.WaitForDeviceAsync(device => device.Name.Contains("My Device")).ConfigureAwait(false)
            );
            #endregion
        }

        public async Task PageUsage()
        {
            #region IPageWaitForDevicePromptAsyncUsage
            var promptTask = Page.WaitForDevicePromptAsync();
            await Task.WhenAll(
                promptTask,
                Page.ClickAsync("#connect-bluetooth"));

            var devicePrompt = await promptTask;
            await devicePrompt.SelectAsync(
                await devicePrompt.WaitForDeviceAsync(device => device.Name.Contains("My Device")).ConfigureAwait(false)
            );
            #endregion
        }

        public async Task FrameUsage()
        {
            var frame = Page.MainFrame;
            #region IFrameWaitForDevicePromptAsyncUsage
            var promptTask = frame.WaitForDevicePromptAsync();
            await Task.WhenAll(
                promptTask,
                Page.ClickAsync("#connect-bluetooth"));

            var devicePrompt = await promptTask;
            await devicePrompt.SelectAsync(
                await devicePrompt.WaitForDeviceAsync(device => device.Name.Contains("My Device")).ConfigureAwait(false)
            );
            #endregion
        }

        [Test, Retry(2), PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should return prompt")]
        public async Task ShouldReturnPrompt()
        {
            var client = new MockCDPSession();
            var timeoutSettings = new TimeoutSettings();
            var manager = new DeviceRequestPromptManager(client, timeoutSettings);
            var promptTask = manager.WaitForDevicePromptAsync();
            var promptData = new DeviceAccessDeviceRequestPromptedResponse()
            {
                Id = "00000000000000000000000000000000",
            };

            client.OnMessage(new ConnectionResponse()
            {
                Method = "DeviceAccess.deviceRequestPrompted",
                Params = ToJsonElement(promptData),
            });

            await promptTask;
        }

        [Test, Retry(2), PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should respect timeout")]
        public void ShouldRespectTimeout()
        {
            var client = new MockCDPSession();
            var timeoutSettings = new TimeoutSettings();
            var manager = new DeviceRequestPromptManager(client, timeoutSettings);
            Assert.ThrowsAsync<TimeoutException>(() => manager.WaitForDevicePromptAsync(new WaitForOptions(1)));
        }

        [Test, Retry(2), PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should respect default timeout when there is no custom timeout")]
        public void ShouldRespectDefaultTimeoutWhenThereIsNoCustomTimeout()
        {
            var client = new MockCDPSession();
            var timeoutSettings = new TimeoutSettings();
            var manager = new DeviceRequestPromptManager(client, timeoutSettings);
            timeoutSettings.Timeout = 1;
            Assert.ThrowsAsync<TimeoutException>(() => manager.WaitForDevicePromptAsync());
        }

        [Test, Retry(2), PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should prioritize exact timeout over default timeout")]
        public void ShouldPrioritizeExactTimeoutOverDefaultTimeout()
        {
            var client = new MockCDPSession();
            var timeoutSettings = new TimeoutSettings();
            var manager = new DeviceRequestPromptManager(client, timeoutSettings);
            timeoutSettings.Timeout = 0;
            Assert.ThrowsAsync<TimeoutException>(() => manager.WaitForDevicePromptAsync(new WaitForOptions(1)));
        }

        [Test, Retry(2), PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should work with no timeout")]
        public async Task ShouldWorkWithNoTimeout()
        {
            var client = new MockCDPSession();
            var timeoutSettings = new TimeoutSettings();
            var manager = new DeviceRequestPromptManager(client, timeoutSettings);
            var promptTask = manager.WaitForDevicePromptAsync(new WaitForOptions(0));
            var promptData = new DeviceAccessDeviceRequestPromptedResponse()
            {
                Id = "00000000000000000000000000000000",
            };

            client.OnMessage(new ConnectionResponse()
            {
                Method = "DeviceAccess.deviceRequestPrompted",
                Params = ToJsonElement(promptData),
            });

            await promptTask;
        }

        [Test, Retry(2), PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should return the same prompt when there are many watchdogs simultaneously")]
        public async Task ShouldReturnTheSamePromptWhenThereAreManyWatchdogsSimultaneously()
        {
            var client = new MockCDPSession();
            var timeoutSettings = new TimeoutSettings();
            var manager = new DeviceRequestPromptManager(client, timeoutSettings);
            var promptTask = manager.WaitForDevicePromptAsync();
            var promptTask2 = manager.WaitForDevicePromptAsync();
            var promptData = new DeviceAccessDeviceRequestPromptedResponse()
            {
                Id = "00000000000000000000000000000000",
            };

            client.OnMessage(new ConnectionResponse()
            {
                Method = "DeviceAccess.deviceRequestPrompted",
                Params = ToJsonElement(promptData),
            });

            await Task.WhenAll(promptTask, promptTask2);
            Assert.AreEqual(promptTask.Result, promptTask2.Result);
        }

        internal static JsonElement ToJsonElement(DeviceAccessDeviceRequestPromptedResponse promptData)
            => JsonSerializer.SerializeToElement(promptData, JsonHelper.DefaultJsonSerializerSettings.Value);
    }
}
