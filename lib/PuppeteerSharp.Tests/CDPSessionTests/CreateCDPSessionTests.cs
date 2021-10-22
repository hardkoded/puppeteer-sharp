using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.CDPSessionTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CreateCDPSessionTests : PuppeteerPageBaseTest
    {
        public CreateCDPSessionTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            var client = await Page.Target.CreateCDPSessionAsync();

            await Task.WhenAll(
              client.SendAsync("Runtime.enable"),
              client.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest { Expression = "window.foo = 'bar'" })
            );
            var foo = await Page.EvaluateExpressionAsync<string>("window.foo");
            Assert.Equal("bar", foo);
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should send events")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSendEvents()
        {
            var client = await Page.Target.CreateCDPSessionAsync();
            await client.SendAsync("Network.enable");
            var events = new List<object>();

            client.MessageReceived += (_, e) =>
            {
                if (e.MessageID == "Network.requestWillBeSent")
                {
                    events.Add(e.MessageData);
                }
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Single(events);
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should enable and disable domains independently")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldEnableAndDisableDomainsIndependently()
        {
            var client = await Page.Target.CreateCDPSessionAsync();
            await client.SendAsync("Runtime.enable");
            await client.SendAsync("Debugger.enable");
            // JS coverage enables and then disables Debugger domain.
            await Page.Coverage.StartJSCoverageAsync();
            await Page.Coverage.StopJSCoverageAsync();
            // generate a script in page and wait for the event.
            var eventTask = WaitEvent(client, "Debugger.scriptParsed");
            await Task.WhenAll(
                eventTask,
                Page.EvaluateExpressionAsync("//# sourceURL=foo.js")
            );
            // expect events to be dispatched.
            Assert.Equal("foo.js", eventTask.Result["url"].Value<string>());
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should be able to detach session")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldBeAbleToDetachSession()
        {
            var client = await Page.Target.CreateCDPSessionAsync();
            await client.SendAsync("Runtime.enable");
            var evalResponse = await client.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest
            {
                Expression = "1 + 2",
                ReturnByValue = true
            });
            Assert.Equal(3, evalResponse["result"]["value"].ToObject<int>());
            await client.DetachAsync();

            var exception = await Assert.ThrowsAnyAsync<Exception>(()
                => client.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest
                {
                    Expression = "3 + 1",
                    ReturnByValue = true
                }));
            Assert.Contains("Session closed.", exception.Message);
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should throw nice errors")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldThrowNiceErrors()
        {
            var client = await Page.Target.CreateCDPSessionAsync();
            async Task TheSourceOfTheProblems() => await client.SendAsync("ThisCommand.DoesNotExist");

            var exception = await Assert.ThrowsAsync<MessageException>(async () =>
            {
                await TheSourceOfTheProblems();
            });
            Assert.Contains("TheSourceOfTheProblems", exception.StackTrace);
            Assert.Contains("ThisCommand.DoesNotExist", exception.Message);
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should expose the underlying connection")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldExposeTheUnderlyingConnection()
            => Assert.NotNull(await Page.Target.CreateCDPSessionAsync());
    }
}
