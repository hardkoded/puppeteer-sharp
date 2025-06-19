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

        [Test, PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should work")]
        public async Task ShouldWork()
            => Assert.That(await Page.EvaluateFunctionHandleAsync("() => window"), Is.Not.Null);

        [Test, PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should accept object handle as an argument")]
        public async Task ShouldAcceptObjectHandleAsAnArgument()
        {
            var navigatorHandle = await Page.EvaluateFunctionHandleAsync("() => navigator");
            var text = await Page.EvaluateFunctionAsync<string>(
                "(e) => e.userAgent",
                navigatorHandle);
            Assert.That(text, Does.Contain("Mozilla"));
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should accept object handle to primitive types")]
        public async Task ShouldAcceptObjectHandleToPrimitiveTypes()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => 5");
            var isFive = await Page.EvaluateFunctionAsync<bool>(
                "(e) => Object.is(e, 5)",
                aHandle);
            Assert.That(isFive, Is.True);
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should warn on nested object handles")]
        public async Task ShouldWarnOnNestedObjectHandles()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => document.body");
            var exception = Assert.ThrowsAsync<PuppeteerException>(() =>
                Page.EvaluateFunctionHandleAsync("(opts) => opts.elem.querySelector('p')", new { aHandle }));
            Assert.That(exception.Message, Does.Contain("Are you passing a nested JSHandle?"));
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should accept object handle to unserializable value")]
        public async Task ShouldAcceptObjectHandleToUnserializableValue()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => Infinity");
            Assert.That(await Page.EvaluateFunctionAsync<bool>(
                "(e) => Object.is(e, Infinity)",
                aHandle), Is.True);
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should use the same JS wrappers")]
        public async Task ShouldUseTheSameJSWrappers()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync(@"() => {
                globalThis.FOO = 123;
                return window;
            }");
            Assert.That(await Page.EvaluateFunctionAsync<int>(
                "(e) => e.FOO",
                aHandle), Is.EqualTo(123));
        }
    }
}
