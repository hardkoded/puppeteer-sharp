using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class WaitForTests : PuppeteerPageBaseTest
    {
        public WaitForTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public async Task ShouldNotAllowYouToSelectAnElementWithSingleSlashXpath()
        {
            await Page.SetContentAsync("<div>some text</div>");
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(() =>
                Page.WaitForSelectorAsync("/html/body/div"));
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task ShouldTimeout()
        {
            var startTime = DateTime.Now;
            var timeout = 42;
            await Page.WaitForTimeoutAsync(timeout);
            Assert.True((DateTime.Now - startTime).TotalMilliseconds > timeout / 2);
        }

        [Fact]
        public async Task ShouldWorkWithMultilineBody()
        {
            var result = await Page.WaitForExpressionAsync(@"
                (() => true)()
            ");
            Assert.True(await result.JsonValueAsync<bool>());
        }

        [Fact]
        public Task ShouldWaitForPredicate()
            => Task.WhenAll(
                Page.WaitForFunctionAsync("() => window.innerWidth < 100"),
                Page.SetViewportAsync(new ViewPortOptions { Width = 10, Height = 10 })
        );

        [Fact]
        public async Task ShouldWaitForPredicateWithArguments()
            => await Page.WaitForFunctionAsync("(arg1, arg2) => arg1 !== arg2", new WaitForFunctionOptions(), 1, 2);
    }
}
