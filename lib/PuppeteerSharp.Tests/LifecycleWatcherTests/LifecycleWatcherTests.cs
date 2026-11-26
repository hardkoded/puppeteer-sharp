using System.Reflection;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests.LifecycleWatcherTests;

public class LifecycleWatcherTests
{
    [
        Test,
        PuppeteerTest(
            "LifecycleWatcher.test.ts",
            "LifecycleWatcher",
            "terminates navigation immediately when the page crashes"
        )
    ]
    public async Task TerminatesNavigationImmediatelyWhenThePageCrashes()
    {
        using var connection = CreateConnection();
        var (page, frameManager, frame) = CreateMockFrame(connection);
        await using var _ = frameManager;
        using var watcher = new LifecycleWatcher(
            frameManager.NetworkManager,
            frame,
            [WaitUntilNavigation.Load],
            30_000
        );

        EmitPageError(page, "Page crashed!");

        var exception = Assert.ThrowsAsync<PuppeteerException>(() => watcher.TerminationTask);
        Assert.That(exception.Message, Is.EqualTo("Page crashed!"));
    }

    [
        Test,
        PuppeteerTest(
            "LifecycleWatcher.test.ts",
            "LifecycleWatcher",
            "removes the page error listener when disposed without a crash"
        )
    ]
    public async Task RemovesThePageErrorListenerWhenDisposedWithoutACrash()
    {
        using var connection = CreateConnection();
        var (page, frameManager, frame) = CreateMockFrame(connection);
        await using var _ = frameManager;
        var watcher = new LifecycleWatcher(
            frameManager.NetworkManager,
            frame,
            [WaitUntilNavigation.Load],
            30_000
        );

        Assert.That(page.HasErrorEventListeners, Is.True);
        watcher.Dispose();
        Assert.That(page.HasErrorEventListeners, Is.False);
    }

    private static Connection CreateConnection() =>
        Connection.CreateFromTransport(
            Substitute.For<IConnectionTransport>(),
            new ConnectOptions()
        );

    private static (Page Page, FrameManager FrameManager, CdpFrame Frame) CreateMockFrame(
        Connection connection
    )
    {
        var session = new CdpCDPSession(connection, TargetType.Page, "page", null);
        var page = Substitute.For<Page>((TaskQueue)null);
        var frameManager = new FrameManager(session, page, new TimeoutSettings(), false, false);
        var frame = new CdpFrame(frameManager, "frame-1", null, session);

        return (page, frameManager, frame);
    }

    // Page.Error can only be raised by the page itself.
    private static void EmitPageError(Page page, string error) =>
        typeof(Page)
            .GetMethod("OnError", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(page, [new ErrorEventArgs(error)]);
}
