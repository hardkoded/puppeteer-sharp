using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class BrowserVersionTests : PuppeteerBrowserBaseTest
    {
        public BrowserVersionTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("browser.spec", "Browser.version", "should return whether we are in headless")]
        public async Task ShouldReturnWhetherWeAreInHeadless()
        {
            var version = await Browser.GetVersionAsync();
            Assert.IsNotEmpty(version);
        }
    }
}
