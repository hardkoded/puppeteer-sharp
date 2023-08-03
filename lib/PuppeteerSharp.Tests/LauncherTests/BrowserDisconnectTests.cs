using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LauncherTests
{
    public class BrowserDisconnectTests : PuppeteerBrowserBaseTest
    {
        public BrowserDisconnectTests(): base()
        {
        }

        [PuppeteerTest("launcher.spec.ts", "Browser.disconnect", "should reject navigation when browser closes")]
        [PuppeteerTimeout]
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
                StringAssert.Contains(
                    new[]
                    {
                        "Navigation failed because browser has disconnected! (Connection disposed)",
                        "Protocol error(Page.navigate): Target closed. (Connection disposed)",
                    },
                    value => value == exception.Message);
            }
        }

        [PuppeteerTest("launcher.spec.ts", "Browser.disconnect", "should reject waitForSelector when browser closes")]
        [PuppeteerTimeout]
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
                var exception = await Assert.ThrowsAnyAsync<WaitTaskTimeoutException>(() => watchdog);
                Assert.AreEqual("Connection disposed", (exception.InnerException as TargetClosedException).CloseReason);
            }
        }
    }
}
