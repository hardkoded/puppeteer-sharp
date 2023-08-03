using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class UserAgentTests : PuppeteerBrowserBaseTest
    {
        public UserAgentTests(): base()
        {
        }

        [PuppeteerTest("browser.spec.ts", "Browser.userAgent", "should include WebKit")]
        [PuppeteerTimeout]
        public async Task ShouldIncludeWebKit()
        {
            var userAgent = await Browser.GetUserAgentAsync();
            Assert.NotEmpty(userAgent);

            if (TestConstants.IsChrome)
            {
                StringAssert.Contains("WebKit", userAgent);
            }
            else
            {
                StringAssert.Contains("Gecko", userAgent);
            }
        }
    }
}