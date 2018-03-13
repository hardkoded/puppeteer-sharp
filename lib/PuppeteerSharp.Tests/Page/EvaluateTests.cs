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
                var result = await page.EvaluateAsync<int>("7 * 3");
                Assert.Equal(21, result);
            }
        }

        [Fact]
        public async Task ShouldAwaitPromise()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var result = await page.EvaluateAsync<int>("Promise.resolve(8 * 7)");
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
                    frameEvaluation = e.Frame.EvaluateAsync<int>("6 * 7");
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
                    return page.EvaluateAsync<object>("not.existing.object.property");
                });

                Assert.NotNull(exception);
                Assert.True(exception.Message.Contains("not is not defined"));
            }
        }
    }
}
