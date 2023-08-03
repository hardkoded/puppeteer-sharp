using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.CDPSessionTests
{
    public class CreateCDPSessionTests : PuppeteerPageBaseTest
    {
        public CreateCDPSessionTests(): base()
        {
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            var client = await Page.Target.CreateCDPSessionAsync();

            await Task.WhenAll(
              client.SendAsync("Runtime.enable"),
              client.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest { Expression = "window.foo = 'bar'" })
            );
            var foo = await Page.EvaluateExpressionAsync<string>("window.foo");
            Assert.AreEqual("bar", foo);
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should send events")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Assert.That(events, Has.Exactly(1).Items);
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should enable and disable domains independently")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Assert.AreEqual("foo.js", eventTask.Result["url"].Value<string>());
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should be able to detach session")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldBeAbleToDetachSession()
        {
            var client = await Page.Target.CreateCDPSessionAsync();
            await client.SendAsync("Runtime.enable");
            var evalResponse = await client.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest
            {
                Expression = "1 + 2",
                ReturnByValue = true
            });
            Assert.AreEqual(3, evalResponse["result"]["value"].ToObject<int>());
            await client.DetachAsync();

            var exception = Assert.ThrowsAsync<Exception>(()
                => client.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest
                {
                    Expression = "3 + 1",
                    ReturnByValue = true
                }));
            StringAssert.Contains("Session closed.", exception.Message);
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should throw nice errors")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldThrowNiceErrors()
        {
            var client = await Page.Target.CreateCDPSessionAsync();
            async Task TheSourceOfTheProblems() => await client.SendAsync("ThisCommand.DoesNotExist");

            var exception = Assert.ThrowsAsync<MessageException>(async () =>
            {
                await TheSourceOfTheProblems();
            });
            StringAssert.Contains("TheSourceOfTheProblems", exception.StackTrace);
            StringAssert.Contains("ThisCommand.DoesNotExist", exception.Message);
        }

        [PuppeteerTest("CDPSession.spec.ts", "Target.createCDPSession", "should expose the underlying connection")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldExposeTheUnderlyingConnection()
            => Assert.NotNull(await Page.Target.CreateCDPSessionAsync());
    }
}
