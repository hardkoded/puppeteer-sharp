using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers.Json;
using System.Numerics;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.PageTests
{
    public class EvaluateTests : PuppeteerPageBaseTest
    {
        public EvaluateTests(): base()
        {
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should work")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should await promise")]
        [TestCase("() => 7 * 3", 21)] //ShouldWork
        [TestCase("() => Promise.resolve(8 * 7)", 56)] //ShouldAwaitPromise
        public async Task BasicIntFunctionEvaluationTest(string script, object expected)
        {
            var result = await Page.EvaluateFunctionAsync<int>(script);
            Assert.AreEqual(expected, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer BigInt")]
        [PuppeteerTimeout]
        public async Task ShouldTransferBigInt()
        {
            var result = await Page.EvaluateFunctionAsync<BigInteger>("a => a", new BigInteger(42));
            Assert.AreEqual(new BigInteger(42), result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer NaN")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer -0")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer Infinity")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer -Infinity")]
        [TestCase(double.NaN)] //ShouldTransferNaN
        [TestCase(-0)] //ShouldTransferNegative0
        [TestCase(double.PositiveInfinity)] //ShouldTransferInfinity
        [TestCase(double.NegativeInfinity)] //ShouldTransferNegativeInfinty
        public async Task BasicTransferTest(object transferObject)
        {
            var result = await Page.EvaluateFunctionAsync<object>("a => a", transferObject);
            Assert.AreEqual(transferObject, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer arrays")]
        [PuppeteerTimeout]
        public async Task ShouldTransferArrays()
        {
            var result = await Page.EvaluateFunctionAsync<int[]>("a => a", new int[] { 1, 2, 3 });
            Assert.AreEqual(new int[] { 1, 2, 3 }, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer arrays as arrays, not objects")]
        [PuppeteerTimeout]
        public async Task ShouldTransferArraysAsArraysNotObjects()
        {
            var result = await Page.EvaluateFunctionAsync<bool>("a => Array.isArray(a)", new int[] { 1, 2, 3 });
            Assert.True(result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should modify global environment")]
        [PuppeteerTimeout]
        public async Task ShouldModifyGlobalEnvironment()
        {
            await Page.EvaluateFunctionAsync("() => window.globalVar = 123");
            Assert.AreEqual(123, await Page.EvaluateFunctionAsync<int>("() => window.globalVar"));
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should evaluate in the page context")]
        [PuppeteerTimeout]
        public async Task ShouldEvaluateInThePageContext()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/global-var.html");
            Assert.AreEqual(123, await Page.EvaluateFunctionAsync<int>("() => window.globalVar"));
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return undefined for objects with symbols")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldReturnUndefinedForObjectsWithSymbols()
            => Assert.Null(await Page.EvaluateFunctionAsync<object>("() => [Symbol('foo4')]"));

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should work with unicode chars")]
        [PuppeteerTimeout]
        public async Task ShouldWorkWithUnicodeChars()
            => Assert.AreEqual(42, await Page.EvaluateFunctionAsync<int>("a => a['中文字符']", new Dictionary<string, int> { ["中文字符"] = 42 }));

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should throw when evaluation triggers reload")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldThrowWhenEvaluationTriggersReload()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(() =>
            {
                return Page.EvaluateFunctionAsync(@"() => {
                    location.reload();
                    return new Promise(() => {});
                }");
            });

            StringAssert.Contains("Protocol error", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should work right after framenavigated")]
        [PuppeteerTimeout]
        public async Task ShouldWorkRightAfterFrameNavigated()
        {
            Task<int> frameEvaluation = null;

            Page.FrameNavigated += (_, e) =>
            {
                frameEvaluation = e.Frame.EvaluateFunctionAsync<int>("() => 6 * 7");
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(42, await frameEvaluation);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should work from-inside an exposed function")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkFromInsideAnExposedFunction()
        {
            await Page.ExposeFunctionAsync("callController", async (int a, int b) =>
            {
                return await Page.EvaluateFunctionAsync<int>("(a, b) => a * b", a, b);
            });
            var result = await Page.EvaluateFunctionAsync<int>(@"async function() {
                return await callController(9, 3);
            }");
            Assert.AreEqual(27, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should reject promise with exception")]
        [PuppeteerTimeout]
        public async Task ShouldRejectPromiseWithExeption()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(() =>
            {
                return Page.EvaluateFunctionAsync("() => not_existing_object.property");
            });

            StringAssert.Contains("not_existing_object", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should support thrown strings as error messages")]
        [PuppeteerTimeout]
        public async Task ShouldSupportThrownStringsAsErrorMessages()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.EvaluateExpressionAsync("throw 'qwerty'"));
            StringAssert.Contains("qwerty", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should support thrown numbers as error messages")]
        [PuppeteerTimeout]
        public async Task ShouldSupportThrownNumbersAsErrorMessages()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(
                            () => Page.EvaluateExpressionAsync("throw 100500"));
            StringAssert.Contains("100500", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return complex objects")]
        [PuppeteerTimeout]
        public async Task SouldReturnComplexObjects()
        {
            dynamic obj = new
            {
                foo = "bar!"
            };
            var result = await Page.EvaluateFunctionAsync("a => a", obj);
            Assert.AreEqual("bar!", result.foo.ToString());
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return BigInt")]
        [PuppeteerTimeout]
        public async Task ShouldReturnBigInt()
        {
            var result = await Page.EvaluateFunctionAsync<object>("() => BigInt(42)");
            Assert.AreEqual(new BigInteger(42), result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return NaN")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return -0")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return Infinity")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return -Infinity")]
        [TestCase("() => NaN", double.NaN)] //ShouldReturnNaN
        [TestCase("() => -0", -0)] //ShouldReturnNegative0
        [TestCase("() => Infinity", double.PositiveInfinity)] //ShouldReturnInfinity
        [TestCase("() => -Infinity", double.NegativeInfinity)] //ShouldReturnNegativeInfinty
        public async Task BasicEvaluationTest(string script, object expected)
        {
            var result = await Page.EvaluateFunctionAsync<object>(script);
            Assert.AreEqual(expected, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept \"undefined\" as one of multiple parameters")]
        [PuppeteerTimeout]
        public async Task ShouldAcceptNullAsOneOfMultipleParameters()
        {
            var result = await Page.EvaluateFunctionAsync<bool>(
                "(a, b) => Object.is(a, null) && Object.is(b, 'foo')",
                null,
                "foo");
            Assert.True(result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return undefined for non-serializable objects")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldReturnNullForNonSerializableObjects()
            => Assert.Null(await Page.EvaluateFunctionAsync("() => window"));

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should fail for circular object")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldFailForCircularObject()
        {
            var result = await Page.EvaluateFunctionAsync(@"() => {
                const a = {};
                const b = {a};
                a.b = b;
                return a;
            }");

            Assert.Null(result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should be able to throw a tricky error")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldBeAbleToThrowATrickyError()
        {
            var windowHandle = await Page.EvaluateFunctionHandleAsync("() => window");
            PuppeteerException exception = Assert.ThrowsAsync<PuppeteerException>(() => windowHandle.JsonValueAsync());
            var errorText = exception.Message;

            exception = Assert.ThrowsAsync<EvaluationFailedException>(() => Page.EvaluateFunctionAsync(@"errorText =>
            {
                throw new Error(errorText);
            }", errorText));
            StringAssert.Contains(errorText, exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept a string")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept a string with semi colons")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept a string with comments")]
        [TestCase("1 + 2;", 3)]
        [TestCase("1 + 5;", 6)]
        [TestCase("2 + 5\n// do some math!'", 7)]
        public async Task BasicIntExressionEvaluationTest(string script, object expected)
        {
            var result = await Page.EvaluateExpressionAsync<int>(script);
            Assert.AreEqual(expected, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept element handle as an argument")]
        [PuppeteerTimeout]
        public async Task ShouldAcceptElementHandleAsAnArgument()
        {
            await Page.SetContentAsync("<section>42</section>");
            var element = await Page.QuerySelectorAsync("section");
            var text = await Page.EvaluateFunctionAsync<string>("e => e.textContent", element);
            Assert.AreEqual("42", text);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should throw if underlying element was disposed")]
        [PuppeteerTimeout]
        public async Task ShouldThrowIfUnderlyingElementWasDisposed()
        {
            await Page.SetContentAsync("<section>39</section>");
            var element = await Page.QuerySelectorAsync("section");
            Assert.NotNull(element);
            await element.DisposeAsync();
            var exception = Assert.ThrowsAsync<PuppeteerException>(()
                => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            StringAssert.Contains("JSHandle is disposed", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should throw if elementHandles are from other frames")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldThrowIfElementHandlesAreFromOtherFrames()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var bodyHandle = await Page.FirstChildFrame().QuerySelectorAsync("body");
            var exception = Assert.ThrowsAsync<PuppeteerException>(()
                => Page.EvaluateFunctionAsync<string>("body => body.innerHTML", bodyHandle));
            StringAssert.Contains("JSHandles can be evaluated only in the context they were created", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should simulate a user gesture")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldSimulateAUserGesture()
            => Assert.True(await Page.EvaluateFunctionAsync<bool>(@"() => {
                document.body.appendChild(document.createTextNode('test'));
                document.execCommand('selectAll');
                return document.execCommand('copy');
            }"));

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should throw a nice error after a navigation")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldThrowANiceErrorAfterANavigation()
        {
            var executionContext = await Page.MainFrame.GetExecutionContextAsync();

            await Task.WhenAll(
                Page.WaitForNavigationAsync(),
                executionContext.EvaluateFunctionAsync("() => window.location.reload()")
            );
            var ex = Assert.ThrowsAsync<EvaluationFailedException>(() =>
            {
                return executionContext.EvaluateFunctionAsync("() => null");
            });
            StringAssert.Contains("navigation", ex.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should not throw an error when evaluation does a navigation")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldNotThrowAnErrorWhenEvaluationDoesANavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            var result = await Page.EvaluateFunctionAsync<int[]>(@"() =>
            {
                window.location = '/empty.html';
                return [42];
            }");
            Assert.AreEqual(new[] { 42 }, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer 100Mb of data from page to node.js")]
        [PuppeteerTimeout]
        public async Task ShouldTransfer100MbOfDataFromPage()
        {
            var a = await Page.EvaluateFunctionAsync<string>("() => Array(100 * 1024 * 1024 + 1).join('a')");
            Assert.AreEqual(100 * 1024 * 1024, a.Length);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should throw error with detailed information on exception inside promise ")]
        [PuppeteerTimeout]
        public async Task ShouldThrowErrorWithDetailedInformationOnExceptionInsidePromise()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(() =>
                Page.EvaluateFunctionAsync(
                    @"() => new Promise(() => {
                        throw new Error('Error in promise');
                    })"));
            StringAssert.Contains("Error in promise", exception.Message);
        }

        [PuppeteerTimeout]
        public async Task ShouldWorkWithDifferentSerializerSettings()
        {
            var result = await Page.EvaluateFunctionAsync<ComplexObjectTestClass>("() => { return { foo: 'bar' }}");
            Assert.AreEqual("bar", result.Foo);

            result = (await Page.EvaluateFunctionAsync<JToken>("() => { return { Foo: 'bar' }}"))
                .ToObject<ComplexObjectTestClass>(new JsonSerializerSettings());
            Assert.AreEqual("bar", result.Foo);

            result = await Page.EvaluateExpressionAsync<ComplexObjectTestClass>("var obj = { foo: 'bar' }; obj;");
            Assert.AreEqual("bar", result.Foo);

            result = (await Page.EvaluateExpressionAsync<JToken>("var obj = { Foo: 'bar' }; obj;"))
                .ToObject<ComplexObjectTestClass>(new JsonSerializerSettings());
            Assert.AreEqual("bar", result.Foo);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should properly serialize null fields")]
        [PuppeteerTimeout]
        public async Task ShouldProperlySerializeNullFields()
        {
            var result = await Page.EvaluateFunctionAsync<Dictionary<string, object>>("() => ({a: null})");
            Assert.True(result.ContainsKey("a"));
            Assert.Null(result["a"]);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept element handle as an argument")]
        [PuppeteerTimeout]
        public async Task ShouldAcceptObjectHandleAsAnArgument()
        {
            await Page.SetContentAsync("<section>42</section>");
            var element = await Page.QuerySelectorAsync("section");
            var text = await Page.EvaluateFunctionAsync<string>("(e) => e.textContent", element);
            Assert.AreEqual("42", text);
        }

        [PuppeteerTimeout]
        public async Task ShouldWorkWithoutGenerics()
        {
            Assert.NotNull(await Page.EvaluateExpressionAsync("var obj = {}; obj;"));
            Assert.NotNull(await Page.EvaluateExpressionAsync("[]"));
            Assert.NotNull(await Page.EvaluateExpressionAsync("''"));

            var objectPopulated = await Page.EvaluateExpressionAsync("var obj = {a:1}; obj;");
            Assert.NotNull(objectPopulated);
            Assert.AreEqual(1, objectPopulated["a"]);

            var arrayPopulated = await Page.EvaluateExpressionAsync("[1]");
            Assert.IsType<JArray>(arrayPopulated);
            Assert.AreEqual(1, ((JArray)arrayPopulated)[0]);

            Assert.AreEqual("1", await Page.EvaluateExpressionAsync("'1'"));
            Assert.AreEqual(1, await Page.EvaluateExpressionAsync("1"));
            Assert.AreEqual(11111111, await Page.EvaluateExpressionAsync("11111111"));
            Assert.AreEqual(11111111111111, await Page.EvaluateExpressionAsync("11111111111111"));
            Assert.AreEqual(1.1, await Page.EvaluateExpressionAsync("1.1"));
        }

        public class ComplexObjectTestClass
        {
            public string Foo { get; set; }
        }
    }
}
