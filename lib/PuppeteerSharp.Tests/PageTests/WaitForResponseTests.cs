using System;
using System.Net;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class WaitForResponseTests : PuppeteerPageBaseTest
    {
        public WaitForResponseTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(TestConstants.ServerUrl + "/digits/2.png");

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"() => {
                    fetch('/digits/1.png');
                    fetch('/digits/2.png');
                    fetch('/digits/3.png');
                }")
            );
            Assert.Equal(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should work with predicate")]
        [PuppeteerFact]
        public async Task ShouldWorkWithPredicate()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(response => response.Url == TestConstants.ServerUrl + "/digits/2.png");

            await Task.WhenAll(
            task,
            Page.EvaluateFunctionAsync(@"() => {
                fetch('/digits/1.png');
                fetch('/digits/2.png');
                fetch('/digits/3.png');
            }")
            );
            Assert.Equal(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should work with async predicate")]
        [PuppeteerFact]
        public async Task ShouldWorkWithAsyncPredicate()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(async (Response response) =>
            {
                await Task.Delay(1);
                return response.Url == TestConstants.ServerUrl + "/digits/2.png";
            });

            await Task.WhenAll(
            task,
            Page.EvaluateFunctionAsync(@"() => {
                fetch('/digits/1.png');
                fetch('/digits/2.png');
                fetch('/digits/3.png');
            }")
            );
            Assert.Equal(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should respect timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await Page.WaitForResponseAsync(_ => false, new WaitForOptions
                {
                    Timeout = 1
                }));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should respect default timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectDefaultTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Page.DefaultTimeout = 1;
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await Page.WaitForResponseAsync(_ => false));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should work with no timeout")]
        [PuppeteerFact]
        public async Task ShouldWorkWithNoTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(TestConstants.ServerUrl + "/digits/2.png", new WaitForOptions
            {
                Timeout = 0
            });

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"() => setTimeout(() => {
                    fetch('/digits/1.png');
                    fetch('/digits/2.png');
                    fetch('/digits/3.png');
                }, 50)")
            );
            Assert.Equal(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }
    }
}
