using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class BrowserVersionTests : PuppeteerBrowserBaseTest
    {
        [Test, Retry(2), PuppeteerTest("browser.spec", "Browser.version", "should return version")]
        public async Task ShouldReturnVersion()
        {
            var version = await Browser.GetVersionAsync();
            Assert.IsNotEmpty(version);
            StringAssert.Contains(PuppeteerTestAttribute.IsChrome ? "chrome" : "firefox", version.ToLower());
        }
    }
}
