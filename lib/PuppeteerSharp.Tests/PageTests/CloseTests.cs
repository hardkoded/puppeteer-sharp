using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CloseTests : PuppeteerPageBaseTest
    {
        public CloseTests(ITestOutputHelper output) : base(output) { }

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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ShouldNotBeVisibleInBrowserPages(bool useDisposeAsync)
        {
            Assert.Contains(Page, await Browser.PagesAsync());
            if (useDisposeAsync)
            {
                // emulates what would happen in a C#8 await using block
                await Page.DisposeAsync();
            }
            else
            {
                await Page.CloseAsync();
            }
            Assert.DoesNotContain(Page, await Browser.PagesAsync());
        }

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

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNotRunBeforeunloadByDefault()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");
            await Page.ClickAsync("body");
            await Page.CloseAsync();
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldSetThePageCloseState()
        {
            Assert.False(Page.IsClosed);
            await Page.CloseAsync();
            Assert.True(Page.IsClosed);
        }

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

        [Fact(Timeout = 10000)]
        public async Task ShouldCloseWhenConnectionBreaksPrematurely()
        {
            Browser.Connection.Dispose();
            await Page.CloseAsync();
        }
    }
}
