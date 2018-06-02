using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.TargetTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class TargetTests : PuppeteerPageBaseTest
    {
        public TargetTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void BrowserTargetsShouldReturnAllOfTheTargets()
        {
            // The pages will be the testing page and the original newtab page
            var targets = Browser.Targets();
            Assert.Contains(targets, target => target.Type == "page"
                && target.Url == TestConstants.AboutBlank);
            Assert.Contains(targets, target => target.Type == "other"
                && target.Url == string.Empty);
        }

        [Fact]
        public async Task BrowserPagesShouldReturnAllOfThePages()
        {
            // The pages will be the testing page and the original newtab page
            var allPages = (await Browser.PagesAsync()).ToArray();
            Assert.Equal(2, allPages.Length);
            Assert.Contains(Page, allPages);
            Assert.NotSame(allPages[0], allPages[1]);
        }

        [Fact]
        public async Task ShouldBeAbleToUseTheDefaultPageInTheBrowser()
        {
            // The pages will be the testing page and the original newtab page
            var allPages = await Browser.PagesAsync();
            var originalPage = allPages.First(p => p != Page);
            Assert.Equal("Hello world", await originalPage.EvaluateExpressionAsync<string>("['Hello', 'world'].join(' ')"));
            Assert.NotNull(await originalPage.QuerySelectorAsync("body"));
        }

        [Fact]
        public async Task ShouldReportWhenANewPageIsCreatedAndClosed()
        {
            var otherPageTaskCompletion = new TaskCompletionSource<Page>();
            async void TargetCreatedEventHandler(object sender, TargetChangedArgs e)
            {
                otherPageTaskCompletion.SetResult(await e.Target.PageAsync());
                Browser.TargetCreated -= TargetCreatedEventHandler;
            }
            Browser.TargetCreated += TargetCreatedEventHandler;
            await Page.EvaluateFunctionHandleAsync("url => window.open(url)", TestConstants.CrossProcessUrl);
            var otherPage = await otherPageTaskCompletion.Task;
            Assert.Contains(TestConstants.CrossProcessUrl, otherPage.Url);

            Assert.Equal("Hello world", await otherPage.EvaluateExpressionAsync<string>("['Hello', 'world'].join(' ')"));
            Assert.NotNull(await otherPage.QuerySelectorAsync("body"));

            var allPages = await Browser.PagesAsync();
            Assert.Contains(Page, allPages);
            Assert.Contains(otherPage, allPages);

            var closePageTaskCompletion = new TaskCompletionSource<Page>();
            async void TargetDestroyedEventHandler(object sender, TargetChangedArgs e)
            {
                closePageTaskCompletion.SetResult(await e.Target.PageAsync());
                Browser.TargetDestroyed -= TargetDestroyedEventHandler;
            }
            Browser.TargetDestroyed += TargetDestroyedEventHandler;
            await otherPage.CloseAsync();
            Assert.Equal(otherPage, await closePageTaskCompletion.Task);

            allPages = await Task.WhenAll(Browser.Targets().Select(target => target.PageAsync()));
            Assert.Contains(Page, allPages);
            Assert.DoesNotContain(otherPage, allPages);
        }

        [Fact]
        public async Task ShouldReportWhenAServiceWorkerIsCreatedAndDestroyed()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var createdTargetTaskCompletion = new TaskCompletionSource<Target>();
            void TargetCreatedEventHandler(object sender, TargetChangedArgs e)
            {
                createdTargetTaskCompletion.SetResult(e.Target);
                Browser.TargetCreated -= TargetCreatedEventHandler;
            }
            Browser.TargetCreated += TargetCreatedEventHandler;
            var registration = await Page.EvaluateExpressionHandleAsync("navigator.serviceWorker.register('sw.js')");

            var createdTarget = await createdTargetTaskCompletion.Task;
            Assert.Equal("service_worker", createdTarget.Type);
            Assert.Equal(TestConstants.ServerUrl + "/sw.js", createdTarget.Url);

            var targetDestroyedTaskCompletion = new TaskCompletionSource<Target>();
            void TargetDestroyedEventHandler(object sender, TargetChangedArgs e)
            {
                targetDestroyedTaskCompletion.SetResult(e.Target);
                Browser.TargetDestroyed -= TargetDestroyedEventHandler;
            }
            Browser.TargetDestroyed += TargetDestroyedEventHandler;
            await Page.EvaluateFunctionAsync("registration => registration.unregister()", registration);
            Assert.Equal(createdTarget, await targetDestroyedTaskCompletion.Task);
        }

        [Fact]
        public async Task ShouldReportWhenATargetUrlChanges()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var changedTargetTaskCompletion = new TaskCompletionSource<Target>();
            void ChangedTargetEventHandler(object sender, TargetChangedArgs e)
            {
                changedTargetTaskCompletion.SetResult(e.Target);
                Browser.TargetChanged -= ChangedTargetEventHandler;
            }
            Browser.TargetChanged += ChangedTargetEventHandler;

            await Page.GoToAsync(TestConstants.CrossProcessUrl + "/");
            var changedTarget = await changedTargetTaskCompletion.Task;
            Assert.Equal(TestConstants.CrossProcessUrl + "/", changedTarget.Url);

            changedTargetTaskCompletion = new TaskCompletionSource<Target>();
            Browser.TargetChanged += ChangedTargetEventHandler;
            await Page.GoToAsync(TestConstants.EmptyPage);
            changedTarget = await changedTargetTaskCompletion.Task;
            Assert.Equal(TestConstants.EmptyPage, changedTarget.Url);
        }

        [Fact]
        public async Task ShouldNotReportUninitializedPages()
        {
            var targetChanged = false;
            EventHandler<TargetChangedArgs> listener = (sender, e) => targetChanged = true;
            Browser.TargetChanged += listener;
            var targetCompletionTask = new TaskCompletionSource<Target>();
            void TargetCreatedEventHandler(object sender, TargetChangedArgs e)
            {
                targetCompletionTask.SetResult(e.Target);
                Browser.TargetCreated -= TargetCreatedEventHandler;
            }
            Browser.TargetCreated += TargetCreatedEventHandler;
            var newPageTask = Browser.NewPageAsync();
            var target = await targetCompletionTask.Task;
            Assert.Equal(TestConstants.AboutBlank, target.Url);

            var newPage = await newPageTask;
            targetCompletionTask = new TaskCompletionSource<Target>();
            Browser.TargetCreated += TargetCreatedEventHandler;
            var evaluateTask = newPage.EvaluateExpressionHandleAsync("window.open('about:blank')");
            var target2 = await targetCompletionTask.Task;
            Assert.Equal(TestConstants.AboutBlank, target2.Url);
            await evaluateTask;
            await newPage.CloseAsync();
            Assert.False(targetChanged, "target should not be reported as changed");
            Browser.TargetChanged -= listener;
        }

        [Fact]
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
            var target = Browser.Targets().First(t => t.Url.Contains("one-style.html"));
            var newPage = await target.PageAsync();
            // Issue a redirect.
            serverResponse.Redirect("/injectedstyle.css");
            serverResponseEnd.SetResult(true);
            // Wait for the new page to load.            
            await WaitForEvents(newPage.Client, "Page.loadEventFired");
            // Cleanup.
            await newPage.CloseAsync();
        }
    }
}