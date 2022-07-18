using System;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.WaitTaskTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FrameWaitForFunctionTests : DevToolsContextBaseTest
    {
        public FrameWaitForFunctionTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should work when resolved right before execution context disposal")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenResolvedRightBeforeExecutionContextDisposal()
        {
            await DevToolsContext.EvaluateFunctionOnNewDocumentAsync("() => window.__RELOADED = true");
            await DevToolsContext.WaitForFunctionAsync(@"() =>
            {
                if (!window.__RELOADED)
                    window.location.reload();
                return true;
            }");
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should poll on interval")]
        [PuppeteerFact]
        public async Task ShouldPollOnInterval()
        {
            var success = false;
            var startTime = DateTime.Now;
            var polling = 100;
            var watchdog = DevToolsContext.WaitForFunctionAsync("() => window.__FOO === 'hit'", new WaitForFunctionOptions { PollingInterval = polling })
                .ContinueWith(_ => success = true);
            await DevToolsContext.EvaluateExpressionAsync("window.__FOO = 'hit'");
            Assert.False(success);
            await DevToolsContext.EvaluateExpressionAsync("document.body.appendChild(document.createElement('div'))");
            await watchdog;
            Assert.True((DateTime.Now - startTime).TotalMilliseconds > polling / 2);
        }
        
        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should poll on interval async")]
        [PuppeteerFact]
        public async Task ShouldPollOnIntervalAsync()
        {
            var success = false;
            var startTime = DateTime.Now;
            var polling = 100;
            var watchdog = DevToolsContext.WaitForFunctionAsync("async () => window.__FOO === 'hit'", new WaitForFunctionOptions { PollingInterval = polling })
                .ContinueWith(_ => success = true);
            await DevToolsContext.EvaluateFunctionAsync("async () => window.__FOO = 'hit'");
            Assert.False(success);
            await DevToolsContext.EvaluateExpressionAsync("document.body.appendChild(document.createElement('div'))");
            await watchdog;
            Assert.True((DateTime.Now - startTime).TotalMilliseconds > polling / 2);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should poll on mutation")]
        [PuppeteerFact]
        public async Task ShouldPollOnMutation()
        {
            var success = false;
            var watchdog = DevToolsContext.WaitForFunctionAsync("() => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Mutation })
                .ContinueWith(_ => success = true);
            await DevToolsContext.EvaluateExpressionAsync("window.__FOO = 'hit'");
            Assert.False(success);
            await DevToolsContext.EvaluateExpressionAsync("document.body.appendChild(document.createElement('div'))");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should poll on mutation async")]
        [PuppeteerFact]
        public async Task ShouldPollOnMutationAsync()
        {
            var success = false;
            var watchdog = DevToolsContext.WaitForFunctionAsync("async () => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Mutation })
                .ContinueWith(_ => success = true);
            await DevToolsContext.EvaluateFunctionAsync("async () => window.__FOO = 'hit'");
            Assert.False(success);
            await DevToolsContext.EvaluateExpressionAsync("document.body.appendChild(document.createElement('div'))");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should poll on raf")]
        [PuppeteerFact]
        public async Task ShouldPollOnRaf()
        {
            var watchdog = DevToolsContext.WaitForFunctionAsync("() => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Raf });
            await DevToolsContext.EvaluateExpressionAsync("window.__FOO = 'hit'");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should poll on raf async")]
        [PuppeteerFact]
        public async Task ShouldPollOnRafAsync()
        {
            var watchdog = DevToolsContext.WaitForFunctionAsync("async () => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Raf });
            await DevToolsContext.EvaluateFunctionAsync("async () => (globalThis.__FOO = 'hit')");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should work with strict CSP policy")]
        [PuppeteerFact]
        public async Task ShouldWorkWithStrictCSPPolicy()
        {
            Server.SetCSP("/empty.html", "script-src " + TestConstants.ServerUrl);
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await Task.WhenAll(
                DevToolsContext.WaitForFunctionAsync("() => window.__FOO === 'hit'", new WaitForFunctionOptions
                {
                    Polling = WaitForFunctionPollingOption.Raf
                }),
                DevToolsContext.EvaluateExpressionAsync("window.__FOO = 'hit'"));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should throw negative polling interval")]
        [PuppeteerFact]
        public async Task ShouldThrowNegativePollingInterval()
        {
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(()
                => DevToolsContext.WaitForFunctionAsync("() => !!document.body", new WaitForFunctionOptions { PollingInterval = -10 }));

            Assert.Contains("Cannot poll with non-positive interval", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should return the success value as a JSHandle")]
        [PuppeteerFact]
        public async Task ShouldReturnTheSuccessValueAsAJSHandle()
            => Assert.Equal(5, await (await DevToolsContext.WaitForFunctionAsync("() => 5")).JsonValueAsync<int>());

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should return the window as a success value")]
        [PuppeteerFact]
        public async Task ShouldReturnTheWindowAsASuccessValue()
            => Assert.NotNull(await DevToolsContext.WaitForFunctionAsync("() => window"));

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should accept ElementHandle arguments")]
        [PuppeteerFact]
        public async Task ShouldAcceptElementHandleArguments()
        {
            await DevToolsContext.SetContentAsync("<div></div>");
            var div = await DevToolsContext.QuerySelectorAsync("div");
            var resolved = false;
            var waitForFunction = DevToolsContext.WaitForFunctionAsync("element => !element.parentElement", div)
                .ContinueWith(_ => resolved = true);
            Assert.False(resolved);
            await DevToolsContext.EvaluateFunctionAsync("element => element.remove()", div);
            await waitForFunction;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should respect timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectTimeout()
        {
            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(()
                => DevToolsContext.WaitForExpressionAsync("false", new WaitForFunctionOptions { Timeout = 10 }));

            Assert.Contains("waiting for function failed: timeout", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should respect default timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectDefaultTimeout()
        {
            DevToolsContext.DefaultTimeout = 1;
            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(()
                => DevToolsContext.WaitForExpressionAsync("false"));

            Assert.Contains("waiting for function failed: timeout", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should disable timeout when its set to 0")]
        [PuppeteerFact]
        public async Task ShouldDisableTimeoutWhenItsSetTo0()
        {
            var watchdog = DevToolsContext.WaitForFunctionAsync(@"() => {
                window.__counter = (window.__counter || 0) + 1;
                return window.__injected;
            }", new WaitForFunctionOptions { Timeout = 0, PollingInterval = 10 });
            await DevToolsContext.WaitForFunctionAsync("() => window.__counter > 10");
            await DevToolsContext.EvaluateExpressionAsync("window.__injected = true");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should survive cross-process navigation")]
        [PuppeteerFact]
        public async Task ShouldSurviveCrossProcessNavigation()
        {
            var fooFound = false;
            var waitForFunction = DevToolsContext.WaitForExpressionAsync("window.__FOO === 1")
                .ContinueWith(_ => fooFound = true);
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.False(fooFound);
            await DevToolsContext.ReloadAsync();
            Assert.False(fooFound);
            await DevToolsContext.GoToAsync(TestConstants.CrossProcessUrl + "/grid.html");
            Assert.False(fooFound);
            await DevToolsContext.EvaluateExpressionAsync("window.__FOO = 1");
            await waitForFunction;
            Assert.True(fooFound);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should survive navigations")]
        [PuppeteerFact]
        public async Task ShouldSurviveNavigations()
        {
            var watchdog = DevToolsContext.WaitForFunctionAsync("() => window.__done");
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/consolelog.html");
            await DevToolsContext.EvaluateFunctionAsync("() => window.__done = true");
            await watchdog;
        }
    }
}
