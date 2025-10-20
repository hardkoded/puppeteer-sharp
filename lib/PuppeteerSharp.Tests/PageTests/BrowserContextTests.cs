using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class BrowserContextTests : PuppeteerPageBaseTest
    {
        public BrowserContextTests() : base()
        {
        }

        [Test, PuppeteerTest("page.spec", "Page Page.browserContext", "should return the correct browser context instance")]
        public void ShouldReturnTheCorrectBrowserInstance() => Assert.That(Page.BrowserContext, Is.SameAs(Context));
    }
}
