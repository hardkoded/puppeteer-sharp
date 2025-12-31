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

        [Test, PuppeteerTest("browser.spec", "Browser.userAgent", "should include WebKit")]
        public async Task ShouldIncludeWebKit()
        {
            var userAgent = await Browser.GetUserAgentAsync();
            Assert.That(userAgent, Is.Not.Empty);

            if (TestConstants.IsChrome)
            {
                Assert.That(userAgent, Does.Contain("WebKit"));
            }
            else
            {
                Assert.That(userAgent, Does.Contain("Gecko"));
            }
        }
    }
}
