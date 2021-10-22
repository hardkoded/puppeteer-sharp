using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class BrowserTests : PuppeteerPageBaseTest
    {
        public BrowserTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.browser", "should return the correct browser instance")]
        [PuppeteerFact]
        public void ShouldReturnTheCorrectBrowserInstance() => Assert.Same(Browser, Page.Browser);
    }
}