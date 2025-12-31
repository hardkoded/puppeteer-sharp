using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests
{
    public class WaitForDevicePromptTests : PuppeteerPageBaseTest
    {
        public async Task Usage()
        {
            #region devicerequestprompt_usage
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
            #region ipagewaitfordevicepromptasync_usage
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
            #region iframewaitfordevicepromptasync_usage
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

        [Test, PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should return prompt")]
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
                Params = promptData.ToJsonElement(),
            });

            await promptTask;
        }

        [Test, PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should respect timeout")]
        public void ShouldRespectTimeout()
        {
            var client = new MockCDPSession();
            var timeoutSettings = new TimeoutSettings();
            var manager = new DeviceRequestPromptManager(client, timeoutSettings);
            Assert.ThrowsAsync<TimeoutException>(() => manager.WaitForDevicePromptAsync(new WaitForOptions(1)));
        }

        [Test, PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should respect default timeout when there is no custom timeout")]
        public void ShouldRespectDefaultTimeoutWhenThereIsNoCustomTimeout()
        {
            var client = new MockCDPSession();
            var timeoutSettings = new TimeoutSettings();
            var manager = new DeviceRequestPromptManager(client, timeoutSettings);
            timeoutSettings.Timeout = 1;
            Assert.ThrowsAsync<TimeoutException>(() => manager.WaitForDevicePromptAsync());
        }

        [Test, PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should prioritize exact timeout over default timeout")]
        public void ShouldPrioritizeExactTimeoutOverDefaultTimeout()
        {
            var client = new MockCDPSession();
            var timeoutSettings = new TimeoutSettings();
            var manager = new DeviceRequestPromptManager(client, timeoutSettings);
            timeoutSettings.Timeout = 0;
            Assert.ThrowsAsync<TimeoutException>(() => manager.WaitForDevicePromptAsync(new WaitForOptions(1)));
        }

        [Test, PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should work with no timeout")]
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
                Params = promptData.ToJsonElement(),
            });

            await promptTask;
        }

        [Test, PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should return the same prompt when there are many watchdogs simultaneously")]
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
                Params = promptData.ToJsonElement(),
            });

            await Task.WhenAll(promptTask, promptTask2);
            Assert.That(promptTask2.Result, Is.EqualTo(promptTask.Result));
        }
    }
}
