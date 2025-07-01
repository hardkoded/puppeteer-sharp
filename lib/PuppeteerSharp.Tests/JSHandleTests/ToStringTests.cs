using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class ToStringTests : PuppeteerPageBaseTest
    {
        public ToStringTests() : base()
        {
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle JSHandle.toString", "should work for primitives")]
        public async Task ShouldWorkForPrimitives()
        {
            var numberHandle = await Page.EvaluateExpressionHandleAsync("2");
            Assert.That(numberHandle.ToString(), Is.EqualTo("JSHandle:2"));
            var stringHandle = await Page.EvaluateExpressionHandleAsync("'a'");
            Assert.That(stringHandle.ToString(), Is.EqualTo("JSHandle:a"));
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle JSHandle.toString", "should work for complicated objects")]
        public async Task ShouldWorkForComplicatedObjects()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("window");
            Assert.That(aHandle.ToString(), Is.EqualTo("JSHandle@object"));
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle JSHandle.toString", "should work with different subtypes")]
        public async Task ShouldWorkWithDifferentSubtypes()
        {
            Assert.That((await Page.EvaluateExpressionHandleAsync("(function(){})")).ToString(), Is.EqualTo("JSHandle@function"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("12")).ToString(), Is.EqualTo("JSHandle:12"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("true")).ToString(), Is.EqualTo("JSHandle:True"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("undefined")).ToString(), Is.EqualTo("JSHandle:undefined"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("'foo'")).ToString(), Is.EqualTo("JSHandle:foo"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("Symbol()")).ToString(), Is.EqualTo("JSHandle@symbol"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("new Map()")).ToString(), Is.EqualTo("JSHandle@map"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("new Set()")).ToString(), Is.EqualTo("JSHandle@set"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("[]")).ToString(), Is.EqualTo("JSHandle@array"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("null")).ToString(), Is.EqualTo("JSHandle:null"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("/foo/")).ToString(), Is.EqualTo("JSHandle@regexp"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("document.body")).ToString(), Is.EqualTo("JSHandle@node"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("new Date()")).ToString(), Is.EqualTo("JSHandle@date"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("new WeakMap()")).ToString(), Is.EqualTo("JSHandle@weakmap"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("new WeakSet()")).ToString(), Is.EqualTo("JSHandle@weakset"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("new Error()")).ToString(), Is.EqualTo("JSHandle@error"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("new Int32Array()")).ToString(), Is.EqualTo("JSHandle@typedarray"));
            Assert.That((await Page.EvaluateExpressionHandleAsync("new Proxy({}, {})")).ToString(), Is.EqualTo("JSHandle@proxy"));
        }
    }
}
