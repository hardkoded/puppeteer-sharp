using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Transport;

#pragma warning disable CA2000 // The mock browser contexts hold no resources, and disposing one would call CloseAsync.

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class BrowserScreenshotMutexTests
    {
        [
            Test,
            PuppeteerTest(
                "Browser.test",
                "Browser screenshot mutex",
                "serializes screenshot operations across separate BrowserContexts"
            )
        ]
        public async Task SerializesScreenshotOperationsAcrossSeparateBrowserContexts()
        {
            var browser = Substitute.ForPartsOf<Browser>();
            var context1 = new MockBrowserContext(browser);
            var context2 = new MockBrowserContext(browser);

            AssertNoScreenshotOperations(context1);
            AssertNoScreenshotOperations(context2);

            var order = new List<string>();

            var guard1Task = context1.StartScreenshotAsync();
            var guard2Task = TrackAsync(
                context2.StartScreenshotAsync(),
                order,
                "screenshot-2-acquired"
            );
            var waitInContext1 = DisposeAfterAcquireAsync(
                TrackAsync(context1.WaitForScreenshotOperationsAsync(), order, "waiter-acquired")
            );

            var guard1 = await guard1Task;
            order.Add("screenshot-1-acquired");
            Assert.That(order, Is.EqualTo(new[] { "screenshot-1-acquired" }));

            guard1.Dispose();

            var guard2 = await guard2Task;
            Assert.That(
                order,
                Is.EqualTo(new[] { "screenshot-1-acquired", "screenshot-2-acquired" })
            );

            guard2.Dispose();
            await waitInContext1;
            Assert.That(
                order,
                Is.EqualTo(
                    new[] { "screenshot-1-acquired", "screenshot-2-acquired", "waiter-acquired" }
                )
            );

            AssertNoScreenshotOperations(context1);
            AssertNoScreenshotOperations(context2);
        }

        [
            Test,
            PuppeteerTest(
                "Browser.test",
                "Browser screenshot mutex",
                "waits for an active waitForScreenshotOperations guard before starting a new screenshot"
            )
        ]
        public async Task WaitsForAnActiveWaitForScreenshotOperationsGuardBeforeStartingANewScreenshot()
        {
            var browser = Substitute.ForPartsOf<Browser>();
            var context1 = new MockBrowserContext(browser);
            var context2 = new MockBrowserContext(browser);

            var order = new List<string>();

            var guard1 = await context1.StartScreenshotAsync();
            var waiterGuardTask = TrackAsync(
                context1.WaitForScreenshotOperationsAsync(),
                order,
                "waiter-acquired"
            );

            guard1.Dispose();
            var waiterGuard = await waiterGuardTask;
            Assert.That(order, Is.EqualTo(new[] { "waiter-acquired" }));

            var guard2Task = TrackAsync(
                context2.StartScreenshotAsync(),
                order,
                "screenshot-2-acquired"
            );

            await Task.Delay(50);
            Assert.That(order, Is.EqualTo(new[] { "waiter-acquired" }));

            order.Add("waiter-disposed");
            waiterGuard.Dispose();

            var guard2 = await guard2Task;
            Assert.That(
                order,
                Is.EqualTo(new[] { "waiter-acquired", "waiter-disposed", "screenshot-2-acquired" })
            );

            guard2.Dispose();
            AssertNoScreenshotOperations(context1);
            AssertNoScreenshotOperations(context2);
        }

        [
            Test,
            PuppeteerTest(
                "Browser.test",
                "Browser screenshot mutex",
                "serializes queued waiters during a screenshot without chaining new waiters afterward"
            )
        ]
        public async Task SerializesQueuedWaitersDuringAScreenshotWithoutChainingNewWaitersAfterward()
        {
            var browser = Substitute.ForPartsOf<Browser>();
            var context1 = new MockBrowserContext(browser);
            var context2 = new MockBrowserContext(browser);

            var order = new List<string>();

            var guard1 = await context1.StartScreenshotAsync();
            var waiter1Task = TrackAsync(
                context1.WaitForScreenshotOperationsAsync(),
                order,
                "waiter-1-acquired"
            );
            var waiter2Task = TrackAsync(
                context2.WaitForScreenshotOperationsAsync(),
                order,
                "waiter-2-acquired"
            );

            guard1.Dispose();

            var waiter1 = await waiter1Task;
            Assert.That(order, Is.EqualTo(new[] { "waiter-1-acquired" }));

            // Once screenshots finish, newly arriving waiters do not chain behind active waiters.
            AssertNoScreenshotOperations(context1);

            // A new screenshot still waits until all queued waiters dispose.
            var guard2Task = TrackAsync(
                context2.StartScreenshotAsync(),
                order,
                "screenshot-2-acquired"
            );

            await Task.Delay(50);
            Assert.That(order, Is.EqualTo(new[] { "waiter-1-acquired" }));

            waiter1.Dispose();
            var waiter2 = await waiter2Task;
            Assert.That(order, Is.EqualTo(new[] { "waiter-1-acquired", "waiter-2-acquired" }));

            await Task.Delay(50);
            Assert.That(order, Is.EqualTo(new[] { "waiter-1-acquired", "waiter-2-acquired" }));

            waiter2.Dispose();
            var guard2 = await guard2Task;
            Assert.That(
                order,
                Is.EqualTo(
                    new[] { "waiter-1-acquired", "waiter-2-acquired", "screenshot-2-acquired" }
                )
            );

            guard2.Dispose();
            AssertNoScreenshotOperations(context1);
        }

        [
            Test,
            PuppeteerTest(
                "puppeteer-sharp",
                "Browser screenshot mutex",
                "should not deadlock when page creation fails while a screenshot is queued"
            )
        ]
        public async Task ShouldNotDeadlockWhenPageCreationFailsWhileAScreenshotIsQueued()
        {
            var timeout = TimeSpan.FromSeconds(10);
            var options = TestConstants.DefaultBrowserOptions();
            options.TransportFactory = async (url, connectionOptions, cancellationToken) =>
                new FailingPageInitializationTransport(
                    await WebSocketTransport.DefaultTransportFactory(
                        url,
                        connectionOptions,
                        cancellationToken
                    )
                );

            await using var browser = (Browser)
                await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            var pageTargetDestroyed = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            browser.TargetDestroyed += (_, e) =>
            {
                if (e.Target.Type == TargetType.Page)
                {
                    pageTargetDestroyed.TrySetResult(true);
                }
            };

            // The screenshot guards are taken by hand to get a deterministic order:
            // a screenshot is running, a new page waits for it and a second screenshot queues up.
            var screenshot1 = await browser.StartScreenshotAsync();
            var newPageTask = browser.NewPageAsync();
            var screenshot2Task = browser.StartScreenshotAsync();
            screenshot1.Dispose();

            // Don't use Assert.ThrowsAsync here. It blocks the thread, which can be the one reading
            // the protocol messages.
            Exception exception = null;
            try
            {
                await newPageTask.WithTimeout(timeout);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.That(exception, Is.InstanceOf<MessageException>());

            var screenshot2 = await screenshot2Task.WithTimeout(timeout);
            screenshot2.Dispose();
            await pageTargetDestroyed.Task.WithTimeout(timeout);
        }

        private static void AssertNoScreenshotOperations(BrowserContext context)
        {
            var task = context.WaitForScreenshotOperationsAsync();
            Assert.That(task.IsCompleted, Is.True);
            Assert.That(task.Result, Is.Null);
        }

        private static async Task<IDisposable> TrackAsync(
            Task<IDisposable> guardTask,
            List<string> order,
            string entry
        )
        {
            var guard = await guardTask;
            order.Add(entry);
            return guard;
        }

        private static async Task DisposeAfterAcquireAsync(Task<IDisposable> guardTask)
        {
            var guard = await guardTask;
            guard.Dispose();
        }

        // Makes Chrome reject one of the commands that initialize a page.
        private sealed class FailingPageInitializationTransport : IConnectionTransport
        {
            private readonly IConnectionTransport _transport;

            public FailingPageInitializationTransport(IConnectionTransport transport) =>
                _transport = transport;

            public event EventHandler<TransportClosedEventArgs> Closed
            {
                add => _transport.Closed += value;
                remove => _transport.Closed -= value;
            }

            public event EventHandler<MessageReceivedEventArgs> MessageReceived
            {
                add => _transport.MessageReceived += value;
                remove => _transport.MessageReceived -= value;
            }

            public bool IsClosed => _transport.IsClosed;

            public void StopReading() => _transport.StopReading();

            public Task SendAsync(byte[] message) =>
                _transport.SendAsync(
                    Encoding.UTF8.GetBytes(
                        Encoding
                            .UTF8.GetString(message)
                            .Replace("Performance.enable", "Performance.unknownMethod")
                    )
                );

            public void Dispose() => _transport.Dispose();
        }

        private sealed class MockBrowserContext : BrowserContext
        {
            public MockBrowserContext(Browser browser) => Browser = browser;

            public override ITarget[] Targets() => throw new NotSupportedException();

            public override Task<IPage[]> PagesAsync(bool includeAll = false) =>
                throw new NotSupportedException();

            public override Task<IPage> NewPageAsync(CreatePageOptions options = null) =>
                throw new NotSupportedException();

            public override Task CloseAsync() => throw new NotSupportedException();

            public override Task OverridePermissionsAsync(
                string origin,
                IEnumerable<OverridePermission> permissions
            ) => throw new NotSupportedException();

            public override Task SetPermissionAsync(
                string origin,
                params PermissionEntry[] permissions
            ) => throw new NotSupportedException();

            public override Task ClearPermissionOverridesAsync() =>
                throw new NotSupportedException();

            public override Task<CookieParam[]> GetCookiesAsync() =>
                throw new NotSupportedException();

            public override Task SetCookieAsync(params CookieData[] cookies) =>
                throw new NotSupportedException();
        }
    }
}
