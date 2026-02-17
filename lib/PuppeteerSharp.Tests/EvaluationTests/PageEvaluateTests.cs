using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EvaluationTests
{
    public class EvaluateTests : PuppeteerPageBaseTest
    {
        [Test]
        [Retry(2)]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should work")]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should await promise")]
        [TestCase("() => 7 * 3", 21)] //ShouldWork
        [TestCase("() => Promise.resolve(8 * 7)", 56)] //ShouldAwaitPromise
        public async Task BasicIntFunctionEvaluationTest(string script, object expected)
        {
            var result = await Page.EvaluateFunctionAsync<int>(script);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should transfer BigInt")]
        public async Task ShouldTransferBigInt()
        {
            var result = await Page.EvaluateFunctionAsync<BigInteger>("a => a", new BigInteger(42));
            Assert.That(result, Is.EqualTo(new BigInteger(42)));
        }

        [Test]
        [Retry(2)]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should transfer NaN")]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should transfer -0")]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should transfer Infinity")]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should transfer -Infinity")]
        [TestCase(double.NaN)] //ShouldTransferNaN
        [TestCase(-0)] //ShouldTransferNegative0
        [TestCase(double.PositiveInfinity)] //ShouldTransferInfinity
        [TestCase(double.NegativeInfinity)] //ShouldTransferNegativeInfinty
        public async Task BasicTransferTest(object transferObject)
        {
            var result = await Page.EvaluateFunctionAsync<object>("a => a", transferObject);
            Assert.That(result, Is.EqualTo(transferObject));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should transfer arrays")]
        public async Task ShouldTransferArrays()
        {
            var result = await Page.EvaluateFunctionAsync<int[]>("a => a", new int[] { 1, 2, 3 });
            Assert.That(result, Is.EqualTo(new int[] { 1, 2, 3 }));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should transfer arrays as arrays, not objects")]
        public async Task ShouldTransferArraysAsArraysNotObjects()
        {
            var result = await Page.EvaluateFunctionAsync<bool>("a => Array.isArray(a)", new int[] { 1, 2, 3 });
            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should modify global environment")]
        public async Task ShouldModifyGlobalEnvironment()
        {
            await Page.EvaluateFunctionAsync("() => window.globalVar = 123");
            Assert.That(await Page.EvaluateFunctionAsync<int>("() => window.globalVar"), Is.EqualTo(123));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should evaluate in the page context")]
        public async Task ShouldEvaluateInThePageContext()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/global-var.html");
            Assert.That(await Page.EvaluateFunctionAsync<int>("() => window.globalVar"), Is.EqualTo(123));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should return undefined for objects with symbols")]
        public async Task ShouldReturnUndefinedForObjectsWithSymbols()
            => Assert.That(await Page.EvaluateFunctionAsync<object>("() => [Symbol('foo4')]"), Is.Null);

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should work with unicode chars")]
        public async Task ShouldWorkWithUnicodeChars()
            => Assert.That(await Page.EvaluateFunctionAsync<int>("a => a['中文字符']", new Dictionary<string, int> { ["中文字符"] = 42 }), Is.EqualTo(42));

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should throw when evaluation triggers reload")]
        public void ShouldThrowWhenEvaluationTriggersReload()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(() =>
            {
                return Page.EvaluateFunctionAsync(@"() => {
                    location.reload();
                    return new Promise(() => {});
                }");
            });

            Assert.That(exception.Message, Does.Contain("Execution context was destroyed")
                .Or.Contain("no such frame"));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should work right after framenavigated")]
        public async Task ShouldWorkRightAfterFrameNavigated()
        {
            Task<int> frameEvaluation = null;

            Page.FrameNavigated += (_, e) =>
            {
                frameEvaluation = e.Frame.EvaluateFunctionAsync<int>("() => 6 * 7");
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(await frameEvaluation, Is.EqualTo(42));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should work from-inside an exposed function")]
        public async Task ShouldWorkFromInsideAnExposedFunction()
        {
            await Page.ExposeFunctionAsync("callController", async (int a, int b) =>
            {
                return await Page.EvaluateFunctionAsync<int>("(a, b) => a * b", a, b);
            });
            var result = await Page.EvaluateFunctionAsync<int>(@"async function() {
                return await callController(9, 3);
            }");
            Assert.That(result, Is.EqualTo(27));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should reject promise with exception")]
        public void ShouldRejectPromiseWithExeption()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(() =>
            {
                return Page.EvaluateFunctionAsync("() => not_existing_object.property");
            });

            Assert.That(exception.Message, Does.Contain("not_existing_object"));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should support thrown strings as error messages")]
        public void ShouldSupportThrownStringsAsErrorMessages()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.EvaluateExpressionAsync("throw 'qwerty'"));
            Assert.That(exception.Message, Does.Contain("qwerty"));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should support thrown numbers as error messages")]
        public void ShouldSupportThrownNumbersAsErrorMessages()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(
                            () => Page.EvaluateExpressionAsync("throw 100500"));
            Assert.That(exception.Message, Does.Contain("100500"));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should return complex objects")]
        public async Task ShouldReturnComplexObjects()
        {
            dynamic obj = new
            {
                foo = "bar!"
            };
            var result = await Page.EvaluateFunctionAsync<JsonElement>("a => a", obj);
            Assert.That("bar!", Is.EqualTo(result.GetProperty("foo").GetString()));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should return BigInt")]
        public async Task ShouldReturnBigInt()
        {
            var result = await Page.EvaluateFunctionAsync<object>("() => BigInt(42)");
            Assert.That(result, Is.EqualTo(new BigInteger(42)));
        }

        [Test]
        [Retry(2)]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should return NaN")]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should return -0")]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should return Infinity")]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should return -Infinity")]
        [TestCase("() => NaN", double.NaN)] //ShouldReturnNaN
        [TestCase("() => -0", -0)] //ShouldReturnNegative0
        [TestCase("() => Infinity", double.PositiveInfinity)] //ShouldReturnInfinity
        [TestCase("() => -Infinity", double.NegativeInfinity)] //ShouldReturnNegativeInfinty
        public async Task BasicEvaluationTest(string script, object expected)
        {
            var result = await Page.EvaluateFunctionAsync<object>(script);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should accept \"undefined\" as one of multiple parameters")]
        public async Task ShouldAcceptNullAsOneOfMultipleParameters()
        {
            var result = await Page.EvaluateFunctionAsync<bool>(

                "(a, b) => Object.is(a, null) && Object.is(b, 'foo')",
                null,
                "foo");
            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should return undefined for non-serializable objects")]
        public async Task ShouldReturnNullForNonSerializableObjects()
            => Assert.That(await Page.EvaluateFunctionAsync<object>("() => window"), Is.Null);

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should fail for circular object")]
        public async Task ShouldFailForCircularObject()
        {
            var result = await Page.EvaluateFunctionAsync<object>(@"() => {
                const a = {};
                const b = {a};
                a.b = b;
                return a;
            }");

            Assert.That(result, Is.Null);
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should be able to throw a tricky error")]
        public async Task ShouldBeAbleToThrowATrickyError()
        {
            var windowHandle = await Page.EvaluateFunctionHandleAsync("() => window");
            PuppeteerException exception = Assert.ThrowsAsync<PuppeteerException>(() => windowHandle.JsonValueAsync());
            var errorText = exception.Message;

            exception = Assert.ThrowsAsync<EvaluationFailedException>(() => Page.EvaluateFunctionAsync(@"errorText =>
            {
                throw new Error(errorText);
            }", errorText));
            Assert.That(exception.Message, Does.Contain(errorText));
        }

        [Test]
        [Retry(2)]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should accept a string")]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should accept a string with semi colons")]
        [PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should accept a string with comments")]
        [TestCase("1 + 2;", 3)]
        [TestCase("1 + 5;", 6)]
        [TestCase("2 + 5\n// do some math!'", 7)]
        public async Task BasicIntExpressionEvaluationTest(string script, object expected)
        {
            var result = await Page.EvaluateExpressionAsync<int>(script);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should accept element handle as an argument")]
        public async Task ShouldAcceptElementHandleAsAnArgument()
        {
            await Page.SetContentAsync("<section>42</section>");
            var element = await Page.QuerySelectorAsync("section");
            var text = await Page.EvaluateFunctionAsync<string>("e => e.textContent", element);
            Assert.That(text, Is.EqualTo("42"));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should throw if underlying element was disposed")]
        public async Task ShouldThrowIfUnderlyingElementWasDisposed()
        {
            await Page.SetContentAsync("<section>39</section>");
            var element = await Page.QuerySelectorAsync("section");
            Assert.That(element, Is.Not.Null);
            await element.DisposeAsync();
            var exception = Assert.ThrowsAsync<PuppeteerException>(()
                => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.That(exception.Message, Does.Contain("JSHandle is disposed"));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should throw if elementHandles are from other frames")]
        public async Task ShouldThrowIfElementHandlesAreFromOtherFrames()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = await Page.FirstChildFrameAsync();
            var bodyHandle = await frame.QuerySelectorAsync("body");
            var exception = Assert.ThrowsAsync<PuppeteerException>(()
                => Page.EvaluateFunctionAsync<string>("body => body.innerHTML", bodyHandle));
            Assert.That(exception.Message, Does.Contain("JSHandles can be evaluated only in the context they were created"));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should simulate a user gesture")]
        public async Task ShouldSimulateAUserGesture()
            => Assert.That(await Page.EvaluateFunctionAsync<bool>(@"() => {
                document.body.appendChild(document.createTextNode('test'));
                document.execCommand('selectAll');
                return document.execCommand('copy');
            }"), Is.True);

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should not throw an error when evaluation does a navigation")]
        public async Task ShouldNotThrowAnErrorWhenEvaluationDoesANavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            var result = await Page.EvaluateFunctionAsync<int[]>(@"() =>
            {
                window.location = '/empty.html';
                return [42];
            }");
            Assert.That(result, Is.EqualTo(new[] { 42 }));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should transfer 100Mb of data from page to node.js")]
        public async Task ShouldTransfer100MbOfDataFromPage()
        {
            var a = await Page.EvaluateFunctionAsync<string>("() => Array(100 * 1024 * 1024 + 1).join('a')");
            Assert.That(a, Has.Length.EqualTo(100 * 1024 * 1024));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should throw error with detailed information on exception inside promise ")]
        public void ShouldThrowErrorWithDetailedInformationOnExceptionInsidePromise()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(() =>
                Page.EvaluateFunctionAsync(
                    @"() => new Promise(() => {
                        throw new Error('Error in promise');
                    })"));
            Assert.That(exception.Message, Does.Contain("Error in promise"));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should properly serialize null fields")]
        public async Task ShouldProperlySerializeNullFields()
        {
            var result = await Page.EvaluateFunctionAsync<Dictionary<string, object>>("() => ({a: null})");
            Assert.That(result.ContainsKey("a"), Is.True);
            Assert.That(result["a"], Is.Null);
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluate", "should accept element handle as an argument")]
        public async Task ShouldAcceptObjectHandleAsAnArgument()
        {
            await Page.SetContentAsync("<section>42</section>");
            var element = await Page.QuerySelectorAsync("section");
            var text = await Page.EvaluateFunctionAsync<string>("(e) => e.textContent", element);
            Assert.That(text, Is.EqualTo("42"));
        }

        public class ComplexObjectTestClass
        {
            public string Foo { get; set; }
        }
    }
}
