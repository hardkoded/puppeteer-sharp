using Newtonsoft.Json.Linq;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class JsonValueTests : DevToolsContextBaseTest
    {
        public JsonValueTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync("({ foo: 'bar'})");
            var json = await aHandle.JsonValueAsync();
            Assert.Equal(JObject.Parse("{ foo: 'bar' }"), json);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "works with jsonValues that are not objects")]
        [PuppeteerFact]
        public async Task WorksWithJsonValuesThatAreNotObjects()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => ['a', 'b']");
            var json = await aHandle.JsonValueAsync<string[]>();
            Assert.Equal(new[] {"a","b" }, json);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "works with jsonValues that are primitives")]
        [PuppeteerFact]
        public async Task WorksWithJsonValuesThatArePrimitives()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => 'foo'");
            var json = await aHandle.JsonValueAsync<string>();
            Assert.Equal("foo", json);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should not work with dates")]
        [PuppeteerFact]
        public async Task ShouldNotWorkWithDates()
        {
            var dateHandle = await DevToolsContext.EvaluateExpressionHandleAsync("new Date('2017-09-26T00:00:00.000Z')");
            var json = await dateHandle.JsonValueAsync();
            Assert.Equal(JObject.Parse("{}"), json);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should throw for circular objects")]
        [PuppeteerFact]
        public async Task ShouldThrowForCircularObjects()
        {
            var windowHandle = await DevToolsContext.EvaluateExpressionHandleAsync("window");
            var exception = await Assert.ThrowsAsync<MessageException>(()
                => windowHandle.JsonValueAsync());

            Assert.Contains("Object reference chain is too long", exception.Message);
        }
    }
}
