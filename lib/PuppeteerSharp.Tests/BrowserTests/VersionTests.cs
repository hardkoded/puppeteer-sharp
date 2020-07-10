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
            string version = await Browser.GetVersionAsync();
            Assert.NotEmpty(version);

            if (TestConstants.IsChrome)
            {
                Assert.Equal(TestConstants.DefaultBrowserOptions().Headless, version.StartsWith("Headless"));
            }
            else
            {
                Assert.StartsWith("Firefox/", version);
            }
        }
    }
}