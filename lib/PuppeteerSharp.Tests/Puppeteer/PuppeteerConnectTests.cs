using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Puppeteer
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class PuppeteerConnectTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldBeAbleToConnectMultipleTimesToSameBrowser()
        {
            var originalOptions = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher();

            using (var originalBrowser = await launcher.LaunchAsync(originalOptions, TestConstants.ChromiumRevision))
            {
                var options = new ConnectOptions()
                {
                    BrowserWSEndpoint = originalBrowser.WebSocketEndpoint
                };
                using (var browser = await PuppeteerSharp.Puppeteer.ConnectAsync(options))
                {
                    using (var page = await browser.NewPageAsync())
                    {
                        var response = await page.EvaluateExpressionAsync<int>("7 * 8");
                        Assert.Equal(response, 56);
                    }
                }

                using (var originalPage = await originalBrowser.NewPageAsync())
                {
                    var response = await originalPage.EvaluateExpressionAsync<int>("7 * 6");
                    Assert.Equal(response, 42);
                }
            }

            Assert.True(launcher.IsChromeClosed);
        }

        [Fact]
        public async Task ShouldBeAbleToReconnectToADisconnectedBrowser()
        {
            var originalOptions = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher();

            using (var originalBrowser = await launcher.LaunchAsync(originalOptions, TestConstants.ChromiumRevision))
            {
                var options = new ConnectOptions()
                {
                    BrowserWSEndpoint = originalBrowser.WebSocketEndpoint
                };

                var page = await originalBrowser.NewPageAsync();
                await page.GoToAsync(TestConstants.CrossProcessHttpPrefix + "/frames/nested-frames.html");

                await originalBrowser.DisconnectAsync();

                using (var browser = await PuppeteerSharp.Puppeteer.ConnectAsync(options))
                {
                    var pages = (await browser.Pages()).ToList();
                    var restoredPage = pages.FirstOrDefault(x => x.Url == TestConstants.CrossProcessHttpPrefix + "/frames/nested-frames.html");
                    Assert.NotNull(restoredPage);
                    var frameDump = FrameUtils.DumpFrames(restoredPage.MainFrame);
                    Assert.Equal(@"http://127.0.0.1:<PORT>/frames/nested-frames.html
    http://127.0.0.1:<PORT>/frames/two-frames.html
        http://127.0.0.1:<PORT>/frames/frame.html
        http://127.0.0.1:<PORT>/frames/frame.html
    http://127.0.0.1:<PORT>/frames/frame.html", frameDump);
                    var response = await restoredPage.EvaluateExpressionAsync<int>("7 * 8");
                    Assert.Equal(response, 56);
                }
            }

            Assert.True(launcher.IsChromeClosed);
        }
    }
}
