using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QueryObjectsTests
{
    public class QueryObjectsTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("queryObjects.spec", "page.queryObjects", "should work")]
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
            Assert.That(
                await Page.EvaluateFunctionAsync<int>(@"objects => {
                    return objects.length;
                }", objectsHandle), Is.EqualTo(1));

            // Check that instances.
            Assert.That(await Page.EvaluateFunctionAsync<bool>(@"objects => {
                return objects[0] === self.customClass;
            }", objectsHandle), Is.True);
        }

        [Test, PuppeteerTest("queryObjects.spec", "page.queryObjects", "should work for non-trivial page")]
        public async Task ShouldWorkForNonTrivialPage()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
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
            Assert.That(
                await Page.EvaluateFunctionAsync<int>(@"objects => {
                    return objects.length;
                }", objectsHandle), Is.EqualTo(1));

            // Check that instances.
            Assert.That(await Page.EvaluateFunctionAsync<bool>(@"objects => {
                return objects[0] === self.customClass;
            }", objectsHandle), Is.True);
        }

        [Test, PuppeteerTest("queryObjects.spec", "page.queryObjects", "should fail for disposed handles")]
        public async Task ShouldFailForDisposedHandles()
        {
            var prototypeHandle = await Page.EvaluateExpressionHandleAsync("HTMLBodyElement.prototype");
            await prototypeHandle.DisposeAsync();
            var exception = Assert.ThrowsAsync<PuppeteerException>(()
                => Page.QueryObjectsAsync(prototypeHandle));
            Assert.That(exception!.Message, Is.EqualTo("Prototype JSHandle is disposed!"));
        }

        [Test, PuppeteerTest("queryObjects.spec", "page.queryObjects", "should fail primitive values as prototypes")]
        public async Task ShouldFailPrimitiveValuesAsPrototypes()
        {
            var prototypeHandle = await Page.EvaluateExpressionHandleAsync("42");
            var exception = Assert.ThrowsAsync<PuppeteerException>(()
                => Page.QueryObjectsAsync(prototypeHandle));
            Assert.That(exception!.Message, Is.EqualTo("Prototype JSHandle must not be referencing primitive value"));
        }
    }
}
