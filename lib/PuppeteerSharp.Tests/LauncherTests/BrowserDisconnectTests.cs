using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LauncherTests
{
    public class BrowserDisconnectTests : PuppeteerBrowserBaseTest
    {
        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Browser.disconnect", "should reject navigation when browser closes")]
        public async Task ShouldRejectNavigationWhenBrowserCloses()
        {
            Server.SetRoute("/one-style.css", _ => Task.Delay(10000));

            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
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
            var exception = Assert.ThrowsAsync<NavigationException>(() => navigationTask);
            Assert.True(
                new[]
                {
                    "Navigating frame was detached",
                    "Protocol error(Page.navigate): Target closed. (Connection disposed)",
                }.Any(value => exception!.Message.Contains(value)));
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Browser.disconnect", "should reject waitForSelector when browser closes")]
        public async Task ShouldRejectWaitForSelectorWhenBrowserCloses()
        {
            Server.SetRoute("/empty.html", _ => Task.Delay(10000));

            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var remote = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = browser.WebSocketEndpoint
            });
            var page = await remote.NewPageAsync();
            var watchdog = page.WaitForSelectorAsync("div", new WaitForSelectorOptions { Timeout = 60000 });
            remote.Disconnect();
            var exception = Assert.ThrowsAsync<WaitTaskTimeoutException>(() => watchdog);
            Assert.True(exception!.Message.Contains("Session closed"));
        }
    }
}
