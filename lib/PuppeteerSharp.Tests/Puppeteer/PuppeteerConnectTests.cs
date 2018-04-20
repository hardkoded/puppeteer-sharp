﻿using System.Linq;
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
            var options = new ConnectOptions()
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint
            };
            var browser = await PuppeteerSharp.Puppeteer.ConnectAsync(options);
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.EvaluateExpressionAsync<int>("7 * 8");
                Assert.Equal(56, response);
            }

            using (var originalPage = await Browser.NewPageAsync())
            {
                var response = await originalPage.EvaluateExpressionAsync<int>("7 * 6");
                Assert.Equal(42, response);
            }
        }

        [Fact]
        public async Task ShouldBeAbleToReconnectToADisconnectedBrowser()
        {
            var options = new ConnectOptions()
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint
            };

            var url = TestConstants.CrossProcessHttpPrefix + "/frames/nested-frames.html";
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(url);

            Browser.Disconnect();

            using (var browser = await PuppeteerSharp.Puppeteer.ConnectAsync(options))
            {
                var pages = (await browser.Pages()).ToList();
                var restoredPage = pages.FirstOrDefault(x => x.Url == url);
                Assert.NotNull(restoredPage);
                var frameDump = FrameUtils.DumpFrames(restoredPage.MainFrame);
                Assert.Equal(@"http://127.0.0.1:<PORT>/frames/nested-frames.html
    http://127.0.0.1:<PORT>/frames/two-frames.html
        http://127.0.0.1:<PORT>/frames/frame.html
        http://127.0.0.1:<PORT>/frames/frame.html
    http://127.0.0.1:<PORT>/frames/frame.html", frameDump);
                var response = await restoredPage.EvaluateExpressionAsync<int>("7 * 8");
                Assert.Equal(56, response);
            }
        }
    }
}
