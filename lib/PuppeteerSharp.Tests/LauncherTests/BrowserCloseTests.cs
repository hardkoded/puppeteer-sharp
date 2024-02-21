using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests.Events
{
    public class BrowserCloseTests : PuppeteerBrowserBaseTest
    {
        public BrowserCloseTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Browser.close", "should terminate network waiters")]
        public async Task ShouldTerminateNetworkWaiters()
        {
            await using (var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions()))
            await using (var remote = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserWSEndpoint = browser.WebSocketEndpoint }))
            {
                var newPage = await remote.NewPageAsync();
                var requestTask = newPage.WaitForRequestAsync(TestConstants.EmptyPage);
                var responseTask = newPage.WaitForResponseAsync(TestConstants.EmptyPage);

                await browser.CloseAsync();

                var exception = Assert.ThrowsAsync<TargetClosedException>(() => requestTask);
                StringAssert.Contains("Target closed", exception.Message);
                StringAssert.DoesNotContain("Timeout", exception.Message);

                exception = Assert.ThrowsAsync<TargetClosedException>(() => responseTask);
                StringAssert.Contains("Target closed", exception.Message);
                StringAssert.DoesNotContain("Timeout", exception.Message);
            }
        }
    }
}
