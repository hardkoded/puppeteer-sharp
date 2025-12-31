using NUnit.Framework;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests;

public class DeviceRequestPromptDevicesTests : PuppeteerPageBaseTest
{
    [Test, PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.devices", "lists devices as they arrive")]
    public void ShouldListDevicesAsTheyArrive()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse() { Id = "00000000000000000000000000000000" });

        Assert.That(prompt.Devices, Is.Empty);

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
            Params = promptData.ToJsonElement(),
        });

        Assert.That(prompt.Devices, Has.Count.EqualTo(1));

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
            Params = promptData.ToJsonElement(),
        });

        Assert.That(prompt.Devices, Has.Count.EqualTo(2));
    }

    [Test, PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.devices", "does not list devices from events of another prompt")]
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
            Params = promptData.ToJsonElement(),
        });

        Assert.That(prompt.Devices, Is.Empty);

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
            Params = promptData.ToJsonElement(),
        });

        Assert.That(prompt.Devices, Is.Empty);
    }
}
