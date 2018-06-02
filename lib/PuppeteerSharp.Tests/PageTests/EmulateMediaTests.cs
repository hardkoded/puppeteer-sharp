using System.Threading.Tasks;
using PuppeteerSharp.Media;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EmulateMediaTests : PuppeteerPageBaseTest
    {
        public EmulateMediaTests(ITestOutputHelper output) : base(output)
        {
        }

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
