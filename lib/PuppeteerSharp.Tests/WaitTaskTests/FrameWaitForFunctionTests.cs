using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests.WaitTaskTests
{
    public sealed class FrameWaitForFunctionTests : PuppeteerPageBaseTest
    {
        private PollerInterceptor _pollerInterceptor;

        public FrameWaitForFunctionTests() : base()
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _pollerInterceptor.Dispose();
        }

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should work when resolved right before execution context disposal")]
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

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should poll on interval")]
        public async Task ShouldPollOnInterval()
        {
            var startTime = DateTime.UtcNow;
            var polling = 100;
            var startedPolling = _pollerInterceptor.WaitForStartPollingAsync();
            var watchdog = Page.WaitForFunctionAsync("() => window.__FOO === 'hit'", new WaitForFunctionOptions { PollingInterval = polling });
            await startedPolling;
            await Page.EvaluateFunctionAsync("() => setTimeout(window.__FOO = 'hit', 50)");
            await watchdog;

            Assert.True((DateTime.UtcNow - startTime).TotalMilliseconds > polling / 2);
        }

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should poll on interval async")]
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

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should poll on mutation")]
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

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should poll on mutation async")]
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

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should poll on raf")]
        public async Task ShouldPollOnRaf()
        {
            var watchdog = Page.WaitForFunctionAsync("() => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Raf });
            await Page.EvaluateExpressionAsync("window.__FOO = 'hit'");
            await watchdog;
        }

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should poll on raf async")]
        public async Task ShouldPollOnRafAsync()
        {
            var watchdog = Page.WaitForFunctionAsync("async () => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Raf });
            await Page.EvaluateFunctionAsync("async () => (globalThis.__FOO = 'hit')");
            await watchdog;
        }

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should work with strict CSP policy")]
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

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should throw negative polling interval")]
        public void ShouldThrowNegativePollingInterval()
        {
            var exception = Assert.ThrowsAsync<ArgumentOutOfRangeException>(()
                => Page.WaitForFunctionAsync("() => !!document.body", new WaitForFunctionOptions { PollingInterval = -10 }));

            StringAssert.Contains("Cannot poll with non-positive interval", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should return the success value as a JSHandle")]
        public async Task ShouldReturnTheSuccessValueAsAJSHandle()
            => Assert.AreEqual(5, await (await Page.WaitForFunctionAsync("() => 5")).JsonValueAsync<int>());

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should return the window as a success value")]
        public async Task ShouldReturnTheWindowAsASuccessValue()
            => Assert.NotNull(await Page.WaitForFunctionAsync("() => window"));

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should accept ElementHandle arguments")]
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

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should respect timeout")]
        public void ShouldRespectTimeout()
        {
            var exception = Assert.ThrowsAsync<WaitTaskTimeoutException>(()
                => Page.WaitForExpressionAsync("false", new WaitForFunctionOptions { Timeout = 10 }));

            StringAssert.Contains("Waiting failed: 10ms exceeded", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should respect default timeout")]
        public void ShouldRespectDefaultTimeout()
        {
            Page.DefaultTimeout = 1;
            var exception = Assert.ThrowsAsync<WaitTaskTimeoutException>(()
                => Page.WaitForExpressionAsync("false"));

            StringAssert.Contains("Waiting failed: 1ms exceeded", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should disable timeout when its set to 0")]
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

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should survive cross-process navigation")]
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

        [Test, Retry(2), PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should survive navigations")]
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
