using System;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Transport;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.WaitTaskTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public sealed class FrameWaitForFunctionTests : PuppeteerPageBaseTest, IDisposable
    {
        private PollerInterceptor _pollerInterceptor;

        public FrameWaitForFunctionTests(): base()
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();

            // Set up a custom TransportFactory to intercept sent messages
            // Some of the tests require making assertions after a WaitForFunction has
            // started, but before it has resolved. We detect that reliably by
            // listening to the message that is sent to start polling.
            // This might not be an issue in upstream puppeteer.js, or may be highly unlikely,
            // due to differences between node.js's task scheduler and .net's.
            DefaultOptions.TransportFactory = async (url, options, cancellationToken) =>
            {
                _pollerInterceptor = new PollerInterceptor(await WebSocketTransport.DefaultTransportFactory(url, options, cancellationToken));
                return _pollerInterceptor;
            };
        }

        public void Dispose()
        {
            _pollerInterceptor.Dispose();
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should work when resolved right before execution context disposal")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenResolvedRightBeforeExecutionContextDisposal()
        {
            await Page.EvaluateFunctionOnNewDocumentAsync("() => window.__RELOADED = true");
            await Page.WaitForFunctionAsync(@"() =>
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
            var startTime = DateTime.UtcNow;
            var polling = 100;
            var startedPolling =  _pollerInterceptor.WaitForStartPollingAsync();
            var watchdog = Page.WaitForFunctionAsync("() => window.__FOO === 'hit'", new WaitForFunctionOptions { PollingInterval = polling });
            await startedPolling;
            await Page.EvaluateFunctionAsync("() => setTimeout(window.__FOO = 'hit', 50)");
            await watchdog;

            Assert.True((DateTime.UtcNow - startTime).TotalMilliseconds > polling / 2);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should poll on interval async")]
        [PuppeteerFact]
        public async Task ShouldPollOnIntervalAsync()
        {
            var startTime = DateTime.UtcNow;
            var polling = 1000;
            var startedPolling = _pollerInterceptor.WaitForStartPollingAsync();
            var watchdog = Page.WaitForFunctionAsync("async () => window.__FOO === 'hit'", new WaitForFunctionOptions { PollingInterval = polling });
            await startedPolling;
            await Page.EvaluateFunctionAsync("async () => setTimeout(window.__FOO = 'hit', 50)");
            await watchdog;
            Assert.True((DateTime.UtcNow - startTime).TotalMilliseconds > polling / 2);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should poll on mutation")]
        [PuppeteerFact]
        public async Task ShouldPollOnMutation()
        {
            var success = false;
            var startedPolling = _pollerInterceptor.WaitForStartPollingAsync();
            var watchdog = Page.WaitForFunctionAsync("() => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Mutation })
                .ContinueWith(_ => success = true);
            await startedPolling;
            await Page.EvaluateExpressionAsync("window.__FOO = 'hit'");
            Assert.False(success);
            await Page.EvaluateExpressionAsync("document.body.appendChild(document.createElement('div'))");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should poll on mutation async")]
        [PuppeteerFact]
        public async Task ShouldPollOnMutationAsync()
        {
            var success = false;
            var startedPolling = _pollerInterceptor.WaitForStartPollingAsync();
            var watchdog = Page.WaitForFunctionAsync("async () => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Mutation })
                .ContinueWith(_ => success = true);
            await startedPolling;
            await Page.EvaluateFunctionAsync("async () => window.__FOO = 'hit'");
            Assert.False(success);
            await Page.EvaluateExpressionAsync("document.body.appendChild(document.createElement('div'))");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should poll on raf")]
        [PuppeteerFact]
        public async Task ShouldPollOnRaf()
        {
            var watchdog = Page.WaitForFunctionAsync("() => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Raf });
            await Page.EvaluateExpressionAsync("window.__FOO = 'hit'");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should poll on raf async")]
        [PuppeteerFact]
        public async Task ShouldPollOnRafAsync()
        {
            var watchdog = Page.WaitForFunctionAsync("async () => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Raf });
            await Page.EvaluateFunctionAsync("async () => (globalThis.__FOO = 'hit')");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should work with strict CSP policy")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithStrictCSPPolicy()
        {
            Server.SetCSP("/empty.html", "script-src " + TestConstants.ServerUrl);
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Task.WhenAll(
                Page.WaitForFunctionAsync("() => window.__FOO === 'hit'", new WaitForFunctionOptions
                {
                    Polling = WaitForFunctionPollingOption.Raf
                }),
                Page.EvaluateExpressionAsync("window.__FOO = 'hit'"));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should throw negative polling interval")]
        [PuppeteerFact]
        public async Task ShouldThrowNegativePollingInterval()
        {
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(()
                => Page.WaitForFunctionAsync("() => !!document.body", new WaitForFunctionOptions { PollingInterval = -10 }));

            Assert.Contains("Cannot poll with non-positive interval", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should return the success value as a JSHandle")]
        [PuppeteerFact]
        public async Task ShouldReturnTheSuccessValueAsAJSHandle()
            => Assert.Equal(5, await (await Page.WaitForFunctionAsync("() => 5")).JsonValueAsync<int>());

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should return the window as a success value")]
        [PuppeteerFact]
        public async Task ShouldReturnTheWindowAsASuccessValue()
            => Assert.NotNull(await Page.WaitForFunctionAsync("() => window"));

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should accept ElementHandle arguments")]
        [PuppeteerFact]
        public async Task ShouldAcceptElementHandleArguments()
        {
            await Page.SetContentAsync("<div></div>");
            var div = await Page.QuerySelectorAsync("div");
            var resolved = false;
            var waitForFunction = Page.WaitForFunctionAsync("element => !element.parentElement", div)
                .ContinueWith(_ => resolved = true);
            Assert.False(resolved);
            await Page.EvaluateFunctionAsync("element => element.remove()", div);
            await waitForFunction;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should respect timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectTimeout()
        {
            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(()
                => Page.WaitForExpressionAsync("false", new WaitForFunctionOptions { Timeout = 10 }));

            Assert.Contains("Waiting failed: 10ms exceeded", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should respect default timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectDefaultTimeout()
        {
            Page.DefaultTimeout = 1;
            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(()
                => Page.WaitForExpressionAsync("false"));

            Assert.Contains("Waiting failed: 1ms exceeded", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should disable timeout when its set to 0")]
        [PuppeteerFact]
        public async Task ShouldDisableTimeoutWhenItsSetTo0()
        {
            var watchdog = Page.WaitForFunctionAsync(@"() => {
                window.__counter = (window.__counter || 0) + 1;
                return window.__injected;
            }", new WaitForFunctionOptions { Timeout = 0, PollingInterval = 10 });
            await Page.WaitForFunctionAsync("() => window.__counter > 10");
            await Page.EvaluateExpressionAsync("window.__injected = true");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should survive cross-process navigation")]
        [PuppeteerFact]
        public async Task ShouldSurviveCrossProcessNavigation()
        {
            var fooFound = false;
            var waitForFunction = Page.WaitForExpressionAsync("window.__FOO === 1")
                .ContinueWith(_ => fooFound = true);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.False(fooFound);
            await Page.ReloadAsync();
            Assert.False(fooFound);
            await Page.GoToAsync(TestConstants.CrossProcessUrl + "/grid.html");
            Assert.False(fooFound);
            await Page.EvaluateExpressionAsync("window.__FOO = 1");
            await waitForFunction;
            Assert.True(fooFound);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should survive navigations")]
        [PuppeteerFact]
        public async Task ShouldSurviveNavigations()
        {
            var watchdog = Page.WaitForFunctionAsync("() => window.__done");
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.GoToAsync(TestConstants.ServerUrl + "/consolelog.html");
            await Page.EvaluateFunctionAsync("() => window.__done = true");
            await watchdog;
        }
    }
}
