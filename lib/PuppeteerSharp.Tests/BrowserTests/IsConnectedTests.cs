using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class IsConnectedTests : PuppeteerBrowserBaseTest
    {
        public IsConnectedTests() : base()
        {
        }

        [Test, PuppeteerTest("browser.spec.ts", "Browser.isConnected", "should set the browser connected state")]
        [PuppeteerTimeout]
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
