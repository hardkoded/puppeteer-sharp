using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class GetPropertiesTests : PuppeteerPageBaseTest
    {
        public GetPropertiesTests()
            : base() { }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle JSHandle.getProperties", "should work")]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync(
                @"({
              foo: 'bar'
            })"
            );
            var properties = await aHandle.GetPropertiesAsync();
            properties.TryGetValue("foo", out var foo);
            Assert.That(foo, Is.Not.Null);
            Assert.That(await foo.JsonValueAsync<string>(), Is.EqualTo("bar"));
        }

        [
            Test,
            PuppeteerTest(
                "jshandle.spec",
                "JSHandle JSHandle.getProperties",
                "should return even non-own properties"
            )
        ]
        public async Task ShouldReturnEvenNonOwnProperties()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync(
                @"() => {
              class A {
                constructor() {
                  this.a = '1';
                }
              }
              class B extends A {
                constructor() {
                  super();
                  this.b = '2';
                }
              }
              return new B();
            }"
            );
            var properties = await aHandle.GetPropertiesAsync();
            Assert.That(await properties["a"].JsonValueAsync<string>(), Is.EqualTo("1"));
            Assert.That(await properties["b"].JsonValueAsync<string>(), Is.EqualTo("2"));
        }

        [
            Test,
            PuppeteerTest(
                "jshandle.spec",
                "JSHandle JSHandle.getProperties",
                "should work with primitive values"
            )
        ]
        public async Task ShouldWorkWithPrimitiveValues()
        {
            await using var numberHandle = await Page.EvaluateFunctionHandleAsync("() => 42");
            var numberProperties = await numberHandle.GetPropertiesAsync();
            Assert.That(numberProperties, Is.Empty);

            await using var stringHandle = await Page.EvaluateFunctionHandleAsync("() => 'ab'");
            var stringProperties = await stringHandle.GetPropertiesAsync();
            Assert.That(stringProperties, Has.Count.EqualTo(2));
            await using var zero = stringProperties["0"];
            Assert.That(zero, Is.Not.Null);
            Assert.That(await zero.JsonValueAsync<string>(), Is.EqualTo("a"));
            await using var one = stringProperties["1"];
            Assert.That(one, Is.Not.Null);
            Assert.That(await one.JsonValueAsync<string>(), Is.EqualTo("b"));
        }

        [
            Test,
            PuppeteerTest(
                "JSHandle.test",
                "CdpJSHandle",
                "should throw if getProperties() is called on a disposed handle"
            )
        ]
        public async Task ShouldThrowIfGetPropertiesIsCalledOnADisposedHandle()
        {
            if (!PuppeteerTestAttribute.IsCdp)
            {
                Assert.Ignore("Upstream test targets CdpJSHandle only");
            }

            var handle = await Page.EvaluateFunctionHandleAsync("() => ({ a: 1 })");
            await handle.DisposeAsync();
            Assert.That(handle.Disposed, Is.True);

            var exception = Assert.ThrowsAsync<PuppeteerException>(() =>
                handle.GetPropertiesAsync()
            );
            Assert.That(exception.Message, Does.Contain("Attempted to use disposed CdpJSHandle."));
        }
    }
}
