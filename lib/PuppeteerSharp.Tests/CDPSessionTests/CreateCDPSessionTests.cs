using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.CDPSessionTests
{
    public class CreateCDPSessionTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("CDPSession.spec", "Target.createCDPSession", "should work")]
        public async Task ShouldWork()
        {
            var client = await Page.CreateCDPSessionAsync();

            await Task.WhenAll(
              client.SendAsync("Runtime.enable"),
              client.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest { Expression = "window.foo = 'bar'" })
            );
            var foo = await Page.EvaluateExpressionAsync<string>("window.foo");
            Assert.That(foo, Is.EqualTo("bar"));
        }

        [Test, PuppeteerTest("CDPSession.spec", "Target.createCDPSession", "should send events")]
        public async Task ShouldSendEvents()
        {
            var client = await Page.CreateCDPSessionAsync();
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

        [Test, PuppeteerTest("CDPSession.spec", "Target.createCDPSession", "should enable and disable domains independently")]
        public async Task ShouldEnableAndDisableDomainsIndependently()
        {
            var client = await Page.CreateCDPSessionAsync();
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
            Assert.That(eventTask.Result.GetProperty("url")!.GetString(), Is.EqualTo("foo.js"));
        }

        [Test, PuppeteerTest("CDPSession.spec", "Target.createCDPSession", "should be able to detach session")]
        public async Task ShouldBeAbleToDetachSession()
        {
            var client = await Page.CreateCDPSessionAsync();
            await client.SendAsync("Runtime.enable");
            var evalResponse = await client.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest
            {
                Expression = "1 + 2",
                ReturnByValue = true
            });
            Assert.That(evalResponse!.Value.GetProperty("result")!.GetProperty("value")!.GetInt32(), Is.EqualTo(3));
            await client.DetachAsync();

            var exception = Assert.ThrowsAsync<TargetClosedException>(()
                => client.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest
                {
                    Expression = "3 + 1",
                    ReturnByValue = true
                }));
            Assert.That(exception!.Message, Does.Contain("Session closed."));
        }

        [Test, PuppeteerTest("CDPSession.spec", "Target.createCDPSession", "should not report created targets for custom CDP sessions")]
        public async Task ShouldNotReportCreatedTargetsForCustomCDPSessions()
        {
            var called = 0;
            async void EventHandler(object sender, TargetChangedArgs e)
            {
                called++;
                if (called > 1)
                {
                    throw new Exception("Too many targets created");
                }

                await e.Target.CreateCDPSessionAsync();
            }
            Page.BrowserContext.TargetCreated += EventHandler;
            await Browser.NewPageAsync();
            Page.BrowserContext.TargetCreated -= EventHandler;
        }

        [Test, PuppeteerTest("CDPSession.spec", "Target.createCDPSession", "should throw nice errors")]
        public async Task ShouldThrowNiceErrors()
        {
            var client = await Page.CreateCDPSessionAsync();
            async Task TheSourceOfTheProblems() => await client.SendAsync("ThisCommand.DoesNotExist");

            var exception = Assert.ThrowsAsync<MessageException>(async () =>
            {
                await TheSourceOfTheProblems();
            });
            Assert.That(exception!.StackTrace, Does.Contain("TheSourceOfTheProblems"));
            Assert.That(exception.Message, Does.Contain("ThisCommand.DoesNotExist"));
        }

        [Test, PuppeteerTest("CDPSession.spec", "Target.createCDPSession", "should respect custom timeout")]
        public async Task ShouldRespectCustomTimeout()
        {
            var client = await Page.CreateCDPSessionAsync();
            var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                await client.SendAsync(
                    "Runtime.evaluate",
                    new RuntimeEvaluateRequest()
                    {
                        Expression = "new Promise(x => {})",
                        AwaitPromise = true,
                    },
                    true,
                    new CommandOptions
                    {
                        Timeout = 50
                    });
            });
            Assert.That(exception!.Message, Does.Contain("Timeout of 50 ms exceeded"));
        }

        [Test, PuppeteerTest("CDPSession.spec", "Target.createCDPSession", "should expose the underlying connection")]
        public async Task ShouldExposeTheUnderlyingConnection()
            => Assert.That(await Page.CreateCDPSessionAsync(), Is.Not.Null);
    }
}
