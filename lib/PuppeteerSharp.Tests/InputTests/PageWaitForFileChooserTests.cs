using System;
using System.Threading.Tasks;
using PuppeteerSharp.Mobile;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.InputTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageWaitForFileChooserTests : PuppeteerPageBaseTest
    {
        public PageWaitForFileChooserTests(ITestOutputHelper output) : base(output)
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
        public async Task ShouldWorkWhenFileInputIsNotAttachedToDOM()
        {
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.EvaluateFunctionAsync(@"() =>
                {
                    const el = document.createElement('input');
                    el.type = 'file';
                    el.click();
                }"));

            Assert.NotNull(waitForTask.Result);
        }

        [Fact]
        public async Task ShouldRespectTimeout()
        {
            var ex = await Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync(new WaitForFileChooserOptions
            {
                Timeout = 1
            }));
        }

        [Fact]
        public async Task ShouldRespectTimeoutWhenThereIsNoCustomTimeout()
        {
            Page.DefaultTimeout = 1;
            var ex = await Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync());
        }

        [Fact]
        public async Task ShouldPrioritizeExactTimeoutOverDefaultTimeout()
        {
            Page.DefaultTimeout = 0;
            var ex = await Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync(new WaitForFileChooserOptions
            {
                Timeout = 1
            }));
        }

        [Fact]
        public async Task ShouldWorkWithNoTimeout()
        {
            var waitForTask = Page.WaitForFileChooserAsync(new WaitForFileChooserOptions { Timeout = 0 });

            await Task.WhenAll(
                waitForTask,
                Page.EvaluateFunctionAsync(@"() => setTimeout(() =>
                {
                    const el = document.createElement('input');
                    el.type = 'file';
                    el.click();
                }, 50)"));
            Assert.NotNull(waitForTask.Result);
        }

        [Fact]
        public async Task ShouldReturnTheSameFileChooserWhenThereAreManyWatchdogsSimultaneously()
        {
            await Page.SetContentAsync("<input type=file>");
            var fileChooserTask1 = Page.WaitForFileChooserAsync();
            var fileChooserTask2 = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                fileChooserTask1,
                fileChooserTask2,
                Page.QuerySelectorAsync("input").EvaluateFunctionAsync("input => input.click()"));
            Assert.Same(fileChooserTask1.Result, fileChooserTask2.Result);
        }
    }
}
