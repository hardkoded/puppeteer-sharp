using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
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

        [PuppeteerTest("page.spec.ts", "Page.setCacheEnabled", "should enable or disable the cache based on the state passed")]
        [PuppeteerFact]
        public async Task ShouldEnableOrDisableTheCacheBasedOnTheStatePassed()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            var waitForRequestTask = Server.WaitForRequest<string>("/cached/one-style.html", (request) => request.Headers["if-modified-since"]);

            await Task.WhenAll(
                waitForRequestTask,
                DevToolsContext.ReloadAsync());

            Assert.False(string.IsNullOrEmpty(waitForRequestTask.Result));

            await DevToolsContext.SetCacheEnabledAsync(false);
            waitForRequestTask = Server.WaitForRequest<string>("/cached/one-style.html", (request) => request.Headers["if-modified-since"]);

            await Task.WhenAll(
                waitForRequestTask,
                DevToolsContext.ReloadAsync());

            Assert.True(string.IsNullOrEmpty(waitForRequestTask.Result));
        }

        [PuppeteerTest("page.spec.ts", "Page.setCacheEnabled", "should stay disabled when toggling request interception on/off")]
        [PuppeteerFact]
        public async Task ShouldStayDisabledWhenTogglingRequestInterceptionOnOff()
        {
            await DevToolsContext.SetCacheEnabledAsync(false);
            await DevToolsContext.SetRequestInterceptionAsync(true);
            await DevToolsContext.SetRequestInterceptionAsync(false);

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            var waitForRequestTask = Server.WaitForRequest<string>("/cached/one-style.html", (request) => request.Headers["if-modified-since"]);

            await Task.WhenAll(
              waitForRequestTask,
              DevToolsContext.ReloadAsync());

            Assert.True(string.IsNullOrEmpty(waitForRequestTask.Result));
        }
    }
}
