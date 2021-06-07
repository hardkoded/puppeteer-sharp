using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.LauncherTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class BrowserDisconnectTests : PuppeteerBrowserBaseTest
    {
        public BrowserDisconnectTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("launcher.spec.ts", "Browser.disconnect", "should reject navigation when browser closes")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldRejectNavigationWhenBrowserCloses()
        {
            Server.SetRoute("/one-style.css", _ => Task.Delay(10000));

            await using (var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions()))
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
                Assert.Contains("Navigation failed because browser has disconnected!", exception.Message);
            }
        }

        [PuppeteerTest("launcher.spec.ts", "Browser.disconnect", "should reject waitForSelector when browser closes")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldRejectWaitForSelectorWhenBrowserCloses()
        {
            Server.SetRoute("/empty.html", _ => Task.Delay(10000));

            await using (var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions()))
            {
                var remote = await Puppeteer.ConnectAsync(new ConnectOptions
                {
                    BrowserWSEndpoint = browser.WebSocketEndpoint
                });
                var page = await remote.NewPageAsync();
                var watchdog = page.WaitForSelectorAsync("div", new WaitForSelectorOptions { Timeout = 60000 });
                remote.Disconnect();
                var exception = await Assert.ThrowsAsync<TargetClosedException>(() => watchdog);
                Assert.Equal("Connection disposed", exception.CloseReason);
            }
        }
    }
}
