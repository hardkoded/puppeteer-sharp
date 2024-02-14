using NUnit.Framework;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests;

public class DeviceRequestPromptDevicesTests : PuppeteerPageBaseTest
{
    [Test, PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.devices", "lists devices as they arrive")]
    [PuppeteerTimeout]
    public void ShouldListDevicesAsTheyArrive()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse() { Id = "00000000000000000000000000000000" });

        Assert.IsEmpty(prompt.Devices);

        var promptData = new DeviceAccessDeviceRequestPromptedResponse()
        {
            Id = "00000000000000000000000000000000",
            Devices = new[]
            {
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device", Id = "0000",
                }
            }
        };

        client.OnMessage(new ConnectionResponse()
        {
            Method = "DeviceAccess.deviceRequestPrompted",
            Params = WaitForDevicePromptTests.ToJToken(promptData),
        });

        Assert.AreEqual(1, prompt.Devices.Count);

        promptData = new DeviceAccessDeviceRequestPromptedResponse()
        {
            Id = "00000000000000000000000000000000",
            Devices =
            [
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device",
                    Id = "0000",
                },
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device 2",
                    Id = "0001",
                }
            ]
        };

        client.OnMessage(new ConnectionResponse()
        {
            Method = "DeviceAccess.deviceRequestPrompted",
            Params = WaitForDevicePromptTests.ToJToken(promptData),
        });

        Assert.AreEqual(2, prompt.Devices.Count);
    }

    [Test, PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.devices", "does not list devices from events of another prompt")]
    [PuppeteerTimeout]
    public void ShouldNotListDevicesFromEventsOfAnotherPrompt()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse() { Id = "00000000000000000000000000000000" });

        var promptData = new DeviceAccessDeviceRequestPromptedResponse() { Id = "00000000000000000000000000000000", };

        client.OnMessage(new ConnectionResponse()
        {
            Method = "DeviceAccess.deviceRequestPrompted",
            Params = WaitForDevicePromptTests.ToJToken(promptData),
        });

        Assert.IsEmpty(prompt.Devices);

        promptData = new DeviceAccessDeviceRequestPromptedResponse()
        {
            Id = "8888888888",
            Devices = new[]
            {
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device", Id = "0000",
                },
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device 2", Id = "0001",
                }
            }
        };

        client.OnMessage(new ConnectionResponse()
        {
            Method = "DeviceAccess.deviceRequestPrompted",
            Params = WaitForDevicePromptTests.ToJToken(promptData),
        });

        Assert.IsEmpty(prompt.Devices);
    }
}
