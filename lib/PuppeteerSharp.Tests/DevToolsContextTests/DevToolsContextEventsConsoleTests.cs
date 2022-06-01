using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using CefSharp.Puppeteer;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextEventsConsoleTests : PuppeteerPageBaseTest
    {
        public DevToolsContextEventsConsoleTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            ConsoleMessage message = null;

            void EventHandler(object sender, ConsoleEventArgs e)
            {
                message = e.Message;
                DevToolsContext.Console -= EventHandler;
            }

            DevToolsContext.Console += EventHandler;

            await DevToolsContext.EvaluateExpressionAsync("console.log('hello', 5, {foo: 'bar'})");

            var obj = new Dictionary<string, object> { { "foo", "bar" } };

            Assert.Equal("hello 5 JSHandle@object", message.Text);
            Assert.Equal(ConsoleType.Log, message.Type);

            Assert.Equal("hello", await message.Args[0].JsonValueAsync());
            Assert.Equal(5, await message.Args[1].JsonValueAsync<float>());
            Assert.Equal(obj, await message.Args[2].JsonValueAsync<Dictionary<string, object>>());
            Assert.Equal("bar", (await message.Args[2].JsonValueAsync<dynamic>()).foo.ToString());
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should work for different console API calls")]
        [PuppeteerFact]
        public async Task ShouldWorkForDifferentConsoleApiCalls()
        {
            var messages = new List<ConsoleMessage>();

            DevToolsContext.Console += (_, e) => messages.Add(e.Message);

            await DevToolsContext.EvaluateFunctionAsync(@"() => {
              // A pair of time/timeEnd generates only one Console API call.
              console.time('calling console.time');
              console.timeEnd('calling console.time');
              console.trace('calling console.trace');
              console.dir('calling console.dir');
              console.warn('calling console.warn');
              console.error('calling console.error');
              console.log(Promise.resolve('should not wait until resolved!'));
            }");

            Assert.Equal(new[]
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

            Assert.Contains("calling console.time", messages[0].Text);

            Assert.Equal(new[]
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
        [PuppeteerFact]
        public async Task ShouldNotFailForWindowObject()
        {
            var consoleTcs = new TaskCompletionSource<string>();

            void EventHandler(object sender, ConsoleEventArgs e)
            {
                consoleTcs.TrySetResult(e.Message.Text);
                DevToolsContext.Console -= EventHandler;
            }

            DevToolsContext.Console += EventHandler;

            await Task.WhenAll(
                consoleTcs.Task,
                DevToolsContext.EvaluateExpressionAsync("console.error(window)")
            );

            Assert.Equal("JSHandle@object", await consoleTcs.Task);
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should trigger correct Log")]
        [PuppeteerFact]
        public async Task ShouldTriggerCorrectLog()
        {
            await DevToolsContext.GoToAsync(TestConstants.AboutBlank);
            var messageTask = new TaskCompletionSource<ConsoleMessage>();

            DevToolsContext.Console += (_, e) => messageTask.TrySetResult(e.Message);

            await DevToolsContext.EvaluateFunctionAsync("async url => fetch(url).catch(e => {})", TestConstants.EmptyPage);
            var message = await messageTask.Task;
            Assert.Contains("No 'Access-Control-Allow-Origin'", message.Text);

            Assert.Equal(ConsoleType.Error, message.Type);
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should have location when fetch fails")]
        [PuppeteerFact]
        public async Task ShouldHaveLocationWhenFetchFails()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var consoleTask = new TaskCompletionSource<ConsoleEventArgs>();
            DevToolsContext.Console += (_, e) => consoleTask.TrySetResult(e);

            await Task.WhenAll(
                consoleTask.Task,
                DevToolsContext.SetContentAsync("<script>fetch('http://wat');</script>"));

            var args = await consoleTask.Task;
            Assert.Contains("ERR_NAME", args.Message.Text);
            Assert.Equal(ConsoleType.Error, args.Message.Type);
            Assert.Equal(new ConsoleMessageLocation
            {
                URL = "http://wat/",
            }, args.Message.Location);
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Console", "should have location and stack trace for console API calls")]
        [PuppeteerFact]
        public async Task ShouldHaveLocationForConsoleAPICalls()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var consoleTask = new TaskCompletionSource<ConsoleEventArgs>();
            DevToolsContext.Console += (_, e) => consoleTask.TrySetResult(e);

            await Task.WhenAll(
                consoleTask.Task,
                DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/consolelog.html"));

            var args = await consoleTask.Task;
            Assert.Equal("yellow", args.Message.Text);
            Assert.Equal(ConsoleType.Log, args.Message.Type);
            Assert.Equal(new ConsoleMessageLocation
            {
                URL = TestConstants.ServerUrl + "/consolelog.html",
                LineNumber = 7,
                ColumnNumber = 14
            }, args.Message.Location);
        }
    }
}
