using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.TargetTests
{
    public class TargetTests : PuppeteerPageBaseTest
    {
        private const int TestTimeout = 10_000;

        public TargetTests()
            : base() { }

        [
            Test,
            PuppeteerTest(
                "target.spec",
                "Target",
                "Browser.targets should return all of the targets"
            )
        ]
        public void BrowserTargetsShouldReturnAllOfTheTargets()
        {
            // The pages will be the testing page and the original newtab page
            var targets = Browser.Targets();
            Assert.That(
                targets.Any(target =>
                    target.Type == TargetType.Page && target.Url == TestConstants.AboutBlank
                ),
                Is.True
            );
            Assert.That(targets.Any(target => target.Type == TargetType.Browser), Is.True);
        }

        [
            Test,
            PuppeteerTest("target.spec", "Target", "Browser.pages should return all of the pages")
        ]
        public async Task BrowserPagesShouldReturnAllOfThePages()
        {
            // The pages will be the testing page and the original newtab page
            var allPages = (await Context.PagesAsync()).ToArray();
            Assert.That(allPages, Has.Exactly(1).Items);
            Assert.That(allPages, Does.Contain(Page));
        }

        [Test, PuppeteerTest("target.spec", "Target", "page should return tab target id")]
        public void PageShouldReturnTabTargetId()
        {
            Assert.That(((Page)Page).TabId.Length, Is.GreaterThan(0));
        }

        [Test, PuppeteerTest("target.spec", "Target", "should contain browser target")]
        public void ShouldContainBrowserTarget()
        {
            var targets = Browser.Targets();
            var browserTarget = targets.FirstOrDefault(target => target.Type == TargetType.Browser);
            Assert.That(browserTarget, Is.Not.Null);
        }

        [
            Test,
            PuppeteerTest(
                "target.spec",
                "Target",
                "should be able to use the default page in the browser"
            )
        ]
        public async Task ShouldBeAbleToUseTheDefaultPageInTheBrowser()
        {
            // The pages will be the testing page and the original newtab page
            await using var browser = await Puppeteer.LaunchAsync(
                TestConstants.DefaultBrowserOptions()
            );
            var allPages = await browser.PagesAsync();
            var originalPage = allPages.First(p => p != Page);
            Assert.That(
                await originalPage.EvaluateExpressionAsync<string>("['Hello', 'world'].join(' ')"),
                Is.EqualTo("Hello world")
            );
            Assert.That(await originalPage.QuerySelectorAsync("body"), Is.Not.Null);
        }

        [
            Test,
            PuppeteerTest(
                "target.spec",
                "Target",
                "should report when a new page is created and closed"
            )
        ]
        public async Task ShouldReportWhenANewPageIsCreatedAndClosed()
        {
            var otherPageTask = Context
                .WaitForTargetAsync(t => t.Url == TestConstants.CrossProcessUrl + "/empty.html")
                .ContinueWith(t => t.Result.PageAsync());

            await Task.WhenAll(
                otherPageTask,
                Page.EvaluateFunctionHandleAsync(
                    "url => window.open(url)",
                    TestConstants.CrossProcessUrl + "/empty.html"
                )
            );

            var otherPage = await otherPageTask.Result;
            Assert.That(otherPage.Url, Does.Contain(TestConstants.CrossProcessUrl));

            Assert.That(
                await otherPage.EvaluateExpressionAsync<string>("['Hello', 'world'].join(' ')"),
                Is.EqualTo("Hello world")
            );
            Assert.That(await otherPage.QuerySelectorAsync("body"), Is.Not.Null);

            var allPages = await Context.PagesAsync();
            Assert.That(allPages, Does.Contain(Page));
            Assert.That(allPages, Does.Contain(otherPage));

            var closePageTaskCompletion = new TaskCompletionSource<IPage>();
            async void TargetDestroyedEventHandler(object sender, TargetChangedArgs e)
            {
                closePageTaskCompletion.SetResult(await e.Target.PageAsync());
                Context.TargetDestroyed -= TargetDestroyedEventHandler;
            }
            Context.TargetDestroyed += TargetDestroyedEventHandler;
            await otherPage.CloseAsync();
            Assert.That(await closePageTaskCompletion.Task, Is.EqualTo(otherPage));

            allPages = await Task.WhenAll(Context.Targets().Select(target => target.PageAsync()));
            Assert.That(allPages, Does.Contain(Page));
            Assert.That(allPages, Does.Not.Contain(otherPage));
        }

        [
            Test,
            PuppeteerTest(
                "target.spec",
                "Target",
                "should report when a service worker is created and destroyed"
            )
        ]
        public async Task ShouldReportWhenAServiceWorkerIsCreatedAndDestroyed()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var createdTargetTaskCompletion = new TaskCompletionSource<ITarget>();
            void TargetCreatedEventHandler(object sender, TargetChangedArgs e)
            {
                createdTargetTaskCompletion.SetResult(e.Target);
                Context.TargetCreated -= TargetCreatedEventHandler;
            }
            Context.TargetCreated += TargetCreatedEventHandler;
            await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/empty/sw.html");

            var createdTarget = await createdTargetTaskCompletion.Task;
            Assert.That(createdTarget.Type, Is.EqualTo(TargetType.ServiceWorker));
            Assert.That(
                createdTarget.Url,
                Is.EqualTo(TestConstants.ServerUrl + "/serviceworkers/empty/sw.js")
            );

            var targetDestroyedTaskCompletion = new TaskCompletionSource<ITarget>();
            void TargetDestroyedEventHandler(object sender, TargetChangedArgs e)
            {
                targetDestroyedTaskCompletion.SetResult(e.Target);
                Context.TargetDestroyed -= TargetDestroyedEventHandler;
            }
            Context.TargetDestroyed += TargetDestroyedEventHandler;
            await Page.EvaluateExpressionAsync(
                "window.registrationPromise.then(registration => registration.unregister())"
            );
            Assert.That(await targetDestroyedTaskCompletion.Task, Is.EqualTo(createdTarget));
        }

        [
            Test,
            PuppeteerTest("target.spec", "Target", "should create a worker from a service worker")
        ]
        public async Task ShouldCreateAWorkerFromAServiceWorker()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/empty/sw.html");

            var target = await Context.WaitForTargetAsync(t => t.Type == TargetType.ServiceWorker);
            var worker = await target.WorkerAsync();
            Assert.That(
                await worker.EvaluateFunctionAsync<string>("() => self.toString()"),
                Is.EqualTo("[object ServiceWorkerGlobalScope]")
            );
        }

        [
            Test,
            PuppeteerTest(
                "target.spec",
                "Target",
                "should emit exceptions on a service worker but not on the page"
            )
        ]
        public async Task ShouldEmitExceptionsOnAServiceWorkerButNotOnThePage()
        {
            if (!PuppeteerTestAttribute.IsCdp)
            {
                Assert.Ignore("RemoteException is only implemented for CDP.");
            }

            await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/empty/sw.html");
            var target = await Context.WaitForTargetAsync(
                t => t.Type == TargetType.ServiceWorker,
                new WaitForOptions(3_000)
            );
            var worker = await target.WorkerAsync();
            var pageExceptions = new List<RemoteException>();
            Page.ExceptionThrown += (_, e) => pageExceptions.Add(e.Exception);
            var tcs = new TaskCompletionSource<RemoteException>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            worker.ExceptionThrown += (_, e) => tcs.TrySetResult(e.Exception);

            await worker.EvaluateFunctionAsync(
                "() => { setTimeout(() => { throw new Error('service worker error'); }); }"
            );

            var exception = await tcs.Task.WithTimeout();
            Assert.That(exception.Text, Does.Contain("service worker error"));
            Assert.That(pageExceptions, Is.Empty);
        }

        [Test, PuppeteerTest("target.spec", "Target", "should close a service worker")]
        public async Task ShouldCloseAServiceWorker()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/empty/sw.html");
            var target = await Context.WaitForTargetAsync(
                t => t.Type == TargetType.ServiceWorker,
                new WaitForOptions(3_000)
            );
            var worker = await target.WorkerAsync();
            var workerDestroyed = new TaskCompletionSource<TargetChangedArgs>();
            Context.TargetDestroyed += (sender, e) => workerDestroyed.TrySetResult(e);
            await worker.CloseAsync();
            Assert.That((await workerDestroyed.Task).Target, Is.SameAs(target));
        }

        [
            Test,
            PuppeteerTest(
                "target.spec",
                "Target",
                "should close a service worker when network rules are set"
            )
        ]
        public async Task ShouldCloseAServiceWorkerWhenNetworkRulesAreSet()
        {
            await using var browser = await LaunchWithNetworkRulesAsync();

            // With network rules, Puppeteer stays attached to service workers, also
            // through the page that registered them.
            var page = await OpenServiceWorkerPageAsync(browser);
            var target = await browser.WaitForTargetAsync(
                t => t.Type == TargetType.ServiceWorker,
                new WaitForOptions(3_000)
            );
            var worker = await target.WorkerAsync();
            var targetDestroyed = new TaskCompletionSource<ITarget>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            browser.TargetDestroyed += (_, e) =>
            {
                if (e.Target == target)
                {
                    targetDestroyed.TrySetResult(e.Target);
                }
            };
            var watcher = await WatchServiceWorkerAsync(page);

            await worker.CloseAsync().WithTimeout(TestTimeout);

            Assert.That(await targetDestroyed.Task.WithTimeout(TestTimeout), Is.SameAs(target));
            await watcher.Destroyed.WithTimeout(TestTimeout);
        }

        [
            Test,
            PuppeteerTest("target.spec", "Target", "should create a worker from a shared worker")
        ]
        public async Task ShouldCreateAWorkerFromASharedWorker()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(
                @"() =>
            {
                new SharedWorker('data:text/javascript,console.log(""hi"")');
            }"
            );
            var target = await Context.WaitForTargetAsync(t => t.Type == TargetType.SharedWorker);
            var worker = await target.WorkerAsync();
            Assert.That(
                await worker.EvaluateFunctionAsync<string>("() => self.toString()"),
                Is.EqualTo("[object SharedWorkerGlobalScope]")
            );
        }

        [Test, PuppeteerTest("target.spec", "Target", "should close a shared worker")]
        public async Task ShouldCloseASharedWorker()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var targetTask = Context.WaitForTargetAsync(
                t => t.Type == TargetType.SharedWorker,
                new WaitForOptions(3_000)
            );
            await Page.EvaluateFunctionAsync(
                @"() => {
                new SharedWorker('data:text/javascript,console.log(""hi2"")');
            }"
            );
            var target = await targetTask;
            var worker = await target.WorkerAsync();
            var workerDestroyed = new TaskCompletionSource<TargetChangedArgs>();
            Context.TargetDestroyed += (sender, e) => workerDestroyed.TrySetResult(e);
            await worker.CloseAsync();
            Assert.That((await workerDestroyed.Task).Target, Is.SameAs(target));
        }

        [Test, PuppeteerTest("target.spec", "Target", "should report when a target url changes")]
        public async Task ShouldReportWhenATargetUrlChanges()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var changedTargetTaskCompletion = new TaskCompletionSource<ITarget>();
            void ChangedTargetEventHandler(object sender, TargetChangedArgs e)
            {
                changedTargetTaskCompletion.SetResult(e.Target);
                Context.TargetChanged -= ChangedTargetEventHandler;
            }
            Context.TargetChanged += ChangedTargetEventHandler;

            await Page.GoToAsync(TestConstants.CrossProcessUrl + "/");
            var changedTarget = await changedTargetTaskCompletion.Task;
            Assert.That(changedTarget.Url, Is.EqualTo(TestConstants.CrossProcessUrl + "/"));

            changedTargetTaskCompletion = new TaskCompletionSource<ITarget>();
            Context.TargetChanged += ChangedTargetEventHandler;
            await Page.GoToAsync(TestConstants.EmptyPage);
            changedTarget = await changedTargetTaskCompletion.Task;
            Assert.That(changedTarget.Url, Is.EqualTo(TestConstants.EmptyPage));
        }

        [Test, PuppeteerTest("target.spec", "Target", "should not report uninitialized pages")]
        public async Task ShouldNotReportUninitializedPages()
        {
            var targetChanged = false;
            void listener(object sender, TargetChangedArgs e) => targetChanged = true;
            Context.TargetChanged += listener;
            var targetCompletionTask = new TaskCompletionSource<ITarget>();
            void TargetCreatedEventHandler(object sender, TargetChangedArgs e)
            {
                targetCompletionTask.SetResult(e.Target);
                Context.TargetCreated -= TargetCreatedEventHandler;
            }
            Context.TargetCreated += TargetCreatedEventHandler;
            var newPageTask = Context.NewPageAsync();
            var target = await targetCompletionTask.Task;
            Assert.That(target.Url, Is.EqualTo(TestConstants.AboutBlank));

            var newPage = await newPageTask;
            targetCompletionTask = new TaskCompletionSource<ITarget>();
            Context.TargetCreated += TargetCreatedEventHandler;
            var evaluateTask = newPage.EvaluateExpressionHandleAsync("window.open('about:blank')");
            var target2 = await targetCompletionTask.Task;
            Assert.That(target2.Url, Is.EqualTo(TestConstants.AboutBlank));
            await evaluateTask;
            await newPage.CloseAsync();
            Assert.That(targetChanged, Is.False, "target should not be reported as changed");
            Context.TargetChanged -= listener;
        }

        [
            Test,
            PuppeteerTest(
                "target.spec",
                "Target",
                "should not crash while redirecting if original request was missed"
            )
        ]
        public async Task ShouldNotCrashWhileRedirectingIfOriginalRequestWasMissed()
        {
            var serverResponseTcs = new TaskCompletionSource<HttpResponse>();
            var serverResponseEnd = new TaskCompletionSource<bool>();
            Server.SetRoute("/one-style.css", context =>
            {
                serverResponseTcs.TrySetResult(context.Response);
                return serverResponseEnd.Task;
            });
            // Open a new page. Use window.open to connect to the page later.
            await Task.WhenAll(
                Page.EvaluateFunctionHandleAsync(
                    "url => window.open(url)",
                    TestConstants.ServerUrl + "/one-style.html"
                ),
                Server.WaitForRequest("/one-style.css")
            );
            // Connect to the opened page.
            var target = await Context.WaitForTargetAsync(t => t.Url.Contains("one-style.html"));
            var newPage = await target.PageAsync();
            var loadEvent = new TaskCompletionSource<bool>();
            newPage.Load += (_, _) => loadEvent.TrySetResult(true);
            // Issue a redirect.
            var serverResponse = await serverResponseTcs.Task;
            serverResponse.Redirect("/injectedstyle.css");
            serverResponseEnd.SetResult(true);
            // Wait for the new page to load.
            await loadEvent.Task;
            // Cleanup.
            await newPage.CloseAsync();
        }

        [Test, PuppeteerTest("target.spec", "Target", "should have an opener")]
        public async Task ShouldHaveAnOpener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var targetCreatedCompletion = new TaskCompletionSource<ITarget>();
            Browser.TargetCreated += (_, e) => targetCreatedCompletion.TrySetResult(e.Target);
            await Page.GoToAsync(TestConstants.ServerUrl + "/popup/window-open.html");
            var createdTarget = await targetCreatedCompletion.Task;

            Assert.That(
                (await createdTarget.PageAsync()).Url,
                Is.EqualTo(TestConstants.ServerUrl + "/popup/popup.html")
            );
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.That(createdTarget.Opener, Is.SameAs(Page.Target));
            Assert.That(Page.Target.Opener, Is.Null);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Test, PuppeteerTest("target.spec", "Target Target.close", "should close the target")]
        public async Task ShouldCloseTheTarget()
        {
            var page = await Context.NewPageAsync();
#pragma warning disable CS0618 // Type or member is obsolete
            var target = page.Target;
#pragma warning restore CS0618 // Type or member is obsolete
            var targetDestroyed = WaitForTargetDestroyedAsync(target);

            await target.CloseAsync().WithTimeout(TestTimeout);

            Assert.That(Context.Targets(), Does.Not.Contain(target));
            Assert.That(await targetDestroyed.WithTimeout(TestTimeout), Is.SameAs(target));
            Assert.That(await Context.PagesAsync(), Does.Not.Contain(page));
        }

        [
            Test,
            PuppeteerTest(
                "target.spec",
                "Target Target.close",
                "should close a service worker target"
            )
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
            var targetDestroyed = WaitForTargetDestroyedAsync(target);

            await target.CloseAsync().WithTimeout(TestTimeout);

            Assert.That(Context.Targets(), Does.Not.Contain(target));
            Assert.That(await targetDestroyed.WithTimeout(TestTimeout), Is.SameAs(target));
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
            await using var browser = await LaunchWithNetworkRulesAsync();

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
                if (e.Target == target)
                {
                    Interlocked.Increment(ref destroyedCount);
                }
            };
            var watcher = await WatchServiceWorkerAsync(page);

            await target.CloseAsync().WithTimeout(TestTimeout);

            Assert.That(browser.Targets(), Does.Not.Contain(target));
            await watcher.Destroyed.WithTimeout(TestTimeout);
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
            await using var browser = await LaunchWithNetworkRulesAsync();

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
            var watcher = await WatchServiceWorkerAsync(page);

            await target.CloseAsync().WithTimeout(TestTimeout);

            await watcher.Destroyed.WithTimeout(TestTimeout);
        }

        [
            Test,
            PuppeteerTest(
                "target.spec",
                "Target Target.close",
                "should close a shared worker target"
            )
        ]
        public async Task ShouldCloseASharedWorkerTarget()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(
                @"() => {
                new SharedWorker('data:text/javascript,console.log(""hi"")');
            }"
            );
            var target = await Context.WaitForTargetAsync(
                t => t.Type == TargetType.SharedWorker,
                new WaitForOptions(3_000)
            );
            var targetDestroyed = WaitForTargetDestroyedAsync(target);

            await target.CloseAsync().WithTimeout(TestTimeout);

            Assert.That(Context.Targets(), Does.Not.Contain(target));
            Assert.That(await targetDestroyed.WithTimeout(TestTimeout), Is.SameAs(target));
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
                @"() => {
                new SharedWorker('data:text/javascript,console.log(""hi"")');
            }"
            );
            var target = await Context.WaitForTargetAsync(
                t => t.Type == TargetType.SharedWorker,
                new WaitForOptions(3_000)
            );

            // A shared worker is gone before the response to `Target.closeTarget`,
            // so a second `Target.closeTarget` would fail.
            await Task.WhenAll(target.CloseAsync(), target.CloseAsync()).WithTimeout(TestTimeout);

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
            var watcher = await WatchServiceWorkerAsync(Page);

            // A session that `worker()` attached while `close()` runs would keep the
            // closed service worker alive.
            var closed = target.CloseAsync();
            Assert.ThrowsAsync<TargetClosedException>(() => target.WorkerAsync());
            await closed.WithTimeout(TestTimeout);

            await watcher.Destroyed.WithTimeout(TestTimeout);
        }

        [
            Test,
            PuppeteerTest(
                "target.spec",
                "Target Target.close",
                "should do nothing if the target is already closed"
            )
        ]
        public async Task ShouldDoNothingIfTheTargetIsAlreadyClosed()
        {
            var page = await Context.NewPageAsync();
#pragma warning disable CS0618 // Type or member is obsolete
            var target = page.Target;
#pragma warning restore CS0618 // Type or member is obsolete
            await target.CloseAsync().WithTimeout(TestTimeout);

            await target.CloseAsync().WithTimeout(TestTimeout);

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
        public async Task ShouldCloseAPageTargetWithoutCreatingItsPage()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.ProtocolTimeout = 3000;
            await using var browser = await Puppeteer.LaunchAsync(
                options,
                TestConstants.LoggerFactory
            );
            var page = await browser.NewPageAsync();
#pragma warning disable CS0618 // Type or member is obsolete
            var targetTask = browser.WaitForTargetAsync(t => t.Opener == page.Target);
#pragma warning restore CS0618 // Type or member is obsolete
            await page.EvaluateFunctionAsync(
                @"() => {
                window.open('about:blank', 'popup');
            }"
            );
            var target = await targetTask;

            // The alert blocks creating a page for the popup until the protocol
            // timeout, so closing the target must not create one.
            var session = await target.CreateCDPSessionAsync();
            await session.SendAsync("Page.enable");
            var dialogOpening = WaitEvent(session, "Page.javascriptDialogOpening");
            await page.EvaluateFunctionAsync(
                @"() => {
                const popup = window.open('', 'popup');
                popup.setTimeout(() => {
                    popup.alert('blocked');
                }, 0);
            }"
            );
            await dialogOpening.WithTimeout(TestTimeout);

            await target.CloseAsync().WithTimeout(TestTimeout);

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

            // Chrome rejects `Target.closeTarget` for the browser target, and
            // chrome-headless-shell reports that it did not close it.
            var exception = await CatchCloseExceptionAsync(browser.Target);

            if (PuppeteerTestAttribute.IsCdp)
            {
                Assert.That(exception, Is.InstanceOf<PuppeteerException>());
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
            await using var browser = await Puppeteer.LaunchAsync(
                options,
                TestConstants.LoggerFactory
            );
            var page = await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            var frameUrl = TestConstants.CrossProcessUrl + "/empty.html";
            await FrameUtils.AttachFrameAsync(page, "frame1", frameUrl);
            var target = await browser.WaitForTargetAsync(t => t.Url == frameUrl);

            var exception = await CatchCloseExceptionAsync(target);

            if (PuppeteerTestAttribute.IsCdp)
            {
                Assert.That(exception, Is.InstanceOf<PuppeteerException>());
                Assert.That(
                    exception.Message,
                    Does.Contain("The target of a frame does not support closing")
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

        // Awaited instead of using Assert.ThrowsAsync, which blocks the thread. The code that follows a
        // launch can run on the thread that reads protocol messages, so blocking it can deadlock.
        private static async Task<Exception> CatchCloseExceptionAsync(ITarget target)
        {
            try
            {
                await target.CloseAsync().WithTimeout(TestTimeout);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private static async Task<IBrowser> LaunchWithNetworkRulesAsync()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.BlockList = ["*://*:*/empty.html"];
            return await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        }

        /// <summary>
        /// Opens a page that registers a service worker and waits until the service
        /// worker is ready, which gives Puppeteer time to attach to it through the page.
        /// </summary>
        private static async Task<IPage> OpenServiceWorkerPageAsync(IBrowser browser)
        {
            var page = await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/empty/sw.html");
            await page.EvaluateFunctionAsync(
                @"async () => {
                await navigator.serviceWorker.ready;
            }"
            );
            return page;
        }

        /// <summary>
        /// Watches the targets of the browser through a CDP session of the page.
        /// The returned <c>Destroyed</c> task completes when Chrome reports the service worker
        /// as destroyed, which it does only after all sessions attached to it detach.
        /// </summary>
        private static async Task<ServiceWorkerWatcher> WatchServiceWorkerAsync(IPage page)
        {
            var session = await page.CreateCDPSessionAsync();
            var destroyed = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            string targetId = null;
            session.MessageReceived += (_, e) =>
            {
                if (e.MessageID == "Target.targetCreated")
                {
                    var targetInfo = e.MessageData.GetProperty("targetInfo");
                    if (targetInfo.GetProperty("type").GetString() == "service_worker")
                    {
                        targetId = targetInfo.GetProperty("targetId").GetString();
                    }
                }
                else if (
                    e.MessageID == "Target.targetDestroyed"
                    && e.MessageData.GetProperty("targetId").GetString() == targetId
                )
                {
                    destroyed.TrySetResult(true);
                }
            };
            await session.SendAsync(
                "Target.setDiscoverTargets",
                new Dictionary<string, object> { ["discover"] = true }
            );
            return new ServiceWorkerWatcher(destroyed.Task);
        }

        private Task<ITarget> WaitForTargetDestroyedAsync(ITarget target)
        {
            var targetDestroyed = new TaskCompletionSource<ITarget>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            Context.TargetDestroyed += (_, e) =>
            {
                if (e.Target == target)
                {
                    targetDestroyed.TrySetResult(e.Target);
                }
            };
            return targetDestroyed.Task;
        }

        private sealed record ServiceWorkerWatcher(Task Destroyed);
    }
}
