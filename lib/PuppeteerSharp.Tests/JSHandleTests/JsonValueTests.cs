using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class JsonValueTests : PuppeteerPageBaseTest
    {
        public JsonValueTests() : base()
        {
        }

        [Test, PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("({ foo: 'bar'})");
            var json = await aHandle.JsonValueAsync();
            Assert.AreEqual(JObject.Parse("{ foo: 'bar' }"), json);
        }

        [Test, PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "works with jsonValues that are not objects")]
        public async Task WorksWithJsonValuesThatAreNotObjects()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => ['a', 'b']");
            var json = await aHandle.JsonValueAsync<string[]>();
            Assert.AreEqual(new[] { "a", "b" }, json);
        }

        [Test, PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "works with jsonValues that are primitives")]
        public async Task WorksWithJsonValuesThatArePrimitives()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => 'foo'");
            var json = await aHandle.JsonValueAsync<string>();
            Assert.AreEqual("foo", json);
        }

        [Test, PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should not work with dates")]
        public async Task ShouldNotWorkWithDates()
        {
            var dateHandle = await Page.EvaluateExpressionHandleAsync("new Date('2017-09-26T00:00:00.000Z')");
            var json = await dateHandle.JsonValueAsync();
            Assert.AreEqual(JObject.Parse("{}"), json);
        }

        [Test, PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should throw for circular objects")]
        [PuppeteerTimeout]
        public async Task ShouldThrowForCircularObjects()
        {
            var windowHandle = await Page.EvaluateExpressionHandleAsync("window");
            var exception = Assert.ThrowsAsync<PuppeteerException>(()
                => windowHandle.JsonValueAsync());

            StringAssert.Contains("Could not serialize referenced object", exception.Message);
        }
    }
}
