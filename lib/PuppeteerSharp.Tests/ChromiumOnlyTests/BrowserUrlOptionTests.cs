using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ChromiumSpecificTests
{
    public class BrowserUrlOptionTests : PuppeteerPageBaseTest
    {
        public BrowserUrlOptionTests() : base()
        {
        }

        [Test, PuppeteerTest("chromiumonly.spec", "Puppeteer.launch |browserURL| option", "should be able to connect using browserUrl, with and without trailing slash")]
        public async Task ShouldBeAbleToConnectUsingBrowserURLWithAndWithoutTrailingSlash()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = new string[] { "--remote-debugging-port=21222" };
            var originalBrowser = await Puppeteer.LaunchAsync(options);
            var browserURL = "http://127.0.0.1:21222";

            var browser1 = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserURL = browserURL });
            var page1 = await browser1.NewPageAsync();
            Assert.That(await page1.EvaluateExpressionAsync<int>("7 * 8"), Is.EqualTo(56));
            browser1.Disconnect();

            var browser2 = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserURL = browserURL + "/" });
            var page2 = await browser2.NewPageAsync();
            Assert.That(await page2.EvaluateExpressionAsync<int>("7 * 8"), Is.EqualTo(56));
            browser2.Disconnect();
            await originalBrowser.CloseAsync();
        }

        [Test, PuppeteerTest("chromiumonly.spec", "Puppeteer.launch |browserURL| option", "should throw when using both browserWSEndpoint and browserURL")]
        public async Task ShouldThrowWhenUsingBothBrowserWSEndpointAndBrowserURL()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = new string[] { "--remote-debugging-port=21222" };
            var originalBrowser = await Puppeteer.LaunchAsync(options);
            var browserURL = "http://127.0.0.1:21222";

            Assert.ThrowsAsync<PuppeteerException>(() => Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserURL = browserURL,
                BrowserWSEndpoint = originalBrowser.WebSocketEndpoint
            }));

            await originalBrowser.CloseAsync();
        }

        [Test, PuppeteerTest("chromiumonly.spec", "Puppeteer.launch |browserURL| option", "should throw when trying to connect to non-existing browser")]
        public async Task ShouldThrowWhenTryingToConnectToNonExistingBrowser()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = new string[] { "--remote-debugging-port=21222" };
            var originalBrowser = await Puppeteer.LaunchAsync(options);
            var browserURL = "http://127.0.0.1:2122";

            Assert.ThrowsAsync<ProcessException>(() => Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserURL = browserURL
            }));

            await originalBrowser.CloseAsync();
        }
    }
}
