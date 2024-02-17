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

        [Test, PuppeteerTimeout, PuppeteerTest("page.spec", "Page Page.browserContext", "should return the correct browser context instance")]
        public void ShouldReturnTheCorrectBrowserInstance() => Assert.AreSame(Context, Page.BrowserContext);
    }
}
