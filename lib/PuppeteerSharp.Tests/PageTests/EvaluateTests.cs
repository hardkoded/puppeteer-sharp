using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

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
                return Page.EvaluateFunctionAsync<object>("() => not.existing.object.property");
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
            dynamic result = await Page.EvaluateFunctionAsync("a => a", obj);
            Assert.Equal("bar!", result.foo.ToString());
        }

        [Theory]
        [InlineData("() => NaN", double.NaN)] //ShouldReturnNaN
        [InlineData("() => -0", -0)] //ShouldReturnNegative0
        [InlineData("() => Infinity", double.PositiveInfinity)] //ShouldReturnInfinity
        [InlineData("() => -Infinity", double.NegativeInfinity)] //ShouldReturnNegativeInfinty
        public async Task BasicEvaluationTest(string script, object expected)
        {
            dynamic result = await Page.EvaluateFunctionAsync(script);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ShouldAcceptNullAsOneOfMultipleParameters()
        {
            bool result = await Page.EvaluateFunctionAsync<bool>("(a, b) => Object.is(a, null) && Object.is(b, 'foo')", null, "foo");
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
        public async Task ShouldFailForWindowObjectUsingEvaluateExpression()
        {
            var window = await Page.EvaluateExpressionAsync("window");
            Assert.Null(window);
        }

        [Fact]
        public async Task ShouldFailForWindowObjectUsingEvaluateFunction()
        {
            var window = await Page.EvaluateFunctionAsync("() => window");
            Assert.Null(window);
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
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
                => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.Contains("JSHandle is disposed", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowIfElementHandlesAreFromOtherFrames()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var bodyHandle = await Page.Frames[1].QuerySelectorAsync("body");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
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
                return await Page.EvaluateFunctionAsync("(a, b) => a * b", a, b);
            });
            var result = await Page.EvaluateFunctionAsync<int>(@"async function() {
                return await callController(9, 3);
            }");
            Assert.Equal(27, result);
        }
    }
}