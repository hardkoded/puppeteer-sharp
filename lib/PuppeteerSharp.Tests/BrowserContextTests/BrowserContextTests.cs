using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserContextTests
{
    public class BrowserContextTests : PuppeteerBrowserBaseTest
    {
        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext", "should have default context")]
        public void ShouldHaveDefaultContext()
        {
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
            var defaultContext = Browser.BrowserContexts()[0];
            var exception = Assert.ThrowsAsync<PuppeteerException>(defaultContext.CloseAsync);
            Assert.That(Browser.DefaultContext, Is.SameAs(defaultContext));
            Assert.That(exception!.Message, Does.Contain("cannot be closed"));
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext", "should create new incognito context")]
        public async Task ShouldCreateNewIncognitoContext()
        {
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
            var context = await Browser.CreateBrowserContextAsync();
            Assert.That(Browser.BrowserContexts(), Has.Length.EqualTo(2));
            Assert.That(Browser.BrowserContexts(), Does.Contain(context));
            await context.CloseAsync();
            Assert.That(context.IsClosed, Is.True);
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext", "should close all belonging targets once closing context")]
        public async Task ShouldCloseAllBelongingTargetsOnceClosingContext()
        {
            Assert.That((await Browser.PagesAsync()), Has.Exactly(1).Items);

            var context = await Browser.CreateBrowserContextAsync();
            await context.NewPageAsync();
            Assert.That((await Browser.PagesAsync()), Has.Length.EqualTo(2));
            Assert.That((await context.PagesAsync()), Has.Exactly(1).Items);
            await context.CloseAsync();
            Assert.That(context.IsClosed, Is.True);
            Assert.That((await Browser.PagesAsync()), Has.Exactly(1).Items);
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext", "window.open should use parent tab context")]
        public async Task WindowOpenShouldUseParentTabContext()
        {
            var context = await Browser.CreateBrowserContextAsync();
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            var popupTargetCompletion = new TaskCompletionSource<ITarget>();
            Browser.TargetCreated += (_, e) => popupTargetCompletion.SetResult(e.Target);

            await Task.WhenAll(
                popupTargetCompletion.Task,
                page.EvaluateFunctionAsync("url => window.open(url)", TestConstants.EmptyPage)
            );

            var popupTarget = await popupTargetCompletion.Task;
            Assert.That(popupTarget.BrowserContext, Is.SameAs(context));
            await context.CloseAsync();
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext", "should fire target events")]
        public async Task ShouldFireTargetEvents()
        {
            var context = await Browser.CreateBrowserContextAsync();
            var events = new List<string>();
            context.TargetCreated += (_, e) => events.Add("CREATED: " + e.Target.Url);
            context.TargetChanged += (_, e) => events.Add("CHANGED: " + e.Target.Url);
            context.TargetDestroyed += (_, e) => events.Add("DESTROYED: " + e.Target.Url);
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            await page.CloseAsync();
            // Just for half a second to get the last event
            await Task.Delay(500);

            Assert.That(events, Is.EqualTo(new[] {
                $"CREATED: {TestConstants.AboutBlank}",
                $"CHANGED: {TestConstants.EmptyPage}",
                $"DESTROYED: {TestConstants.EmptyPage}"
            }));
            await context.CloseAsync();
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext", "should isolate localStorage and cookies")]
        public async Task ShouldIsolateLocalStorageAndCookies()
        {
            // Create two incognito contexts.
            var context1 = await Browser.CreateBrowserContextAsync();
            var context2 = await Browser.CreateBrowserContextAsync();
            Assert.That(context1.Targets(), Is.Empty);
            Assert.That(context2.Targets(), Is.Empty);

            // Create a page in first incognito context.
            var page1 = await context1.NewPageAsync();
            await page1.GoToAsync(TestConstants.EmptyPage);
            await page1.EvaluateExpressionAsync(@"{
                localStorage.setItem('name', 'page1');
                document.cookie = 'name=page1';
            }");

            Assert.That(context1.Targets(), Has.Exactly(1).Items);
            Assert.That(context2.Targets(), Is.Empty);

            // Create a page in second incognito context.
            var page2 = await context2.NewPageAsync();
            await page2.GoToAsync(TestConstants.EmptyPage);
            await page2.EvaluateExpressionAsync(@"{
                localStorage.setItem('name', 'page2');
                document.cookie = 'name=page2';
            }");

            Assert.That(context1.Targets(), Has.Exactly(1).Items);
            Assert.That(await context1.Targets()[0].PageAsync(), Is.EqualTo(page1));
            Assert.That(context2.Targets(), Has.Exactly(1).Items);
            Assert.That(await context2.Targets()[0].PageAsync(), Is.EqualTo(page2));

            // Make sure pages don't share localstorage or cookies.
            Assert.That(await page1.EvaluateExpressionAsync<string>("localStorage.getItem('name')"), Is.EqualTo("page1"));
            Assert.That(await page1.EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo("name=page1"));
            Assert.That(await page2.EvaluateExpressionAsync<string>("localStorage.getItem('name')"), Is.EqualTo("page2"));
            Assert.That(await page2.EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo("name=page2"));

            // Cleanup contexts.
            await Task.WhenAll(context1.CloseAsync(), context2.CloseAsync());
            Assert.That(context1.IsClosed, Is.True);
            Assert.That(context2.IsClosed, Is.True);
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext", "should work across sessions")]
        public async Task ShouldWorkAcrossSessions()
        {
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
            var context = await Browser.CreateBrowserContextAsync();
            Assert.That(Browser.BrowserContexts(), Has.Length.EqualTo(2));

            var remoteBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint
            });
            var contexts = remoteBrowser.BrowserContexts();
            Assert.That(contexts, Has.Length.EqualTo(2));
            remoteBrowser.Disconnect();
            await context.CloseAsync();
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext", "should provide a context id")]
        public async Task ShouldProvideAContextId()
        {
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
            Assert.That(Browser.BrowserContexts()[0].Id, Is.Null);

            var context = await Browser.CreateBrowserContextAsync();
            Assert.That(Browser.BrowserContexts(), Has.Length.EqualTo(2));
            Assert.That(Browser.BrowserContexts()[1].Id, Is.Not.Null);
            await context.CloseAsync();
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext", "should wait for a target")]
        public async Task ShouldWaitForTarget()
        {
            var context = await Browser.CreateBrowserContextAsync();
            var targetPromise = context.WaitForTargetAsync((target) => target.Url == TestConstants.EmptyPage);
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            var promiseTarget = await targetPromise;
            var targetPage = await promiseTarget.PageAsync();
            Assert.That(page, Is.EqualTo(targetPage));
            await context.CloseAsync();
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext", "should timeout waiting for a non-existent target")]
        public async Task ShouldTimeoutWaitingForNonExistentTarget()
        {
            var context = await Browser.CreateBrowserContextAsync();
            Assert.ThrowsAsync<TimeoutException>(()
                => context.WaitForTargetAsync((target) => target.Url == TestConstants.EmptyPage, new WaitForOptions(1)));
            await context.CloseAsync();
        }
    }
}
