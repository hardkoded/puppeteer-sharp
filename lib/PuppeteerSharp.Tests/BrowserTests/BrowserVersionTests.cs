using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class BrowserVersionTests : PuppeteerBrowserBaseTest
    {
        public BrowserVersionTests(): base()
        {
        }

        [PuppeteerTest("browser.spec.ts", "Browser.version", "should return whether we are in headless")]
        [PuppeteerFact]
        public async Task ShouldReturnWhetherWeAreInHeadless()
        {
            var version= await Browser.GetVersionAsync();
            Assert.NotEmpty(version);
        }
    }
}