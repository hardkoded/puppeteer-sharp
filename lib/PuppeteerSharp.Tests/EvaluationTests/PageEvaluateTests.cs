using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Helpers.Json;
using System.Numerics;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EvaluateTests : PuppeteerPageBaseTest
    {
        public EvaluateTests(): base()
        {
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should work")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should await promise")]
        [Theory]
        [InlineData("() => 7 * 3", 21)] //ShouldWork
        [InlineData("() => Promise.resolve(8 * 7)", 56)] //ShouldAwaitPromise
        public async Task BasicIntFunctionEvaluationTest(string script, object expected)
        {
            var result = await Page.EvaluateFunctionAsync<int>(script);
            Assert.Equal(expected, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer BigInt")]
        [PuppeteerFact]
        public async Task ShouldTransferBigInt()
        {
            var result = await Page.EvaluateFunctionAsync<BigInteger>("a => a", new BigInteger(42));
            Assert.Equal(new BigInteger(42), result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer NaN")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer -0")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer Infinity")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer -Infinity")]
        [Theory]
        [InlineData(double.NaN)] //ShouldTransferNaN
        [InlineData(-0)] //ShouldTransferNegative0
        [InlineData(double.PositiveInfinity)] //ShouldTransferInfinity
        [InlineData(double.NegativeInfinity)] //ShouldTransferNegativeInfinty
        public async Task BasicTransferTest(object transferObject)
        {
            var result = await Page.EvaluateFunctionAsync<object>("a => a", transferObject);
            Assert.Equal(transferObject, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer arrays")]
        [PuppeteerFact]
        public async Task ShouldTransferArrays()
        {
            var result = await Page.EvaluateFunctionAsync<int[]>("a => a", new int[] { 1, 2, 3 });
            Assert.Equal(new int[] { 1, 2, 3 }, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer arrays as arrays, not objects")]
        [PuppeteerFact]
        public async Task ShouldTransferArraysAsArraysNotObjects()
        {
            var result = await Page.EvaluateFunctionAsync<bool>("a => Array.isArray(a)", new int[] { 1, 2, 3 });
            Assert.True(result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should modify global environment")]
        [PuppeteerFact]
        public async Task ShouldModifyGlobalEnvironment()
        {
            await Page.EvaluateFunctionAsync("() => window.globalVar = 123");
            Assert.Equal(123, await Page.EvaluateFunctionAsync<int>("() => window.globalVar"));
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should evaluate in the page context")]
        [PuppeteerFact]
        public async Task ShouldEvaluateInThePageContext()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/global-var.html");
            Assert.Equal(123, await Page.EvaluateFunctionAsync<int>("() => window.globalVar"));
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return undefined for objects with symbols")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldReturnUndefinedForObjectsWithSymbols()
            => Assert.Null(await Page.EvaluateFunctionAsync<object>("() => [Symbol('foo4')]"));

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should work with unicode chars")]
        [PuppeteerFact]
        public async Task ShouldWorkWithUnicodeChars()
            => Assert.Equal(42, await Page.EvaluateFunctionAsync<int>("a => a['中文字符']", new Dictionary<string, int> { ["中文字符"] = 42 }));

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should throw when evaluation triggers reload")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldThrowWhenEvaluationTriggersReload()
        {
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(() =>
            {
                return Page.EvaluateFunctionAsync(@"() => {
                    location.reload();
                    return new Promise(() => {});
                }");
            });

            Assert.Contains("Protocol error", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should work right after framenavigated")]
        [PuppeteerFact]
        public async Task ShouldWorkRightAfterFrameNavigated()
        {
            Task<int> frameEvaluation = null;

            Page.FrameNavigated += (_, e) =>
            {
                frameEvaluation = e.Frame.EvaluateFunctionAsync<int>("() => 6 * 7");
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(42, await frameEvaluation);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should work from-inside an exposed function")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkFromInsideAnExposedFunction()
        {
            await Page.ExposeFunctionAsync("callController", async (int a, int b) =>
            {
                return await Page.EvaluateFunctionAsync<int>("(a, b) => a * b", a, b);
            });
            var result = await Page.EvaluateFunctionAsync<int>(@"async function() {
                return await callController(9, 3);
            }");
            Assert.Equal(27, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should reject promise with exception")]
        [PuppeteerFact]
        public async Task ShouldRejectPromiseWithExeption()
        {
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(() =>
            {
                return Page.EvaluateFunctionAsync("() => not_existing_object.property");
            });

            Assert.Contains("not_existing_object", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should support thrown strings as error messages")]
        [PuppeteerFact]
        public async Task ShouldSupportThrownStringsAsErrorMessages()
        {
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.EvaluateExpressionAsync("throw 'qwerty'"));
            Assert.Contains("qwerty", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should support thrown numbers as error messages")]
        [PuppeteerFact]
        public async Task ShouldSupportThrownNumbersAsErrorMessages()
        {
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(
                            () => Page.EvaluateExpressionAsync("throw 100500"));
            Assert.Contains("100500", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return complex objects")]
        [PuppeteerFact]
        public async Task SouldReturnComplexObjects()
        {
            dynamic obj = new
            {
                foo = "bar!"
            };
            var result = await Page.EvaluateFunctionAsync("a => a", obj);
            Assert.Equal("bar!", result.foo.ToString());
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return BigInt")]
        [PuppeteerFact]
        public async Task ShouldReturnBigInt()
        {
            var result = await Page.EvaluateFunctionAsync<object>("() => BigInt(42)");
            Assert.Equal(new BigInteger(42), result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return NaN")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return -0")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return Infinity")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return -Infinity")]
        [Theory]
        [InlineData("() => NaN", double.NaN)] //ShouldReturnNaN
        [InlineData("() => -0", -0)] //ShouldReturnNegative0
        [InlineData("() => Infinity", double.PositiveInfinity)] //ShouldReturnInfinity
        [InlineData("() => -Infinity", double.NegativeInfinity)] //ShouldReturnNegativeInfinty
        public async Task BasicEvaluationTest(string script, object expected)
        {
            var result = await Page.EvaluateFunctionAsync<object>(script);
            Assert.Equal(expected, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept \"undefined\" as one of multiple parameters")]
        [PuppeteerFact]
        public async Task ShouldAcceptNullAsOneOfMultipleParameters()
        {
            var result = await Page.EvaluateFunctionAsync<bool>(
                "(a, b) => Object.is(a, null) && Object.is(b, 'foo')",
                null,
                "foo");
            Assert.True(result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should return undefined for non-serializable objects")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldReturnNullForNonSerializableObjects()
            => Assert.Null(await Page.EvaluateFunctionAsync("() => window"));

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should fail for circular object")]
        [SkipBrowserFact(skipFirefox: true)]
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
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldBeAbleToThrowATrickyError()
        {
            var windowHandle = await Page.EvaluateFunctionHandleAsync("() => window");
            PuppeteerException exception = await Assert.ThrowsAsync<PuppeteerException>(() => windowHandle.JsonValueAsync());
            var errorText = exception.Message;

            exception = await Assert.ThrowsAsync<EvaluationFailedException>(() => Page.EvaluateFunctionAsync(@"errorText =>
            {
                throw new Error(errorText);
            }", errorText));
            Assert.Contains(errorText, exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept a string")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept a string with semi colons")]
        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept a string with comments")]
        [Theory]
        [InlineData("1 + 2;", 3)]
        [InlineData("1 + 5;", 6)]
        [InlineData("2 + 5\n// do some math!'", 7)]
        public async Task BasicIntExressionEvaluationTest(string script, object expected)
        {
            var result = await Page.EvaluateExpressionAsync<int>(script);
            Assert.Equal(expected, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept element handle as an argument")]
        [PuppeteerFact]
        public async Task ShouldAcceptElementHandleAsAnArgument()
        {
            await Page.SetContentAsync("<section>42</section>");
            var element = await Page.QuerySelectorAsync("section");
            var text = await Page.EvaluateFunctionAsync<string>("e => e.textContent", element);
            Assert.Equal("42", text);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should throw if underlying element was disposed")]
        [PuppeteerFact]
        public async Task ShouldThrowIfUnderlyingElementWasDisposed()
        {
            await Page.SetContentAsync("<section>39</section>");
            var element = await Page.QuerySelectorAsync("section");
            Assert.NotNull(element);
            await element.DisposeAsync();
            var exception = await Assert.ThrowsAnyAsync<PuppeteerException>(()
                => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.Contains("JSHandle is disposed", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should throw if elementHandles are from other frames")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldThrowIfElementHandlesAreFromOtherFrames()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var bodyHandle = await Page.FirstChildFrame().QuerySelectorAsync("body");
            var exception = await Assert.ThrowsAnyAsync<PuppeteerException>(()
                => Page.EvaluateFunctionAsync<string>("body => body.innerHTML", bodyHandle));
            Assert.Contains("JSHandles can be evaluated only in the context they were created", exception.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should simulate a user gesture")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSimulateAUserGesture()
            => Assert.True(await Page.EvaluateFunctionAsync<bool>(@"() => {
                document.body.appendChild(document.createTextNode('test'));
                document.execCommand('selectAll');
                return document.execCommand('copy');
            }"));

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should throw a nice error after a navigation")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldThrowANiceErrorAfterANavigation()
        {
            var executionContext = await Page.MainFrame.GetExecutionContextAsync();

            await Task.WhenAll(
                Page.WaitForNavigationAsync(),
                executionContext.EvaluateFunctionAsync("() => window.location.reload()")
            );
            var ex = await Assert.ThrowsAsync<EvaluationFailedException>(() =>
            {
                return executionContext.EvaluateFunctionAsync("() => null");
            });
            Assert.Contains("navigation", ex.Message);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should not throw an error when evaluation does a navigation")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNotThrowAnErrorWhenEvaluationDoesANavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            var result = await Page.EvaluateFunctionAsync<int[]>(@"() =>
            {
                window.location = '/empty.html';
                return [42];
            }");
            Assert.Equal(new[] { 42 }, result);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should transfer 100Mb of data from page to node.js")]
        [PuppeteerFact]
        public async Task ShouldTransfer100MbOfDataFromPage()
        {
            var a = await Page.EvaluateFunctionAsync<string>("() => Array(100 * 1024 * 1024 + 1).join('a')");
            Assert.Equal(100 * 1024 * 1024, a.Length);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should throw error with detailed information on exception inside promise ")]
        [PuppeteerFact]
        public async Task ShouldThrowErrorWithDetailedInformationOnExceptionInsidePromise()
        {
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(() =>
                Page.EvaluateFunctionAsync(
                    @"() => new Promise(() => {
                        throw new Error('Error in promise');
                    })"));
            Assert.Contains("Error in promise", exception.Message);
        }

        [PuppeteerFact]
        public async Task ShouldWorkWithDifferentSerializerSettings()
        {
            var result = await Page.EvaluateFunctionAsync<ComplexObjectTestClass>("() => { return { foo: 'bar' }}");
            Assert.Equal("bar", result.Foo);

            result = (await Page.EvaluateFunctionAsync<JToken>("() => { return { Foo: 'bar' }}"))
                .ToObject<ComplexObjectTestClass>(new JsonSerializerSettings());
            Assert.Equal("bar", result.Foo);

            result = await Page.EvaluateExpressionAsync<ComplexObjectTestClass>("var obj = { foo: 'bar' }; obj;");
            Assert.Equal("bar", result.Foo);

            result = (await Page.EvaluateExpressionAsync<JToken>("var obj = { Foo: 'bar' }; obj;"))
                .ToObject<ComplexObjectTestClass>(new JsonSerializerSettings());
            Assert.Equal("bar", result.Foo);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should properly serialize null fields")]
        [PuppeteerFact]
        public async Task ShouldProperlySerializeNullFields()
        {
            var result = await Page.EvaluateFunctionAsync<Dictionary<string, object>>("() => ({a: null})");
            Assert.True(result.ContainsKey("a"));
            Assert.Null(result["a"]);
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluate", "should accept element handle as an argument")]
        [PuppeteerFact]
        public async Task ShouldAcceptObjectHandleAsAnArgument()
        {
            await Page.SetContentAsync("<section>42</section>");
            var element = await Page.QuerySelectorAsync("section");
            var text = await Page.EvaluateFunctionAsync<string>("(e) => e.textContent", element);
            Assert.Equal("42", text);
        }

        [PuppeteerFact]
        public async Task ShouldWorkWithoutGenerics()
        {
            Assert.NotNull(await Page.EvaluateExpressionAsync("var obj = {}; obj;"));
            Assert.NotNull(await Page.EvaluateExpressionAsync("[]"));
            Assert.NotNull(await Page.EvaluateExpressionAsync("''"));

            var objectPopulated = await Page.EvaluateExpressionAsync("var obj = {a:1}; obj;");
            Assert.NotNull(objectPopulated);
            Assert.Equal(1, objectPopulated["a"]);

            var arrayPopulated = await Page.EvaluateExpressionAsync("[1]");
            Assert.IsType<JArray>(arrayPopulated);
            Assert.Equal(1, ((JArray)arrayPopulated)[0]);

            Assert.Equal("1", await Page.EvaluateExpressionAsync("'1'"));
            Assert.Equal(1, await Page.EvaluateExpressionAsync("1"));
            Assert.Equal(11111111, await Page.EvaluateExpressionAsync("11111111"));
            Assert.Equal(11111111111111, await Page.EvaluateExpressionAsync("11111111111111"));
            Assert.Equal(1.1, await Page.EvaluateExpressionAsync("1.1"));
        }

        public class ComplexObjectTestClass
        {
            public string Foo { get; set; }
        }
    }
}
