using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EmulateMediaTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            Assert.True(await Page.EvaluateExpressionAsync<bool>("window.matchMedia('screen').matches"));
            Assert.False(await Page.EvaluateExpressionAsync<bool>("window.matchMedia('print').matches"));
            await Page.EmulateMediaAsync(MediaType.Print);
            Assert.False(await Page.EvaluateExpressionAsync<bool>("window.matchMedia('screen').matches"));
            Assert.True(await Page.EvaluateExpressionAsync<bool>("window.matchMedia('print').matches"));
            await Page.EmulateMediaAsync(MediaType.None);
            Assert.True(await Page.EvaluateExpressionAsync<bool>("window.matchMedia('screen').matches"));
            Assert.False(await Page.EvaluateExpressionAsync<bool>("window.matchMedia('print').matches"));
        }
    }
}
