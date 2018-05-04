using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ExposeFunctionTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            var result = await Page.EvaluateExpressionAsync<int>("compute(9, 4)");
            Assert.Equal(36, result);
        }

        [Fact]
        public async Task ShouldSurviveNavigation()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            await Page.GoToAsync(TestConstants.EmptyPage);
            var result = await Page.EvaluateExpressionAsync<int>("compute(9, 4)");
            Assert.Equal(36, result);
        }

        [Fact]
        public async Task ShouldAwaitReturnedPromise()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            var result = await Page.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }

        [Fact]
        public async Task ShouldWorkOnFrames()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            var frame = Page.Frames[1];
            var result = await frame.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }

        [Fact]
        public async Task ShouldWorkOnFramesBeforeNavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));

            var frame = Page.Frames[1];
            var result = await frame.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }
    }
}
