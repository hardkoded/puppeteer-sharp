using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageEmulateNetworkConditionsTests : PuppeteerPageBaseTest
    {
        public PageEmulateNetworkConditionsTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateNetworkConditions", "should change navigator.connection.effectiveType")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldChangeNavigatorConnectionEffectiveType()
        {
            var slow3G = Puppeteer.NetworkConditions[NetworkConditions.Slow3G];
            var fast3G = Puppeteer.NetworkConditions[NetworkConditions.Fast3G];

            Assert.Equal("4g", await Page.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false));
            await Page.EmulateNetworkConditionsAsync(fast3G);
            Assert.Equal("3g", await Page.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false));
            await Page.EmulateNetworkConditionsAsync(slow3G);
            Assert.Equal("2g", await Page.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false));
            await Page.EmulateNetworkConditionsAsync(null);
        }
    }
}
