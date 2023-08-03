#pragma warning disable CS0618 // WaitForXPathAsync is obsolete but we test the funcionatlity anyway
using System;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.WaitForTests
{
    public class PageWaitForTests : PuppeteerPageBaseTest
    {
        public PageWaitForTests(): base()
        {
        }

        [PuppeteerTest("waittask.spec.ts", "Page.waitFor", "should wait for selector")]
        [PuppeteerTimeout]
        public async Task ShouldWaitForSelector()
        {
            var found = false;
            var waitFor = Page.WaitForSelectorAsync("div").ContinueWith(_ => found = true);
            await Page.GoToAsync(TestConstants.EmptyPage);

            Assert.False(found);

            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await waitFor;
            Assert.True(found);
        }

        [PuppeteerTest("waittask.spec.ts", "Page.waitFor", "should wait for an xpath")]
        [PuppeteerTimeout]
        public async Task ShouldWaitForAnXpath()
        {
            var found = false;
            var waitFor = Page.WaitForXPathAsync("//div").ContinueWith(_ => found = true);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.False(found);
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await waitFor;
            Assert.True(found);
        }

        [PuppeteerTest("waittask.spec.ts", "Page.waitFor", "should not allow you to select an element with single slash xpath")]
        [PuppeteerTimeout]
        public async Task ShouldNotAllowYouToSelectAnElementWithSingleSlashXpath()
        {
            await Page.SetContentAsync("<div>some text</div>");
            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(() =>
                Page.WaitForSelectorAsync("/html/body/div"));
            Assert.NotNull(exception);
        }

        [PuppeteerTest("waittask.spec.ts", "Page.waitFor", "should timeout")]
        [PuppeteerTimeout]
        public async Task ShouldTimeout()
        {
            var startTime = DateTime.UtcNow;
            var timeout = 42;
            await Page.WaitForTimeoutAsync(timeout);
            Assert.True((DateTime.UtcNow - startTime).TotalMilliseconds > timeout / 2);
        }

        [PuppeteerTest("waittask.spec.ts", "Page.waitFor", "should work with multiline body")]
        [PuppeteerTimeout]
        public async Task ShouldWorkWithMultilineBody()
        {
            var result = await Page.WaitForExpressionAsync(@"
                (() => true)()
            ");
            Assert.True(await result.JsonValueAsync<bool>());
        }

        [PuppeteerTest("waittask.spec.ts", "Page.waitFor", "should wait for predicate")]
        [PuppeteerTimeout]
        public Task ShouldWaitForPredicate()
            => Task.WhenAll(
                Page.WaitForFunctionAsync("() => window.innerWidth < 100"),
                Page.SetViewportAsync(new ViewPortOptions { Width = 10, Height = 10 }));

        [PuppeteerTest("waittask.spec.ts", "Page.waitFor", "should wait for predicate with arguments")]
        [PuppeteerTimeout]
        public async Task ShouldWaitForPredicateWithArguments()
            => await Page.WaitForFunctionAsync("(arg1, arg2) => arg1 !== arg2", new WaitForFunctionOptions(), 1, 2);
    }
}
#pragma warning restore CS0618