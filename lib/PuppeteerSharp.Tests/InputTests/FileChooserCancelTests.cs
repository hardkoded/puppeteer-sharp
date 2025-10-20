using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InputTests
{
    public class FileChooserCancelTests : PuppeteerPageBaseTest
    {
        public FileChooserCancelTests() : base()
        {
        }

        [Test, PuppeteerTest("input.spec", "FileChooser.cancel", "should cancel dialog")]
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

        [Test, PuppeteerTest("input.spec", "FileChooser.cancel", "should fail when canceling file chooser twice")]
        public async Task ShouldFailWhenCancelingFileChooserTwice()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            var fileChooser = waitForTask.Result;
            await fileChooser.CancelAsync();

            var ex = Assert.ThrowsAsync<PuppeteerException>(() => fileChooser.CancelAsync());
            Assert.That(ex.Message, Is.EqualTo("Cannot accept FileChooser which is already handled!"));
        }
    }
}
