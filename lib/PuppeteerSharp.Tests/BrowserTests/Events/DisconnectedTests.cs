using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.BrowserTests.Events
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class DisconnectedTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldEmittedWhenBrowserGetsClosedDisconnectedOrUnderlyingWebsocketGetsClosed()
        {
            var originalBrowser = await PuppeteerSharp.Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.ChromiumRevision);
            var connectOptions = new ConnectOptions { BrowserWSEndpoint = originalBrowser.WebSocketEndpoint };
            var remoteBrowser1 = await PuppeteerSharp.Puppeteer.ConnectAsync(connectOptions);
            var remoteBrowser2 = await PuppeteerSharp.Puppeteer.ConnectAsync(connectOptions);

            var disconnectedOriginal = 0;
            var disconnectedRemote1 = 0;
            var disconnectedRemote2 = 0;
            originalBrowser.Disconnected += (sender, e) => ++disconnectedOriginal;
            remoteBrowser1.Disconnected += (sender, e) => ++disconnectedRemote1;
            remoteBrowser2.Disconnected += (sender, e) => ++disconnectedRemote2;

            remoteBrowser2.Disconnect();
            Assert.Equal(0, disconnectedOriginal);
            Assert.Equal(0, disconnectedRemote1);
            Assert.Equal(1, disconnectedRemote2);

            await originalBrowser.CloseAsync();
            Assert.Equal(1, disconnectedOriginal);
            Assert.Equal(1, disconnectedRemote1);
            Assert.Equal(1, disconnectedRemote2);
        }
    }
}
