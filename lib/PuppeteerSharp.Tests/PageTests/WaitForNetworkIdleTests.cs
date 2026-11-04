using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class WaitForNetworkIdleTests : PuppeteerPageBaseTest
    {
        public WaitForNetworkIdleTests() : base()
        {
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForNetworkIdle", "should work")]
        public async Task ShouldWork()
        {
            var t1 = DateTime.UtcNow;
            var t2 = DateTime.UtcNow;

            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForNetworkIdleAsync()
                .ContinueWith(x =>
                {
                    if (x.IsFaulted) throw x.Exception;
                    return t1 = DateTime.UtcNow;
                });

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"async () => {
                    await Promise.all([
                            fetch('/digits/1.png'),
                            fetch('/digits/2.png'),
                        ]);
                    await new Promise((resolve) => setTimeout(resolve, 200));
                    await fetch('/digits/3.png');
                    await new Promise((resolve) => setTimeout(resolve, 200));
                    await fetch('/digits/4.png');
                }").ContinueWith(x =>
                {
                    if (x.IsFaulted) throw x.Exception;
                    t2 = DateTime.UtcNow;
                })
            );

            Assert.That(t1, Is.GreaterThan(t2));
            Assert.That((t1 - t2).TotalMilliseconds, Is.GreaterThanOrEqualTo(400));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForNetworkIdle", "should respect timeout")]
        public async Task ShouldRespectTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
                await Page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 1 }));

            Assert.That(exception.Message, Does.Contain("Timeout of 1 ms exceeded"));
        }

        // This should work on Firefox, this ignore should be temporal
        // PRs are welcome :)
        [Test, PuppeteerTest("page.spec", "Page Page.waitForNetworkIdle", "should respect idleTime")]
        public async Task ShouldRespectIdleTimeout()
        {
            var t1 = DateTime.UtcNow;
            var t2 = DateTime.UtcNow;

            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { IdleTime = 10 })
                .ContinueWith(x =>
                {
                    if (x.IsFaulted) throw x.Exception;
                    return t1 = DateTime.UtcNow;
                });

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"async () => {
                    await Promise.all([
                    fetch('/digits/1.png'),
                    fetch('/digits/2.png'),
                    ]);
                    await new Promise((resolve) => setTimeout(resolve, 250));
                }").ContinueWith(x =>
                {
                    if (x.IsFaulted) throw x.Exception;
                    return t2 = DateTime.UtcNow;
                })
            );

            Assert.That(t2, Is.GreaterThan(t1));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForNetworkIdle", "should work with aborted requests")]
        public async Task ShouldWorkWithAbortedRequests()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/abort-request.html");

            await using var element = await Page.QuerySelectorAsync("#abort");
            await element.ClickAsync();

            var error = false;
            try
            {
                await Page.WaitForNetworkIdleAsync();
            }
            catch
            {
                error = true;
            }

            Assert.That(error, Is.False);
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForNetworkIdle", "should work with delayed response")]
        public async Task ShouldWorkWithDelayedResponse()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var responseTcs = new TaskCompletionSource<HttpResponse>();
            Server.SetRoute("/fetch-request-b.js", context =>
            {
                responseTcs.SetResult(context.Response);
                // Don't complete the response yet - wait for external signal
                return Task.Delay(10000);
            });

            var stopwatch = Stopwatch.StartNew();
            var idleTask = Page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { IdleTime = 100 });

            // Start the fetch which will be delayed
            var fetchTask = Page.EvaluateFunctionAsync("async () => { await fetch('/fetch-request-b.js'); }");

            // Wait for the server to receive the request
            var response = await responseTcs.Task;

            // Wait 300ms then complete the response
            await Task.Delay(300);
            var t2 = stopwatch.ElapsedMilliseconds;
            response.StatusCode = 200;
            await response.CompleteAsync();

            await idleTask;
            var t1 = stopwatch.ElapsedMilliseconds;

            Assert.That(t1, Is.GreaterThan(t2));
            // request finished + idle time.
            Assert.That(t1, Is.GreaterThanOrEqualTo(400));
            // request finished + idle time - request finished.
            Assert.That(t1 - t2, Is.GreaterThanOrEqualTo(100));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForNetworkIdle", "should work with no timeout")]
        public async Task ShouldWorkWithNoTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Task.WhenAll(
                Page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 0 }),
                Page.EvaluateFunctionAsync(@"() => setTimeout(() => {
                        fetch('/digits/1.png');
                        fetch('/digits/2.png');
                        fetch('/digits/3.png');
                    }, 50)")
            );
        }
    }
}
