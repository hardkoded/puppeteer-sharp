using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NavigationTests
{
    public class PageReloadTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("navigation.spec", "navigation Page.reload", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync("() => (globalThis._foo = 10)");
            await Page.ReloadAsync();
            Assert.That(await Page.EvaluateFunctionAsync<int?>("() => globalThis._foo"), Is.Null);
        }
    
        [Test, PuppeteerTest("navigation.spec", "navigation Page.reload", "should enable or disable the cache based on reload params")]
        public async Task ShouldEnableOrDisableTheCacheBasedOnReloadParams()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            var waitForRequestTask = Server.WaitForRequest<string>("/cached/one-style.html", (request) => request.Headers["if-modified-since"]);

            await Task.WhenAll(
                waitForRequestTask,
                Page.ReloadAsync());

            Assert.That(string.IsNullOrEmpty(waitForRequestTask.Result), Is.False);

            waitForRequestTask = Server.WaitForRequest<string>("/cached/one-style.html", (request) => request.Headers["if-modified-since"]);

            await Task.WhenAll(
                waitForRequestTask,
                Page.ReloadAsync(new ReloadOptions { IgnoreCache = true }));

            Assert.That(string.IsNullOrEmpty(waitForRequestTask.Result), Is.True);
        }
}
}
