using System;
using System.Threading.Tasks;
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

            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForNetworkIdleAsync().ContinueWith(x => t1 = DateTime.Now);

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"
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
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await Page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 1 }));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        // This should work on Firefox, this ignore should be temporal
        // PRs are welcome :)
        [PuppeteerTest("page.spec.ts", "Page.waitForNetworkIdle", "should respect idleTime")]
        [SkipBrowserFact(skipFirefox: true)] 
        public async Task ShouldRespectIdleTimeout()
        {
            var t1 = DateTime.Now;
            var t2 = DateTime.Now;

            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { IdleTime = 10 }).ContinueWith(x => t1 = DateTime.Now);

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"() =>
                (async () => {
                    await Promise.all([
                    fetch('/digits/1.png'),
                    fetch('/digits/2.png'),
                    ]);
                    await new Promise((resolve) => setTimeout(resolve, 250));
                })()").ContinueWith(x => t2 = DateTime.Now)
            );

            Assert.True(t2 > t1);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForNetworkIdle", "should work with no timeout")]
        [PuppeteerFact]
        public async Task ShouldWorkWithNoTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Task.WhenAll(
                Page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 0 }),
                Page.EvaluateFunctionAsync(@"() => setTimeout(() => {
                        fetch('/digits/1.png');
                        fetch('/digits/2.png');
                        fetch('/digits/3.png');
                    }, 50)")
            );
        }
    }
}
