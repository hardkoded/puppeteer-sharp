using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ProcessTests : PuppeteerBrowserBaseTest
    {
        [Fact]
        public async Task ShouldReturnProcessInstance()
        {
            var process = Browser.Process;
            Assert.True(process.Id > 0);
            var browserWSEndpoint = Browser.WebSocketEndpoint;
            var remoteBrowser = await PuppeteerSharp.Puppeteer.ConnectAsync(
                new ConnectOptions { BrowserWSEndpoint = browserWSEndpoint });
            Assert.Null(remoteBrowser.Process);
            remoteBrowser.Disconnect();
        }
    }
}