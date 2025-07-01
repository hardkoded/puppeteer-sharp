using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LauncherTests
{
    public class BrowserEventsDisconnectedTests : PuppeteerBrowserBaseTest
    {
        public BrowserEventsDisconnectedTests() : base()
        {
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Browser.Events.disconnected", "should be emitted when: browser gets closed, disconnected or underlying websocket gets closed")]
        public async Task ShouldEmittedWhenBrowserGetsClosedDisconnectedOrUnderlyingWebsocketGetsClosed()
        {
            var originalBrowser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.LoggerFactory);
            var connectOptions = new ConnectOptions { BrowserWSEndpoint = originalBrowser.WebSocketEndpoint };
            var remoteBrowser1 = await Puppeteer.ConnectAsync(connectOptions, TestConstants.LoggerFactory);
            var remoteBrowser2 = await Puppeteer.ConnectAsync(connectOptions, TestConstants.LoggerFactory);

            var disconnectedOriginal = 0;
            var disconnectedRemote1 = 0;
            var disconnectedRemote2 = 0;
            originalBrowser.Disconnected += (_, _) => ++disconnectedOriginal;
            remoteBrowser1.Disconnected += (_, _) => ++disconnectedRemote1;
            remoteBrowser2.Disconnected += (_, _) => ++disconnectedRemote2;

            var remoteBrowser2Disconnected = WaitForBrowserDisconnect(remoteBrowser2);
            remoteBrowser2.Disconnect();
            await remoteBrowser2Disconnected;

            Assert.That(disconnectedOriginal, Is.EqualTo(0));
            Assert.That(disconnectedRemote1, Is.EqualTo(0));
            Assert.That(disconnectedRemote2, Is.EqualTo(1));

            var remoteBrowser1Disconnected = WaitForBrowserDisconnect(remoteBrowser1);
            var originalBrowserDisconnected = WaitForBrowserDisconnect(originalBrowser);

            await Task.WhenAll(
                originalBrowser.CloseAsync(),
                remoteBrowser1Disconnected,
                originalBrowserDisconnected
            );

            Assert.That(disconnectedOriginal, Is.EqualTo(1));
            Assert.That(disconnectedRemote1, Is.EqualTo(1));
            Assert.That(disconnectedRemote2, Is.EqualTo(1));
        }
    }
}
