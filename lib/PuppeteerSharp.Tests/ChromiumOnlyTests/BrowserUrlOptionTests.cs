using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.ChromiumSpecificTests
{
    public class BrowserUrlOptionTests : PuppeteerPageBaseTest
    {
        public BrowserUrlOptionTests(): base()
        {
        }

        [PuppeteerTest("chromiumonly.spec.ts", "Puppeteer.launch |browserURL| option", "should be able to connect using browserUrl, with and without trailing slash")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldBeAbleToConnectUsingBrowserURLWithAndWithoutTrailingSlash()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = new string[] { "--remote-debugging-port=21222" };
            var originalBrowser = await Puppeteer.LaunchAsync(options);
            var browserURL = "http://127.0.0.1:21222";

            var browser1 = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserURL = browserURL });
            var page1 = await browser1.NewPageAsync();
            Assert.AreEqual(56, await page1.EvaluateExpressionAsync<int>("7 * 8"));
            browser1.Disconnect();

            var browser2 = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserURL = browserURL + "/" });
            var page2 = await browser2.NewPageAsync();
            Assert.AreEqual(56, await page2.EvaluateExpressionAsync<int>("7 * 8"));
            browser2.Disconnect();
            await originalBrowser.CloseAsync();
        }

        [PuppeteerTest("chromiumonly.spec.ts", "Puppeteer.launch |browserURL| option", "should throw when using both browserWSEndpoint and browserURL")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("chromiumonly.spec.ts", "Puppeteer.launch |browserURL| option", "should throw when trying to connect to non-existing browser")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
