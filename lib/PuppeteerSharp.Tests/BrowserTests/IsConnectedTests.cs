using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class IsConnectedTests : PuppeteerBrowserBaseTest
    {
        public IsConnectedTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
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