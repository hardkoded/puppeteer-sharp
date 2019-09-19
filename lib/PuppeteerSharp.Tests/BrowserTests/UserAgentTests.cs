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
            var userAgent = await Browser.GetUserAgentAsync();
            Assert.NotEmpty(userAgent);
            Assert.Contains("WebKit", userAgent);
        }
    }
}