using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InputTests
{
    public class PageWaitForFileChooserTests : PuppeteerPageBaseTest
    {
        public PageWaitForFileChooserTests()
            : base() { }

        [
            Test,
            PuppeteerTest(
                "input.spec",
                "input tests Page.waitForFileChooser",
                "should work when file input is attached to DOM"
            )
        ]
        public async Task ShouldWorkWhenFileInputIsAttachedToDOM()
        {
            await Page.SetContentAsync("<input type=file>");
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(waitForTask, Page.ClickAsync("input"));

            Assert.That(waitForTask.Result, Is.Not.Null);
        }

        [
            Test,
            PuppeteerTest(
                "input.spec",
                "input tests Page.waitForFileChooser",
                "should work when file input is not attached to DOM"
            )
        ]
        public async Task ShouldWorkWhenFileInputIsNotAttachedToDOM()
        {
            var waitForTask = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                waitForTask,
                Page.EvaluateFunctionAsync(
                    @"() =>
                {
                    const el = document.createElement('input');
                    el.type = 'file';
                    el.click();
                }"
                )
            );

            Assert.That(waitForTask.Result, Is.Not.Null);
        }

        [
            Test,
            PuppeteerTest(
                "input.spec",
                "input tests Page.waitForFileChooser",
                "should respect timeout"
            )
        ]
        public void ShouldRespectTimeout()
        {
            Assert.ThrowsAsync<TimeoutException>(() =>
                Page.WaitForFileChooserAsync(new WaitForOptions { Timeout = 1 })
            );
        }

        [
            Test,
            PuppeteerTest(
                "input.spec",
                "input tests Page.waitForFileChooser",
                "should respect default timeout when there is no custom timeout"
            )
        ]
        public void ShouldRespectTimeoutWhenThereIsNoCustomTimeout()
        {
            Page.DefaultTimeout = 1;
            Assert.ThrowsAsync<TimeoutException>(() => Page.WaitForFileChooserAsync());
        }

        [
            Test,
            PuppeteerTest(
                "input.spec",
                "input tests Page.waitForFileChooser",
                "should prioritize exact timeout over default timeout"
            )
        ]
        public void ShouldPrioritizeExactTimeoutOverDefaultTimeout()
        {
            Page.DefaultTimeout = 0;
            Assert.ThrowsAsync<TimeoutException>(() =>
                Page.WaitForFileChooserAsync(new WaitForOptions { Timeout = 1 })
            );
        }

        [
            Test,
            PuppeteerTest(
                "input.spec",
                "input tests Page.waitForFileChooser",
                "should work with no timeout"
            )
        ]
        public async Task ShouldWorkWithNoTimeout()
        {
            var waitForTask = Page.WaitForFileChooserAsync(new WaitForOptions { Timeout = 0 });

            await Task.WhenAll(
                waitForTask,
                Page.EvaluateFunctionAsync(
                    @"() => setTimeout(() =>
                {
                    const el = document.createElement('input');
                    el.type = 'file';
                    el.click();
                }, 50)"
                )
            );
            Assert.That(waitForTask.Result, Is.Not.Null);
        }

        [
            Test,
            PuppeteerTest(
                "input.spec",
                "input tests Page.waitForFileChooser",
                "should return the same file chooser when there are many watchdogs simultaneously"
            )
        ]
        public async Task ShouldReturnTheSameFileChooserWhenThereAreManyWatchdogsSimultaneously()
        {
            await Page.SetContentAsync("<input type=file>");
            var fileChooserTask1 = Page.WaitForFileChooserAsync();
            var fileChooserTask2 = Page.WaitForFileChooserAsync();

            await Task.WhenAll(
                fileChooserTask1,
                fileChooserTask2,
                Page.QuerySelectorAsync("input").EvaluateFunctionAsync("input => input.click()")
            );
            Assert.That(fileChooserTask2.Result, Is.SameAs(fileChooserTask1.Result));
        }

        [
            Test,
            PuppeteerTest(
                "input.spec",
                "input tests Page.waitForFileChooser",
                "should not throw unhandled rejection when file picker is opened without an input element"
            )
        ]
        public async Task ShouldNotThrowUnhandledRejectionWhenFilePickerIsOpenedWithoutAnInputElement()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync(
                "<button onclick=\"window.showOpenFilePicker?.().catch(() => {})\"></button>"
            );

            Assert.ThrowsAsync<TimeoutException>(() =>
                Task.WhenAll(
                    Page.WaitForFileChooserAsync(new WaitForOptions { Timeout = 200 }),
                    Page.ClickAsync("button")
                )
            );

            Assert.That(await Page.EvaluateExpressionAsync<int>("1 + 1"), Is.EqualTo(2));
        }

        [
            Test,
            PuppeteerTest(
                "input.spec",
                "input tests Page.waitForFileChooser",
                "should stop intercepting file choosers when no longer waiting"
            )
        ]
        public async Task ShouldStopInterceptingFileChoosersWhenNoLongerWaiting()
        {
            await Page.SetContentAsync("<input type=\"file\" />");
            var client = ((CdpPage)Page).Client;
            var fileChooserOpenedEvents = 0;
            void OnFileChooserOpened(object sender, MessageEventArgs e)
            {
                if (e.MessageID == "Page.fileChooserOpened")
                {
                    Interlocked.Increment(ref fileChooserOpenedEvents);
                }
            }

            client.MessageReceived += OnFileChooserOpened;
            try
            {
                var waitForTask = Page.WaitForFileChooserAsync();
                await Task.WhenAll(waitForTask, Page.ClickAsync("input"));
                await waitForTask.Result.AcceptAsync(TestConstants.FileToUpload);
                Assert.That(fileChooserOpenedEvents, Is.EqualTo(1));

                // Nobody is waiting, so this file chooser must not be intercepted.
                await Page.ClickAsync("input");

                // Events arrive in order, so an intercepted file chooser from the click
                // above would be reported before this one.
                await Task.WhenAll(Page.WaitForFileChooserAsync(), Page.ClickAsync("input"));
                Assert.That(fileChooserOpenedEvents, Is.EqualTo(2));
            }
            finally
            {
                client.MessageReceived -= OnFileChooserOpened;
            }
        }

        [
            Test,
            PuppeteerTest(
                "input.spec",
                "input tests Page.waitForFileChooser",
                "should stop intercepting file choosers after a timeout"
            )
        ]
        public async Task ShouldStopInterceptingFileChoosersAfterATimeout()
        {
            await Page.SetContentAsync("<input type=\"file\" />");
            var client = ((CdpPage)Page).Client;
            var fileChooserOpenedEvents = 0;
            void OnFileChooserOpened(object sender, MessageEventArgs e)
            {
                if (e.MessageID == "Page.fileChooserOpened")
                {
                    Interlocked.Increment(ref fileChooserOpenedEvents);
                }
            }

            client.MessageReceived += OnFileChooserOpened;
            try
            {
                Assert.ThrowsAsync<TimeoutException>(() =>
                    Page.WaitForFileChooserAsync(new WaitForOptions { Timeout = 1 })
                );

                // Nobody is waiting, so this file chooser must not be intercepted.
                await Page.ClickAsync("input");

                // Events arrive in order, so an intercepted file chooser from the click
                // above would be reported before this one.
                await Task.WhenAll(Page.WaitForFileChooserAsync(), Page.ClickAsync("input"));
                Assert.That(fileChooserOpenedEvents, Is.EqualTo(1));
            }
            finally
            {
                client.MessageReceived -= OnFileChooserOpened;
            }
        }
    }
}
