using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class SetCacheEnabledTests : PuppeteerPageBaseTest
    {
        public SetCacheEnabledTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldEnableOrDisableTheCacheBasedOnTheStatePassed()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            var waitForRequestTask = Server.WaitForRequest<string>("/cached/one-style.html", (request) => request.Headers["if-modified-since"]);

            await Task.WhenAll(
                waitForRequestTask,
                Page.ReloadAsync());

            Assert.False(string.IsNullOrEmpty(waitForRequestTask.Result));

            await Page.SetCacheEnabledAsync(false);
            waitForRequestTask = Server.WaitForRequest<string>("/cached/one-style.html", (request) => request.Headers["if-modified-since"]);

            await Task.WhenAll(
                waitForRequestTask,
                Page.ReloadAsync());

            Assert.True(string.IsNullOrEmpty(waitForRequestTask.Result));
        }

        [Fact]
        public async Task ShouldStayDisabledWhenTogglingRequestInterceptionOnOff()
        {
            await Page.SetCacheEnabledAsync(false);
            await Page.SetRequestInterceptionAsync(true);
            await Page.SetRequestInterceptionAsync(false);

            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            var waitForRequestTask = Server.WaitForRequest<string>("/cached/one-style.html", (request) => request.Headers["if-modified-since"]);

            await Task.WhenAll(
              waitForRequestTask,
              Page.ReloadAsync());

            Assert.True(string.IsNullOrEmpty(waitForRequestTask.Result));
        }
    }
}
