using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Puppeteer
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ConnectTests : PuppeteerBaseTest
    {
        [Fact(Skip = "WIP")]
        public async Task ShouldBeAbleToConnectMultipleTimesToTheSameBrowser()
        {
            using (var originalBrowser = await PuppeteerSharp.Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.ChromiumRevision))
            {
                var browser = await PuppeteerSharp.Puppeteer.ConnectAsync(new ConnectOptions
                {
                    BrowserWSEndpoint = originalBrowser.WebSocketEndpoint
                });
                var page = await browser.NewPageAsync();
                // TODO: Assert.Equal(56, await page.EvaluateAsync("() => 7 * 8"));
                browser.Disconnect();

                var secondPage = await originalBrowser.NewPageAsync();
                // TODO: Assert.Equal(42, await secondPage.EvaluateAsync("() => 7 * 6"));
            }
        }

        [Fact(Skip = "WIP")]
        public async Task ShouldBeAbleToReconnectToADisconnectedBrowser()
        {
            var originalBrowser = await PuppeteerSharp.Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.ChromiumRevision);

            var browserWSEndpoint = originalBrowser.WebSocketEndpoint;
            var page = await originalBrowser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            originalBrowser.Disconnect();


            using (var browser = await PuppeteerSharp.Puppeteer.ConnectAsync(new ConnectOptions { BrowserWSEndpoint = browserWSEndpoint }))
            {
                var pages = await browser.GetPagesAsync();
                var restoredPage = pages.First(p => p.Url == TestConstants.ServerUrl + "/frames/nested-frames.html");
                Assert.Equal("reconnect-nested-frames.txt", FrameUtils.DumpFrames(restoredPage.MainFrame));
                // TODO: Assert.Equal(56, await restoredPage.EvaluateAsync("() => 7 * 8"));
            }
        }
    }
}
