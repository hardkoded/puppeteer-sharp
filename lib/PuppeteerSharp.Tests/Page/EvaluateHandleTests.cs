using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvaluateHandleTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            var windowHandle = await Page.EvaluateExpressionHandleAsync("window");
            Assert.NotNull(windowHandle);
        }
    }
}
