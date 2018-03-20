using System;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvaluateTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var result = await page.EvaluateAsync<int>("() => 7 * 3");
                Assert.Equal(21, result);
            }
        }

        [Fact]
        public async Task ShouldAwaitPromise()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var result = await page.EvaluateAsync<int>("() => Promise.resolve(8 * 7)");
                Assert.Equal(56, result);
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
                    frameEvaluation = e.Frame.EvaluateAsync<int>("() => 6 * 7");
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
                    return page.EvaluateAsync<object>("() => not.existing.object.property");
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
                dynamic result = await page.EvaluateAsync("a => a", obj);
                Assert.Equal("bar!", result.foo.ToString());
            }
        }

        [Fact]
        public async Task ShouldReturnNaN()
        {
            using (var page = await Browser.NewPageAsync())
            {
                dynamic result = await page.EvaluateAsync<dynamic>("() => NaN");
                Assert.Equal(double.NaN, result);
            }
        }

        [Fact]
        public async Task ShouldReturnNegative0()
        {
            using (var page = await Browser.NewPageAsync())
            {
                dynamic result = await page.EvaluateAsync<dynamic>("() => -0");
                Assert.Equal(-0, result);
            }
        }

        [Fact]
        public async Task ShouldReturnInfinity()
        {
            using (var page = await Browser.NewPageAsync())
            {
                dynamic result = await page.EvaluateAsync<dynamic>("() => Infinity");
                Assert.Equal(double.PositiveInfinity, result);
            }
        }

        [Fact]
        public async Task ShouldReturnNegativeInfinty()
        {
            using (var page = await Browser.NewPageAsync())
            {
                dynamic result = await page.EvaluateAsync<dynamic>("() => -Infinity");
                Assert.Equal(double.NegativeInfinity, result);
            }
        }

        [Fact]
        public async Task ShouldAcceptNullAsOneOfMultipleParameters()
        {
            using (var page = await Browser.NewPageAsync())
            {
                dynamic result = await page.EvaluateAsync<bool>("(a, b) => Object.is(a, null) && Object.is(b, 'foo')", null, "foo");
                Assert.True(result);
            }
        }

        [Fact]
        public async Task ShouldProperlySerializeNullFields()
        {
            using (var page = await Browser.NewPageAsync())
            {
                dynamic result = await page.EvaluateAsync<dynamic>("() => ({a: undefined})");
                Assert.Null(result.a);
            }
        }

        [Fact]
        public async Task ShouldAcceptAString()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var result = await page.EvaluateAsync<int>("1 + 2");
                Assert.Equal(3, result);
            }
        }

        [Fact]
        public async Task ShouldAcceptSemiColons()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var result = await page.EvaluateAsync<int>("1 + 5;");
                Assert.Equal(6, result);
            }
        }

        [Fact]
        public async Task ShouldAceptStringComments()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var result = await page.EvaluateAsync<int>("2 + 5\n// do some math!'");
                Assert.Equal(7, result);
            }
        }
    }
}
