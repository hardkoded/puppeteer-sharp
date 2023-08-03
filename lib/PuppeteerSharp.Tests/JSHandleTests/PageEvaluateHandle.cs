using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class PageEvaluateHandle : PuppeteerPageBaseTest
    {
        public PageEvaluateHandle(): base()
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
            => Assert.NotNull(await Page.EvaluateFunctionHandleAsync("() => window"));

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should accept object handle as an argument")]
        [PuppeteerTimeout]
        public async Task ShouldAcceptObjectHandleAsAnArgument()
        {
            var navigatorHandle = await Page.EvaluateFunctionHandleAsync("() => navigator");
            var text = await Page.EvaluateFunctionAsync<string>(
                "(e) => e.userAgent",
                navigatorHandle);
            StringAssert.Contains("Mozilla", text);
        }

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should accept object handle to primitive types")]
        [PuppeteerTimeout]
        public async Task ShouldAcceptObjectHandleToPrimitiveTypes()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => 5");
            var isFive = await Page.EvaluateFunctionAsync<bool>(
                "(e) => Object.is(e, 5)",
                aHandle);
            Assert.True(isFive);
        }

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should warn on nested object handles")]
        [PuppeteerTimeout]
        public async Task ShouldWarnOnNestedObjectHandles()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => document.body");
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(() =>
                Page.EvaluateFunctionHandleAsync("(opts) => opts.elem.querySelector('p')", new { aHandle }));
            StringAssert.Contains("Are you passing a nested JSHandle?", exception.Message);
        }

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should accept object handle to unserializable value")]
        [PuppeteerTimeout]
        public async Task ShouldAcceptObjectHandleToUnserializableValue()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => Infinity");
            Assert.True(await Page.EvaluateFunctionAsync<bool>(
                "(e) => Object.is(e, Infinity)",
                aHandle));
        }

        [PuppeteerTest("jshandle.spec.ts", "Page.evaluateHandle", "should use the same JS wrappers")]
        [PuppeteerTimeout]
        public async Task ShouldUseTheSameJSWrappers()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync(@"() => {
                globalThis.FOO = 123;
                return window;
            }");
            Assert.AreEqual(123, await Page.EvaluateFunctionAsync<int>(
                "(e) => e.FOO",
                aHandle));
        }
    }
}