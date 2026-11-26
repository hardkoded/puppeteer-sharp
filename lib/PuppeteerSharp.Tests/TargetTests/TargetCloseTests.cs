using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.TargetTests;

public class TargetCloseTests : PuppeteerPageBaseTest
{
    [Test, PuppeteerTest("target.spec", "Target Target.close", "should close the target")]
    [Obsolete]
    public async Task ShouldCloseTheTarget()
    {
        var page = await Context.NewPageAsync();
        var target = page.Target;
        var targetDestroyed = WaitForTargetDestroyed(Context, target);

        await target.CloseAsync();

        Assert.That(Context.Targets(), Does.Not.Contain(target));
        Assert.That(await targetDestroyed, Is.SameAs(target));
        Assert.That(await Context.PagesAsync(), Does.Not.Contain(page));
    }

    [
        Test,
        PuppeteerTest("target.spec", "Target Target.close", "should close a service worker target")
    ]
    public async Task ShouldCloseAServiceWorkerTarget()
    {
        await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/empty/sw.html");
        var target = await Context.WaitForTargetAsync(
            t => t.Type == TargetType.ServiceWorker,
            new WaitForOptions(3_000)
        );

        // Creating the worker attaches Puppeteer to the service worker. Chrome
        // reports the closed service worker as destroyed only after the detach.
        await target.WorkerAsync();
        var targetDestroyed = WaitForTargetDestroyed(Context, target);

        await target.CloseAsync();

        Assert.That(Context.Targets(), Does.Not.Contain(target));
        Assert.That(await targetDestroyed, Is.SameAs(target));
    }

    [
        Test,
        PuppeteerTest(
            "target.spec",
            "Target Target.close",
            "should close a service worker target used by two pages when network rules are set"
        )
    ]
    public async Task ShouldCloseAServiceWorkerTargetUsedByTwoPagesWhenNetworkRulesAreSet()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/empty.html"];
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);

        // With network rules, Puppeteer stays attached to service workers,
        // also through each page that uses them.
        var page = await OpenServiceWorkerPageAsync(browser);
        await OpenServiceWorkerPageAsync(browser);
        var target = await browser.WaitForTargetAsync(
            t => t.Type == TargetType.ServiceWorker,
            new WaitForOptions(3_000)
        );
        var destroyedCount = 0;
        browser.TargetDestroyed += (_, e) =>
        {
            if (ReferenceEquals(e.Target, target))
            {
                Interlocked.Increment(ref destroyedCount);
            }
        };
        var destroyed = await WatchServiceWorkerAsync(page);

        await target.CloseAsync();

        Assert.That(browser.Targets(), Does.Not.Contain(target));
        await destroyed;
        Assert.That(destroyedCount, Is.EqualTo(1));
    }

    [
        Test,
        PuppeteerTest(
            "target.spec",
            "Target Target.close",
            "should close a service worker target after a page that used it closed when network rules are set"
        )
    ]
    public async Task ShouldCloseAServiceWorkerTargetAfterAPageThatUsedItClosedWhenNetworkRulesAreSet()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://*:*/empty.html"];
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);

        var page = await OpenServiceWorkerPageAsync(browser);

        // Puppeteer reports the service worker as gone when a page that uses
        // it closes, and creates a new target for it when another page
        // attaches. Puppeteer still keeps the sessions of the old target.
        await (await OpenServiceWorkerPageAsync(browser)).CloseAsync();
        await OpenServiceWorkerPageAsync(browser);
        var target = await browser.WaitForTargetAsync(
            t => t.Type == TargetType.ServiceWorker,
            new WaitForOptions(3_000)
        );
        var destroyed = await WatchServiceWorkerAsync(page);

        await target.CloseAsync();

        await destroyed;
    }

    [
        Test,
        PuppeteerTest("target.spec", "Target Target.close", "should close a shared worker target")
    ]
    public async Task ShouldCloseASharedWorkerTarget()
    {
        await Page.GoToAsync(TestConstants.EmptyPage);
        await Page.EvaluateFunctionAsync(
            "() => { new SharedWorker('data:text/javascript,console.log(\"hi\")'); }"
        );
        var target = await Context.WaitForTargetAsync(
            t => t.Type == TargetType.SharedWorker,
            new WaitForOptions(3_000)
        );
        var targetDestroyed = WaitForTargetDestroyed(Context, target);

        await target.CloseAsync();

        Assert.That(Context.Targets(), Does.Not.Contain(target));
        Assert.That(await targetDestroyed, Is.SameAs(target));
    }

    [
        Test,
        PuppeteerTest(
            "target.spec",
            "Target Target.close",
            "should not throw if close() is called concurrently"
        )
    ]
    public async Task ShouldNotThrowIfCloseIsCalledConcurrently()
    {
        await Page.GoToAsync(TestConstants.EmptyPage);
        await Page.EvaluateFunctionAsync(
            "() => { new SharedWorker('data:text/javascript,console.log(\"hi\")'); }"
        );
        var target = await Context.WaitForTargetAsync(
            t => t.Type == TargetType.SharedWorker,
            new WaitForOptions(3_000)
        );

        // A shared worker is gone before the response to Target.closeTarget,
        // so a second Target.closeTarget would fail.
        await Task.WhenAll(target.CloseAsync(), target.CloseAsync());

        Assert.That(Context.Targets(), Does.Not.Contain(target));
    }

    [
        Test,
        PuppeteerTest(
            "target.spec",
            "Target Target.close",
            "should reject worker() while a service worker target closes"
        )
    ]
    public async Task ShouldRejectWorkerWhileAServiceWorkerTargetCloses()
    {
        await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/empty/sw.html");
        var target = await Context.WaitForTargetAsync(
            t => t.Type == TargetType.ServiceWorker,
            new WaitForOptions(3_000)
        );
        var destroyed = await WatchServiceWorkerAsync(Page);

        // A session that WorkerAsync() attached while CloseAsync() runs would keep the
        // closed service worker alive.
        var closed = target.CloseAsync();
        Assert.ThrowsAsync<TargetClosedException>(() => target.WorkerAsync());
        await closed;

        await destroyed;
    }

    [
        Test,
        PuppeteerTest(
            "target.spec",
            "Target Target.close",
            "should do nothing if the target is already closed"
        )
    ]
    [Obsolete]
    public async Task ShouldDoNothingIfTheTargetIsAlreadyClosed()
    {
        var page = await Context.NewPageAsync();
        var target = page.Target;
        await target.CloseAsync();

        await target.CloseAsync();

        Assert.That(Context.Targets(), Does.Not.Contain(target));
    }

    [
        Test,
        PuppeteerTest(
            "target.spec",
            "Target Target.close",
            "should close a page target without creating its page"
        )
    ]
    [Obsolete]
    public async Task ShouldCloseAPageTargetWithoutCreatingItsPage()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.ProtocolTimeout = 3000;
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);

        var page = await browser.NewPageAsync();
        var targetTask = browser.WaitForTargetAsync(t => ReferenceEquals(t.Opener, page.Target));
        await page.EvaluateFunctionAsync("() => { window.open('about:blank', 'popup'); }");
        var target = await targetTask;

        // The alert blocks creating a page for the popup until the protocol
        // timeout, so closing the target must not create one.
        var session = await target.CreateCDPSessionAsync();
        await session.SendAsync("Page.enable");
        var dialogOpening = new TaskCompletionSource<bool>();
        session.MessageReceived += (_, e) =>
        {
            if (e.MessageID == "Page.javascriptDialogOpening")
            {
                dialogOpening.TrySetResult(true);
            }
        };
        await page.EvaluateFunctionAsync(
            @"() => {
                const popup = window.open('', 'popup');
                popup.setTimeout(() => {
                    popup.alert('blocked');
                }, 0);
            }"
        );
        await dialogOpening.Task;

        await target.CloseAsync();

        Assert.That(browser.Targets(), Does.Not.Contain(target));
    }

    [
        Test,
        PuppeteerTest(
            "target.spec",
            "Target Target.close",
            "should throw if the target does not support closing"
        )
    ]
    public async Task ShouldThrowIfTheTargetDoesNotSupportClosing()
    {
        await using var browser = await Puppeteer.LaunchAsync(
            TestConstants.DefaultBrowserOptions(),
            TestConstants.LoggerFactory
        );

        // Chrome rejects Target.closeTarget for the browser target, and
        // chrome-headless-shell reports that it did not close it.
        var exception = await CatchAsync(() => browser.Target.CloseAsync());
        if (PuppeteerTestAttribute.IsCdp)
        {
            Assert.That(exception.Message, Does.Contain("support closing"));
        }
        else
        {
            Assert.That(exception, Is.InstanceOf<NotSupportedException>());
        }

        Assert.That(browser.IsConnected, Is.True);
    }

    [
        Test,
        PuppeteerTest(
            "target.spec",
            "Target Target.close",
            "should not close the page through the target of a frame"
        )
    ]
    public async Task ShouldNotCloseThePageThroughTheTargetOfAFrame()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.Args = [.. options.Args ?? [], "--site-per-process"];
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);

        var page = await browser.NewPageAsync();
        await page.GoToAsync(TestConstants.EmptyPage);
        var frameUrl = TestConstants.CrossProcessUrl + "/empty.html";
        await FrameUtils.AttachFrameAsync(page, "frame1", frameUrl);
        var target = await browser.WaitForTargetAsync(t => t.Url == frameUrl);

        var exception = await CatchAsync(() => target.CloseAsync());
        if (PuppeteerTestAttribute.IsCdp)
        {
            Assert.That(
                exception.Message,
                Is.EqualTo("The target of a frame does not support closing")
            );
        }
        else
        {
            Assert.That(exception, Is.InstanceOf<NotSupportedException>());
        }

        Assert.That(page.IsClosed, Is.False);
        Assert.That(
            await page.EvaluateFunctionAsync<int>(
                "() => document.querySelectorAll('iframe').length"
            ),
            Is.EqualTo(1)
        );
    }

    /// <summary>
    /// Opens a page that registers a service worker and waits until the service
    /// worker is ready, which gives Puppeteer time to attach to it through the page.
    /// </summary>
    internal static async Task<IPage> OpenServiceWorkerPageAsync(IBrowser browser)
    {
        var page = await browser.NewPageAsync();
        await page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/empty/sw.html");
        await page.EvaluateFunctionAsync("async () => { await navigator.serviceWorker.ready; }");
        return page;
    }

    /// <summary>
    /// Watches the targets of the browser through a CDP session of the page.
    /// The returned task completes when Chrome reports the service worker as destroyed,
    /// which it does only after all sessions attached to it detach.
    /// </summary>
    internal static async Task<Task> WatchServiceWorkerAsync(IPage page)
    {
        var session = await page.CreateCDPSessionAsync();
        var destroyed = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        string targetId = null;
        session.MessageReceived += (_, e) =>
        {
            switch (e.MessageID)
            {
                case "Target.targetCreated"
                    when e.MessageData.GetProperty("targetInfo").GetProperty("type").GetString()
                        == "service_worker":
                    targetId = e.MessageData
                        .GetProperty("targetInfo")
                        .GetProperty("targetId")
                        .GetString();
                    break;
                case "Target.targetDestroyed"
                    when e.MessageData.GetProperty("targetId").GetString() == targetId:
                    destroyed.TrySetResult(true);
                    break;
            }
        };
        await session.SendAsync("Target.setDiscoverTargets", new { discover = true });
        return destroyed.Task;
    }

    private static async Task<Exception> CatchAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            return ex;
        }

        Assert.Fail("Expected an exception");
        return null;
    }

    private static Task<ITarget> WaitForTargetDestroyed(IBrowserContext context, ITarget target)
    {
        var tcs = new TaskCompletionSource<ITarget>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        context.TargetDestroyed += (_, e) =>
        {
            if (ReferenceEquals(e.Target, target))
            {
                tcs.TrySetResult(e.Target);
            }
        };
        return tcs.Task;
    }
}
