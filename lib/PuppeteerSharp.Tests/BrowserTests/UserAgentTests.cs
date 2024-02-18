using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class UserAgentTests : PuppeteerBrowserBaseTest
    {
        public UserAgentTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("browser.spec", "Browser.userAgent", "should include WebKit")]
        public async Task ShouldIncludeWebKit()
        {
            var userAgent = await Browser.GetUserAgentAsync();
            Assert.IsNotEmpty(userAgent);

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
