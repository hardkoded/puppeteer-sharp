using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class VersionTests : PuppeteerBrowserBaseTest
    {
        public VersionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldReturnWhetherWeAreInHeadless()
        {
            var version = await Browser.GetVersionAsync();
            Assert.NotEmpty(version);
            Assert.Equal(TestConstants.DefaultBrowserOptions().Headless, version.StartsWith("Headless"));
        }
    }
}