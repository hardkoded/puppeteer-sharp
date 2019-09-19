using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ChromiumSpecificTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageWaitForFileChooserTests : PuppeteerPageBaseTest
    {
        public PageWaitForFileChooserTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldFailGracefullyWhenTryingToWorkWithFilechoosersWithinMultipleConnections()
        {
            // 1. Launch a browser and connect to all pages.
            var originalBrowser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            await originalBrowser.PagesAsync();
            // 2. Connect a remote browser and connect to first page.
            var remoteBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = originalBrowser.WebSocketEndpoint
            });
            var page = (await remoteBrowser.PagesAsync())[0];
            // 3. Make sure |page.waitForFileChooser()| does not work with multiclient.
            var ex = await Assert.ThrowsAsync<PuppeteerException>(() => page.WaitForFileChooserAsync());
            Assert.Equal("File chooser handling does not work with multiple connections to the same page", ex.Message);
            await originalBrowser.CloseAsync();
        }
    }
}
