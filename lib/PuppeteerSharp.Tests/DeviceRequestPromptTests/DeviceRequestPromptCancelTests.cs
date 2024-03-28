using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests;

public class DeviceRequestPromptCancelTests : PuppeteerPageBaseTest
{
    [Test, Retry(2), PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.cancel", "should succeed on first call")]
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

    [Test, Retry(2), PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.cancel", "should fail when canceling prompt twice")]
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
