using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class SetCacheEnabledTests : PuppeteerPageBaseTest
    {
        public SetCacheEnabledTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setCacheEnabled", "should enable or disable the cache based on the state passed")]
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

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setCacheEnabled", "should stay disabled when toggling request interception on/off")]
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
