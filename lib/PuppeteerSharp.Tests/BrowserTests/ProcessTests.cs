using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ProcessTests : PuppeteerBrowserBaseTest
    {
        public ProcessTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task ShouldReturnProcessInstance()
        {
            var process = Browser.Process;
            Assert.True(process.Id > 0);
            var browserWSEndpoint = Browser.WebSocketEndpoint;
            var remoteBrowser = await Puppeteer.ConnectAsync(
                new ConnectOptions { BrowserWSEndpoint = browserWSEndpoint }, TestConstants.LoggerFactory);
            Assert.Null(remoteBrowser.Process);
            remoteBrowser.Disconnect();
        }
    }
}