using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InputTests
{
    public class FileChooserIsMultipleTests : PuppeteerPageBaseTest
    {
        public FileChooserIsMultipleTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("input.spec", "FileChooser.isMultiple", "should work for single file pick")]
        public async Task ShouldWorkForSingleFilePick()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Assert.That(waitForTask.Result.IsMultiple, Is.False);
        }

        [Test, Retry(2), PuppeteerTest("input.spec", "FileChooser.isMultiple", "should work for \"multiple\"")]
        public async Task ShouldWorkForMultiple()
        {
            await Page.SetContentAsync("<input type=file multiple>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Assert.That(waitForTask.Result.IsMultiple, Is.True);
        }

        [Test, Retry(2), PuppeteerTest("input.spec", "FileChooser.isMultiple", "should work for \"webkitdirectory\"")]
        public async Task ShouldWorkForWebkitDirectory()
        {
            await Page.SetContentAsync("<input type=file multiple webkitdirectory>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Assert.That(waitForTask.Result.IsMultiple, Is.True);
        }
    }
}
