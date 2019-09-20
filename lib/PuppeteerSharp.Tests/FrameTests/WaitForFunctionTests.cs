using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class WaitForFunctionTests : PuppeteerPageBaseTest
    {
        public WaitForFunctionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
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

        [Fact]
        public async Task ShouldPollOnInterval()
        {
            var success = false;
            var startTime = DateTime.Now;
            var polling = 100;
            var watchdog = Page.WaitForFunctionAsync("() => window.__FOO === 'hit'", new WaitForFunctionOptions { PollingInterval = polling })
                .ContinueWith(_ => success = true);
            await Page.EvaluateExpressionAsync("window.__FOO = 'hit'");
            Assert.False(success);
            await Page.EvaluateExpressionAsync("document.body.appendChild(document.createElement('div'))");
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
            await Page.EvaluateExpressionAsync("window.__FOO = 'hit'");
            Assert.False(success);
            await Page.EvaluateExpressionAsync("document.body.appendChild(document.createElement('div'))");
            await watchdog;
        }

        [Fact]
        public async Task ShouldPollOnRaf()
        {
            var watchdog = Page.WaitForFunctionAsync("() => window.__FOO === 'hit'",
                new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Raf });
            await Page.EvaluateExpressionAsync("window.__FOO = 'hit'");
            await watchdog;
        }

        [Fact]
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

        [Fact]
        public async Task ShouldThrowNegativePollingInterval()
        {
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(()
                => Page.WaitForFunctionAsync("() => !!document.body", new WaitForFunctionOptions { PollingInterval = -10 }));

            Assert.Contains("Cannot poll with non-positive interval", exception.Message);
        }

        [Fact]
        public async Task ShouldReturnTheSuccessValueAsAJSHandle()
            => Assert.Equal(5, await (await Page.WaitForFunctionAsync("() => 5")).JsonValueAsync<int>());

        [Fact]
        public async Task ShouldReturnTheWindowAsASuccessValue()
            => Assert.NotNull(await Page.WaitForFunctionAsync("() => window"));

        [Fact]
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

        [Fact]
        public async Task ShouldRespectTimeout()
        {
            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(()
                => Page.WaitForExpressionAsync("false", new WaitForFunctionOptions { Timeout = 10 }));

            Assert.Contains("waiting for function failed: timeout", exception.Message);
        }

        [Fact]
        public async Task ShouldRespectDefaultTimeout()
        {
            Page.DefaultTimeout = 1;
            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(()
                => Page.WaitForExpressionAsync("false"));

            Assert.Contains("waiting for function failed: timeout", exception.Message);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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