using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class GetPropertiesTests : PuppeteerPageBaseTest
    {
        public GetPropertiesTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.getProperties", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync(@"({
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
            Assert.Equal("1", await properties["a"].JsonValueAsync<string>());
            Assert.Equal("2", await properties["b"].JsonValueAsync<string>());
        }

        [PuppeteerFact]
        public async Task ShouldReturnPropertyValue()
        {
            const int expected = 2;

            var aHandle = await Page.EvaluateExpressionHandleAsync(@"({
              one: 1,
              two: 2,
              three: 3
            })");

            var actual = await aHandle.GetPropertyValueAsync<int>("two");

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldReturnElementHandlePropertyValue()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/playground.html");

            const string expected = "A text area";

            var aHandle = await Page.QuerySelectorAsync("textarea");

            var actual = await aHandle.GetPropertyValueAsync<string>("value");

            Assert.Equal(expected, actual);
        }
    }
}
