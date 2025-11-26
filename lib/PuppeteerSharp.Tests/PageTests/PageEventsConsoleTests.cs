using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsConsoleTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.Events.Console", "should work")]
        public async Task ShouldWork()
        {
            var messageTask = new TaskCompletionSource<ConsoleMessage>();

            Page.Console += (_, e) => messageTask.TrySetResult(e.Message);

            await Task.WhenAll(
                messageTask.Task,
                Page.EvaluateFunctionAsync(@"() => {
                    return console.log('hello', 5, {foo: 'bar'});
                }"));

            var message = await messageTask.Task;

            Assert.That(message.Text, Is.EqualTo("hello 5 JSHandle@object"));
            Assert.That(message.Type, Is.EqualTo(ConsoleType.Log));
            Assert.That(message.Args.Count, Is.EqualTo(3));

            Assert.That(await message.Args[0].JsonValueAsync<string>(), Is.EqualTo("hello"));
            Assert.That(await message.Args[1].JsonValueAsync<int>(), Is.EqualTo(5));

            // Third argument is an object {foo: "bar"}
            // CDP returns a JsonElement, BiDi returns RemoteValueDictionary
            // Use a property accessor to extract and verify the value
            var fooProperty = await message.Args[2].GetPropertyAsync("foo");
            Assert.That(await fooProperty.JsonValueAsync<string>(), Is.EqualTo("bar"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Console", "should work on script call right after navigation")]
        public async Task ShouldWorkOnScriptCallRightAfterNavigation()
        {
            var messageTask = new TaskCompletionSource<ConsoleMessage>();

            Page.Console += (_, e) => messageTask.TrySetResult(e.Message);

            await Task.WhenAll(
                messageTask.Task,
                // Firefox prints warn if <!DOCTYPE html> is not present
                Page.GoToAsync("data:text/html,<!DOCTYPE html><script>console.log('SOME_LOG_MESSAGE');</script>"));

            var message = await messageTask.Task;
            Assert.That(message.Text, Is.EqualTo("SOME_LOG_MESSAGE"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Console", "should work for different console API calls with logging functions")]
        public async Task ShouldWorkForDifferentConsoleApiCallsWithLoggingFunctions()
        {
            var messages = new List<ConsoleMessage>();

            Page.Console += (_, e) => messages.Add(e.Message);

            // All console events will be reported before `page.evaluate` is finished.
            await Page.EvaluateFunctionAsync(@"() => {
              console.trace('calling console.trace');
              console.dir('calling console.dir');
              console.warn('calling console.warn');
              console.error('calling console.error');
              console.log(Promise.resolve('should not wait until resolved!'));
            }");

            Assert.That(messages
                .Select(_ => _.Type)
                .ToArray(), Is.EqualTo(new[]
            {
                ConsoleType.Trace,
                ConsoleType.Dir,
                ConsoleType.Warning,
                ConsoleType.Error,
                ConsoleType.Log
            }));

            Assert.That(messages
                .Select(msg => msg.Text)
                .ToArray(), Is.EqualTo(new[]
            {
                "calling console.trace",
                "calling console.dir",
                "calling console.warn",
                "calling console.error",
                "JSHandle@promise"
            }));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Console", "should work for different console API calls with timing functions")]
        public async Task ShouldWorkForDifferentConsoleApiCallsWithTimingFunctions()
        {
            var messages = new List<ConsoleMessage>();

            Page.Console += (_, e) => messages.Add(e.Message);

            // All console events will be reported before `page.evaluate` is finished.
            await Page.EvaluateFunctionAsync(@"() => {
              // A pair of time/timeEnd generates only one Console API call.
              console.time('calling console.time');
              console.timeEnd('calling console.time');
            }");

            Assert.That(messages
                .Select(_ => _.Type)
                .ToArray(), Is.EqualTo(new[]
            {
                ConsoleType.TimeEnd
            }));

            Assert.That(messages[0].Text, Does.Contain("calling console.time"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Console", "should work for different console API calls with group functions")]
        public async Task ShouldWorkForDifferentConsoleApiCallsWithGroupFunctions()
        {
            var messages = new List<ConsoleMessage>();

            Page.Console += (_, e) => messages.Add(e.Message);

            // All console events will be reported before `page.evaluate` is finished.
            await Page.EvaluateFunctionAsync(@"() => {
              console.group('calling console.group');
              console.groupEnd();
            }");

            Assert.That(messages
                .Select(_ => _.Type)
                .ToArray(), Is.EqualTo(new[]
            {
                ConsoleType.StartGroup,
                ConsoleType.EndGroup
            }));

            // We should be able to check both messages, but Chrome report text
            Assert.That(messages[0].Text, Does.Contain("calling console.group"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Console", "should not fail for window object")]
        public async Task ShouldNotFailForWindowObject()
        {
            var messageTask = new TaskCompletionSource<ConsoleMessage>();

            Page.Console += (_, e) => messageTask.TrySetResult(e.Message);

            await Task.WhenAll(
                messageTask.Task,
                Page.EvaluateFunctionAsync(@"() => {
                    return console.error(window);
                }"));

            var message = await messageTask.Task;
            Assert.That(
                new[] { "JSHandle@object", "JSHandle@window" },
                Does.Contain(message.Text));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Console", "should return remote objects")]
        public async Task ShouldReturnRemoteObjects()
        {
            var logTask = new TaskCompletionSource<ConsoleMessage>();

            Page.Console += (_, e) => logTask.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync(@"() => {
                globalThis.test = 1;
                console.log(1, 2, 3, globalThis);
            }");

            var log = await logTask.Task;

            Assert.That(
                new[] { "1 2 3 JSHandle@object", "1 2 3 JSHandle@window" },
                Does.Contain(log.Text));
            Assert.That(log.Args.Count, Is.EqualTo(4));

            var property = await log.Args[3].GetPropertyAsync("test");
            Assert.That(await property.JsonValueAsync<int>(), Is.EqualTo(1));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Console", "should trigger correct Log")]
        public async Task ShouldTriggerCorrectLog()
        {
            // Navigate to about:blank first (different origin than the test server)
            await Page.GoToAsync(TestConstants.AboutBlank);
            var messageTask = new TaskCompletionSource<ConsoleMessage>();

            Page.Console += (_, e) => messageTask.TrySetResult(e.Message);

            // Fetch from a different origin to trigger CORS error
            await Task.WhenAll(
                messageTask.Task,
                Page.EvaluateFunctionAsync(
                    "async url => await fetch(url).catch(() => {})",
                    TestConstants.EmptyPage));

            var message = await messageTask.Task;
            Assert.That(message.Text, Does.Contain("Access-Control-Allow-Origin"));

            if (TestConstants.IsChrome)
            {
                Assert.That(message.Type, Is.EqualTo(ConsoleType.Error));
            }
            else
            {
                Assert.That(message.Type, Is.EqualTo(ConsoleType.Warning));
            }
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Console", "should have location when fetch fails")]
        public async Task ShouldHaveLocationWhenFetchFails()
        {
            // The point of this test is to make sure that we report console messages from
            // Log domain: https://vanilla.aslushnikov.com/?Log.entryAdded
            await Page.GoToAsync(TestConstants.EmptyPage);
            var consoleTask = new TaskCompletionSource<ConsoleMessage>();
            Page.Console += (_, e) =>
            {
                // Wait for the specific network error message
                if (e.Message.Text.Contains("ERR_NAME"))
                {
                    consoleTask.TrySetResult(e.Message);
                }
            };

            await Task.WhenAll(
                consoleTask.Task,
                Page.SetContentAsync("<script>fetch('http://wat');</script>"));

            var message = await consoleTask.Task;
            Assert.That(message.Text, Does.Contain("ERR_NAME"));
            Assert.That(message.Type, Is.EqualTo(ConsoleType.Error));
            Assert.That(message.Location, Is.EqualTo(new ConsoleMessageLocation
            {
                URL = "http://wat/",
                LineNumber = null,
            }));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Console", "should have location and stack trace for console API calls")]
        public async Task ShouldHaveLocationForConsoleAPICalls()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var consoleTask = new TaskCompletionSource<ConsoleMessage>();

            void ConsoleHandler(object sender, ConsoleEventArgs e)
            {
                // Wait for the console.trace message specifically
                if (e.Message.Type == ConsoleType.Trace)
                {
                    consoleTask.TrySetResult(e.Message);
                    Page.Console -= ConsoleHandler;
                }
            }

            Page.Console += ConsoleHandler;

            await Task.WhenAll(
                consoleTask.Task,
                Page.GoToAsync(TestConstants.ServerUrl + "/consoletrace.html"));

            var message = await consoleTask.Task;
            Assert.That(message.Text, Is.EqualTo("yellow"));
            Assert.That(message.Type, Is.EqualTo(ConsoleType.Trace));
            Assert.That(message.Location, Is.EqualTo(new ConsoleMessageLocation
            {
                URL = TestConstants.ServerUrl + "/consoletrace.html",
                LineNumber = 8,
                ColumnNumber = 16,
            }));
            Assert.That(message.StackTrace, Is.EqualTo(new[]
            {
                new ConsoleMessageLocation
                {
                    URL = TestConstants.ServerUrl + "/consoletrace.html",
                    LineNumber = 8,
                    ColumnNumber = 16,
                },
                new ConsoleMessageLocation
                {
                    URL = TestConstants.ServerUrl + "/consoletrace.html",
                    LineNumber = 11,
                    ColumnNumber = 8,
                },
                new ConsoleMessageLocation
                {
                    URL = TestConstants.ServerUrl + "/consoletrace.html",
                    LineNumber = 13,
                    ColumnNumber = 6,
                },
            }));
        }

        // @see https://github.com/puppeteer/puppeteer/issues/3865
        [Test, PuppeteerTest("page.spec", "Page Page.Events.Console", "should not throw when there are console messages in detached iframes")]
        public async Task ShouldNotThrowWhenThereAreConsoleMessagesInDetachedIframes()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"async () =>
            {
                // 1. Create a popup that Puppeteer is not connected to.
                const win = window.open(
                    window.location.href,
                    'Title',
                    'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=780,height=200,top=0,left=0'
                );
                await new Promise(x => { return (win.onload = x); });
                // 2. In this popup, create an iframe that console.logs a message.
                win.document.body.innerHTML = `<iframe src='/consolelog.html'></iframe>`;
                const frame = win.document.querySelector('iframe');
                await new Promise(x => { return (frame.onload = x); });
                // 3. After that, remove the iframe.
                frame.remove();
            }");
            // 4. The target will always be the last one.
#pragma warning disable CS0618 // Type or member is obsolete
            var targets = Page.BrowserContext.Targets();
            var popupTarget = targets.Last(t => t != Page.Target);
#pragma warning restore CS0618 // Type or member is obsolete
            // 5. Connect to the popup and make sure it doesn't throw and is not the same page.
            Assert.That(await popupTarget.PageAsync(), Is.Not.EqualTo(Page));
        }
    }
}
