using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.JSHandle
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class GetPropertiesTests : PuppeteerPageBaseTest
    {
        [Fact]
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

        [Fact]
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
    }
}