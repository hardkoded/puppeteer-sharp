using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.PageTests
{
    public class BrowserTests : PuppeteerPageBaseTest
    {
        public BrowserTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.browser", "should return the correct browser instance")]
        [PuppeteerTimeout]
        public void ShouldReturnTheCorrectBrowserInstance() => Assert.AreSame(Browser, Page.Browser);
    }
}