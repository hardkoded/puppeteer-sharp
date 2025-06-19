using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class BrowserVersionTests : PuppeteerBrowserBaseTest
    {
        [Test, PuppeteerTest("browser.spec", "Browser.version", "should return version")]
        public async Task ShouldReturnVersion()
        {
            var version = await Browser.GetVersionAsync();
            Assert.That(version, Is.Not.Empty);
            Assert.That(version.ToLower(), Does.Contain(PuppeteerTestAttribute.IsChrome ? "chrome" : "firefox"));
        }
    }
}
