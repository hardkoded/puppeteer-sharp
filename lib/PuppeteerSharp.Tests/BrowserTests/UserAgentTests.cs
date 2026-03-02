using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class UserAgentTests : PuppeteerBrowserBaseTest
    {
        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.userAgent", "should include WebKit")]
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

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.userAgent", "should include Browser engine")]
        public async Task ShouldIncludeBrowserEngine()
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

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.userAgent", "should include Browser name")]
        public async Task ShouldIncludeBrowserName()
        {
            var userAgent = await Browser.GetUserAgentAsync();
            Assert.That(userAgent, Is.Not.Empty);

            if (TestConstants.IsChrome)
            {
                Assert.That(userAgent, Does.Contain("Chrome"));
            }
            else
            {
                Assert.That(userAgent, Does.Contain("Firefox"));
            }
        }
    }
}
