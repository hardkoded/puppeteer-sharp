using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Target
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class Tests : PuppeteerPageBaseTest
    {
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
            var allPages = (await Browser.Pages()).ToArray();
            Assert.Equal(2, allPages.Length);
            Assert.Contains(Page, allPages);
            Assert.NotSame(allPages[0], allPages[1]);
        }

        [Fact]
        public async Task ShouldBeAbleToUseTheDefaultPageInTheBrowser()
        {
            // The pages will be the testing page and the original newtab page
            var allPages = await Browser.Pages();
            var originalPage = allPages.First(p => p != Page);
            Assert.Equal("Hello world", await originalPage.EvaluateExpressionAsync<string>("['Hello', 'world'].join(' ')"));
            Assert.NotNull(await originalPage.GetElementAsync("body"));
        }

        [Fact(Skip = "wip")]
        public async Task ShouldReportWhenANewPageIsCreatedAndClosed()
        {
            var otherPageTaskCompletion = new TaskCompletionSource<Task<PuppeteerSharp.Page>>();
            void TargetCreatedEventHandler(object sender, TargetChangedArgs e)
            {
                otherPageTaskCompletion.SetResult(e.Target.Page());
                Browser.TargetCreated -= TargetCreatedEventHandler;
            }
            Browser.TargetCreated += TargetCreatedEventHandler;
            await Page.EvaluateFunctionAsync("url => { window.open(url); return true; }", TestConstants.CrossProcessUrl);
            var otherPage = await (await otherPageTaskCompletion.Task);
            Assert.Contains(TestConstants.CrossProcessUrl, otherPage.Url);

            Assert.Equal("Hello world", await otherPage.EvaluateExpressionAsync<string>("['Hello', 'world'].join(' ')"));
            Assert.NotNull(await otherPage.GetElementAsync("body"));

            var allPages = await Browser.Pages();
            Assert.Contains(Page, allPages);
            Assert.Contains(otherPage, allPages);

            var closePageTaskCompletion = new TaskCompletionSource<Task<PuppeteerSharp.Page>>();
            void TargetDestroyedEventHandler(object sender, TargetChangedArgs e)
            {
                closePageTaskCompletion.SetResult(e.Target.Page());
                Browser.TargetDestroyed -= TargetDestroyedEventHandler;
            }
            Browser.TargetDestroyed += TargetDestroyedEventHandler;
            await otherPage.CloseAsync();
            await closePageTaskCompletion.Task;
            var closedPage = await closePageTaskCompletion.Task.Result;
            Assert.Equal(otherPage, closedPage);
            
            allPages = await Task.WhenAll(Browser.Targets().Select(target => target.Page()));
            Assert.Contains(Page, allPages);
            Assert.Contains(otherPage, allPages);
        }
    }
}