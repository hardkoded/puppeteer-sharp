using System;
using System.Threading.Tasks;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InputTests
{
    public class FileChooserAcceptTests : PuppeteerPageBaseTest
    {
        public FileChooserAcceptTests(): base()
        {
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.accept", "should accept single file")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

            Assert.AreEqual(1, await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<int>("input => input.files.length"));
            Assert.AreEqual(
                "file-to-upload.txt",
                await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<string>("input => input.files[0].name"));
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.accept", "should be able to read selected file")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldBeAbleToReadSelectedFile()
        {
            await Page.SetContentAsync("<input type=file>");
            _ = Page.WaitForFileChooserAsync().ContinueWith(t => t.Result.AcceptAsync(TestConstants.FileToUpload));

            Assert.AreEqual(
                "contents of the file",
                await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<string>(@"async picker =>
                {
                    picker.click();
                    await new Promise(x => picker.oninput = x);
                    const reader = new FileReader();
                    const promise = new Promise(fulfill => reader.onload = fulfill);
                    reader.readAsText(picker.files[0]);
                    return promise.then(() => reader.result);
                }"));
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.accept", "should be able to reset selected files with empty file list")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldBeAbleToResetSelectedFilesWithEmptyFileList()
        {
            await Page.SetContentAsync("<input type=file>");
            _ = Page.WaitForFileChooserAsync().ContinueWith(t => t.Result.AcceptAsync(TestConstants.FileToUpload));

            Assert.AreEqual(
                1,
                await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<int>(@"async picker =>
                {
                picker.click();
                await new Promise(x => picker.oninput = x);
                return picker.files.length;
            }"));

            _ = Page.WaitForFileChooserAsync().ContinueWith(t => t.Result.AcceptAsync());

            Assert.AreEqual(
                0,
                await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<int>(@"async picker =>
                {
                picker.click();
                await new Promise(x => picker.oninput = x);
                return picker.files.length;
            }"));
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.accept", "should not accept multiple files for single-file input")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldNotAcceptMultipleFilesForSingleFileInput()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            await Assert.ThrowsAsync<PuppeteerException>(() => waitForTask.Result.AcceptAsync(
                "./assets/file-to-upload.txt",
                "./assets/pptr.png"));
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.accept", "should fail for non-existent files")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldFailForNonExistentFiles()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            await Assert.ThrowsAsync<PuppeteerException>(() => waitForTask.Result.AcceptAsync("file-does-not-exist.txt"));
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.accept", "should fail when accepting file chooser twice")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldFailWhenAcceptingFileChooserTwice()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            var fileChooser = waitForTask.Result;
            await fileChooser.AcceptAsync();
            var ex = await Assert.ThrowsAsync<PuppeteerException>(() => waitForTask.Result.AcceptAsync());
            Assert.AreEqual("Cannot accept FileChooser which is already handled!", ex.Message);
        }
    }
}
