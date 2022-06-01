using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ExposeFunctionTests : PuppeteerPageBaseTest
    {
        public ExposeFunctionTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            var result = await DevToolsContext.EvaluateExpressionAsync<int>("compute(9, 4)");
            Assert.Equal(36, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should throw exception in page context")]
        [PuppeteerFact]
        public async Task ShouldThrowExceptionInPageContext()
        {
            await DevToolsContext.ExposeFunctionAsync("woof", () => throw new Exception("WOOF WOOF"));
            var result = await DevToolsContext.EvaluateFunctionAsync<JToken>(@" async () =>{
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

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should survive navigation")]
        [PuppeteerFact]
        public async Task ShouldBeCallableFromInsideEvaluateOnNewDocument()
        {
            var called = false;
            await DevToolsContext.ExposeFunctionAsync("woof", () => called = true);
            await DevToolsContext.EvaluateFunctionOnNewDocumentAsync("() => woof()");
            await DevToolsContext.ReloadAsync();
            Assert.True(called);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work")]
        [PuppeteerFact]
        public async Task ShouldSurviveNavigation()
        {
            await DevToolsContext.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var result = await DevToolsContext.EvaluateExpressionAsync<int>("compute(9, 4)");
            Assert.Equal(36, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should await returned promise")]
        [PuppeteerFact]
        public async Task ShouldAwaitReturnedValueTask()
        {
            await DevToolsContext.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            var result = await DevToolsContext.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work on frames")]
        [PuppeteerFact]
        public async Task ShouldWorkOnFrames()
        {
            await DevToolsContext.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            var frame = DevToolsContext.FirstChildFrame();
            var result = await frame.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work on frames before navigation")]
        [PuppeteerFact]
        public async Task ShouldWorkOnFramesBeforeNavigation()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            await DevToolsContext.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));

            var frame = DevToolsContext.FirstChildFrame();
            var result = await frame.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work with complex objects")]
        [PuppeteerFact]
        public async Task ShouldWorkWithComplexObjects()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            await DevToolsContext.ExposeFunctionAsync("complexObject", (dynamic a, dynamic b) => Task.FromResult(new { X = a.x + b.x }));

            var result = await DevToolsContext.EvaluateExpressionAsync<JToken>("complexObject({x: 5}, {x: 2})");
            Assert.Equal(7, result.SelectToken("x").ToObject<int>());
        }

        [PuppeteerFact]
        public async Task ShouldAwaitReturnedTask()
        {
            var called = false;
            await DevToolsContext.ExposeFunctionAsync("changeFlag", () =>
            {
                called = true;
                return Task.CompletedTask;
            });
            await DevToolsContext.EvaluateExpressionAsync("changeFlag()");
            Assert.True(called);
        }

        [PuppeteerFact]
        public async Task ShouldWorkWithAction()
        {
            var called = false;
            await DevToolsContext.ExposeFunctionAsync("changeFlag", () =>
            {
                called = true;
            });
            await DevToolsContext.EvaluateExpressionAsync("changeFlag()");
            Assert.True(called);
        }

        [PuppeteerFact]
        public async Task ShouldKeepTheCallbackClean()
        {
            await DevToolsContext.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            await DevToolsContext.EvaluateExpressionAsync<int>("compute(9, 4)");
            Assert.False(DevToolsContext.Client.HasPendingCallbacks());
        }
    }
}
