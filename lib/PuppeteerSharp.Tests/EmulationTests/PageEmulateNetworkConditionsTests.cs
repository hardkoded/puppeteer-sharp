using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class PageEmulateNetworkConditionsTests : PuppeteerPageBaseTest
    {
        public PageEmulateNetworkConditionsTests(): base()
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateNetworkConditions", "should change navigator.connection.effectiveType")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldChangeNavigatorConnectionEffectiveType()
        {
            var slow3G = Puppeteer.NetworkConditions[NetworkConditions.Slow3G];
            var fast3G = Puppeteer.NetworkConditions[NetworkConditions.Fast3G];

            Assert.AreEqual("4g", await Page.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false));
            await Page.EmulateNetworkConditionsAsync(fast3G);
            Assert.AreEqual("3g", await Page.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false));
            await Page.EmulateNetworkConditionsAsync(slow3G);
            Assert.AreEqual("2g", await Page.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false));
            await Page.EmulateNetworkConditionsAsync(null);
        }
    }
}
