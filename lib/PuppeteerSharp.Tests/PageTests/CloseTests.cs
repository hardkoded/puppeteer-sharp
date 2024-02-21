using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class CloseTests : PuppeteerPageBaseTest
    {
        public CloseTests() : base() { }

        // [Test,  Retry(2),  PuppeteerTest("page.spec", "Page Page.close", "should reject all promises when page is closed")]
        // [Ignore("TODO: See how to refactor this on nunit")]
        public void ShouldRejectAllPromisesWhenPageIsClosed()
        {
            /*
            var exceptionTask = Assert.ThrowsAsync<TargetClosedException>(() => Page.EvaluateFunctionAsync("() => new Promise(r => {})"));

            await Task.WhenAll(
                exceptionTask,
                Page.CloseAsync()
            );

            var exception = await exceptionTask;
            StringAssert.Contains("Protocol error", exception.Message);
            Assert.AreEqual("Target.detachedFromTarget", exception.CloseReason);
            */
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.close", "should not be visible in browser.pages")]
        public async Task ShouldNotBeVisibleInBrowserPages()
        {
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var page = await browser.NewPageAsync();
            Assert.Contains(page, await browser.PagesAsync());
            await page.CloseAsync();
            Assert.That(await browser.PagesAsync(), Does.Not.Contains(page));
        }

        public async Task ShouldNotBeVisibleInBrowserPagesWithDisposeAsync()
        {
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var page = await browser.NewPageAsync();
            Assert.Contains(page, await browser.PagesAsync());
            await page.DisposeAsync();
            Assert.That(await browser.PagesAsync(), Does.Not.Contains(page));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.close", "should run beforeunload if asked for")]
        public async Task ShouldRunBeforeunloadIfAskedFor()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");

            var dialogTask = new TaskCompletionSource<bool>();
            Page.Dialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.BeforeUnload, e.Dialog.DialogType);
                Assert.AreEqual(string.Empty, e.Dialog.DefaultValue);

                if (TestConstants.IsChrome)
                {
                    Assert.AreEqual(string.Empty, e.Dialog.Message);
                }
                else
                {
                    Assert.AreEqual("This page is asking you to confirm that you want to leave - data you have entered may not be saved.", e.Dialog.Message);
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

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.close", "should *not* run beforeunload by default")]
        public async Task ShouldNotRunBeforeunloadByDefault()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");
            await Page.ClickAsync("body");
            await Page.CloseAsync();
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.close", "should set the page close state")]
        public async Task ShouldSetThePageCloseState()
        {
            var page = await Context.NewPageAsync();
            Assert.False(page.IsClosed);
            await page.CloseAsync();
            Assert.True(page.IsClosed);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.close", "should terminate network waiters")]
        public async Task ShouldTerminateNetworkWaiters()
        {
            var newPage = await Context.NewPageAsync();
            var requestTask = newPage.WaitForRequestAsync(TestConstants.EmptyPage);
            var responseTask = newPage.WaitForResponseAsync(TestConstants.EmptyPage);

            await newPage.CloseAsync();

            var exception = Assert.ThrowsAsync<TargetClosedException>(() => requestTask);
            StringAssert.Contains("Target closed", exception.Message);
            StringAssert.DoesNotContain("Timeout", exception.Message);

            exception = Assert.ThrowsAsync<TargetClosedException>(() => responseTask);
            StringAssert.Contains("Target closed", exception.Message);
            StringAssert.DoesNotContain("Timeout", exception.Message);
        }

        [Test, Retry(2)]
        public async Task ShouldCloseWhenConnectionBreaksPrematurely()
        {
            Browser.Disconnect();
            await Page.CloseAsync();
        }
    }
}
