using System;
using System.Net;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.PageTests
{
    public class WaitForResponseTests : PuppeteerPageBaseTest
    {
        public WaitForResponseTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should work")]
        [PuppeteerTimeout]
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
            Assert.AreEqual(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should work with predicate")]
        [PuppeteerTimeout]
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
            Assert.AreEqual(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should work with async predicate")]
        [PuppeteerTimeout]
        public async Task ShouldWorkWithAsyncPredicate()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(async (IResponse response) =>
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
            Assert.AreEqual(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should respect timeout")]
        [PuppeteerTimeout]
        public async Task ShouldRespectTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
                await Page.WaitForResponseAsync(_ => false, new WaitTimeoutOptions(1)));

            StringAssert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should respect default timeout")]
        [PuppeteerTimeout]
        public async Task ShouldRespectDefaultTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Page.DefaultTimeout = 1;
            var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
                await Page.WaitForResponseAsync(_ => false));

            StringAssert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.waitForResponse", "should work with no timeout")]
        [PuppeteerTimeout]
        public async Task ShouldWorkWithNoTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(TestConstants.ServerUrl + "/digits/2.png", new WaitTimeoutOptions(0));

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
