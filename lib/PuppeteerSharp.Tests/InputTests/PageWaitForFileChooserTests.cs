using System;
using System.Threading.Tasks;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InputTests
{
    public class PageWaitForFileChooserTests : PuppeteerPageBaseTest
    {
        public PageWaitForFileChooserTests(): base()
        {
        }

        [PuppeteerTest("input.spec.ts", "Page.waitForFileChooser", "should work when file input is attached to DOM")]
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

        [PuppeteerTest("input.spec.ts", "Page.waitForFileChooser", "should work when file input is not attached to DOM")]
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

        [PuppeteerTest("input.spec.ts", "Page.waitForFileChooser", "should respect timeout")]
        [SkipBrowserFact(skipFirefox: true)]
        public Task ShouldRespectTimeout()
        {
            return Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync(new WaitForFileChooserOptions
            {
                Timeout = 1
            }));
        }

        [PuppeteerTest("input.spec.ts", "Page.waitForFileChooser", "should respect default timeout when there is no custom timeout")]
        [SkipBrowserFact(skipFirefox: true)]
        public Task ShouldRespectTimeoutWhenThereIsNoCustomTimeout()
        {
            Page.DefaultTimeout = 1;
            return Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync());
        }

        [PuppeteerTest("input.spec.ts", "Page.waitForFileChooser", "should prioritize exact timeout over default timeout")]
        [SkipBrowserFact(skipFirefox: true)]
        public Task ShouldPrioritizeExactTimeoutOverDefaultTimeout()
        {
            Page.DefaultTimeout = 0;
            return Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync(new WaitForFileChooserOptions
            {
                Timeout = 1
            }));
        }

        [PuppeteerTest("input.spec.ts", "Page.waitForFileChooser", "should work with no timeout")]
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

        [PuppeteerTest("input.spec.ts", "Page.waitForFileChooser", "should return the same file chooser when there are many watchdogs simultaneously")]
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
