using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvaluateTests : PuppeteerBaseTest
    {
        [Theory]
        [InlineData("1 + 5;", 6)] //ShouldAcceptSemiColons
        [InlineData("2 + 5\n// do some math!'", 7)] //ShouldAceptStringComments
        public async Task BasicIntExressionEvaluationTest(string script, object expected)
        {
            using (var page = await Browser.NewPageAsync())
            {
                var result = await page.EvaluateExpressionAsync<int>(script);
                Assert.Equal(expected, result);
            }
        }

        [Theory]
        [InlineData("() => 7 * 3", 21)] //ShouldWork
        [InlineData("() => Promise.resolve(8 * 7)", 56)] //ShouldAwaitPromise
        public async Task BasicIntFunctionEvaluationTest(string script, object expected)
        {
            using (var page = await Browser.NewPageAsync())
            {
                var result = await page.EvaluateFunctionAsync<int>(script);
                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public async Task ShouldWorkRightAfterFrameNavigated()
        {
            Task<int> frameEvaluation = null;

            using (var page = await Browser.NewPageAsync())
            {
                page.FrameNavigated += (sender, e) =>
                {
                    frameEvaluation = e.Frame.EvaluateFunctionAsync<int>("() => 6 * 7");
                };

                await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal(42, await frameEvaluation);
            }
        }

        [Fact]
        public async Task ShouldRejectPromiseWithExeption()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var exception = await Assert.ThrowsAsync<EvaluationFailedException>(() =>
                {
                    return page.EvaluateFunctionAsync<object>("() => not.existing.object.property");
                });

                Assert.NotNull(exception);
                Assert.True(exception.Message.Contains("not is not defined"));
            }
        }

        [Fact]
        public async Task SouldReturnComplexObjects()
        {
            using (var page = await Browser.NewPageAsync())
            {
                dynamic obj = new
                {
                    foo = "bar!"
                };
                dynamic result = await page.EvaluateFunctionAsync("a => a", obj);
                Assert.Equal("bar!", result.foo.ToString());
            }
        }

        [Theory]
        [InlineData("() => NaN", double.NaN)] //ShouldReturnNaN
        [InlineData("() => -0", -0)] //ShouldReturnNegative0
        [InlineData("() => Infinity", double.PositiveInfinity)] //ShouldReturnInfinity
        [InlineData("() => -Infinity", double.NegativeInfinity)] //ShouldReturnNegativeInfinty
        public async Task BasicEvaluationTest(string script, object expected)
        {
            using (var page = await Browser.NewPageAsync())
            {
                dynamic result = await page.EvaluateFunctionAsync(script);
                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public async Task ShouldAcceptNullAsOneOfMultipleParameters()
        {
            using (var page = await Browser.NewPageAsync())
            {
                dynamic result = await page.EvaluateFunctionAsync<bool>("(a, b) => Object.is(a, null) && Object.is(b, 'foo')", null, "foo");
                Assert.True(result);
            }
        }

        [Fact]
        public async Task ShouldProperlyIgnoreUndefinedFields()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var result = await page.EvaluateFunctionAsync<Dictionary<string, object>>("() => ({a: undefined})");
                Assert.False(result.ContainsKey("a"));
            }
        }

        [Fact]
        public async Task ShouldProperlySerializeNullFields()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var result = await page.EvaluateFunctionAsync<Dictionary<string, object>>("() => ({a: null})");
                Assert.True(result.ContainsKey("a"));
                Assert.Null(result["a"]);
            }
        }

    }
}
