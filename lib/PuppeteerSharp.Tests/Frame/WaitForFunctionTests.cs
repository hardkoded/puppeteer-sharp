using System;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Frame
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class WaitForFunctionTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldPollOnInterval()
        {
            var success = false;
            var startTime = DateTime.Now;
            var polling = 100;
            var watchdog = Page.WaitForFunctionAsync("() => window.__FOO === 'hit'", new WaitForFunctionOptions { PollingInterval = polling })
                .ContinueWith(_ => success = true);
            await Page.EvaluateExpressionHandleAsync("window.__FOO = 'hit'");
            Assert.False(success);
            await Page.EvaluateExpressionHandleAsync("document.body.appendChild(document.createElement('div'))");
            await watchdog;
            Assert.True((DateTime.Now - startTime).TotalMilliseconds > polling / 2);
        }

        [Fact]
        public async Task ShouldPollOnMutation()
        {
            var success = false;
            var watchdog = Page.WaitForFunctionAsync("() => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Mutation })
                .ContinueWith(_ => success = true);
            await Page.EvaluateExpressionHandleAsync("window.__FOO = 'hit'");
            Assert.False(success);
            await Page.EvaluateExpressionHandleAsync("document.body.appendChild(document.createElement('div'))");
            await watchdog;
        }

        [Fact]
        public async Task ShouldPollOnRaf()
        {
            var watchdog = Page.WaitForFunctionAsync("() => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Raf });
            await Page.EvaluateExpressionHandleAsync("window.__FOO = 'hit'");
            await watchdog;
        }

        [Fact]
        public async Task ShouldThrowNegativePollingInterval()
        {
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(()
                => Page.WaitForFunctionAsync("() => !!document.body", new WaitForFunctionOptions { PollingInterval = -10 }));

            Assert.Contains("Cannot poll with non-positive interval", exception.Message);
        }

        [Fact]
        public async Task ShouldReturnTheSuccessValueAsAJSHandle()
        {
            Assert.Equal(5, await (await Page.WaitForFunctionAsync("() => 5")).JsonValue<int>());
        }

        [Fact]
        public async Task ShouldReturnTheWindowAsASuccessValue()
        {
            Assert.NotNull(await Page.WaitForFunctionAsync("() => window"));
        }

        [Fact]
        public async Task ShouldAcceptElementHandleArguments()
        {
            await Page.SetContentAsync("<div></div>");
            var div = await Page.GetElementAsync("div");
            var resolved = false;
            var waitForFunction = Page.WaitForFunctionAsync("element => !element.parentElement", div)
                .ContinueWith(_ => resolved = true);
            Assert.False(resolved);
            await Page.EvaluateFunctionHandleAsync("element => element.remove()", div);
            await waitForFunction;
        }
    }
}
