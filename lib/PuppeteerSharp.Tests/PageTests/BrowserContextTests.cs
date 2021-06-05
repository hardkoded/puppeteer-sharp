using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class BrowserContextTests : PuppeteerPageBaseTest
    {
        public BrowserContextTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.browserContext", "should return the correct browser context instance")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public void ShouldReturnTheCorrectBrowserInstance() => Assert.Same(Context, Page.BrowserContext);
    }
}