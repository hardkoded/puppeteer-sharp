using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class CloseTests : PuppeteerPageBaseTest
    {
        public CloseTests() : base() { }

        [Test, PuppeteerTest("page.spec", "Page Page.close", "should reject all promises when page is closed")]
        public async Task ShouldRejectAllPromisesWhenPageIsClosed()
        {
            TargetClosedException exception = null;

            var exceptionTask = Assert.ThatAsync(async () =>
            {
                try
                {
                    await Page.EvaluateFunctionAsync("() => new Promise(r => {})");
                }
                catch (TargetClosedException e)
                {
                    exception = e;
                    throw;
                }
            }, Throws.InstanceOf<TargetClosedException>());

            await Task.WhenAll(
                exceptionTask,
                Page.CloseAsync()
            );

            Assert.That(exception.Message, Does.Contain("Protocol error"));
            Assert.That(exception.CloseReason, Is.EqualTo("Target.detachedFromTarget"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.close", "should not be visible in browser.pages")]
        public async Task ShouldNotBeVisibleInBrowserPages()
        {
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var page = await browser.NewPageAsync();
            Assert.That(await browser.PagesAsync(), Does.Contain(page));
            await page.CloseAsync();
            Assert.That(await browser.PagesAsync(), Does.Not.Contains(page));
        }

        [Test, Ignore("previously not marked as a test")]
        public async Task ShouldNotBeVisibleInBrowserPagesWithDisposeAsync()
        {
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var page = await browser.NewPageAsync();
            Assert.That(await browser.PagesAsync(), Does.Contain(page));
            await page.DisposeAsync();
            Assert.That(await browser.PagesAsync(), Does.Not.Contains(page));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.close", "should run beforeunload if asked for")]
        public async Task ShouldRunBeforeunloadIfAskedFor()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");

            var dialogTask = new TaskCompletionSource<bool>();
            Page.Dialog += async (_, e) =>
            {
                Assert.That(e.Dialog.DialogType, Is.EqualTo(DialogType.BeforeUnload));
                Assert.That(e.Dialog.DefaultValue, Is.EqualTo(string.Empty));

                if (TestConstants.IsChrome)
                {
                    Assert.That(e.Dialog.Message, Is.EqualTo(string.Empty));
                }
                else
                {
                    Assert.That(e.Dialog.Message, Is.EqualTo("This page is asking you to confirm that you want to leave - data you have entered may not be saved."));
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

        [Test, PuppeteerTest("page.spec", "Page Page.close", "should *not* run beforeunload by default")]
        public async Task ShouldNotRunBeforeunloadByDefault()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");
            await Page.ClickAsync("body");
            await Page.CloseAsync();
        }

        [Test, PuppeteerTest("page.spec", "Page Page.close", "should set the page close state")]
        public async Task ShouldSetThePageCloseState()
        {
            var page = await Context.NewPageAsync();
            Assert.That(page.IsClosed, Is.False);
            await page.CloseAsync();
            Assert.That(page.IsClosed, Is.True);
        }

        [Test, PuppeteerTest("page.spec", "Page Page.close", "should terminate network waiters")]
        public async Task ShouldTerminateNetworkWaiters()
        {
            var newPage = await Context.NewPageAsync();
            var requestTask = newPage.WaitForRequestAsync(TestConstants.EmptyPage);
            var responseTask = newPage.WaitForResponseAsync(TestConstants.EmptyPage);

            await newPage.CloseAsync();

            var exception = Assert.ThrowsAsync<TargetClosedException>(() => requestTask);
            Assert.That(exception.Message, Does.Contain("Target closed"));
            Assert.That(exception.Message, Does.Not.Contain("Timeout"));

            exception = Assert.ThrowsAsync<TargetClosedException>(() => responseTask);
            Assert.That(exception.Message, Does.Contain("Target closed"));
            Assert.That(exception.Message, Does.Not.Contain("Timeout"));
        }

        [Test, Retry(2)]
        public async Task ShouldCloseWhenConnectionBreaksPrematurely()
        {
            Browser.Disconnect();
            await Page.CloseAsync();
        }
    }
}
