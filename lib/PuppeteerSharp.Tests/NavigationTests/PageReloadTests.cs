using System;
using System.Threading;
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

        [Test, PuppeteerTest("page.spec", "Page Page.reload", "should enable or disable the cache based on reload params")]
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

        [Test, PuppeteerTest("navigation.spec", "navigation Page.reload", "should not reload when the signal is already aborted")]
        public async Task ShouldNotReloadWhenTheSignalIsAlreadyAborted()
        {
            var requests = 0;
            Server.SetRoute("/empty.html", _ =>
            {
                Interlocked.Increment(ref requests);
                return Task.CompletedTask;
            });
            await Page.GoToAsync(TestConstants.EmptyPage);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<OperationCanceledException>(() => Page.ReloadAsync(
                new ReloadOptions { CancellationToken = cts.Token }));
            await Task.Delay(200);
            Assert.That(requests, Is.EqualTo(1));
        }
    }
}
