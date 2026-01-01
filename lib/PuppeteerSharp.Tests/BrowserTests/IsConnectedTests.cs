using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class IsConnectedTests : PuppeteerBrowserBaseTest
    {
        [Test, PuppeteerTest("browser.spec", "Browser.isConnected", "should set the browser connected state")]
        public async Task ShouldSetTheBrowserConnectedState()
        {
            var newBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint,
                Protocol = ((Browser)Browser).Protocol,
            });
            Assert.That(newBrowser.IsConnected, Is.True);
            newBrowser.Disconnect();
            Assert.That(newBrowser.IsConnected, Is.False);
        }
    }
}
