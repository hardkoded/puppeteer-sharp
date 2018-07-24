using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class CloseTests : PuppeteerBrowserBaseTest
    {
        public CloseTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task ShouldRejectAllPromisesWhenPageIsClosed()
        {
            var newPage = await Browser.NewPageAsync();
            var neverResolves = newPage.EvaluateFunctionAsync("() => new Promise(r => {})");

            // Put into a var to avoid warning
            var t = newPage.CloseAsync();

            var exception = await Assert.ThrowsAsync<TargetClosedException>(async () => await neverResolves);

            Assert.Contains("Protocol error", exception.Message);
        }

        [Fact]
        public async Task ShouldNotBeVisibleInBrowserPages()
        {
            var newPage = await Browser.NewPageAsync();
            Assert.Contains(newPage, await Browser.PagesAsync());
            await newPage.CloseAsync();
            Assert.DoesNotContain(newPage, await Browser.PagesAsync());
        }

        [Fact]
        public async Task ShouldRunBeforeunloadIfAskedFor()
        {
            var newPage = await Browser.NewPageAsync();
            await newPage.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");

            var dialogTask = new TaskCompletionSource<bool>();
            newPage.Dialog += async (sender, e) =>
            {
                Assert.Equal(DialogType.BeforeUnload, e.Dialog.DialogType);
                Assert.Equal(string.Empty, e.Dialog.Message);
                Assert.Equal(string.Empty, e.Dialog.DefaultValue);

                await e.Dialog.Accept();
                dialogTask.TrySetResult(true);
            };

            var closeTask = new TaskCompletionSource<bool>();
            newPage.Close += (sender, e) => closeTask.TrySetResult(true);

            // We have to interact with a page so that 'beforeunload' handlers
            // fire.
            await newPage.ClickAsync("body");
            await newPage.CloseAsync(new PageCloseOptions { RunBeforeUnload = true });

            await Task.WhenAll(
                dialogTask.Task,
                closeTask.Task
            );
        }
    }
}