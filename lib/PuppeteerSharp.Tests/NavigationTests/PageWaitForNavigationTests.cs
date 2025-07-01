using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageWaitForNavigationTests : PuppeteerPageBaseTest
    {
        public PageWaitForNavigationTests() : base()
        {
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.waitForNavigation", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var waitForNavigationResult = Page.WaitForNavigationAsync();
            await Task.WhenAll(
                waitForNavigationResult,
                Page.EvaluateFunctionAsync("url => window.location.href = url", TestConstants.ServerUrl + "/grid.html")
            );
            var response = await waitForNavigationResult;
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Url, Does.Contain("grid.html"));
        }

        [Test, PuppeteerTest("navigation.spec", "Page.waitForNavigation", "should work with both domcontentloaded and load")]
        public async Task ShouldWorkWithBothDomcontentloadedAndLoad()
        {
            var responseCompleted = new TaskCompletionSource<bool>();
            Server.SetRoute("/one-style.css", _ =>
            {
                return responseCompleted.Task;
            });

            var waitForRequestTask = Server.WaitForRequest("/one-style.css");
            var navigationTask = Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            var domContentLoadedTask = Page.WaitForNavigationAsync(new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded }
            });

            var bothFired = false;
            var bothFiredTask = Page.WaitForNavigationAsync(new NavigationOptions
            {
                WaitUntil = new[]
                {
                    WaitUntilNavigation.Load,
                    WaitUntilNavigation.DOMContentLoaded
                }
            }).ContinueWith(_ => bothFired = true);

            await waitForRequestTask.WithTimeout(5_000);
            await domContentLoadedTask.WithTimeout();
            Assert.That(bothFired, Is.False);
            responseCompleted.SetResult(true);
            await bothFiredTask.WithTimeout();
            await navigationTask.WithTimeout();
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.waitForNavigation", "should work with clicking on anchor links")]
        public async Task ShouldWorkWithClickingOnAnchorLinks()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a href='#foobar'>foobar</a>");
            var navigationTask = Page.WaitForNavigationAsync();
            await Task.WhenAll(
                navigationTask,
                Page.ClickAsync("a")
            );
            Assert.That(await navigationTask, Is.Null);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.EmptyPage + "#foobar"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.waitForNavigation", "should work with history.pushState()")]
        public async Task ShouldWorkWithHistoryPushState()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync(@"
              <a onclick='javascript:pushState()'>SPA</a>
              <script>
                function pushState() { history.pushState({}, '', 'wow.html') }
              </script>
            ");
            var navigationTask = Page.WaitForNavigationAsync();
            await Task.WhenAll(
                navigationTask,
                Page.ClickAsync("a")
            );
            Assert.That(await navigationTask, Is.Null);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/wow.html"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.waitForNavigation", "should work with history.replaceState()")]
        public async Task ShouldWorkWithHistoryReplaceState()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync(@"
              <a onclick='javascript:pushState()'>SPA</a>
              <script>
                function pushState() { history.pushState({}, '', 'replaced.html') }
              </script>
            ");
            var navigationTask = Page.WaitForNavigationAsync();
            await Task.WhenAll(
                navigationTask,
                Page.ClickAsync("a")
            );
            Assert.That(await navigationTask, Is.Null);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/replaced.html"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.waitForNavigation", "should work with DOM history.back()/history.forward()")]
        public async Task ShouldWorkWithDOMHistoryBackAndHistoryForward()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync(@"
              <a id=back onclick='javascript:goBack()'>back</a>
              <a id=forward onclick='javascript:goForward()'>forward</a>
              <script>
                function goBack() { history.back(); }
                function goForward() { history.forward(); }
                history.pushState({}, '', '/first.html');
                history.pushState({}, '', '/second.html');
              </script>
            ");
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/second.html"));
            var navigationTask = Page.WaitForNavigationAsync();
            await Task.WhenAll(
                navigationTask,
                Page.ClickAsync("a#back")
            );
            Assert.That(await navigationTask, Is.Null);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/first.html"));
            navigationTask = Page.WaitForNavigationAsync();
            await Task.WhenAll(
                navigationTask,
                Page.ClickAsync("a#forward")
            );
            Assert.That(await navigationTask, Is.Null);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/second.html"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.waitForNavigation", "should work when subframe issues window.stop()")]
        public async Task ShouldWorkWhenSubframeIssuesWindowStop()
        {
            Server.SetRoute("/frames/style.css", _ => Task.CompletedTask);
            var navigationTask = Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            var frameAttachedTaskSource = new TaskCompletionSource<IFrame>();
            Page.FrameAttached += (_, e) =>
            {
                frameAttachedTaskSource.SetResult(e.Frame);
            };

            var frame = await frameAttachedTaskSource.Task;
            var frameNavigatedTaskSource = new TaskCompletionSource<bool>();
            Page.FrameNavigated += (_, e) =>
            {
                if (e.Frame == frame)
                {
                    frameNavigatedTaskSource.TrySetResult(true);
                }
            };
            await frameNavigatedTaskSource.Task;
            await Task.WhenAll(
                frame.EvaluateFunctionAsync("() => window.stop()"),
                navigationTask
            );
        }
    }
}
