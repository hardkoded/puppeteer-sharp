using System.Threading.Tasks;
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

        [Fact]
        public void ShouldReturnTheCorrectBrowserInstance() => Assert.Same(Browser, Page.Browser);
    }
}