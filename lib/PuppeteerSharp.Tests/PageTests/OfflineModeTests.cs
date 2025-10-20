using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class OfflineModeTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.setOfflineMode", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetOfflineModeAsync(true);
            Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.EmptyPage));

            await Page.SetOfflineModeAsync(false);
            var response = await Page.ReloadAsync();
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.setOfflineMode", "should emulate navigator.onLine")]
        public async Task ShouldEmulateNavigatorOnLine()
        {
            Assert.That(await Page.EvaluateExpressionAsync<bool>("window.navigator.onLine"), Is.True);

            await Page.SetOfflineModeAsync(true);
            Assert.That(await Page.EvaluateExpressionAsync<bool>("window.navigator.onLine"), Is.False);

            await Page.SetOfflineModeAsync(false);
            Assert.That(await Page.EvaluateExpressionAsync<bool>("window.navigator.onLine"), Is.True);
        }
    }
}
