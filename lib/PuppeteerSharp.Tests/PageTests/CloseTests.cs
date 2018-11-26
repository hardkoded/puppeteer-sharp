using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class CloseTests : PuppeteerPageBaseTest
    {
        public CloseTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task ShouldRejectAllPromisesWhenPageIsClosed()
        {
            var neverResolves = Page.EvaluateFunctionAsync("() => new Promise(r => {})");
            _ = Page.CloseAsync();

            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(async () => await neverResolves);
            Assert.IsType<TargetClosedException>(exception.InnerException);
            Assert.Contains("Protocol error", exception.Message);
            Assert.Equal("Target.detachedFromTarget", ((TargetClosedException)exception.InnerException).CloseReason);
        }

        [Fact]
        public async Task ShouldNotBeVisibleInBrowserPages()
        {
            Assert.Contains(Page, await Browser.PagesAsync());
            await Page.CloseAsync();
            Assert.DoesNotContain(Page, await Browser.PagesAsync());
        }

        [Fact]
        public async Task ShouldRunBeforeunloadIfAskedFor()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");

            var dialogTask = new TaskCompletionSource<bool>();
            Page.Dialog += async (sender, e) =>
            {
                Assert.Equal(DialogType.BeforeUnload, e.Dialog.DialogType);
                Assert.Equal(string.Empty, e.Dialog.Message);
                Assert.Equal(string.Empty, e.Dialog.DefaultValue);

                await e.Dialog.Accept();
                dialogTask.TrySetResult(true);
            };

            var closeTask = new TaskCompletionSource<bool>();
            Page.Close += (sender, e) => closeTask.TrySetResult(true);

            // We have to interact with a page so that 'beforeunload' handlers
            // fire.
            await Page.ClickAsync("body");
            await Page.CloseAsync(new PageCloseOptions { RunBeforeUnload = true });

            await Task.WhenAll(
                dialogTask.Task,
                closeTask.Task
            );
        }

        [Fact]
        public async Task ShouldSetThePageCloseState()
        {
            Assert.False(Page.IsClosed);
            await Page.CloseAsync();
            Assert.True(Page.IsClosed);
        }
    }
}