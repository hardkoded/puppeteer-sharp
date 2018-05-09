﻿using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.PageTests.Events
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ConsoleTests : PuppeteerPageBaseTest
    {
        [Fact]
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
            
            var obj = new Dictionary<string, object> {{"foo", "bar"}};

            Assert.Equal("hello 5 JSHandle@object", message.Text);
            Assert.Equal(ConsoleType.Log, message.Type);

            Assert.Equal("hello", await message.Args[0].JsonValueAsync());
            Assert.Equal(5, await message.Args[1].JsonValueAsync<float>());
            Assert.Equal(obj, await message.Args[2].JsonValueAsync<Dictionary<string, object>>());
            Assert.Equal("bar", (await message.Args[2].JsonValueAsync<dynamic>()).foo.ToString());
        }

        [Fact]
        public async Task ShouldWorkForDifferentConsoleApiCalls()
        {
            var messages = new List<ConsoleMessage>();

            Page.Console += (sender, e) => messages.Add(e.Message);

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

        [Fact]
        public async Task ShouldNotFailForWindowObject()
        {
            ConsoleMessage message = null;

            void EventHandler(object sender, ConsoleEventArgs e)
            {
                message = e.Message;
                Page.Console -= EventHandler;
            }

            Page.Console += EventHandler;

            await Page.EvaluateExpressionAsync("console.error(window)");

            Assert.Equal("JSHandle@object", message.Text);
        }
    }
}