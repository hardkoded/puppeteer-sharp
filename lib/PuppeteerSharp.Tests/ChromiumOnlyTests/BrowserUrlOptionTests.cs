using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ChromiumSpecificTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class BrowserUrlOptionTests : PuppeteerPageBaseTest
    {
        public BrowserUrlOptionTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("chromiumonly.spec.ts", "Puppeteer.launch |browserURL| option", "should be able to connect using browserUrl, with and without trailing slash")]
        [SkipBrowserFact(skipFirefox: true)]
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

        [PuppeteerTest("chromiumonly.spec.ts", "Puppeteer.launch |browserURL| option", "should throw when using both browserWSEndpoint and browserURL")]
        [SkipBrowserFact(skipFirefox: true)]
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

        [PuppeteerTest("chromiumonly.spec.ts", "Puppeteer.launch |browserURL| option", "should throw when trying to connect to non-existing browser")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldThrowWhenTryingToConnectToNonExistingBrowser()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = new string[] { "--remote-debugging-port=21222" };
            var originalBrowser = await Puppeteer.LaunchAsync(options);
            var browserURL = "http://127.0.0.1:2122";

            await Assert.ThrowsAsync<ProcessException>(() => Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserURL = browserURL
            }));

            await originalBrowser.CloseAsync();
        }
    }
}
