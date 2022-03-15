using System;
using System.Collections.Generic;
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
                await Page.WaitForNetworkIdleAsync(timeout:1));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForNetworkIdle", "should respect idleTime")]
        [PuppeteerFact]
        public async Task ShouldRespectIdleTimeout()
        {
            var t1 = DateTime.Now;
            var t2 = DateTime.Now;

            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForNetworkIdleAsync(idleTime: 10).ContinueWith(x => t1 = DateTime.Now);

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"
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
            var responseCount = 0;
            var responseUrls = new List<string>(3);

            await Page.GoToAsync(TestConstants.EmptyPage);

            Page.FrameManager.NetworkManager.Response += (sender, args) =>
            {
                responseCount++;
                responseUrls.Add(args.Response.Url);
            };

            await Task.WhenAll(
                Page.WaitForNetworkIdleAsync(timeout: 0),
                Page.EvaluateFunctionAsync(@"() => setTimeout(() => {
                        fetch('/digits/1.png');
                        fetch('/digits/2.png');
                        fetch('/digits/3.png');
                    }, 50)")
            );

            Assert.Equal(3, responseCount);
            Assert.All(responseUrls, x =>
            {
                Assert.StartsWith(TestConstants.ServerUrl + "/digits", x);
                Assert.EndsWith(".png", x);
            });
        }
    }
}
