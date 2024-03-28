using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class ExposeFunctionTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.exposeFunction", "should work")]
        public async Task ShouldWork()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            var result = await Page.EvaluateFunctionAsync<int>("async () => compute(9, 4)");
            Assert.AreEqual(36, result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.exposeFunction", "should throw exception in page context")]
        public async Task ShouldThrowExceptionInPageContext()
        {
            await Page.ExposeFunctionAsync("woof", () => throw new Exception("WOOF WOOF"));
            var result = await Page.EvaluateFunctionAsync<JToken>(@" async () =>{
                try
                {
                    await woof();
                }
                catch (e)
                {
                    return { message: e.message, stack: e.stack};
                }
            }");
            Assert.AreEqual("WOOF WOOF", result.SelectToken("message").ToObject<string>());
            StringAssert.Contains("ExposeFunctionTests", result.SelectToken("stack").ToObject<string>());
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.exposeFunction", "should be callable from-inside evaluateOnNewDocument")]
        public async Task ShouldBeCallableFromInsideEvaluateOnNewDocument()
        {
            var called = false;
            await Page.ExposeFunctionAsync("woof", () => called = true);
            await Page.EvaluateFunctionOnNewDocumentAsync("async () => woof()");
            await Page.ReloadAsync();
            Assert.True(called);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.exposeFunction", "should work")]
        public async Task ShouldSurviveNavigation()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            await Page.GoToAsync(TestConstants.EmptyPage);
            var result = await Page.EvaluateFunctionAsync<int>("async () => compute(9, 4)");
            Assert.AreEqual(36, result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.exposeFunction", "should await returned promise")]
        public async Task ShouldAwaitReturnedValueTask()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            var result = await Page.EvaluateFunctionAsync<int>("async () => compute(3, 5)");
            Assert.AreEqual(15, result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.exposeFunction", "should work on frames")]
        public async Task ShouldWorkOnFrames()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            var frame = Page.FirstChildFrame();
            var result = await frame.EvaluateFunctionAsync<int>("async () => compute(3, 5)");
            Assert.AreEqual(15, result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.exposeFunction", "should work with loading frames")]
        public async Task ShouldWorkWithLoadingFrames()
        {
            await Page.SetRequestInterceptionAsync(true);

            var requestTcs = new TaskCompletionSource<IRequest>();
            Page.Request += (sender, e) =>
            {
                if (e.Request.Url.EndsWith("/frames/frame.html"))
                {
                    requestTcs.TrySetResult(e.Request);
                }
                else
                {
                    e.Request.ContinueAsync();
                }
            };

            var navTask = Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html",
                WaitUntilNavigation.Networkidle0);

            var request = await requestTcs.Task;

            var exposeTask = Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));

            await Task.WhenAll(request.ContinueAsync(), exposeTask);
            await navTask;
            var frame = Page.FirstChildFrame();
            var result = await frame.EvaluateFunctionAsync<int>("() => globalThis.compute(3, 5)");
            Assert.AreEqual(15, result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.exposeFunction", "should work on frames before navigation")]
        public async Task ShouldWorkOnFramesBeforeNavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));

            var frame = Page.FirstChildFrame();
            var result = await frame.EvaluateFunctionAsync<int>("async () => compute(3, 5)");
            Assert.AreEqual(15, result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.exposeFunction", "should work with complex objects")]
        public async Task ShouldWorkWithComplexObjects()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            await Page.ExposeFunctionAsync("complexObject", (dynamic a, dynamic b) => Task.FromResult(new { X = a.x + b.x }));

            var result = await Page.EvaluateFunctionAsync<JToken>("async () => complexObject({x: 5}, {x: 2})");
            Assert.AreEqual(7, result.SelectToken("x").ToObject<int>());
        }

        [Test, Retry(2), PuppeteerTest("puppeteer-sharp", "ExposeFunctionTests", "should await returned task")]
        public async Task ShouldAwaitReturnedTask()
        {
            var called = false;
            await Page.ExposeFunctionAsync("changeFlag", () =>
            {
                called = true;
                return Task.CompletedTask;
            });
            await Page.EvaluateFunctionAsync("async () => changeFlag()");
            Assert.True(called);
        }

        [Test, Retry(2), PuppeteerTest("puppeteer-sharp", "ExposeFunctionTests", "should work with action")]
        public async Task ShouldWorkWithAction()
        {
            var called = false;
            await Page.ExposeFunctionAsync("changeFlag", () =>
            {
                called = true;
            });
            await Page.EvaluateFunctionAsync("async () => changeFlag()");
            Assert.True(called);
        }

        [Test, Retry(2), PuppeteerTest("puppeteer-sharp", "ExposeFunctionTests", "should keel the callback clean")]
        public async Task ShouldKeepTheCallbackClean()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            await Page.EvaluateFunctionAsync<int>("async () => await compute(9, 4)");

            // Giving a tiny wait to let the connection clear the callback list.
            await Task.Delay(300);

            // For CI/CD debugging purposes
            var session = (CdpCDPSession)Page.Client;
            var message = "Expected an empty callback list. Found: \n";

            if (session.HasPendingCallbacks())
            {
                foreach (var pendingMessage in session.GetPendingMessages())
                {
                    message += $" - {pendingMessage.Message}\n";
                }
            }

            Assert.False(((CdpCDPSession)Page.Client).HasPendingCallbacks(), message);
        }
    }
}
