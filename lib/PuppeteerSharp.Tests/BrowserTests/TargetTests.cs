using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class TargetTests : PuppeteerBrowserBaseTest
    {
        public TargetTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ShouldReturnBrowserTarget()
            => Assert.Equal(TargetType.Browser, Browser.Target.Type);
    }
}