using System;
using System.Threading.Tasks;
using PuppeteerSharp.Mobile;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.InputTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FileChooserCancelTests : PuppeteerPageBaseTest
    {
        public FileChooserCancelTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldCancelDialog()
        {
            // Consider file chooser canceled if we can summon another one.
            // There's no reliable way in WebPlatform to see that FileChooser was
            // canceled.
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            var fileChooser = waitForTask.Result;
            await fileChooser.CancelAsync();

            await Task.WhenAll(
                Page.WaitForFileChooserAsync(),
                Page.ClickAsync("input"));
        }

        [Fact]
        public async Task ShouldFailWhenCancelingFileChooserTwice()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            var fileChooser = waitForTask.Result;
            await fileChooser.CancelAsync();

            var ex = await Assert.ThrowsAsync<PuppeteerException>(() => fileChooser.CancelAsync());
            Assert.Equal("Cannot accept FileChooser which is already handled!", ex.Message);
        }
    }
}
