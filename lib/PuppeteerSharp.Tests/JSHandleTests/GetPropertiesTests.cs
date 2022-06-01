using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class GetPropertiesTests : DevToolsContextBaseTest
    {
        public GetPropertiesTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.getProperties", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync(@"({
              foo: 'bar'
            })");
            var properties = await aHandle.GetPropertiesAsync();
            properties.TryGetValue("foo", out var foo);
            Assert.NotNull(foo);
            Assert.Equal("bar", await foo.JsonValueAsync<string>());
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.getProperties", "should return even non-own properties")]
        [PuppeteerFact]
        public async Task ShouldReturnEvenNonOwnProperties()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync(@"() => {
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
            Assert.Equal("1", await properties["a"].JsonValueAsync<string>());
            Assert.Equal("2", await properties["b"].JsonValueAsync<string>());
        }
    }
}