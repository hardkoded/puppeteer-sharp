using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class VersionTests : PuppeteerBrowserBaseTest
    {
        [Fact]
        public async Task ShouldReturnWhetherWeAreInHeadless()
        {
            var version = await Browser.GetVersionAsync();
            Assert.NotEmpty(version);
            Assert.Equal(TestConstants.DefaultBrowserOptions().Headless, version.StartsWith("Headless"));
        }
    }
}