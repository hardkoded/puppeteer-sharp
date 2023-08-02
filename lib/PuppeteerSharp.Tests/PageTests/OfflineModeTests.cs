using System.Net;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class OfflineModeTests : PuppeteerPageBaseTest
    {
        public OfflineModeTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.setOfflineMode", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            await Page.SetOfflineModeAsync(true);
            await Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.EmptyPage));

            await Page.SetOfflineModeAsync(false);
            var response = await Page.ReloadAsync();
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("page.spec.ts", "Page.setOfflineMode", "should emulate navigator.onLine")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldEmulateNavigatorOnLine()
        {
            Assert.True(await Page.EvaluateExpressionAsync<bool>("window.navigator.onLine"));

            await Page.SetOfflineModeAsync(true);
            Assert.False(await Page.EvaluateExpressionAsync<bool>("window.navigator.onLine"));

            await Page.SetOfflineModeAsync(false);
            Assert.True(await Page.EvaluateExpressionAsync<bool>("window.navigator.onLine"));
        }
    }
}
