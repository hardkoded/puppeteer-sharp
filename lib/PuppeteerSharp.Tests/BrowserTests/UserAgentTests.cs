using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
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

        [PuppeteerTest("browser.spec.ts", "Browser.userAgent", "should include WebKit")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldIncludeWebKit()
        {
            var userAgent = await Browser.GetUserAgentAsync();
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