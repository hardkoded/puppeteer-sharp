using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.PageTests.Events
{
    public class PageEventsConsoleTests : PuppeteerPageBaseTest
    {
        public PageEventsConsoleTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            ConsoleMessage message = null;

            void EventHandler(object sender, ConsoleEventArgs e)
            {
                message = e.Message;
                Page.Console -= EventHandler;
            }

            Page.Console += EventHandler;

            await Page.EvaluateExpressionAsync("console.log('hello', 5, {foo: 'bar'})");

            var obj = new Dictionary<string, object> { { "foo", "bar" } };

            Assert.AreEqual("hello 5 JSHandle@object", message.Text);
            Assert.AreEqual(ConsoleType.Log, message.Type);

            Assert.AreEqual("hello", await message.Args[0].JsonValueAsync());
            Assert.AreEqual(5, await message.Args[1].JsonValueAsync<float>());
            Assert.AreEqual(obj, await message.Args[2].JsonValueAsync<Dictionary<string, object>>());
            Assert.AreEqual("bar", (await message.Args[2].JsonValueAsync<dynamic>()).foo.ToString());
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should work for different console API calls")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkForDifferentConsoleApiCalls()
        {
            var messages = new List<ConsoleMessage>();

            Page.Console += (_, e) => messages.Add(e.Message);

            await Page.EvaluateFunctionAsync(@"() => {
              // A pair of time/timeEnd generates only one Console API call.
              console.time('calling console.time');
              console.timeEnd('calling console.time');
              console.trace('calling console.trace');
              console.dir('calling console.dir');
              console.warn('calling console.warn');
              console.error('calling console.error');
              console.log(Promise.resolve('should not wait until resolved!'));
            }");

            Assert.AreEqual(new[]
            {
                ConsoleType.TimeEnd,
                ConsoleType.Trace,
                ConsoleType.Dir,
                ConsoleType.Warning,
                ConsoleType.Error,
                ConsoleType.Log
            }, messages
                .Select(_ => _.Type)
                .ToArray());

            StringAssert.Contains("calling console.time", messages[0].Text);

            Assert.AreEqual(new[]
            {
                "calling console.trace",
                "calling console.dir",
                "calling console.warn",
                "calling console.error",
                "JSHandle@promise"
            }, messages
                .Skip(1)
                .Select(msg => msg.Text)
                .ToArray());
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should not fail for window object")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldNotFailForWindowObject()
        {
            var consoleTcs = new TaskCompletionSource<string>();

            void EventHandler(object sender, ConsoleEventArgs e)
            {
                consoleTcs.TrySetResult(e.Message.Text);
                Page.Console -= EventHandler;
            }

            Page.Console += EventHandler;

            await Task.WhenAll(
                consoleTcs.Task,
                Page.EvaluateExpressionAsync("console.error(window)")
            );

            Assert.AreEqual("JSHandle@object", await consoleTcs.Task);
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should trigger correct Log")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldTriggerCorrectLog()
        {
            await Page.GoToAsync(TestConstants.AboutBlank);
            var messageTask = new TaskCompletionSource<ConsoleMessage>();

            Page.Console += (_, e) => messageTask.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("async url => fetch(url).catch(e => {})", TestConstants.EmptyPage);
            var message = await messageTask.Task;
            StringAssert.Contains("No 'Access-Control-Allow-Origin'", message.Text);

            if (TestConstants.IsChrome)
            {
                Assert.AreEqual(ConsoleType.Error, message.Type);
            }
            else
            {
                Assert.AreEqual(ConsoleType.Warning, message.Type);
            }
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should have location when fetch fails")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldHaveLocationWhenFetchFails()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var consoleTask = new TaskCompletionSource<ConsoleEventArgs>();
            Page.Console += (_, e) => consoleTask.TrySetResult(e);

            await Task.WhenAll(
                consoleTask.Task,
                Page.SetContentAsync("<script>fetch('http://wat');</script>"));

            var args = await consoleTask.Task;
            StringAssert.Contains("ERR_NAME", args.Message.Text);
            Assert.AreEqual(ConsoleType.Error, args.Message.Type);
            Assert.AreEqual(new ConsoleMessageLocation
            {
                URL = "http://wat/",
            }, args.Message.Location);
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should have location and stack trace for console API calls")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldHaveLocationForConsoleAPICalls()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var consoleTask = new TaskCompletionSource<ConsoleEventArgs>();
            Page.Console += (_, e) => consoleTask.TrySetResult(e);

            await Task.WhenAll(
                consoleTask.Task,
                Page.GoToAsync(TestConstants.ServerUrl + "/consolelog.html"));

            var args = await consoleTask.Task;
            Assert.AreEqual("yellow", args.Message.Text);
            Assert.AreEqual(ConsoleType.Log, args.Message.Type);
            Assert.AreEqual(new ConsoleMessageLocation
            {
                URL = TestConstants.ServerUrl + "/consolelog.html",
                LineNumber = 7,
                ColumnNumber = 14
            }, args.Message.Location);
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should not throw when there are console messages in detached iframes")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldNotThrowWhenThereAreConsoleMessagesInDetachedIframes()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"async () =>
            {
                // 1. Create a popup that Puppeteer is not connected to.
                const win = window.open(window.location.href, 'Title', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=780,height=200,top=0,left=0');
                await new Promise(x => win.onload = x);
                // 2. In this popup, create an iframe that console.logs a message.
                win.document.body.innerHTML = `<iframe src='/consolelog.html'></iframe>`;
                const frame = win.document.querySelector('iframe');
                await new Promise(x => frame.onload = x);
                // 3. After that, remove the iframe.
                frame.remove();
            }");
            var popupTarget = Page.BrowserContext.Targets().First(target => target != Page.Target);
            // 4. Connect to the popup and make sure it doesn't throw.
            await popupTarget.PageAsync();
        }

        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldNotFailForNullArgument()
        {
            var consoleTcs = new TaskCompletionSource<string>();

            void EventHandler(object sender, ConsoleEventArgs e)
            {
                consoleTcs.TrySetResult(e.Message.Text);
                Page.Console -= EventHandler;
            }

            Page.Console += EventHandler;

            await Task.WhenAll(
                consoleTcs.Task,
                Page.EvaluateExpressionAsync("console.debug(null);")
            );

            Assert.AreEqual("JSHandle:null", await consoleTcs.Task);
        }
    }
}
