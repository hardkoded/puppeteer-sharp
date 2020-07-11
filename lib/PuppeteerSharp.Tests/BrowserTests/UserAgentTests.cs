using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class UserAgentTests : PuppeteerBrowserBaseTest
    {
        public UserAgentTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldIncludeWebKit()
        {
            string userAgent = await Browser.GetUserAgentAsync();
            Assert.NotEmpty(userAgent);

            if (TestConstants.IsChrome)
            {
                Assert.Contains("WebKit", userAgent);
            }
            else
            {
                Assert.Contains("Gecko", userAgent);
            }
        }
    }
}