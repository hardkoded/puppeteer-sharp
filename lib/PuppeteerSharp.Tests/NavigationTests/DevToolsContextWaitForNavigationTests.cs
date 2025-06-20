using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using CefSharp.Dom;
using CefSharp.Dom.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextWaitForNavigationTests : DevToolsContextBaseTest
    {
        public DevToolsContextWaitForNavigationTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("navigation.spec.ts", "Page.waitForNavigation", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var waitForNavigationResult = DevToolsContext.WaitForNavigationAsync();
            await Task.WhenAll(
                waitForNavigationResult,
                DevToolsContext.EvaluateFunctionAsync("url => window.location.href = url", TestConstants.ServerUrl + "/grid.html")
            );
            var response = await waitForNavigationResult;
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Contains("grid.html", response.Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.waitForNavigation", "should work with both domcontentloaded and load")]
        [PuppeteerRetryFact()]
        public async Task ShouldWorkWithBothDomcontentloadedAndLoad()
        {
            var responseCompleted = new TaskCompletionSource<bool>();
            Server.SetRoute("/one-style.css", _ =>
            {
                return responseCompleted.Task;
            });

            var waitForRequestTask = Server.WaitForRequest("/one-style.css");
            var navigationTask = DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            var domContentLoadedTask = DevToolsContext.WaitForNavigationAsync(new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded }
            });

            var bothFired = false;
            var bothFiredTask = DevToolsContext.WaitForNavigationAsync(new NavigationOptions
            {
                WaitUntil = new[]
                {
                    WaitUntilNavigation.Load,
                    WaitUntilNavigation.DOMContentLoaded
                }
            }).ContinueWith(_ => bothFired = true);

            await waitForRequestTask.WithTimeout(5_000);
            await domContentLoadedTask.WithTimeout();
            Assert.False(bothFired);
            responseCompleted.SetResult(true);
            await bothFiredTask.WithTimeout();
            await navigationTask.WithTimeout();
        }

        [PuppeteerTest("navigation.spec.ts", "Page.waitForNavigation", "should work with clicking on anchor links")]
        [PuppeteerFact]
        public async Task ShouldWorkWithClickingOnAnchorLinks()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.SetContentAsync("<a href='#foobar'>foobar</a>");
            var navigationTask = DevToolsContext.WaitForNavigationAsync();
            await Task.WhenAll(
                navigationTask,
                DevToolsContext.ClickAsync("a")
            );
            Assert.Null(await navigationTask);
            Assert.Equal(TestConstants.EmptyPage + "#foobar", DevToolsContext.Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.waitForNavigation", "should work with history.pushState()")]
        [PuppeteerFact]
        public async Task ShouldWorkWithHistoryPushState()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.SetContentAsync(@"
              <a onclick='javascript:pushState()'>SPA</a>
              <script>
                function pushState() { history.pushState({}, '', 'wow.html') }
              </script>
            ");
            var navigationTask = DevToolsContext.WaitForNavigationAsync();
            await Task.WhenAll(
                navigationTask,
                DevToolsContext.ClickAsync("a")
            );
            Assert.Null(await navigationTask);
            Assert.Equal(TestConstants.ServerUrl + "/wow.html", DevToolsContext.Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.waitForNavigation", "should work with history.replaceState()")]
        [PuppeteerFact]
        public async Task ShouldWorkWithHistoryReplaceState()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.SetContentAsync(@"
              <a onclick='javascript:pushState()'>SPA</a>
              <script>
                function pushState() { history.pushState({}, '', 'replaced.html') }
              </script>
            ");
            var navigationTask = DevToolsContext.WaitForNavigationAsync();
            await Task.WhenAll(
                navigationTask,
                DevToolsContext.ClickAsync("a")
            );
            Assert.Null(await navigationTask);
            Assert.Equal(TestConstants.ServerUrl + "/replaced.html", DevToolsContext.Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.waitForNavigation", "should work with DOM history.back()/history.forward()")]
        [PuppeteerFact]
        public async Task ShouldWorkWithDOMHistoryBackAndHistoryForward()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.SetContentAsync(@"
              <a id=back onclick='javascript:goBack()'>back</a>
              <a id=forward onclick='javascript:goForward()'>forward</a>
              <script>
                function goBack() { history.back(); }
                function goForward() { history.forward(); }
                history.pushState({}, '', '/first.html');
                history.pushState({}, '', '/second.html');
              </script>
            ");
            Assert.Equal(TestConstants.ServerUrl + "/second.html", DevToolsContext.Url);
            var navigationTask = DevToolsContext.WaitForNavigationAsync();
            await Task.WhenAll(
                navigationTask,
                DevToolsContext.ClickAsync("a#back")
            );
            Assert.Null(await navigationTask);
            Assert.Equal(TestConstants.ServerUrl + "/first.html", DevToolsContext.Url);
            navigationTask = DevToolsContext.WaitForNavigationAsync();
            await Task.WhenAll(
                navigationTask,
                DevToolsContext.ClickAsync("a#forward")
            );
            Assert.Null(await navigationTask);
            Assert.Equal(TestConstants.ServerUrl + "/second.html", DevToolsContext.Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.waitForNavigation", "should work when subframe issues window.stop()")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenSubframeIssuesWindowStop()
        {
            Server.SetRoute("/frames/style.css", _ => Task.CompletedTask);
            var navigationTask = DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            var frameAttachedTaskSource = new TaskCompletionSource<Frame>();
            DevToolsContext.FrameAttached += (_, e) =>
            {
                frameAttachedTaskSource.SetResult(e.Frame);
            };

            var frame = await frameAttachedTaskSource.Task;
            var frameNavigatedTaskSource = new TaskCompletionSource<bool>();
            DevToolsContext.FrameNavigated += (_, e) =>
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
