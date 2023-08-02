using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class ToStringTests : PuppeteerPageBaseTest
    {
        public ToStringTests(): base()
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.toString", "should work for primitives")]
        [PuppeteerFact]
        public async Task ShouldWorkForPrimitives()
        {
            var numberHandle = await Page.EvaluateExpressionHandleAsync("2");
            Assert.Equal("JSHandle:2", numberHandle.ToString());
            var stringHandle = await Page.EvaluateExpressionHandleAsync("'a'");
            Assert.Equal("JSHandle:a", stringHandle.ToString());
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.toString", "should work for complicated objects")]
        [PuppeteerFact]
        public async Task ShouldWorkForComplicatedObjects()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("window");
            Assert.Equal("JSHandle@object", aHandle.ToString());
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.toString", "should work with different subtypes")]
        [PuppeteerFact]
        public async Task ShouldWorkWithDifferentSubtypes()
        {
            Assert.Equal("JSHandle@function", (await Page.EvaluateExpressionHandleAsync("(function(){})")).ToString());
            Assert.Equal("JSHandle:12", (await Page.EvaluateExpressionHandleAsync("12")).ToString());
            Assert.Equal("JSHandle:True", (await Page.EvaluateExpressionHandleAsync("true")).ToString());
            Assert.Equal("JSHandle:undefined", (await Page.EvaluateExpressionHandleAsync("undefined")).ToString());
            Assert.Equal("JSHandle:foo", (await Page.EvaluateExpressionHandleAsync("'foo'")).ToString());
            Assert.Equal("JSHandle@symbol", (await Page.EvaluateExpressionHandleAsync("Symbol()")).ToString());
            Assert.Equal("JSHandle@map", (await Page.EvaluateExpressionHandleAsync("new Map()")).ToString());
            Assert.Equal("JSHandle@set", (await Page.EvaluateExpressionHandleAsync("new Set()")).ToString());
            Assert.Equal("JSHandle@array", (await Page.EvaluateExpressionHandleAsync("[]")).ToString());
            Assert.Equal("JSHandle:null", (await Page.EvaluateExpressionHandleAsync("null")).ToString());
            Assert.Equal("JSHandle@regexp", (await Page.EvaluateExpressionHandleAsync("/foo/")).ToString());
            Assert.Equal("JSHandle@node", (await Page.EvaluateExpressionHandleAsync("document.body")).ToString());
            Assert.Equal("JSHandle@date", (await Page.EvaluateExpressionHandleAsync("new Date()")).ToString());
            Assert.Equal("JSHandle@weakmap", (await Page.EvaluateExpressionHandleAsync("new WeakMap()")).ToString());
            Assert.Equal("JSHandle@weakset", (await Page.EvaluateExpressionHandleAsync("new WeakSet()")).ToString());
            Assert.Equal("JSHandle@error", (await Page.EvaluateExpressionHandleAsync("new Error()")).ToString());
            Assert.Equal("JSHandle@typedarray", (await Page.EvaluateExpressionHandleAsync("new Int32Array()")).ToString());
            Assert.Equal("JSHandle@proxy", (await Page.EvaluateExpressionHandleAsync("new Proxy({}, {})")).ToString());
        }
    }
}