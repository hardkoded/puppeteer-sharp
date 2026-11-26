using System;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LifecycleWatcher;

public class LifecycleWatcherTests : PuppeteerPageBaseTest
{
    [
        Test,
        PuppeteerTest(
            "LifecycleWatcher.test",
            "LifecycleWatcher",
            "terminates navigation immediately when the page crashes"
        )
    ]
    public async Task TerminatesNavigationImmediatelyWhenThePageCrashes()
    {
        var errored = new TaskCompletionSource<bool>();
        Page.Error += (_, _) => errored.TrySetResult(true);
        if (!PuppeteerTestAttribute.IsCdp)
        {
            Assert.Ignore("LifecycleWatcher is CDP only.");
        }

        var frame = (CdpFrame)Page.MainFrame;
        using var watcher = new Cdp.LifecycleWatcher(
            frame.FrameManager.NetworkManager,
            frame,
            null,
            30_000
        );

        typeof(CdpPage)
            .GetMethod("OnTargetCrashed", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(Page, null);

        var ex = Assert.ThrowsAsync<PuppeteerException>(() => watcher.TerminationTask);
        Assert.That(ex.Message, Is.EqualTo("Page crashed!"));
    }

    [
        Test,
        PuppeteerTest(
            "LifecycleWatcher.test",
            "LifecycleWatcher",
            "removes the page error listener when disposed without a crash"
        )
    ]
    public void RemovesThePageErrorListenerWhenDisposedWithoutACrash()
    {
        if (!PuppeteerTestAttribute.IsCdp)
        {
            Assert.Ignore("LifecycleWatcher is CDP only.");
        }

        var frame = (CdpFrame)Page.MainFrame;
        var cdpPage = (CdpPage)Page;
        Assert.That(cdpPage.HasErrorEventListeners, Is.False);
        var watcher = new Cdp.LifecycleWatcher(
            frame.FrameManager.NetworkManager,
            frame,
            null,
            30_000
        );
        Assert.That(cdpPage.HasErrorEventListeners, Is.True);
        watcher.Dispose();
        Assert.That(cdpPage.HasErrorEventListeners, Is.False);
    }
}
