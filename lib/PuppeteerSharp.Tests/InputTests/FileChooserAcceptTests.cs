using System;
using System.Threading.Tasks;
using PuppeteerSharp.Mobile;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.InputTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FileChooserAcceptTests : PuppeteerPageBaseTest
    {
        public FileChooserAcceptTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWorkWhenFileInputIsAttachedToDOM()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Assert.NotNull(waitForTask.Result);
        }

        [Fact]
        public async Task ShouldAcceptSingleFile()
        {
            await Page.SetContentAsync("<input type=file oninput='javascript:console.timeStamp()'>");
            var waitForTask = Page.WaitForFileChooserAsync();
            var metricsTcs = new TaskCompletionSource<bool>();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Page.Metrics += (sender, e) => metricsTcs.TrySetResult(true);

            await Task.WhenAll(
                waitForTask.Result.AcceptAsync(TestConstants.FileToUpload),
                metricsTcs.Task);

            Assert.Equal(1, await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<int>("input => input.files.length"));
            Assert.Equal(
                "file-to-upload.txt",
                await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<string>("input => input.files[0].name"));
        }

        [Fact]
        public async Task ShouldBeAbleToReadSelectedFile()
        {
            await Page.SetContentAsync("<input type=file>");
            _ = Page.WaitForFileChooserAsync().ContinueWith(t => t.Result.AcceptAsync(TestConstants.FileToUpload));

            Assert.Equal(
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

        [Fact]
        public async Task ShouldBeAbleToResetSelectedFilesWithEmptyFileList()
        {
            await Page.SetContentAsync("<input type=file>");
            _ = Page.WaitForFileChooserAsync().ContinueWith(t => t.Result.AcceptAsync(TestConstants.FileToUpload));

            Assert.Equal(
                1,
                await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<int>(@"async picker =>
                {
                picker.click();
                await new Promise(x => picker.oninput = x);
                return picker.files.length;
            }"));

            _ = Page.WaitForFileChooserAsync().ContinueWith(t => t.Result.AcceptAsync());

            Assert.Equal(
                0,
                await Page.QuerySelectorAsync("input").EvaluateFunctionAsync<int>(@"async picker =>
                {
                picker.click();
                await new Promise(x => picker.oninput = x);
                return picker.files.length;
            }"));
        }

        [Fact]
        public async Task ShouldNotAcceptMultipleFilesForSingleFileInput()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            var ex = await Assert.ThrowsAsync<MessageException>(() => waitForTask.Result.AcceptAsync(
                "./assets/file-to-upload.txt",
                "./assets/pptr.png"));
        }

        [Fact]
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
            Assert.Equal("Cannot accept FileChooser which is already handled!", ex.Message);
        }
    }
}
