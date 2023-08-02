using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class CloseTests : PuppeteerPageBaseTest
    {
        public CloseTests(): base() { }

        [PuppeteerTest("page.spec.ts", "Page.close", "should reject all promises when page is closed")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldRejectAllPromisesWhenPageIsClosed()
        {
            var exceptionTask = Assert.ThrowsAsync<TargetClosedException>(() => Page.EvaluateFunctionAsync("() => new Promise(r => {})"));

            await Task.WhenAll(
                exceptionTask,
                Page.CloseAsync()
            );

            var exception = await exceptionTask;
            Assert.Contains("Protocol error", exception.Message);
            Assert.Equal("Target.detachedFromTarget", exception.CloseReason);
        }

        [PuppeteerTest("page.spec.ts", "Page.close", "should not be visible in browser.pages")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNotBeVisibleInBrowserPages()
        {
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var page = await browser.NewPageAsync();
            Assert.Contains(page, await browser.PagesAsync());
            await page.CloseAsync();
            Assert.DoesNotContain(page, await browser.PagesAsync());
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNotBeVisibleInBrowserPagesWithDisposeAsync()
        {
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var page = await browser.NewPageAsync();
            Assert.Contains(page, await browser.PagesAsync());
            await page.DisposeAsync();
            Assert.DoesNotContain(page, await browser.PagesAsync());
        }

        [PuppeteerTest("page.spec.ts", "Page.close", "should run beforeunload if asked for")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldRunBeforeunloadIfAskedFor()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");

            var dialogTask = new TaskCompletionSource<bool>();
            Page.Dialog += async (_, e) =>
            {
                Assert.Equal(DialogType.BeforeUnload, e.Dialog.DialogType);
                Assert.Equal(string.Empty, e.Dialog.DefaultValue);

                if (TestConstants.IsChrome)
                {
                    Assert.Equal(string.Empty, e.Dialog.Message);
                }
                else
                {
                    Assert.Equal("This page is asking you to confirm that you want to leave - data you have entered may not be saved.", e.Dialog.Message);
                }

                await e.Dialog.Accept();
                dialogTask.TrySetResult(true);
            };

            var closeTask = new TaskCompletionSource<bool>();
            Page.Close += (_, _) => closeTask.TrySetResult(true);

            // We have to interact with a page so that 'beforeunload' handlers
            // fire.
            await Page.ClickAsync("body");
            await Page.CloseAsync(new PageCloseOptions { RunBeforeUnload = true });

            await Task.WhenAll(
                dialogTask.Task,
                closeTask.Task
            );
        }

        [PuppeteerTest("page.spec.ts", "Page.close", "should *not* run beforeunload by default")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNotRunBeforeunloadByDefault()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");
            await Page.ClickAsync("body");
            await Page.CloseAsync();
        }

        [PuppeteerTest("page.spec.ts", "Page.close", "should set the page close state")]
        [PuppeteerFact]
        public async Task ShouldSetThePageCloseState()
        {
            var page = await Context.NewPageAsync();
            Assert.False(page.IsClosed);
            await page.CloseAsync();
            Assert.True(page.IsClosed);
        }

        [PuppeteerTest("page.spec.ts", "Page.close", "should terminate network waiters")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldTerminateNetworkWaiters()
        {
            var newPage = await Context.NewPageAsync();
            var requestTask = newPage.WaitForRequestAsync(TestConstants.EmptyPage);
            var responseTask = newPage.WaitForResponseAsync(TestConstants.EmptyPage);

            await newPage.CloseAsync();

            var exception = await Assert.ThrowsAsync<TargetClosedException>(() => requestTask);
            Assert.Contains("Target closed", exception.Message);
            Assert.DoesNotContain("Timeout", exception.Message);

            exception = await Assert.ThrowsAsync<TargetClosedException>(() => responseTask);
            Assert.Contains("Target closed", exception.Message);
            Assert.DoesNotContain("Timeout", exception.Message);
        }

        [PuppeteerFact(Timeout = 10000)]
        public async Task ShouldCloseWhenConnectionBreaksPrematurely()
        {
            Browser.Disconnect();
            await Page.CloseAsync();
        }
    }
}
