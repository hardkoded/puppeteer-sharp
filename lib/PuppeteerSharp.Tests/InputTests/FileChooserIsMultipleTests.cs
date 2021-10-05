using System;
using System.Threading.Tasks;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.InputTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FileChooserIsMultipleTests : PuppeteerPageBaseTest
    {
        public FileChooserIsMultipleTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("input.spec.ts", "FileChooser.isMultiple", "should work for single file pick")]
        [Fact]
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
        [Fact]
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
        [Fact]
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
