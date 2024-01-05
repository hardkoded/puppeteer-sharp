using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests;

public class DeviceRequestPromptSelectTests : PuppeteerPageBaseTest
{
    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.select",
        "should succeed with listed device")]
    [PuppeteerTimeout]
    public async Task ShouldSucceedWithListedDevice()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse() { Id = "000" });

        var deviceTask = prompt.WaitForDeviceAsync(device => device.Name.Contains("1"));

        var promptData = new DeviceAccessDeviceRequestPromptedResponse()
        {
            Id = "000",
            Devices = new[]
            {
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device 1", Id = "0000",
                }
            }
        };

        client.OnMessage(new ConnectionResponse()
        {
            Method = "DeviceAccess.deviceRequestPrompted",
            Params = WaitForDevicePromptTests.ToJToken(promptData),
        });

        var device = await deviceTask;
        await prompt.SelectAsync(device);
    }

    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.select",
        "should error for device not listed in devices")]
    [PuppeteerTimeout]
    public void ShouldErrorForDeviceNotListedInDevices()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse() { Id = "000" });

        var exception = Assert.ThrowsAsync<PuppeteerException>(() =>
            prompt.SelectAsync(new DeviceRequestPromptDevice("My Device 2", "0001")));

        Assert.AreEqual("Cannot select unknown device!", exception.Message);
    }

    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.select",
        "should fail when selecting prompt twice")]
    [PuppeteerTimeout]
    public async Task ShouldFailWhenSelectingPromptTwice()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse() { Id = "000" });

        var deviceTask = prompt.WaitForDeviceAsync(device => device.Name.Contains("1"));

        var promptData = new DeviceAccessDeviceRequestPromptedResponse()
        {
            Id = "000",
            Devices = new[]
            {
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device 1", Id = "0000",
                }
            }
        };

        client.OnMessage(new ConnectionResponse()
        {
            Method = "DeviceAccess.deviceRequestPrompted",
            Params = WaitForDevicePromptTests.ToJToken(promptData),
        });

        var device = await deviceTask;
        await prompt.SelectAsync(device);

        var exception = Assert.ThrowsAsync<PuppeteerException>(() => prompt.SelectAsync(device));
        Assert.AreEqual("Cannot select DeviceRequestPrompt which is already handled!", exception.Message);
    }
}
