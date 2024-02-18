using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class GetPropertiesTests : PuppeteerPageBaseTest
    {
        public GetPropertiesTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("jshandle.spec", "JSHandle JSHandle.getProperties", "should work")]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync(@"({
              foo: 'bar'
            })");
            var properties = await aHandle.GetPropertiesAsync();
            properties.TryGetValue("foo", out var foo);
            Assert.NotNull(foo);
            Assert.AreEqual("bar", await foo.JsonValueAsync<string>());
        }

        [Test, Retry(2), PuppeteerTest("jshandle.spec", "JSHandle JSHandle.getProperties", "should return even non-own properties")]
        public async Task ShouldReturnEvenNonOwnProperties()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync(@"() => {
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
            }");
            var properties = await aHandle.GetPropertiesAsync();
            Assert.AreEqual("1", await properties["a"].JsonValueAsync<string>());
            Assert.AreEqual("2", await properties["b"].JsonValueAsync<string>());
        }
    }
}
