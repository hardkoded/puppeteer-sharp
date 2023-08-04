using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.BrowserContextTests
{
    public class BrowserContextTests : PuppeteerBrowserBaseTest
    {
        public BrowserContextTests(): base()
        {
        }

        [PuppeteerTest("browsercontext.spec.ts", "BrowserContext", "should have default context")]
        [PuppeteerTimeout]
        public void ShouldHaveDefaultContext()
        {
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
            var defaultContext = Browser.BrowserContexts()[0];
            Assert.False(defaultContext.IsIncognito);
            var exception = Assert.ThrowsAsync<PuppeteerException>(defaultContext.CloseAsync);
            Assert.AreSame(defaultContext, Browser.DefaultContext);
            StringAssert.Contains("cannot be closed", exception.Message);
        }

        [PuppeteerTest("browsercontext.spec.ts", "BrowserContext", "should create new incognito context")]
        [PuppeteerTimeout]
        public async Task ShouldCreateNewIncognitoContext()
        {
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            Assert.True(context.IsIncognito);
            Assert.AreEqual(2, Browser.BrowserContexts().Length);
            Assert.Contains(context, Browser.BrowserContexts());
            await context.CloseAsync();
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
        }

        [PuppeteerTest("browsercontext.spec.ts", "BrowserContext", "should close all belonging targets once closing context")]
        [PuppeteerTimeout]
        public async Task ShouldCloseAllBelongingTargetsOnceClosingContext()
        {
            Assert.That((await Browser.PagesAsync()), Has.Exactly(1).Items);

            var context = await Browser.CreateIncognitoBrowserContextAsync();
            await context.NewPageAsync();
            Assert.AreEqual(2, (await Browser.PagesAsync()).Length);
            Assert.That((await context.PagesAsync()), Has.Exactly(1).Items);
            await context.CloseAsync();
            Assert.That((await Browser.PagesAsync()), Has.Exactly(1).Items);
        }

        [PuppeteerTest("browsercontext.spec.ts", "BrowserContext", "window.open should use parent tab context")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task WindowOpenShouldUseParentTabContext()
        {
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            var popupTargetCompletion = new TaskCompletionSource<ITarget>();
            Browser.TargetCreated += (_, e) => popupTargetCompletion.SetResult(e.Target);

            await Task.WhenAll(
                popupTargetCompletion.Task,
                page.EvaluateFunctionAsync("url => window.open(url)", TestConstants.EmptyPage)
            );

            var popupTarget = await popupTargetCompletion.Task;
            Assert.AreSame(context, popupTarget.BrowserContext);
            await context.CloseAsync();
        }

        [PuppeteerTest("browsercontext.spec.ts", "BrowserContext", "should fire target events")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldFireTargetEvents()
        {
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            var events = new List<string>();
            context.TargetCreated += (_, e) => events.Add("CREATED: " + e.Target.Url);
            context.TargetChanged += (_, e) => events.Add("CHANGED: " + e.Target.Url);
            context.TargetDestroyed += (_, e) => events.Add("DESTROYED: " + e.Target.Url);
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            await page.CloseAsync();
            Assert.AreEqual(new[] {
                $"CREATED: {TestConstants.AboutBlank}",
                $"CHANGED: {TestConstants.EmptyPage}",
                $"DESTROYED: {TestConstants.EmptyPage}"
            }, events);
            await context.CloseAsync();
        }

        [PuppeteerTest("browsercontext.spec.ts", "BrowserContext", "should isolate localStorage and cookies")]
        [PuppeteerTimeout]
        public async Task ShouldIsolateLocalStorageAndCookies()
        {
            // Create two incognito contexts.
            var context1 = await Browser.CreateIncognitoBrowserContextAsync();
            var context2 = await Browser.CreateIncognitoBrowserContextAsync();
            Assert.IsEmpty(context1.Targets());
            Assert.IsEmpty(context2.Targets());

            // Create a page in first incognito context.
            var page1 = await context1.NewPageAsync();
            await page1.GoToAsync(TestConstants.EmptyPage);
            await page1.EvaluateExpressionAsync(@"{
                localStorage.setItem('name', 'page1');
                document.cookie = 'name=page1';
            }");

            Assert.That(context1.Targets(), Has.Exactly(1).Items);
            Assert.IsEmpty(context2.Targets());

            // Create a page in second incognito context.
            var page2 = await context2.NewPageAsync();
            await page2.GoToAsync(TestConstants.EmptyPage);
            await page2.EvaluateExpressionAsync(@"{
                localStorage.setItem('name', 'page2');
                document.cookie = 'name=page2';
            }");

            Assert.That(context1.Targets(), Has.Exactly(1).Items);
            Assert.AreEqual(page1.Target, context1.Targets()[0]);
            Assert.That(context2.Targets(), Has.Exactly(1).Items);
            Assert.AreEqual(page2.Target, context2.Targets()[0]);

            // Make sure pages don't share localstorage or cookies.
            Assert.AreEqual("page1", await page1.EvaluateExpressionAsync<string>("localStorage.getItem('name')"));
            Assert.AreEqual("name=page1", await page1.EvaluateExpressionAsync<string>("document.cookie"));
            Assert.AreEqual("page2", await page2.EvaluateExpressionAsync<string>("localStorage.getItem('name')"));
            Assert.AreEqual("name=page2", await page2.EvaluateExpressionAsync<string>("document.cookie"));

            // Cleanup contexts.
            await Task.WhenAll(context1.CloseAsync(), context2.CloseAsync());
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
        }

        [PuppeteerTest("browsercontext.spec.ts", "BrowserContext", "should work across sessions")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkAcrossSessions()
        {
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            Assert.AreEqual(2, Browser.BrowserContexts().Length);

            var remoteBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint
            });
            var contexts = remoteBrowser.BrowserContexts();
            Assert.AreEqual(2, contexts.Length);
            remoteBrowser.Disconnect();
            await context.CloseAsync();
        }

        [PuppeteerTest("browsercontext.spec.ts", "BrowserContext", "should provide a context id")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldProvideAContextId()
        {
            Assert.That(Browser.BrowserContexts(), Has.Exactly(1).Items);
            Assert.Null(Browser.BrowserContexts()[0].Id);

            var context = await Browser.CreateIncognitoBrowserContextAsync();
            Assert.AreEqual(2, Browser.BrowserContexts().Length);
            Assert.NotNull(Browser.BrowserContexts()[1].Id);
            await context.CloseAsync();
        }

        [PuppeteerTest("browsercontext.spec.ts", "BrowserContext", "should wait for a target")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWaitForTarget()
        {
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            var targetPromise = context.WaitForTargetAsync((target) => target.Url == TestConstants.EmptyPage);
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            var promiseTarget = await targetPromise;
            var targetPage = await promiseTarget.PageAsync();
            Assert.AreEqual(targetPage, page);
            await context.CloseAsync();
        }

        [PuppeteerTest("browsercontext.spec.ts", "BrowserContext", "should timeout waiting for non-existent target")]
        [PuppeteerTimeout]
        public async Task ShouldTimeoutWaitingForNonExistantTarget()
        {
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            Assert.ThrowsAsync<TimeoutException>(()
                => context.WaitForTargetAsync((target) => target.Url == TestConstants.EmptyPage, new WaitForOptions { Timeout = 1 }));
            await context.CloseAsync();
        }
    }
}
