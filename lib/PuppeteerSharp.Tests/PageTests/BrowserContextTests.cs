using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class BrowserContextTests : PuppeteerPageBaseTest
    {
        public BrowserContextTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.browserContext", "should return the correct browser context instance")]
        [PuppeteerTimeout]
        public void ShouldReturnTheCorrectBrowserInstance() => Assert.Same(Context, Page.BrowserContext);
    }
}