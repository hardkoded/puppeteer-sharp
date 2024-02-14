using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class BrowserVersionTests : PuppeteerBrowserBaseTest
    {
        public BrowserVersionTests() : base()
        {
        }

        [Test, PuppeteerTest("browser.spec.ts", "Browser.version", "should return whether we are in headless")]
        [PuppeteerTimeout]
        public async Task ShouldReturnWhetherWeAreInHeadless()
        {
            var version = await Browser.GetVersionAsync();
            Assert.IsNotEmpty(version);
        }
    }
}
