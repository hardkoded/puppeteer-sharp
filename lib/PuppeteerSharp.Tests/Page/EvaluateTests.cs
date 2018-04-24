using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvaluateTests : PuppeteerPageBaseTest
    {
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
    }
}
