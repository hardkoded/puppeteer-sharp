using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests;

public class DeviceRequestPromptWaitForDeviceTests : PuppeteerPageBaseTest
{
    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.waitForDevice", "should return first matching device")]
    [PuppeteerTimeout]
    public async Task ShouldReturnFirstMatchingDevice()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse() { Id = "00000000000000000000000000000000" });
        var promptTask = prompt.WaitForDeviceAsync(device => device.Name == "My Device 1");

        var promptData = new DeviceAccessDeviceRequestPromptedResponse()
        {
            Id = "00000000000000000000000000000000",
            Devices =
            [
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device 0",
                    Id = "0000",
                }
            ]
        };

        client.OnMessage(new ConnectionResponse()
        {
            Method = "DeviceAccess.deviceRequestPrompted",
            Params = WaitForDevicePromptTests.ToJToken(promptData),
        });

        promptData = new DeviceAccessDeviceRequestPromptedResponse()
        {
            Id = "00000000000000000000000000000000",
            Devices =
            [
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device 0",
                    Id = "0000",
                },
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device 1",
                    Id = "0001",
                }
            ]
        };

        client.OnMessage(new ConnectionResponse()
        {
            Method = "DeviceAccess.deviceRequestPrompted",
            Params = WaitForDevicePromptTests.ToJToken(promptData),
        });

        var device = await promptTask;
        Assert.AreEqual("My Device 1", device.Name);
    }

    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.waitForDevice", "should return first matching device from already known devices")]
    [PuppeteerTimeout]
    public async Task ShouldReturnFirstMatchingDeviceFromAlreadyKnownDevices()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse()
            {
                Id = "000",
                Devices =
                [
                    new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice() { Name = "My Device 0", Id = "0000", },
                    new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice() { Name = "My Device 1", Id = "0001", }
                ]
            });
        await prompt.WaitForDeviceAsync(device => device.Name == "My Device 1");
    }

    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.waitForDevice", "should return device in the devices list")]
    [PuppeteerTimeout]
    public async Task ShouldReturnDeviceInTheDevicesList()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse()
            {
                Id = "000",
            });

        var promptTask = prompt.WaitForDeviceAsync(device => device.Name == "My Device 1");

        var promptData = new DeviceAccessDeviceRequestPromptedResponse()
        {
            Id = "000",
            Devices = new[]
            {
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device 0", Id = "0000",
                },
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device 1", Id = "0001",
                }
            }
        };

        client.OnMessage(new ConnectionResponse()
        {
            Method = "DeviceAccess.deviceRequestPrompted",
            Params = WaitForDevicePromptTests.ToJToken(promptData),
        });

        var device = await promptTask;
        Assert.Contains(device, prompt.Devices.ToArray());
    }

    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.waitForDevice", "should respect timeout")]
    [PuppeteerTimeout]
    public void ShouldRespectTimeout()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse()
            {
                Id = "000",
            });
        Assert.ThrowsAsync<TimeoutException>(() => prompt.WaitForDeviceAsync(device => device.Name == "My Device 1", new WaitForOptions(1)));
    }

    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.waitForDevice", "should respect default timeout when there is no custom timeout")]
    [PuppeteerTimeout]
    public void ShouldRespectDefaultTimeoutWhenThereIsNoCustomTimeout()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse()
            {
                Id = "000",
            });
        timeoutSettings.Timeout = 1;
        Assert.ThrowsAsync<TimeoutException>(() => prompt.WaitForDeviceAsync(device => device.Name == "My Device 1"));
    }

    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.waitForDevice", "should prioritize exact timeout over default timeout")]
    [PuppeteerTimeout]
    public void ShouldPrioritizeExactTimeoutOverDefaultTimeout()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse()
            {
                Id = "000",
            });
        timeoutSettings.Timeout = 0;
        Assert.ThrowsAsync<TimeoutException>(() => prompt.WaitForDeviceAsync(device => device.Name == "My Device 1", new WaitForOptions(1)));
    }

    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.waitForDevice", "should work with no timeout")]
    [PuppeteerTimeout]
    public async Task ShouldWorkWithNoTimeout()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse()
            {
                Id = "000",
            });
        var deviceTask = prompt.WaitForDeviceAsync(device => device.Name == "My Device 1", new WaitForOptions(0));

        var promptData = new DeviceAccessDeviceRequestPromptedResponse()
        {
            Id = "000",
            Devices =
            [
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device 0",
                    Id = "0000",
                },
            ]
        };

        client.OnMessage(new ConnectionResponse()
        {
            Method = "DeviceAccess.deviceRequestPrompted",
            Params = WaitForDevicePromptTests.ToJToken(promptData),
        });

        promptData = new DeviceAccessDeviceRequestPromptedResponse()
        {
            Id = "000",
            Devices = new[]
            {
                new DeviceAccessDeviceRequestPromptedResponse.DeviceAccessDevice()
                {
                    Name = "My Device 1", Id = "0001",
                }
            }
        };

        client.OnMessage(new ConnectionResponse()
        {
            Method = "DeviceAccess.deviceRequestPrompted",
            Params = WaitForDevicePromptTests.ToJToken(promptData),
        });

        await deviceTask;
    }
}
