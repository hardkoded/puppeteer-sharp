using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class GetPropertyTests : DevToolsContextBaseTest
    {
        public GetPropertyTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.getProperty", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync(@"({
              one: 1,
              two: 2,
              three: 3
            })");
            var twoHandle = await aHandle.GetPropertyAsync("two");
            Assert.Equal(2, await twoHandle.JsonValueAsync<int>());
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

        [PuppeteerFact]
        public async Task ShouldWorkForPropertyValue()
        {
            const int expected = 2;

            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync(@"({
              one: 1,
              two: 2,
              three: 3
            })");

            var actual = await aHandle.GetPropertyValueAsync<int>("two");

            Assert.Equal(expected, actual);
        }
    }
}
