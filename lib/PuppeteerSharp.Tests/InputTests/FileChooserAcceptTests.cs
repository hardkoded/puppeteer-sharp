using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InputTests
{
    public class FileChooserAcceptTests : PuppeteerPageBaseTest
    {
        public FileChooserAcceptTests() : base()
        {
        }

        [Test, PuppeteerTest("input.spec", "FileChooser.accept", "should accept single file")]
        public async Task ShouldAcceptSingleFile()
        {
            await Page.SetContentAsync("<input type=file oninput='javascript:console.timeStamp()'>");
            var waitForTask = Page.WaitForFileChooserAsync();
            var metricsTcs = new TaskCompletionSource<bool>();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Page.Metrics += (_, _) => metricsTcs.TrySetResult(true);

            await Task.WhenAll(
                waitForTask.Result.AcceptAsync(TestConstants.FileToUpload),
                metricsTcs.Task);

            Assert.That(await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<int>("input => input.files.length"), Is.EqualTo(1));
            Assert.That(
                await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<string>("input => input.files[0].name"),
                Is.EqualTo("file-to-upload.txt"));
        }

        [Test, PuppeteerTest("input.spec", "FileChooser.accept", "should be able to read selected file")]
        public async Task ShouldBeAbleToReadSelectedFile()
        {
            await Page.SetContentAsync("<input type=file>");
            _ = Page.WaitForFileChooserAsync().ContinueWith(t => t.Result.AcceptAsync(TestConstants.FileToUpload));

            Assert.That(
                await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<string>(@"async picker =>
                {
                    picker.click();
                    await new Promise(x => picker.oninput = x);
                    const reader = new FileReader();
                    const promise = new Promise(fulfill => reader.onload = fulfill);
                    reader.readAsText(picker.files[0]);
                    return promise.then(() => reader.result);
                }"), Is.EqualTo("contents of the file"));
        }

        [Test, PuppeteerTest("input.spec", "FileChooser.accept", "should be able to reset selected files with empty file list")]
        public async Task ShouldBeAbleToResetSelectedFilesWithEmptyFileList()
        {
            await Page.SetContentAsync("<input type=file>");
            _ = Page.WaitForFileChooserAsync().ContinueWith(t => t.Result.AcceptAsync(TestConstants.FileToUpload));

            Assert.That(
                await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<int>(@"async picker =>
                {
                picker.click();
                await new Promise(x => picker.oninput = x);
                return picker.files.length;
            }"), Is.EqualTo(1));

            _ = Page.WaitForFileChooserAsync().ContinueWith(t => t.Result.AcceptAsync());

            Assert.That(
                await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<int>(@"async picker =>
                {
                picker.click();
                await new Promise(x => picker.oninput = x);
                return picker.files.length;
            }"), Is.EqualTo(0));
        }

        [Test, PuppeteerTest("input.spec", "FileChooser.accept", "should not accept multiple files for single-file input")]
        public async Task ShouldNotAcceptMultipleFilesForSingleFileInput()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Assert.ThrowsAsync<PuppeteerException>(() => waitForTask.Result.AcceptAsync(
                "./assets/file-to-upload.txt",
                "./assets/pptr.png"));
        }

        [Test, PuppeteerTest("input.spec", "FileChooser.accept", "should fail for non-existent files")]
        public async Task ShouldFailForNonExistentFiles()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Assert.ThrowsAsync<PuppeteerException>(() => waitForTask.Result.AcceptAsync("file-does-not-exist.txt"));
        }

        [Test, PuppeteerTest("input.spec", "FileChooser.accept", "should fail when accepting file chooser twice")]
        public async Task ShouldFailWhenAcceptingFileChooserTwice()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            var fileChooser = waitForTask.Result;
            await fileChooser.AcceptAsync();
            var ex = Assert.ThrowsAsync<PuppeteerException>(() => waitForTask.Result.AcceptAsync());
            Assert.That(ex.Message, Is.EqualTo("Cannot accept FileChooser which is already handled!"));
        }
    }
}
