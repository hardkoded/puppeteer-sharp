using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ExposeFunctionTests : PuppeteerPageBaseTest
    {
        public ExposeFunctionTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            var result = await Page.EvaluateExpressionAsync<int>("compute(9, 4)");
            Assert.Equal(36, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should throw exception in page context")]
        [SkipBrowserFact(skipFirefox: true)]
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
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldBeCallableFromInsideEvaluateOnNewDocument()
        {
            var called = false;
            await Page.ExposeFunctionAsync("woof", () => called = true);
            await Page.EvaluateFunctionOnNewDocumentAsync("() => woof()");
            await Page.ReloadAsync();
            Assert.True(called);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSurviveNavigation()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            await Page.GoToAsync(TestConstants.EmptyPage);
            var result = await Page.EvaluateExpressionAsync<int>("compute(9, 4)");
            Assert.Equal(36, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should await returned promise")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldAwaitReturnedValueTask()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            var result = await Page.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work on frames")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkOnFrames()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            var frame = Page.FirstChildFrame();
            var result = await frame.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work on frames before navigation")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkOnFramesBeforeNavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));

            var frame = Page.FirstChildFrame();
            var result = await frame.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work with complex objects")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithComplexObjects()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            await Page.ExposeFunctionAsync("complexObject", (dynamic a, dynamic b) => Task.FromResult(new { X = a.x + b.x }));

            var result = await Page.EvaluateExpressionAsync<JToken>("complexObject({x: 5}, {x: 2})");
            Assert.Equal(7, result.SelectToken("x").ToObject<int>());
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldAwaitReturnedTask()
        {
            var called = false;
            await Page.ExposeFunctionAsync("changeFlag", () =>
            {
                called = true;
                return Task.CompletedTask;
            });
            await Page.EvaluateExpressionAsync("changeFlag()");
            Assert.True(called);
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithAction()
        {
            var called = false;
            await Page.ExposeFunctionAsync("changeFlag", () =>
            {
                called = true;
            });
            await Page.EvaluateExpressionAsync("changeFlag()");
            Assert.True(called);
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldKeepTheCallbackClean()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            await Page.EvaluateExpressionAsync<int>("compute(9, 4)");
            Assert.False(Page.Client.HasPendingCallbacks());
        }
    }
}
