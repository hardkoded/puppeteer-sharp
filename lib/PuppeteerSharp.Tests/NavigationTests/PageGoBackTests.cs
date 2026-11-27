using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NavigationTests
{
    public class PageGoBackTests : PuppeteerPageBaseTest
    {
        public PageGoBackTests()
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();

            if (TestConstants.IsChrome)
            {
                // These tests need going back and forward to request the page. Chrome can restore
                // it from the back/forward cache instead, even with "Cache-Control: no-store" and
                // the HTTP cache disabled. Then there is no request and no response.
                DefaultOptions.Args =
                [
                    .. DefaultOptions.Args,
                    "--disable-features=BackForwardCache",
                ];
            }
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goBack", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetCacheEnabledAsync(false);
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            var response = await Page.GoBackAsync();
            Assert.That(response.Ok, Is.True);
            Assert.That(response.Url, Is.EqualTo(TestConstants.EmptyPage));

            response = await Page.GoForwardAsync();
            Assert.That(response.Ok, Is.True);
            Assert.That(response.Url, Does.Contain("grid"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goBack", "should be cancellable")]
        public async Task ShouldBeCancellable()
        {
            await Page.SetCacheEnabledAsync(false);
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            // Hang for the request going back makes.
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));
            using var cts = new CancellationTokenSource();
            var requestTask = Server.WaitForRequest("/empty.html");
            var navigation = Page.GoBackAsync(
                new NavigationOptions { CancellationToken = cts.Token }
            );

            // Fail instead of hanging if going back ends without requesting the page.
            await await Task.WhenAny(requestTask, navigation);
            Assert.That(
                requestTask.IsCompleted,
                Is.True,
                "Going back ended without requesting the page"
            );

            cts.Cancel();
            Assert.CatchAsync<OperationCanceledException>(() => navigation);
        }

        [
            Test,
            PuppeteerTest(
                "navigation.spec",
                "navigation Page.goBack",
                "should error if no history is found"
            )
        ]
        public async Task ShouldErrorIfNoHistoryIsFound()
        {
            var exception = Assert.CatchAsync<PuppeteerException>(async () =>
                await Page.GoBackAsync()
            );
            Assert.That(exception, Is.Not.Null);
            Assert.That(
                exception.Message,
                Does.Contain("History entry to navigate to not found.")
                    .Or.Contain("no such history entry")
            );
        }

        [
            Test,
            PuppeteerTest(
                "navigation.spec",
                "navigation Page.goBack",
                "should work with HistoryAPI"
            )
        ]
        public async Task ShouldWorkWithHistoryAPI()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync(
                @"
              history.pushState({ }, '', '/first.html');
              history.pushState({ }, '', '/second.html');
            "
            );
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/second.html"));

            var response = await Page.GoBackAsync();
            Assert.That(response, Is.Null);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/first.html"));
            await Page.GoBackAsync();
            Assert.That(Page.Url, Is.EqualTo(TestConstants.EmptyPage));
            response = await Page.GoForwardAsync();
            Assert.That(response, Is.Null);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/first.html"));
        }

        [
            Test,
            PuppeteerTest(
                "navigation.spec",
                "navigation Page.goBack",
                "should not go back when the signal is already aborted"
            )
        ]
        public async Task ShouldNotGoBackWhenTheSignalIsAlreadyAborted()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<OperationCanceledException>(() =>
                Page.GoBackAsync(new NavigationOptions { CancellationToken = cts.Token })
            );
            await Task.Delay(200);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/grid.html"));
        }
    }
}
