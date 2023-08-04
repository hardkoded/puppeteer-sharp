using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.PageTests
{
    public class BrowserContextTests : PuppeteerPageBaseTest
    {
        public BrowserContextTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.browserContext", "should return the correct browser context instance")]
        [PuppeteerTimeout]
        public void ShouldReturnTheCorrectBrowserInstance() => Assert.AreSame(Context, Page.BrowserContext);
    }
}