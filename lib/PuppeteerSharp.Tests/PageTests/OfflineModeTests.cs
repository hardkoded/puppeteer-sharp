using System.Net;
using System.Threading.Tasks;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class OfflineModeTests : PuppeteerPageBaseTest
    {
        public OfflineModeTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.setOfflineMode", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.SetOfflineModeAsync(true);
            await Assert.ThrowsAsync<NavigationException>(async () => await DevToolsContext.GoToAsync(TestConstants.EmptyPage));

            await DevToolsContext.SetOfflineModeAsync(false);
            var response = await DevToolsContext.ReloadAsync();
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("page.spec.ts", "Page.setOfflineMode", "should emulate navigator.onLine")]
        [PuppeteerFact]
        public async Task ShouldEmulateNavigatorOnLine()
        {
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("window.navigator.onLine"));

            await DevToolsContext.SetOfflineModeAsync(true);
            Assert.False(await DevToolsContext.EvaluateExpressionAsync<bool>("window.navigator.onLine"));

            await DevToolsContext.SetOfflineModeAsync(false);
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("window.navigator.onLine"));
        }
    }
}
