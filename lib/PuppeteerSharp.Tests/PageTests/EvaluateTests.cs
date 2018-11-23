using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvaluateTests : PuppeteerPageBaseTest
    {
        public EvaluateTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("1 + 5;", 6)] //ShouldAcceptSemiColons
        [InlineData("2 + 5\n// do some math!'", 7)] //ShouldAceptStringComments
        public async Task BasicIntExressionEvaluationTest(string script, object expected)
        {
            var result = await Page.EvaluateExpressionAsync<int>(script);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("() => 7 * 3", 21)] //ShouldWork
        [InlineData("() => Promise.resolve(8 * 7)", 56)] //ShouldAwaitPromise
        public async Task BasicIntFunctionEvaluationTest(string script, object expected)
        {
            var result = await Page.EvaluateFunctionAsync<int>(script);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ShouldWorkRightAfterFrameNavigated()
        {
            Task<int> frameEvaluation = null;

            Page.FrameNavigated += (sender, e) =>
            {
                frameEvaluation = e.Frame.EvaluateFunctionAsync<int>("() => 6 * 7");
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(42, await frameEvaluation);
        }

        [Fact]
        public async Task ShouldRejectPromiseWithExeption()
        {
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(() =>
            {
                return Page.EvaluateFunctionAsync("() => not.existing.object.property");
            });

            Assert.Contains("not is not defined", exception.Message);
        }

        [Fact]
        public async Task SouldReturnComplexObjects()
        {
            dynamic obj = new
            {
                foo = "bar!"
            };
            var result = await Page.EvaluateFunctionAsync("a => a", obj);
            Assert.Equal("bar!", result.foo.ToString());
        }

        [Fact]
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

        [Fact]
        public async Task ShouldAcceptNullAsOneOfMultipleParameters()
        {
            var result = await Page.EvaluateFunctionAsync<bool>(
                "(a, b) => Object.is(a, null) && Object.is(b, 'foo')",
                null,
                "foo");
            Assert.True(result);
        }

        [Fact]
        public async Task ShouldProperlyIgnoreUndefinedFields()
        {
            var result = await Page.EvaluateFunctionAsync<Dictionary<string, object>>("() => ({a: undefined})");
            Assert.Empty(result);
        }

        [Fact]
        public async Task ShouldProperlySerializeNullFields()
        {
            var result = await Page.EvaluateFunctionAsync<Dictionary<string, object>>("() => ({a: null})");
            Assert.True(result.ContainsKey("a"));
            Assert.Null(result["a"]);
        }

        [Fact]
        public async Task ShouldReturnNullForNonSerializableObjects()
        {
            Assert.Null(await Page.EvaluateFunctionAsync("() => window"));
            Assert.Null(await Page.EvaluateFunctionAsync("() => [Symbol('foo4')]"));
        }

        [Fact]
        public async Task ShouldAcceptElementHandleAsAnArgument()
        {
            await Page.SetContentAsync("<section>42</section>");
            var element = await Page.QuerySelectorAsync("section");
            var text = await Page.EvaluateFunctionAsync<string>("e => e.textContent", element);
            Assert.Equal("42", text);
        }

        [Fact]
        public async Task ShouldThrowIfUnderlyingElementWasDisposed()
        {
            await Page.SetContentAsync("<section>39</section>");
            var element = await Page.QuerySelectorAsync("section");
            Assert.NotNull(element);
            await element.DisposeAsync();
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(()
                => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.Contains("JSHandle is disposed", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowIfElementHandlesAreFromOtherFrames()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var bodyHandle = await Page.FirstChildFrame().QuerySelectorAsync("body");
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(()
                => Page.EvaluateFunctionAsync<string>("body => body.innerHTML", bodyHandle));
            Assert.Contains("JSHandles can be evaluated only in the context they were created", exception.Message);
        }

        [Fact]
        public async Task ShouldAcceptObjectHandleAsAnArgument()
        {
            var navigatorHandle = await Page.EvaluateExpressionHandleAsync("navigator");
            var text = await Page.EvaluateFunctionAsync<string>("e => e.userAgent", navigatorHandle);
            Assert.Contains("Mozilla", text);
        }

        [Fact]
        public async Task ShouldAcceptObjectHandleToPrimitiveTypes()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("5");
            var isFive = await Page.EvaluateFunctionAsync<bool>("e => Object.is(e, 5)", aHandle);
            Assert.True(isFive);
        }

        [Fact]
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

        [Fact]
        public async Task ShouldSupportThrownStringsAsErrorMessages()
        {
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.EvaluateExpressionAsync("throw 'qwerty'"));
            Assert.Contains("qwerty", exception.Message);
        }

        [Fact]
        public async Task ShouldSupportThrownNumbersAsErrorMessages()
        {
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(
                            () => Page.EvaluateExpressionAsync("throw 100500"));
            Assert.Contains("100500", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowWhenEvaluationTriggersReload()
        {
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(() =>
            {
                return Page.EvaluateFunctionAsync(@"() => {
                    location.reload();
                    return new Promise(resolve => {
                        setTimeout(() => resolve(1), 0);
                    });
                }");
            });

            Assert.Contains("Protocol error", exception.Message);
        }

        [Fact]
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

        [Fact]
        public Task ShouldSimulateAUserGesture()
            => Page.EvaluateExpressionAsync(@"(
            function playAudio()
            {
                const audio = document.createElement('audio');
                audio.src = 'data:audio/wav;base64,UklGRiQAAABXQVZFZm10IBAAAAABAAEARKwAAIhYAQACABAAZGF0YQAAAAA=';
                // This returns a promise which throws if it was not triggered by a user gesture.
                return audio.play();
            })()");

        [Fact]
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

        [Fact]
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