using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class TargetTests : PuppeteerBrowserBaseTest
    {
        public TargetTests(): base()
        {
        }

        [PuppeteerTest("browser.spec.ts", "Browser.target", "should return browser target")]
        [PuppeteerFact]
        public void ShouldReturnBrowserTarget()
            => Assert.Equal(TargetType.Browser, Browser.Target.Type);
    }
}