using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.TargetTests
{
    public class TargetTests : PuppeteerPageBaseTest
    {
        public TargetTests() : base()
        {
        }

        [PuppeteerTest("target.spec.ts", "Target", "Browser.targets should return all of the targets")]
        [PuppeteerTimeout]
        public void BrowserTargetsShouldReturnAllOfTheTargets()
        {
            // The pages will be the testing page and the original newtab page
            var targets = Browser.Targets();
            Assert.True(targets.Any(target => target.Type == TargetType.Page
                && target.Url == TestConstants.AboutBlank));
            Assert.True(targets.Any(target => target.Type == TargetType.Browser));
        }

        [PuppeteerTest("target.spec.ts", "Target", "Browser.pages should return all of the pages")]
        [PuppeteerTimeout]
        public async Task BrowserPagesShouldReturnAllOfThePages()
        {
            // The pages will be the testing page and the original newtab page
            var allPages = (await Context.PagesAsync()).ToArray();
            Assert.That(allPages, Has.Exactly(1).Items);
            Assert.Contains(Page, allPages);
        }

        [PuppeteerTest("target.spec.ts", "Target", "should contain browser target")]
        [PuppeteerTimeout]
        public void ShouldContainBrowserTarget()
        {
            var targets = Browser.Targets();
            var browserTarget = targets.FirstOrDefault(target => target.Type == TargetType.Browser);
            Assert.NotNull(browserTarget);
        }

        [PuppeteerTest("target.spec.ts", "Target", "should be able to use the default page in the browser")]
        [PuppeteerTimeout]
        public async Task ShouldBeAbleToUseTheDefaultPageInTheBrowser()
        {
            // The pages will be the testing page and the original newtab page
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var allPages = await browser.PagesAsync();
            var originalPage = allPages.First(p => p != Page);
            Assert.AreEqual("Hello world", await originalPage.EvaluateExpressionAsync<string>("['Hello', 'world'].join(' ')"));
            Assert.NotNull(await originalPage.QuerySelectorAsync("body"));
        }

        [PuppeteerTest("target.spec.ts", "Target", "should report when a new page is created and closed")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldReportWhenANewPageIsCreatedAndClosed()
        {
            var otherPageTask = Context.WaitForTargetAsync(t => t.Url == TestConstants.CrossProcessUrl + "/empty.html")
                .ContinueWith(t => t.Result.PageAsync());

            await Task.WhenAll(
                otherPageTask,
                Page.EvaluateFunctionHandleAsync("url => window.open(url)", TestConstants.CrossProcessUrl + "/empty.html")
                );

            var otherPage = await otherPageTask.Result;
            StringAssert.Contains(TestConstants.CrossProcessUrl, otherPage.Url);

            Assert.AreEqual("Hello world", await otherPage.EvaluateExpressionAsync<string>("['Hello', 'world'].join(' ')"));
            Assert.NotNull(await otherPage.QuerySelectorAsync("body"));

            var allPages = await Context.PagesAsync();
            Assert.Contains(Page, allPages);
            Assert.Contains(otherPage, allPages);

            var closePageTaskCompletion = new TaskCompletionSource<IPage>();
            async void TargetDestroyedEventHandler(object sender, TargetChangedArgs e)
            {
                closePageTaskCompletion.SetResult(await e.Target.PageAsync());
                Context.TargetDestroyed -= TargetDestroyedEventHandler;
            }
            Context.TargetDestroyed += TargetDestroyedEventHandler;
            await otherPage.CloseAsync();
            Assert.AreEqual(otherPage, await closePageTaskCompletion.Task);

            allPages = await Task.WhenAll(Context.Targets().Select(target => target.PageAsync()));
            Assert.Contains(Page, allPages);
            Assert.That(allPages, Does.Not.Contain(otherPage));
        }

        [PuppeteerTest("target.spec.ts", "Target", "should report when a service worker is created and destroyed")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Assert.AreEqual(TargetType.ServiceWorker, createdTarget.Type);
            Assert.AreEqual(TestConstants.ServerUrl + "/serviceworkers/empty/sw.js", createdTarget.Url);

            var targetDestroyedTaskCompletion = new TaskCompletionSource<ITarget>();
            void TargetDestroyedEventHandler(object sender, TargetChangedArgs e)
            {
                targetDestroyedTaskCompletion.SetResult(e.Target);
                Context.TargetDestroyed -= TargetDestroyedEventHandler;
            }
            Context.TargetDestroyed += TargetDestroyedEventHandler;
            await Page.EvaluateExpressionAsync("window.registrationPromise.then(registration => registration.unregister())");
            Assert.AreEqual(createdTarget, await targetDestroyedTaskCompletion.Task);
        }

        [PuppeteerTest("target.spec.ts", "Target", "should create a worker from a service worker")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldCreateAWorkerFromAServiceWorker()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/empty/sw.html");

            var target = await Context.WaitForTargetAsync(t => t.Type == TargetType.ServiceWorker);
            var worker = await target.WorkerAsync();
            Assert.AreEqual("[object ServiceWorkerGlobalScope]", await worker.EvaluateFunctionAsync<string>("() => self.toString()"));
        }

        [PuppeteerTest("target.spec.ts", "Target", "should create a worker from a shared worker")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldCreateAWorkerFromASharedWorker()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"() =>
            {
                new SharedWorker('data:text/javascript,console.log(""hi"")');
            }");
            var target = await Context.WaitForTargetAsync(t => t.Type == TargetType.SharedWorker);
            var worker = await target.WorkerAsync();
            Assert.AreEqual("[object SharedWorkerGlobalScope]", await worker.EvaluateFunctionAsync<string>("() => self.toString()"));
        }

        [PuppeteerTest("target.spec.ts", "Target", "should report when a target url changes")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Assert.AreEqual(TestConstants.CrossProcessUrl + "/", changedTarget.Url);

            changedTargetTaskCompletion = new TaskCompletionSource<ITarget>();
            Context.TargetChanged += ChangedTargetEventHandler;
            await Page.GoToAsync(TestConstants.EmptyPage);
            changedTarget = await changedTargetTaskCompletion.Task;
            Assert.AreEqual(TestConstants.EmptyPage, changedTarget.Url);
        }

        [PuppeteerTest("target.spec.ts", "Target", "should not report uninitialized pages")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Assert.AreEqual(TestConstants.AboutBlank, target.Url);

            var newPage = await newPageTask;
            targetCompletionTask = new TaskCompletionSource<ITarget>();
            Context.TargetCreated += TargetCreatedEventHandler;
            var evaluateTask = newPage.EvaluateExpressionHandleAsync("window.open('about:blank')");
            var target2 = await targetCompletionTask.Task;
            Assert.AreEqual(TestConstants.AboutBlank, target2.Url);
            await evaluateTask;
            await newPage.CloseAsync();
            Assert.False(targetChanged, "target should not be reported as changed");
            Context.TargetChanged -= listener;
        }

        [PuppeteerTest("target.spec.ts", "Target", "should not crash while redirecting if original request was missed")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldNotCrashWhileRedirectingIfOriginalRequestWasMissed()
        {
            var serverResponseEnd = new TaskCompletionSource<bool>();
            var serverResponse = (HttpResponse)null;
            Server.SetRoute("/one-style.css", context => { serverResponse = context.Response; return serverResponseEnd.Task; });
            // Open a new page. Use window.open to connect to the page later.
            await Task.WhenAll(
              Page.EvaluateFunctionHandleAsync("url => window.open(url)", TestConstants.ServerUrl + "/one-style.html"),
              Server.WaitForRequest("/one-style.css")
            );
            // Connect to the opened page.
            var target = await Context.WaitForTargetAsync(t => t.Url.Contains("one-style.html"));
            var newPage = await target.PageAsync();
            // Issue a redirect.
            serverResponse.Redirect("/injectedstyle.css");
            serverResponseEnd.SetResult(true);
            // Wait for the new page to load.
            await WaitEvent(newPage.Client, "Page.loadEventFired");
            // Cleanup.
            await newPage.CloseAsync();
        }

        [PuppeteerTest("target.spec.ts", "Target", "should have an opener")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldHaveAnOpener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var targetCreatedCompletion = new TaskCompletionSource<ITarget>();
            Browser.TargetCreated += (_, e) => targetCreatedCompletion.TrySetResult(e.Target);
            await Page.GoToAsync(TestConstants.ServerUrl + "/popup/window-open.html");
            var createdTarget = await targetCreatedCompletion.Task;

            Assert.AreEqual(TestConstants.ServerUrl + "/popup/popup.html", (await createdTarget.PageAsync()).Url);
            Assert.AreSame(Page.Target, createdTarget.Opener);
            Assert.Null(Page.Target.Opener);
        }
    }
}
