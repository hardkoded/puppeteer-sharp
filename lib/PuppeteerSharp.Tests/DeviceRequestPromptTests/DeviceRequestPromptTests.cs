using System;
using System.Threading;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests;

public class DeviceRequestPromptTests : PuppeteerPageBaseTest
{
    public DeviceRequestPromptTests() : base()
    {
        DefaultOptions = TestConstants.DefaultBrowserOptions();
        DefaultOptions.AcceptInsecureCerts = true;
        DefaultOptions.Args = [.. DefaultOptions.Args, "--enable-features=WebBluetoothNewPermissionsBackend"];
    }

    [Test, PuppeteerTest("device-request-prompt.spec", "device request prompt", "does not crash")]
    public void DoesNotCrash()
    {
        var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
        {
            await Page.GoToAsync($"{TestConstants.HttpsPrefix}/empty.html");

            await Page.WaitForDevicePromptAsync(new WaitForOptions(10));
        });

        Assert.That(exception, Is.Not.Null);
    }

    [Test, PuppeteerTest("device-request-prompt.spec", "device request prompt", "can be aborted")]
    public void CanBeAborted()
    {
        using var cts = new CancellationTokenSource();
        var task = Page.WaitForDevicePromptAsync(new WaitForOptions
        {
            CancellationToken = cts.Token,
        });

        cts.Cancel();

        Assert.CatchAsync<OperationCanceledException>(async () => await task);
    }
}
