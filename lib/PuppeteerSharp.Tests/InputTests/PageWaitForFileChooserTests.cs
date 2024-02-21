using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InputTests
{
    public class PageWaitForFileChooserTests : PuppeteerPageBaseTest
    {
        public PageWaitForFileChooserTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("input.spec", "Page.waitForFileChooser", "should work when file input is attached to DOM")]
        public async Task ShouldWorkWhenFileInputIsAttachedToDOM()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.ClickAsync("input"));

            Assert.NotNull(waitForTask.Result);
        }

        [Test, Retry(2), PuppeteerTest("input.spec", "Page.waitForFileChooser", "should work when file input is not attached to DOM")]
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

        [Test, Retry(2), PuppeteerTest("input.spec", "Page.waitForFileChooser", "should respect timeout")]
        public void ShouldRespectTimeout()
        {
            Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync(new WaitForOptions
            {
                Timeout = 1
            }));
        }

        [Test, Retry(2), PuppeteerTest("input.spec", "Page.waitForFileChooser", "should respect default timeout when there is no custom timeout")]
        public void ShouldRespectTimeoutWhenThereIsNoCustomTimeout()
        {
            Page.DefaultTimeout = 1;
            Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync());
        }

        [Test, Retry(2), PuppeteerTest("input.spec", "Page.waitForFileChooser", "should prioritize exact timeout over default timeout")]
        public void ShouldPrioritizeExactTimeoutOverDefaultTimeout()
        {
            Page.DefaultTimeout = 0;
            Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync(new WaitForOptions
            {
                Timeout = 1
            }));
        }

        [Test, Retry(2), PuppeteerTest("input.spec", "Page.waitForFileChooser", "should work with no timeout")]
        public async Task ShouldWorkWithNoTimeout()
        {
            var waitForTask = Page.WaitForFileChooserAsync(new WaitForOptions { Timeout = 0 });

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

        [Test, Retry(2), PuppeteerTest("input.spec", "Page.waitForFileChooser", "should return the same file chooser when there are many watchdogs simultaneously")]
        public async Task ShouldReturnTheSameFileChooserWhenThereAreManyWatchdogsSimultaneously()
        {
            await Page.SetContentAsync("<input type=file>");
            var fileChooserTask1 = Page.WaitForFileChooserAsync();
            var fileChooserTask2 = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                fileChooserTask1,
                fileChooserTask2,
                Page.QuerySelectorAsync("input").EvaluateFunctionAsync("input => input.click()"));
            Assert.AreSame(fileChooserTask1.Result, fileChooserTask2.Result);
        }
    }
}
