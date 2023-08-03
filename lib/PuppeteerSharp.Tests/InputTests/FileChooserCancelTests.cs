using System;
using System.Threading.Tasks;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.InputTests
{
    public class FileChooserCancelTests : PuppeteerPageBaseTest
    {
        public FileChooserCancelTests(): base()
        {
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.cancel", "should cancel dialog")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            fileChooser.Cancel();

            await Task.WhenAll(
                Page.WaitForFileChooserAsync(),
                Page.ClickAsync("input"));
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.cancel", "should fail when canceling file chooser twice")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldFailWhenCancelingFileChooserTwice()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            var fileChooser = waitForTask.Result;
            fileChooser.Cancel();

            var ex = Assert.Throws<PuppeteerException>(() => fileChooser.Cancel());
            Assert.AreEqual("Cannot accept FileChooser which is already handled!", ex.Message);
        }
    }
}
