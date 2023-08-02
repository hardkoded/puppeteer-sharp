using Newtonsoft.Json.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class JsonValueTests : PuppeteerPageBaseTest
    {
        public JsonValueTests(): base()
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("({ foo: 'bar'})");
            var json = await aHandle.JsonValueAsync();
            Assert.Equal(JObject.Parse("{ foo: 'bar' }"), json);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "works with jsonValues that are not objects")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task WorksWithJsonValuesThatAreNotObjects()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => ['a', 'b']");
            var json = await aHandle.JsonValueAsync<string[]>();
            Assert.Equal(new[] {"a","b" }, json);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "works with jsonValues that are primitives")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task WorksWithJsonValuesThatArePrimitives()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => 'foo'");
            var json = await aHandle.JsonValueAsync<string>();
            Assert.Equal("foo", json);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should not work with dates")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldNotWorkWithDates()
        {
            var dateHandle = await Page.EvaluateExpressionHandleAsync("new Date('2017-09-26T00:00:00.000Z')");
            var json = await dateHandle.JsonValueAsync();
            Assert.Equal(JObject.Parse("{}"), json);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should throw for circular objects")]
        [PuppeteerTimeout]
        public async Task ShouldThrowForCircularObjects()
        {
            var windowHandle = await Page.EvaluateExpressionHandleAsync("window");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
                => windowHandle.JsonValueAsync());

            Assert.Contains("Could not serialize referenced object", exception.Message);
        }
    }
}