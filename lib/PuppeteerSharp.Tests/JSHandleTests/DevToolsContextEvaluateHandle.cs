using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextEvaluateHandle : DevToolsContextBaseTest
    {
        public DevToolsContextEvaluateHandle(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
            => Assert.NotNull(await DevToolsContext.EvaluateFunctionHandleAsync("() => window"));

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should accept object handle as an argument")]
        [PuppeteerFact]
        public async Task ShouldAcceptObjectHandleAsAnArgument()
        {
            var navigatorHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => navigator");
            var text = await DevToolsContext.EvaluateFunctionAsync<string>(
                "(e) => e.userAgent",
                navigatorHandle);
            Assert.Contains("Mozilla", text);
        }

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should accept object handle to primitive types")]
        [PuppeteerFact]
        public async Task ShouldAcceptObjectHandleToPrimitiveTypes()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => 5");
            var isFive = await DevToolsContext.EvaluateFunctionAsync<bool>(
                "(e) => Object.is(e, 5)",
                aHandle);
            Assert.True(isFive);
        }

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should warn on nested object handles")]
        [PuppeteerFact]
        public async Task ShouldWarnOnNestedObjectHandles()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => document.body");
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(() =>
                DevToolsContext.EvaluateFunctionHandleAsync("(opts) => opts.elem.querySelector('p')", new { aHandle }));
            Assert.Contains("Are you passing a nested JSHandle?", exception.Message);
        }

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should accept object handle to unserializable value")]
        [PuppeteerFact]
        public async Task ShouldAcceptObjectHandleToUnserializableValue()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => Infinity");
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>(
                "(e) => Object.is(e, Infinity)",
                aHandle));
        }

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should use the same JS wrappers")]
        [PuppeteerFact]
        public async Task ShouldUseTheSameJSWrappers()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync(@"() => {
                globalThis.FOO = 123;
                return window;
            }");
            Assert.Equal(123, await DevToolsContext.EvaluateFunctionAsync<int>(
                "(e) => e.FOO",
                aHandle));
        }
    }
}
