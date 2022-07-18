using System.Net;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EmulateNetworkConditionsTests : DevToolsContextBaseTest
    {
        public EmulateNetworkConditionsTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateNetworkConditions", "should change navigator.connection.effectiveType")]
        [PuppeteerFact]
        public async Task ShouldChangeNavigatorConnectionEffectiveType()
        {
            var slow3G = Emulation.NetworkConditions[NetworkConditions.Slow3G];
            var fast3G = Emulation.NetworkConditions[NetworkConditions.Fast3G];

            Assert.Equal("4g", await DevToolsContext.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false));
            await DevToolsContext.EmulateNetworkConditionsAsync(fast3G);
            Assert.Equal("3g", await DevToolsContext.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false));
            await DevToolsContext.EmulateNetworkConditionsAsync(slow3G);
            Assert.Equal("2g", await DevToolsContext.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false));
            await DevToolsContext.EmulateNetworkConditionsAsync(null);
        }
    }
}
