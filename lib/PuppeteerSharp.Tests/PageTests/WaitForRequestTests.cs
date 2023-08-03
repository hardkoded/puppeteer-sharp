using System;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.PageTests
{
    public class WaitForRequestTests : PuppeteerPageBaseTest
    {
        public WaitForRequestTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForRequest", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForRequestAsync(TestConstants.ServerUrl + "/digits/2.png");

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"() => {
                    fetch('/digits/1.png');
                    fetch('/digits/2.png');
                    fetch('/digits/3.png');
                }")
            );
            Assert.AreEqual(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForRequest", "should work with predicate")]
        [PuppeteerTimeout]
        public async Task ShouldWorkWithPredicate()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForRequestAsync(request => request.Url == TestConstants.ServerUrl + "/digits/2.png");

            await Task.WhenAll(
            task,
            Page.EvaluateFunctionAsync(@"() => {
                fetch('/digits/1.png');
                fetch('/digits/2.png');
                fetch('/digits/3.png');
            }")
            );
            Assert.AreEqual(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForRequest", "should respect timeout")]
        [PuppeteerTimeout]
        public async Task ShouldRespectTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await Page.WaitForRequestAsync(_ => false, new WaitForOptions
                {
                    Timeout = 1
                }));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForRequest", "should respect default timeout")]
        [PuppeteerTimeout]
        public async Task ShouldRespectDefaultTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Page.DefaultTimeout = 1;
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await Page.WaitForRequestAsync(_ => false));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForRequest", "should work with no timeout")]
        [PuppeteerTimeout]
        public async Task ShouldWorkWithNoTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForRequestAsync(TestConstants.ServerUrl + "/digits/2.png", new WaitForOptions
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
            Assert.AreEqual(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }
    }
}
