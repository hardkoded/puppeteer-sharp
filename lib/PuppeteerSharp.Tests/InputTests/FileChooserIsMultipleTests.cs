using System;
using System.Threading.Tasks;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InputTests
{
    public class FileChooserIsMultipleTests : PuppeteerPageBaseTest
    {
        public FileChooserIsMultipleTests(): base()
        {
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.isMultiple", "should work for single file pick")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkForSingleFilePick()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Assert.False(waitForTask.Result.IsMultiple);
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.isMultiple", "should work for \"multiple\"")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkForMultiple()
        {
            await Page.SetContentAsync("<input type=file multiple>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Assert.True(waitForTask.Result.IsMultiple);
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.isMultiple", "should work for \"webkitdirectory\"")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkForWebkitDirectory()
        {
            await Page.SetContentAsync("<input type=file multiple webkitdirectory>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Assert.True(waitForTask.Result.IsMultiple);
        }
    }
}
