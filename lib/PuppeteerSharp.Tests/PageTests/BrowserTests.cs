using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class BrowserTests : PuppeteerPageBaseTest
    {
        public BrowserTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.browser", "should return the correct browser instance")]
        public void ShouldReturnTheCorrectBrowserInstance() => Assert.That(Page.Browser, Is.SameAs(Browser));
    }
}
