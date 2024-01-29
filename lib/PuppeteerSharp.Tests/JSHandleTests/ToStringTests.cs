using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class ToStringTests : PuppeteerPageBaseTest
    {
        public ToStringTests() : base()
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.toString", "should work for primitives")]
        [PuppeteerTimeout]
        public async Task ShouldWorkForPrimitives()
        {
            var numberHandle = await Page.EvaluateExpressionHandleAsync("2");
            Assert.AreEqual("JSHandle:2", numberHandle.ToString());
            var stringHandle = await Page.EvaluateExpressionHandleAsync("'a'");
            Assert.AreEqual("JSHandle:a", stringHandle.ToString());
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.toString", "should work for complicated objects")]
        [PuppeteerTimeout]
        public async Task ShouldWorkForComplicatedObjects()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("window");
            Assert.AreEqual("JSHandle@object", aHandle.ToString());
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.toString", "should work with different subtypes")]
        [PuppeteerTimeout]
        public async Task ShouldWorkWithDifferentSubtypes()
        {
            Assert.AreEqual("JSHandle@function", (await Page.EvaluateExpressionHandleAsync("(function(){})")).ToString());
            Assert.AreEqual("JSHandle:12", (await Page.EvaluateExpressionHandleAsync("12")).ToString());
            Assert.AreEqual("JSHandle:True", (await Page.EvaluateExpressionHandleAsync("true")).ToString());
            Assert.AreEqual("JSHandle:undefined", (await Page.EvaluateExpressionHandleAsync("undefined")).ToString());
            Assert.AreEqual("JSHandle:foo", (await Page.EvaluateExpressionHandleAsync("'foo'")).ToString());
            Assert.AreEqual("JSHandle@symbol", (await Page.EvaluateExpressionHandleAsync("Symbol()")).ToString());
            Assert.AreEqual("JSHandle@map", (await Page.EvaluateExpressionHandleAsync("new Map()")).ToString());
            Assert.AreEqual("JSHandle@set", (await Page.EvaluateExpressionHandleAsync("new Set()")).ToString());
            Assert.AreEqual("JSHandle@array", (await Page.EvaluateExpressionHandleAsync("[]")).ToString());
            Assert.AreEqual("JSHandle:null", (await Page.EvaluateExpressionHandleAsync("null")).ToString());
            Assert.AreEqual("JSHandle@regexp", (await Page.EvaluateExpressionHandleAsync("/foo/")).ToString());
            Assert.AreEqual("JSHandle@node", (await Page.EvaluateExpressionHandleAsync("document.body")).ToString());
            Assert.AreEqual("JSHandle@date", (await Page.EvaluateExpressionHandleAsync("new Date()")).ToString());
            Assert.AreEqual("JSHandle@weakmap", (await Page.EvaluateExpressionHandleAsync("new WeakMap()")).ToString());
            Assert.AreEqual("JSHandle@weakset", (await Page.EvaluateExpressionHandleAsync("new WeakSet()")).ToString());
            Assert.AreEqual("JSHandle@error", (await Page.EvaluateExpressionHandleAsync("new Error()")).ToString());
            Assert.AreEqual("JSHandle@typedarray", (await Page.EvaluateExpressionHandleAsync("new Int32Array()")).ToString());
            Assert.AreEqual("JSHandle@proxy", (await Page.EvaluateExpressionHandleAsync("new Proxy({}, {})")).ToString());
        }
    }
}
