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

        [Fact]
        public async Task ShouldRejectNavigationWhenBrowserCloses()
        {
            Server.SetRoute("/one-style.css", context => Task.Delay(10000));

            using (var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions()))
            {
                var remote = await Puppeteer.ConnectAsync(new ConnectOptions
                {
                    BrowserWSEndpoint = browser.WebSocketEndpoint
                });
                var page = await remote.NewPageAsync();
                var navigationTask = page.GoToAsync(TestConstants.ServerUrl + "/one-style.html", new NavigationOptions
                {
                    Timeout = 60000
                });
                await Server.WaitForRequest("/one-style.css");
                remote.Disconnect();
                var exception = await Assert.ThrowsAsync<NavigationException>(() => navigationTask);
                Assert.Equal("Navigation failed because browser has disconnected!", exception.Message);
            }
        }

        [Fact]
        public async Task ShouldRejectWaitForSelectorWhenBrowserCloses()
        {
            Server.SetRoute("/empty.html", context => Task.Delay(10000));

            using (var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions()))
            {
                var remote = await Puppeteer.ConnectAsync(new ConnectOptions
                {
                    BrowserWSEndpoint = browser.WebSocketEndpoint
                });
                var page = await remote.NewPageAsync();
                var watchdog = page.WaitForSelectorAsync("div", new WaitForSelectorOptions { Timeout = 60000 });
                remote.Disconnect();
                var exception = await Assert.ThrowsAsync<EvaluationFailedException>(() => watchdog);
                //Using the type instead of the message because the exception could come
                //Whether from the Connection rejecting a message from the CDPSession 
                //or from the CDPSession trying to send a message to a closed connection
                Assert.IsType<TargetClosedException>(exception.InnerException);
                Assert.Equal("Connection disposed", ((TargetClosedException)exception.InnerException).CloseReason);
            }
        }
    }
}