using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserTests.Events
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class DisconnectedTests : PuppeteerBrowserBaseTest
    {
        public DisconnectedTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldEmittedWhenBrowserGetsClosedDisconnectedOrUnderlyingWebsocketGetsClosed()
        {
            var originalBrowser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.LoggerFactory);
            var connectOptions = new ConnectOptions { BrowserWSEndpoint = originalBrowser.WebSocketEndpoint };
            var remoteBrowser1 = await Puppeteer.ConnectAsync(connectOptions, TestConstants.LoggerFactory);
            var remoteBrowser2 = await Puppeteer.ConnectAsync(connectOptions, TestConstants.LoggerFactory);

            var disconnectedOriginal = 0;
            var disconnectedRemote1 = 0;
            var disconnectedRemote2 = 0;
            originalBrowser.Disconnected += (sender, e) => ++disconnectedOriginal;
            remoteBrowser1.Disconnected += (sender, e) => ++disconnectedRemote1;
            remoteBrowser2.Disconnected += (sender, e) => ++disconnectedRemote2;

            var remoteBrowser2Disconnected = WaitForBrowserDisconnect(remoteBrowser2);
            remoteBrowser2.Disconnect();
            await remoteBrowser2Disconnected;

            Assert.Equal(0, disconnectedOriginal);
            Assert.Equal(0, disconnectedRemote1);
            Assert.Equal(1, disconnectedRemote2);

            var remoteBrowser1Disconnected = WaitForBrowserDisconnect(remoteBrowser1);
            var originalBrowserDisconnected = WaitForBrowserDisconnect(originalBrowser);

            await Task.WhenAll(
                originalBrowser.CloseAsync(),
                remoteBrowser1Disconnected,
                originalBrowserDisconnected
            );

            Assert.Equal(1, disconnectedOriginal);
            Assert.Equal(1, disconnectedRemote1);
            Assert.Equal(1, disconnectedRemote2);
        }
    }
}