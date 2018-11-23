using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.TargetTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class CreateCDPSessionTests : PuppeteerPageBaseTest
    {
        public CreateCDPSessionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            var client = await Page.Target.CreateCDPSessionAsync();

            await Task.WhenAll(
              client.SendAsync("Runtime.enable"),
              client.SendAsync("Runtime.evaluate", new { expression = "window.foo = 'bar'" })
            );
            var foo = await Page.EvaluateExpressionAsync<string>("window.foo");
            Assert.Equal("bar", foo);
        }

        [Fact]
        public async Task ShouldSendEvents()
        {
            var client = await Page.Target.CreateCDPSessionAsync();
            await client.SendAsync("Network.enable");
            var events = new List<object>();

            client.MessageReceived += (sender, e) =>
            {
                if (e.MessageID == "Network.requestWillBeSent")
                {
                    events.Add(e.MessageData);
                }
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Single(events);
        }

        [Fact]
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

        [Fact]
        public async Task ShouldBeAbleToDetachSession()
        {
            var client = await Page.Target.CreateCDPSessionAsync();
            await client.SendAsync("Runtime.enable");
            var evalResponse = await client.SendAsync("Runtime.evaluate", new { expression = "1 + 2", returnByValue = true });
            Assert.Equal(3, evalResponse["result"]["value"].ToObject<int>());
            await client.DetachAsync();

            var exception = await Assert.ThrowsAnyAsync<Exception>(()
                => client.SendAsync("Runtime.evaluate", new { expression = "3 + 1", returnByValue = true }));
            Assert.Contains("Session closed.", exception.Message);
        }
    }
}
