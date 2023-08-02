using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class UserAgentTests : PuppeteerBrowserBaseTest
    {
        public UserAgentTests(): base()
        {
        }

        [PuppeteerTest("browser.spec.ts", "Browser.userAgent", "should include WebKit")]
        [PuppeteerFact]
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