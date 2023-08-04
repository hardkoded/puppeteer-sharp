using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.PageTests
{
    public class QueryObjectsTests : PuppeteerPageBaseTest
    {
        public QueryObjectsTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "ExecutionContext.queryObjects", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            // Create a custom class
            var classHandle = await Page.EvaluateFunctionHandleAsync(@"() => {
                return class CustomClass { };
            }");

            // Create an instance.
            await Page.EvaluateFunctionAsync(@"CustomClass => {
                self.customClass = new CustomClass();
            }", classHandle);

            // Validate only one has been added.
            var prototypeHandle = await Page.EvaluateFunctionHandleAsync(@"CustomClass => {
                return CustomClass.prototype;
            }", classHandle);

            var objectsHandle = await Page.QueryObjectsAsync(prototypeHandle);
            Assert.AreEqual(
                1,
                await Page.EvaluateFunctionAsync<int>(@"objects => {
                    return objects.length;
                }", objectsHandle));
      
            // Check that instances.
            Assert.True(await Page.EvaluateFunctionAsync<bool>(@"objects => {
                return objects[0] === self.customClass;
            }", objectsHandle));
        }

        [PuppeteerTest("page.spec.ts", "ExecutionContext.queryObjects", "should fail for disposed handles")]
        [PuppeteerTimeout]
        public async Task ShouldFailForDisposedHandles()
        {
            var prototypeHandle = await Page.EvaluateExpressionHandleAsync("HTMLBodyElement.prototype");
            await prototypeHandle.DisposeAsync();
            var exception = Assert.ThrowsAsync<PuppeteerException>(()
                => Page.QueryObjectsAsync(prototypeHandle));
            Assert.AreEqual("Prototype JSHandle is disposed!", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "ExecutionContext.queryObjects", "should fail primitive values as prototypes")]
        [PuppeteerTimeout]
        public async Task ShouldFailPrimitiveValuesAsPrototypes()
        {
            var prototypeHandle = await Page.EvaluateExpressionHandleAsync("42");
            var exception = Assert.ThrowsAsync<PuppeteerException>(()
                => Page.QueryObjectsAsync(prototypeHandle));
            Assert.AreEqual("Prototype JSHandle must not be referencing primitive value", exception.Message);
        }
    }
}
