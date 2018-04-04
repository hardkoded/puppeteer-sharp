using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Puppeteer
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ConnectTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldBeAbleToConnectMultipleTimesToTheSameBrowser()
        {
            var secondBrowser = await PuppeteerSharp.Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint
            });
            var page = await secondBrowser.NewPageAsync();
            Assert.Equal(56, await page.EvaluateFunctionAsync<int>("() => 7 * 8"));
            secondBrowser.Disconnect();

            var secondPage = await Browser.NewPageAsync();
            Assert.Equal(42, await secondPage.EvaluateFunctionAsync<int>("() => 7 * 6"));
        }

        [Fact(Skip = "Test Hangs on line30; NavigationWatcher.NavigationTask doesn't complete")]
        public async Task ShouldBeAbleToReconnectToADisconnectedBrowser()
        {
            var browserWSEndpoint = Browser.WebSocketEndpoint;
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            Browser.Disconnect();

            using (var secondBrowser = await PuppeteerSharp.Puppeteer.ConnectAsync(new ConnectOptions { BrowserWSEndpoint = browserWSEndpoint }))
            {
                var pages = await secondBrowser.GetPagesAsync();
                var restoredPage = pages.First(p => p.Url == TestConstants.ServerUrl + "/frames/nested-frames.html");
                Assert.Equal(GoldenUtils.ReconnectNestedFramesTxt, FrameUtils.DumpFrames(restoredPage.MainFrame));
                Assert.Equal(56, await restoredPage.EvaluateFunctionAsync<int>("() => 7 * 8"));
            }
        }
    }
}
