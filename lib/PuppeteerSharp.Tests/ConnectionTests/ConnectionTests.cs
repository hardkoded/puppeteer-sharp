using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ConnectionsTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ConnectionTests : PuppeteerPageBaseTest
    {
        public ConnectionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldThrowNiceErrors()
        {
            var exception = await Assert.ThrowsAsync<MessageException>(async () =>
            {
                await TheSourceOfTheProblems();
            });
            Assert.Contains("TheSourceOfTheProblems", exception.StackTrace);
            Assert.Contains("ThisCommand.DoesNotExist", exception.Message);
        }

        [Fact]
        public async Task ShouldCleanCallbackList()
        {
            await Browser.GetVersionAsync();
            await Browser.GetVersionAsync();
            Assert.False(Browser.Connection.HasPendingCallbacks());

            await Page.SetJavaScriptEnabledAsync(false);
            await Page.SetJavaScriptEnabledAsync(true);
            Assert.False(Page.Client.HasPendingCallbacks());
        }

        [Fact]
        public async Task ShouldBeAbleToConnectUsingBrowserURLWithAndWithoutTrailingSlash()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = new string[] { "--remote-debugging-port=21222" };
            var originalBrowser = await Puppeteer.LaunchAsync(options);
            var browserURL = "http://127.0.0.1:21222";

            var browser1 = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserURL = browserURL });
            var page1 = await browser1.NewPageAsync();
            Assert.Equal(56, await page1.EvaluateExpressionAsync<int>("7 * 8"));
            browser1.Disconnect();

            var browser2 = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserURL = browserURL + "/" });
            var page2 = await browser2.NewPageAsync();
            Assert.Equal(56, await page2.EvaluateExpressionAsync<int>("7 * 8"));
            browser2.Disconnect();
            await originalBrowser.CloseAsync();
        }

        [Fact]
        public async Task ShouldThrowWhenUsingBothBrowserWSEndpointAndBrowserURL()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = new string[] { "--remote-debugging-port=21222" };
            var originalBrowser = await Puppeteer.LaunchAsync(options);
            var browserURL = "http://127.0.0.1:21222";

            await Assert.ThrowsAsync<PuppeteerException>(() => Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserURL = browserURL,
                BrowserWSEndpoint = originalBrowser.WebSocketEndpoint
            }));

            await originalBrowser.CloseAsync();
        }

        [Fact]
        public async Task ShouldThrowWhenTryingToConnectToNonExistingBrowser()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = new string[] { "--remote-debugging-port=21222" };
            var originalBrowser = await Puppeteer.LaunchAsync(options);
            var browserURL = "http://127.0.0.1:2122";

            await Assert.ThrowsAsync<ChromiumProcessException>(() => Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserURL = browserURL
            }));

            await originalBrowser.CloseAsync();
        }

        private async Task TheSourceOfTheProblems() => await Page.Client.SendAsync("ThisCommand.DoesNotExist");
    }
}
