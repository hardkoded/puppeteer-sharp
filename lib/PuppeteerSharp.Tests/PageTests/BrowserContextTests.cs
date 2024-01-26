using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.PageTests
{
    public class BrowserContextTests : PuppeteerPageBaseTest
    {
        public BrowserContextTests() : base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.browserContext", "should return the correct browser context instance")]
        [PuppeteerTimeout]
        public void ShouldReturnTheCorrectBrowserInstance() => Assert.AreSame(Context, Page.BrowserContext);
    }
}
