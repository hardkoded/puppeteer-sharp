using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class SetCacheEnabledTests : PuppeteerPageBaseTest
    {
        public SetCacheEnabledTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.setCacheEnabled", "should enable or disable the cache based on the state passed")]
        [PuppeteerTimeout]
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

        [PuppeteerTest("page.spec.ts", "Page.setCacheEnabled", "should stay disabled when toggling request interception on/off")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
