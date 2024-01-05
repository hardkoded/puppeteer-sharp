using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests;

public class DeviceRequestPromptCancelTests : PuppeteerPageBaseTest
{
    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.cancel",
        "should succeed on first call")]
    [PuppeteerTimeout]
    public async Task ShouldSucceedOnFirstCall()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse() { Id = "000" });

        await prompt.CancelAsync();
    }

    [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.cancel",
        "should fail when canceling prompt twice")]
    [PuppeteerTimeout]
    public async Task ShouldFailWhenCancelingPromptTwice()
    {
        var client = new MockCDPSession();
        var timeoutSettings = new TimeoutSettings();
        var prompt = new DeviceRequestPrompt(
            client,
            timeoutSettings,
            new DeviceAccessDeviceRequestPromptedResponse() { Id = "000" });

        await prompt.CancelAsync();
        Assert.ThrowsAsync<PuppeteerException>(async () => await prompt.CancelAsync());
    }
}
