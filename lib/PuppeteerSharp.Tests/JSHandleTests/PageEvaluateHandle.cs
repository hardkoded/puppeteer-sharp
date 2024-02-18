using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class PageEvaluateHandle : PuppeteerPageBaseTest
    {
        public PageEvaluateHandle() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should work")]
        public async Task ShouldWork()
            => Assert.NotNull(await Page.EvaluateFunctionHandleAsync("() => window"));

        [Test, Retry(2), PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should accept object handle as an argument")]
        public async Task ShouldAcceptObjectHandleAsAnArgument()
        {
            var navigatorHandle = await Page.EvaluateFunctionHandleAsync("() => navigator");
            var text = await Page.EvaluateFunctionAsync<string>(
                "(e) => e.userAgent",
                navigatorHandle);
            StringAssert.Contains("Mozilla", text);
        }

        [Test, Retry(2), PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should accept object handle to primitive types")]
        public async Task ShouldAcceptObjectHandleToPrimitiveTypes()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => 5");
            var isFive = await Page.EvaluateFunctionAsync<bool>(
                "(e) => Object.is(e, 5)",
                aHandle);
            Assert.True(isFive);
        }

        [Test, Retry(2), PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should warn on nested object handles")]
        public async Task ShouldWarnOnNestedObjectHandles()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => document.body");
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(() =>
                Page.EvaluateFunctionHandleAsync("(opts) => opts.elem.querySelector('p')", new { aHandle }));
            StringAssert.Contains("Are you passing a nested JSHandle?", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should accept object handle to unserializable value")]
        public async Task ShouldAcceptObjectHandleToUnserializableValue()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => Infinity");
            Assert.True(await Page.EvaluateFunctionAsync<bool>(
                "(e) => Object.is(e, Infinity)",
                aHandle));
        }

        [Test, Retry(2), PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should use the same JS wrappers")]
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
