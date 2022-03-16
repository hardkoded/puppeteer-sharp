using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class WaitForNetworkIdleTests : PuppeteerPageBaseTest
    {
        public WaitForNetworkIdleTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForNetworkIdle", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var t1 = DateTime.Now;
            var t2 = DateTime.Now;

            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var task = DevToolsContext.WaitForNetworkIdleAsync().ContinueWith(x => t1 = DateTime.Now);

            await Task.WhenAll(
                task,
                DevToolsContext.EvaluateFunctionAsync(@"
                    (async () => {
                        await Promise.all([
                                fetch('/digits/1.png'),
                                fetch('/digits/2.png'),
                            ]);
                        await new Promise((resolve) => setTimeout(resolve, 200));
                        await fetch('/digits/3.png');
                        await new Promise((resolve) => setTimeout(resolve, 200));
                        await fetch('/digits/4.png');
                    })();").ContinueWith(x => t2 = DateTime.Now)
            );

            Assert.True(t1 > t2);
            Assert.True((t1 - t2).TotalMilliseconds >= 400);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForNetworkIdle", "should respect timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectTimeout()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await DevToolsContext.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 1 }));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForNetworkIdle", "should respect idleTime")]
        [PuppeteerFact]
        public async Task ShouldRespectIdleTimeout()
        {
            var t1 = DateTime.Now;
            var t2 = DateTime.Now;

            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var task = DevToolsContext.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { IdleTime = 10 }).ContinueWith(x => t1 = DateTime.Now);

            await Task.WhenAll(
                task,
                DevToolsContext.EvaluateFunctionAsync(@"
                    (async () => {
                        await Promise.all([
                            fetch('/digits/1.png'),
                            fetch('/digits/2.png'),
                        ]);
                        await new Promise((resolve) => setTimeout(resolve, 250));
                    })();").ContinueWith(x => t2 = DateTime.Now)
            );

            Assert.True(t1 > t2);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForNetworkIdle", "should work with no timeout")]
        [PuppeteerFact]
        public async Task ShouldWorkWithNoTimeout()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);

            await Task.WhenAll(
                DevToolsContext.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 0 }),
                DevToolsContext.EvaluateFunctionAsync(@"() => setTimeout(() => {
                        fetch('/digits/1.png');
                        fetch('/digits/2.png');
                        fetch('/digits/3.png');
                    }, 50)")
            );
        }
    }
}
