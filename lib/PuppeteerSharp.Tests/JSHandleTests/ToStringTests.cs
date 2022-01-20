using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ToStringTests : PuppeteerPageBaseTest
    {
        public ToStringTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.toString", "should work for primitives")]
        [PuppeteerFact]
        public async Task ShouldWorkForPrimitives()
        {
            var numberHandle = await DevToolsContext.EvaluateExpressionHandleAsync("2");
            Assert.Equal("JSHandle:2", numberHandle.ToString());
            var stringHandle = await DevToolsContext.EvaluateExpressionHandleAsync("'a'");
            Assert.Equal("JSHandle:a", stringHandle.ToString());
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.toString", "should work for complicated objects")]
        [PuppeteerFact]
        public async Task ShouldWorkForComplicatedObjects()
        {
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync("window");
            Assert.Equal("JSHandle@object", aHandle.ToString());
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.toString", "should work with different subtypes")]
        [PuppeteerFact]
        public async Task ShouldWorkWithDifferentSubtypes()
        {
            Assert.Equal("JSHandle@function", (await DevToolsContext.EvaluateExpressionHandleAsync("(function(){})")).ToString());
            Assert.Equal("JSHandle:12", (await DevToolsContext.EvaluateExpressionHandleAsync("12")).ToString());
            Assert.Equal("JSHandle:True", (await DevToolsContext.EvaluateExpressionHandleAsync("true")).ToString());
            Assert.Equal("JSHandle:undefined", (await DevToolsContext.EvaluateExpressionHandleAsync("undefined")).ToString());
            Assert.Equal("JSHandle:foo", (await DevToolsContext.EvaluateExpressionHandleAsync("'foo'")).ToString());
            Assert.Equal("JSHandle@symbol", (await DevToolsContext.EvaluateExpressionHandleAsync("Symbol()")).ToString());
            Assert.Equal("JSHandle@map", (await DevToolsContext.EvaluateExpressionHandleAsync("new Map()")).ToString());
            Assert.Equal("JSHandle@set", (await DevToolsContext.EvaluateExpressionHandleAsync("new Set()")).ToString());
            Assert.Equal("JSHandle@array", (await DevToolsContext.EvaluateExpressionHandleAsync("[]")).ToString());
            Assert.Equal("JSHandle:null", (await DevToolsContext.EvaluateExpressionHandleAsync("null")).ToString());
            Assert.Equal("JSHandle@regexp", (await DevToolsContext.EvaluateExpressionHandleAsync("/foo/")).ToString());
            Assert.Equal("JSHandle@node", (await DevToolsContext.EvaluateExpressionHandleAsync("document.body")).ToString());
            Assert.Equal("JSHandle@date", (await DevToolsContext.EvaluateExpressionHandleAsync("new Date()")).ToString());
            Assert.Equal("JSHandle@weakmap", (await DevToolsContext.EvaluateExpressionHandleAsync("new WeakMap()")).ToString());
            Assert.Equal("JSHandle@weakset", (await DevToolsContext.EvaluateExpressionHandleAsync("new WeakSet()")).ToString());
            Assert.Equal("JSHandle@error", (await DevToolsContext.EvaluateExpressionHandleAsync("new Error()")).ToString());
            Assert.Equal("JSHandle@typedarray", (await DevToolsContext.EvaluateExpressionHandleAsync("new Int32Array()")).ToString());
            Assert.Equal("JSHandle@proxy", (await DevToolsContext.EvaluateExpressionHandleAsync("new Proxy({}, {})")).ToString());
        }
    }
}