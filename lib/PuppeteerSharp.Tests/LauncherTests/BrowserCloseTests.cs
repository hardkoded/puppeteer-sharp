using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserTests.Events
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class BrowserCloseTests : PuppeteerBrowserBaseTest
    {
        public BrowserCloseTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("launcher.spec.ts", "Browser.close", "should terminate network waiters")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldTerminateNetworkWaiters()
        {
            await using (var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions()))
            await using (var remote = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserWSEndpoint = browser.WebSocketEndpoint }))
            {
                var newPage = await remote.NewPageAsync();
                var requestTask = newPage.WaitForRequestAsync(TestConstants.EmptyPage);
                var responseTask = newPage.WaitForResponseAsync(TestConstants.EmptyPage);

                await browser.CloseAsync();

                var exception = await Assert.ThrowsAsync<TargetClosedException>(() => requestTask);
                Assert.Contains("Target closed", exception.Message);
                Assert.DoesNotContain("Timeout", exception.Message);

                exception = await Assert.ThrowsAsync<TargetClosedException>(() => responseTask);
                Assert.Contains("Target closed", exception.Message);
                Assert.DoesNotContain("Timeout", exception.Message);
            }
        }
    }
}
