using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserContextTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class BrowserContextTests : PuppeteerBrowserBaseTest
    {
        public BrowserContextTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldHaveDefaultContext()
        {
            Assert.Single(Browser.BrowserContexts());
            var defaultContext = Browser.BrowserContexts()[0];
            Assert.False(defaultContext.IsIncognito);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(defaultContext.CloseAsync);
            Assert.Same(defaultContext, Browser.DefaultContext);
            Assert.Contains("cannot be closed", exception.Message);
        }

        [Fact]
        public async Task ShouldCreateNewIncognitoContext()
        {
            Assert.Single(Browser.BrowserContexts());
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            Assert.True(context.IsIncognito);
            Assert.Equal(2, Browser.BrowserContexts().Length);
            Assert.Contains(context, Browser.BrowserContexts());
            await context.CloseAsync();
            Assert.Single(Browser.BrowserContexts());
        }

        [Fact]
        public async Task ShouldCloseAllBelongingTargetsOnceClosingContext()
        {
            Assert.Single((await Browser.PagesAsync()));

            var context = await Browser.CreateIncognitoBrowserContextAsync();
            await context.NewPageAsync();
            Assert.Equal(2, (await Browser.PagesAsync()).Length);
            Assert.Single((await context.PagesAsync()));
            await context.CloseAsync();
            Assert.Single((await Browser.PagesAsync()));
        }

        [Fact]
        public async Task WindowOpenShouldUseParentTabContext()
        {
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            var popupTargetCompletion = new TaskCompletionSource<Target>();
            Browser.TargetCreated += (sender, e) => popupTargetCompletion.SetResult(e.Target);

            await Task.WhenAll(
                popupTargetCompletion.Task,
                page.EvaluateFunctionAsync("url => window.open(url)", TestConstants.EmptyPage)
            );

            var popupTarget = await popupTargetCompletion.Task;
            Assert.Same(context, popupTarget.BrowserContext);
            await context.CloseAsync();
        }

        [Fact]
        public async Task ShouldFireTargetEvents()
        {
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            var events = new List<string>();
            context.TargetCreated += (sender, e) => events.Add("CREATED: " + e.Target.Url);
            context.TargetChanged += (sender, e) => events.Add("CHANGED: " + e.Target.Url);
            context.TargetDestroyed += (sender, e) => events.Add("DESTROYED: " + e.Target.Url);
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            await page.CloseAsync();
            Assert.Equal(new[] {
                $"CREATED: {TestConstants.AboutBlank}",
                $"CHANGED: {TestConstants.EmptyPage}",
                $"DESTROYED: {TestConstants.EmptyPage}"
            }, events);
            await context.CloseAsync();
        }

        [Fact]
        public async Task ShouldIsolateLocalStorageAndCookies()
        {
            // Create two incognito contexts.
            var context1 = await Browser.CreateIncognitoBrowserContextAsync();
            var context2 = await Browser.CreateIncognitoBrowserContextAsync();
            Assert.Empty(context1.Targets());
            Assert.Empty(context2.Targets());

            // Create a page in first incognito context.
            var page1 = await context1.NewPageAsync();
            await page1.GoToAsync(TestConstants.EmptyPage);
            await page1.EvaluateExpressionAsync(@"{
                localStorage.setItem('name', 'page1');
                document.cookie = 'name=page1';
            }");

            Assert.Single(context1.Targets());
            Assert.Empty(context2.Targets());

            // Create a page in second incognito context.
            var page2 = await context2.NewPageAsync();
            await page2.GoToAsync(TestConstants.EmptyPage);
            await page2.EvaluateExpressionAsync(@"{
                localStorage.setItem('name', 'page2');
                document.cookie = 'name=page2';
            }");

            Assert.Single(context1.Targets());
            Assert.Equal(page1.Target, context1.Targets()[0]);
            Assert.Single(context2.Targets());
            Assert.Equal(page2.Target, context2.Targets()[0]);

            // Make sure pages don't share localstorage or cookies.
            Assert.Equal("page1", await page1.EvaluateExpressionAsync<string>("localStorage.getItem('name')"));
            Assert.Equal("name=page1", await page1.EvaluateExpressionAsync<string>("document.cookie"));
            Assert.Equal("page2", await page2.EvaluateExpressionAsync<string>("localStorage.getItem('name')"));
            Assert.Equal("name=page2", await page2.EvaluateExpressionAsync<string>("document.cookie"));

            // Cleanup contexts.
            await Task.WhenAll(context1.CloseAsync(), context2.CloseAsync());
            Assert.Single(Browser.BrowserContexts());
        }

        [Fact]
        public async Task ShouldWorkAcrossSessions()
        {
            Assert.Single(Browser.BrowserContexts());
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            Assert.Equal(2, Browser.BrowserContexts().Length);

            var remoteBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint
            });
            var contexts = remoteBrowser.BrowserContexts();
            Assert.Equal(2, contexts.Length);
            remoteBrowser.Disconnect();
            await context.CloseAsync();
        }

        [Fact]
        public async Task ShouldWaitForTarget()
        {
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            var targetPromise = context.WaitForTargetAsync((target) => target.Url == TestConstants.EmptyPage);
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            var promiseTarget = await targetPromise;
            var targetPage = await promiseTarget.PageAsync();
            Assert.Equal(targetPage, page);
            await context.CloseAsync();
        }

        [Fact]
        public async Task ShouldTimeoutWaitingForNonExistantTarget()
        {
            var context = await Browser.CreateIncognitoBrowserContextAsync();
            var exception = await Assert.ThrowsAsync<TimeoutException>(()
                => context.WaitForTargetAsync((target) => target.Url == TestConstants.EmptyPage, new WaitForOptions { Timeout = 1 }));
            await context.CloseAsync(); 
        }
    }
}