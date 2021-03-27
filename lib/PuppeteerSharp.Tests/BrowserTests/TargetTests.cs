using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class TargetTests : PuppeteerBrowserBaseTest
    {
        public TargetTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("browser.spec.ts", "Browser.target", "should return browser target")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public void ShouldReturnBrowserTarget()
            => Assert.Equal(TargetType.Browser, Browser.Target.Type);
    }
}