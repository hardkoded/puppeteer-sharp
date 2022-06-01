using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using CefSharp.Puppeteer;
using CefSharp.Puppeteer.Helpers;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class WaitForRequestTests : DevToolsContextBaseTest
    {
        public WaitForRequestTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForRequest", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var task = DevToolsContext.WaitForRequestAsync(TestConstants.ServerUrl + "/digits/2.png");

            await Task.WhenAll(
                task,
                DevToolsContext.EvaluateFunctionAsync(@"() => {
                    fetch('/digits/1.png');
                    fetch('/digits/2.png');
                    fetch('/digits/3.png');
                }")
            );
            Assert.Equal(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForRequest", "should work with predicate")]
        [PuppeteerFact]
        public async Task ShouldWorkWithPredicate()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var task = DevToolsContext.WaitForRequestAsync(request => request.Url == TestConstants.ServerUrl + "/digits/2.png");

            await Task.WhenAll(
            task,
            DevToolsContext.EvaluateFunctionAsync(@"() => {
                fetch('/digits/1.png');
                fetch('/digits/2.png');
                fetch('/digits/3.png');
            }")
            );
            Assert.Equal(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForRequest", "should respect timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectTimeout()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await DevToolsContext.WaitForRequestAsync(_ => false, new WaitForOptions
                {
                    Timeout = 1
                }));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForRequest", "should respect default timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectDefaultTimeout()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            DevToolsContext.DefaultTimeout = 1;
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await DevToolsContext.WaitForRequestAsync(_ => false));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForRequest", "should work with no timeout")]
        [PuppeteerFact]
        public async Task ShouldWorkWithNoTimeout()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var task = DevToolsContext.WaitForRequestAsync(TestConstants.ServerUrl + "/digits/2.png", new WaitForOptions
            {
                Timeout = 0
            });

            await Task.WhenAll(
                task,
                DevToolsContext.EvaluateFunctionAsync(@"() => setTimeout(() => {
                    fetch('/digits/1.png');
                    fetch('/digits/2.png');
                    fetch('/digits/3.png');
                }, 50)")
            );
            Assert.Equal(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }
    }
}
