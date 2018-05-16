using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class UserAgentTests : PuppeteerBrowserBaseTest
    {
        [Fact]
        public async Task ShouldIncludeWebKit()
        {
            var userAgent = await Browser.GetUserAgentAsync();
            Assert.NotEmpty(userAgent);
            Assert.Contains("WebKit", userAgent);
        }
    }
}