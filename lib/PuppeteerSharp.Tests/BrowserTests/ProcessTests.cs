using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class ProcessTests : PuppeteerBrowserBaseTest
    {
        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.process", "should return child_process instance")]
        public void ShouldReturnProcessInstance()
        {
            var process = Browser.Process;
            Assert.That(process.Id, Is.GreaterThan(0));
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.process", "should not return child_process for remote browser")]
        public async Task ShouldNotReturnChildProcessForRemoteBrowser()
        {
            var browserWSEndpoint = Browser.WebSocketEndpoint;
            var remoteBrowser = await Puppeteer.ConnectAsync(
                new ConnectOptions
                {
                    BrowserWSEndpoint = browserWSEndpoint,
                    Protocol = ((Browser)Browser).Protocol,
                },
                TestConstants.LoggerFactory);
            Assert.That(remoteBrowser.Process, Is.Null);

            remoteBrowser.Disconnect();
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.process", "should keep connected after the last page is closed")]
        public async Task ShouldKeepConnectedAfterTheLastPageIsClosed()
        {
            await using var browser = await Puppeteer.LaunchAsync(
                TestConstants.DefaultBrowserOptions(),
                TestConstants.LoggerFactory);

            var pages = await browser.PagesAsync();
            await Task.WhenAll(pages.Select(page => page.CloseAsync()));

            // Verify the browser is still connected.
            Assert.That(browser.IsConnected, Is.True);

            // Verify the browser can open a new page.
            await browser.NewPageAsync();
        }
    }
}
