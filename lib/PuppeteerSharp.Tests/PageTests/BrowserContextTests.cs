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

        [Fact]
        public void ShouldReturnTheCorrectBrowserInstance() => Assert.Same(Context, Page.BrowserContext);
    }
}