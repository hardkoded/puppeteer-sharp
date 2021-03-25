using System;
using System.Threading.Tasks;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Tests.Attributes;
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

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWhenFileInputIsAttachedToDOM()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Assert.NotNull(waitForTask.Result);
        }

        [SkipBrowserFact(skipFirefox: true)]
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

        [SkipBrowserFact(skipFirefox: true)]
        public Task ShouldRespectTimeout()
        {
            return Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync(new WaitForFileChooserOptions
            {
                Timeout = 1
            }));
        }

        [SkipBrowserFact(skipFirefox: true)]
        public Task ShouldRespectTimeoutWhenThereIsNoCustomTimeout()
        {
            Page.DefaultTimeout = 1;
            return Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync());
        }

        [SkipBrowserFact(skipFirefox: true)]
        public Task ShouldPrioritizeExactTimeoutOverDefaultTimeout()
        {
            Page.DefaultTimeout = 0;
            return Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync(new WaitForFileChooserOptions
            {
                Timeout = 1
            }));
        }

        [SkipBrowserFact(skipFirefox: true)]
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

        [SkipBrowserFact(skipFirefox: true)]
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
