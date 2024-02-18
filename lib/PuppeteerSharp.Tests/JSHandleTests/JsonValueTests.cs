using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class JsonValueTests : PuppeteerPageBaseTest
    {
        public JsonValueTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("jshandle.spec", "JSHandle JSHandle.jsonValue", "should work")]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("({ foo: 'bar'})");
            var json = await aHandle.JsonValueAsync();
            Assert.AreEqual(JObject.Parse("{ foo: 'bar' }"), json);
        }

        [Test, Retry(2),
         PuppeteerTest("jshandle.spec", "JSHandle JSHandle.jsonValue", "works with jsonValues that are not objects")]
        public async Task WorksWithJsonValuesThatAreNotObjects()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => ['a', 'b']");
            var json = await aHandle.JsonValueAsync<string[]>();
            Assert.AreEqual(new[] { "a", "b" }, json);
        }

        [Test, Retry(2),
         PuppeteerTest("jshandle.spec", "JSHandle JSHandle.jsonValue", "works with jsonValues that are primitives")]
        public async Task WorksWithJsonValuesThatArePrimitives()
        {
            var aHandle = await Page.EvaluateFunctionHandleAsync("() => 'foo'");
            var json = await aHandle.JsonValueAsync<string>();
            Assert.AreEqual("foo", json);
        }

        [Test, Retry(2), PuppeteerTest("jshandle.spec", "JSHandle JSHandle.jsonValue", "should work with dates")]
        public async Task ShouldWorkWithDates()
        {
            var dateHandle = await Page.EvaluateExpressionHandleAsync("new Date('2017-09-26T00:00:00.000Z')");
            var date = await dateHandle.JsonValueAsync<DateTime>();
            Assert.AreEqual(new DateTime(2017, 9, 26), date);
        }

        [Test, Retry(2), PuppeteerTest("jshandle.spec", "JSHandle JSHandle.jsonValue", "should not throw for circular objects")]
        public async Task ShouldNotThrowForCircularObjects()
        {
            var windowHandle = await Page.EvaluateFunctionHandleAsync(@"() => {
                const t = {g: 1};
                t.t = t;
                return t;
              }");
            await windowHandle.JsonValueAsync();
        }
    }
}
