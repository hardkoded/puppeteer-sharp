using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class PageEmulateNetworkConditionsTests : PuppeteerPageBaseTest
    {
        public PageEmulateNetworkConditionsTests() : base()
        {
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateNetworkConditions", "should support offline")]
        public async Task ShouldSupportOffline()
        {
            await Page.EmulateNetworkConditionsAsync(new NetworkConditions
            {
                Offline = true,
                Download = 0,
                Upload = 0,
                Latency = 0,
            });

            Assert.ThrowsAsync<NavigationException>(async () =>
            {
                await Page.GoToAsync(TestConstants.EmptyPage);
            });
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateNetworkConditions", "should support offline without additional parameters")]
        public async Task ShouldSupportOfflineWithoutAdditionalParameters()
        {
            await Page.EmulateNetworkConditionsAsync(new NetworkConditions
            {
                Offline = true,
            });

            var exception = Assert.ThrowsAsync<NavigationException>(async () =>
            {
                await Page.GoToAsync(TestConstants.EmptyPage);
            });
            Assert.That(exception!.Message, Does.Contain(TestConstants.IsChrome ? "ERR_INTERNET_DISCONNECTED" : "NS_ERROR_OFFLINE"));

            await Page.EmulateNetworkConditionsAsync(null);

            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.Status, Is.EqualTo(System.Net.HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateNetworkConditions", "should change navigator.connection.effectiveType")]
        public async Task ShouldChangeNavigatorConnectionEffectiveType()
        {
            var fast4G = Puppeteer.NetworkConditions[NetworkConditions.Fast4G];
            var slow4G = Puppeteer.NetworkConditions[NetworkConditions.Slow4G];
            var fast3G = Puppeteer.NetworkConditions[NetworkConditions.Fast3G];
            var slow3G = Puppeteer.NetworkConditions[NetworkConditions.Slow3G];

            Assert.That(await Page.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false), Is.EqualTo("4g"));
            await Page.EmulateNetworkConditionsAsync(fast4G);
            Assert.That(await Page.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false), Is.EqualTo("4g"));
            await Page.EmulateNetworkConditionsAsync(slow4G);
            Assert.That(await Page.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false), Is.EqualTo("3g"));
            await Page.EmulateNetworkConditionsAsync(fast3G);
            Assert.That(await Page.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false), Is.EqualTo("3g"));
            await Page.EmulateNetworkConditionsAsync(slow3G);
            Assert.That(await Page.EvaluateExpressionAsync<string>("window.navigator.connection.effectiveType").ConfigureAwait(false), Is.EqualTo("2g"));
            await Page.EmulateNetworkConditionsAsync(null);
        }
    }
}
