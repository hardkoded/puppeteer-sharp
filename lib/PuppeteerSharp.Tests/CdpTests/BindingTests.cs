using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.CdpTests
{
    public class BindingTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("binding.spec", "CDP binding handle disposal", "should release temporary handles after queryOne")]
        public async Task ShouldReleaseTemporaryHandlesAfterQueryOne()
        {
            var client = await Page.CreateCDPSessionAsync();
            await Page.SetContentAsync("<div><button>Submit</button><button>Submit</button></div>");

            var button = await Page.WaitForSelectorAsync("aria/Submit");
            Assert.That(button, Is.Not.Null);
            Assert.That(await button.EvaluateFunctionAsync<string>("button => button.textContent"), Is.EqualTo("Submit"));
            await button.DisposeAsync();

            await AssertNoButtonsRetainedAsync(client);
        }

        [Test, PuppeteerTest("binding.spec", "CDP binding handle disposal", "should release temporary handles after queryAll")]
        public async Task ShouldReleaseTemporaryHandlesAfterQueryAll()
        {
            var client = await Page.CreateCDPSessionAsync();
            await Page.SetContentAsync("<div><button>Submit</button><button>Submit</button></div>");

            var buttons = await Page.QuerySelectorAllAsync("div ::-p-aria(Submit)");
            Assert.That(buttons, Has.Length.EqualTo(2));
            foreach (var button in buttons)
            {
                Assert.That(await button.EvaluateFunctionAsync<string>("button => button.textContent"), Is.EqualTo("Submit"));
                await button.DisposeAsync();
            }

            await AssertNoButtonsRetainedAsync(client);
        }

        [TestCase("resolve")]
        [TestCase("error")]
        [PuppeteerTest("binding.spec", "CDP binding handle disposal", "should preserve node arguments when exposed functions resolve")]
        public async Task ShouldPreserveNodeArgumentsWhenExposedFunctionsFinish(string outcome)
        {
            IElementHandle received = null;
            await Page.ExposeFunctionAsync("inspectNode", async (object nodeArg) =>
            {
                // The binding deserializes arguments by declared type, so node arguments are received as object.
                var node = (IElementHandle)nodeArg;
                received = node;
                Assert.That(await node.EvaluateFunctionAsync<string>("node => node.nodeName"), Is.EqualTo("BODY"));
                if (outcome == "error")
                {
                    throw new System.Exception("failed");
                }

                return "success";
            });

            var result = await Page.EvaluateFunctionAsync<string>(@"async () => {
                try {
                    return await globalThis.inspectNode(document.body);
                } catch (error) {
                    return error.message;
                }
            }");
            Assert.That(result, Is.EqualTo(outcome == "resolve" ? "success" : "failed"));

            // Wait for the binding to finish its response round trip.
            await Page.EvaluateFunctionAsync("() => {}");
            Assert.That(received, Is.Not.Null);
            await using var node = received;
            Assert.That(node.Disposed, Is.False);
            Assert.That(await node.EvaluateFunctionAsync<string>("node => node.nodeName"), Is.EqualTo("BODY"));
        }

        [Test, PuppeteerTest("binding.spec", "CDP binding handle disposal", "should preserve node arguments returned by exposed functions")]
        public async Task ShouldPreserveNodeArgumentsReturnedByExposedFunctions()
        {
            IElementHandle received = null;
            await Page.ExposeFunctionAsync("echoNode", async (object nodeArg) =>
            {
                // The binding deserializes arguments by declared type, so node arguments are received as object.
                var node = (IElementHandle)nodeArg;
                received = node;
                await node.EvaluateFunctionAsync<string>("node => node.nodeName");
                return nodeArg;
            });

            Assert.That(
                await Page.EvaluateFunctionAsync<bool>("async () => (await globalThis.echoNode(document.body)) === document.body"),
                Is.True);
            await Page.EvaluateFunctionAsync("() => {}");
            Assert.That(received, Is.Not.Null);
            await using var node = received;
            Assert.That(node.Disposed, Is.False);
            Assert.That(await node.EvaluateFunctionAsync<string>("node => node.nodeName"), Is.EqualTo("BODY"));
        }

        [Test, PuppeteerTest("binding.spec", "CDP binding handle disposal", "should preserve handles returned by exposed functions")]
        public async Task ShouldPreserveHandlesReturnedByExposedFunctions()
        {
            await using var handle = await Page.EvaluateFunctionHandleAsync("() => ({ value: 42 })");
            await Page.ExposeFunctionAsync("getValue", () => handle);

            for (var i = 0; i < 2; i++)
            {
                Assert.That(await Page.EvaluateFunctionAsync<int>("async () => (await globalThis.getValue()).value"), Is.EqualTo(42));
                Assert.That(handle.Disposed, Is.False);
            }
        }

        private async Task AssertNoButtonsRetainedAsync(ICDPSession client)
        {
            await using var prototype = await Page.EvaluateFunctionHandleAsync("() => HTMLButtonElement.prototype");
            await Page.EvaluateFunctionAsync("() => document.body.replaceChildren()");

            // Replace the last evaluated result in the utility world before collecting.
            await ((Frame)Page.MainFrame).IsolatedRealm.EvaluateFunctionAsync("() => 0");

            var count = await CountButtonsAsync();
            for (var i = 0; count != 0 && i < 4; i++)
            {
                count = await CountButtonsAsync();
            }

            Assert.That(count, Is.Zero);

            async Task<int> CountButtonsAsync()
            {
                await client.SendAsync("HeapProfiler.collectGarbage");
                await using var objects = await Page.QueryObjectsAsync(prototype);
                return await objects.EvaluateFunctionAsync<int>("objects => objects.length");
            }
        }
    }
}
