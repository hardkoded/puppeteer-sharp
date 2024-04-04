using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class IsConnectedTests : PuppeteerBrowserBaseTest
    {
        public IsConnectedTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("browser.spec", "Browser.isConnected", "should set the browser connected state")]
        public async Task ShouldSetTheBrowserConnectedState()
        {
            var newBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint
            });
            Assert.True(newBrowser.IsConnected);
            newBrowser.Disconnect();
            Assert.False(newBrowser.IsConnected);
        }
    }
}
