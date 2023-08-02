using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class ExposeFunctionTests : PuppeteerPageBaseTest
    {
        public ExposeFunctionTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            var result = await Page.EvaluateFunctionAsync<int>("async () => compute(9, 4)");
            Assert.Equal(36, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should throw exception in page context")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Assert.Equal("WOOF WOOF", result.SelectToken("message").ToObject<string>());
            Assert.Contains("ExposeFunctionTests", result.SelectToken("stack").ToObject<string>());
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should be callable from-inside evaluateOnNewDocument")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldBeCallableFromInsideEvaluateOnNewDocument()
        {
            var called = false;
            await Page.ExposeFunctionAsync("woof", () => called = true);
            await Page.EvaluateFunctionOnNewDocumentAsync("async () => woof()");
            await Page.ReloadAsync();
            Assert.True(called);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldSurviveNavigation()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            await Page.GoToAsync(TestConstants.EmptyPage);
            var result = await Page.EvaluateFunctionAsync<int>("async () => compute(9, 4)");
            Assert.Equal(36, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should await returned promise")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldAwaitReturnedValueTask()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            var result = await Page.EvaluateFunctionAsync<int>("async () => compute(3, 5)");
            Assert.Equal(15, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work on frames")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkOnFrames()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            var frame = Page.FirstChildFrame();
            var result = await frame.EvaluateFunctionAsync<int>("async () => compute(3, 5)");
            Assert.Equal(15, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work on frames before navigation")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkOnFramesBeforeNavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));

            var frame = Page.FirstChildFrame();
            var result = await frame.EvaluateFunctionAsync<int>("async () => compute(3, 5)");
            Assert.Equal(15, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work with complex objects")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkWithComplexObjects()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            await Page.ExposeFunctionAsync("complexObject", (dynamic a, dynamic b) => Task.FromResult(new { X = a.x + b.x }));

            var result = await Page.EvaluateFunctionAsync<JToken>("async () => complexObject({x: 5}, {x: 2})");
            Assert.Equal(7, result.SelectToken("x").ToObject<int>());
        }

        [Skip(SkipAttribute.Targets.Firefox)]
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

        [Skip(SkipAttribute.Targets.Firefox)]
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

        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldKeepTheCallbackClean()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            await Page.EvaluateFunctionAsync<int>("async () => await compute(9, 4)");

            // Giving a tiny wait to let the connection clear the callback list.
            await Task.Delay(300);

            // For CI/CD debugging purposes
            var session = (CDPSession)Page.Client;
            var message = "Expected an empty callback list. Found: \n";

            if (session.HasPendingCallbacks())
            {
                foreach(var pendingMessage in session.GetPendingMessages())
                {
                    message += $" - {pendingMessage.Message}\n";
                }
            }

            Assert.False(((CDPSession)Page.Client).HasPendingCallbacks(), message);
        }
    }
}
