using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class BrowserConnectedTests : PuppeteerBaseTest
    {
        [
            Test,
            PuppeteerTest(
                "browser.spec",
                "Browser specs Browser.connected",
                "should set the browser connected state"
            )
        ]
        public async Task ShouldSetTheBrowserConnectedState()
        {
            var browser = await Puppeteer.LaunchAsync(
                TestConstants.DefaultBrowserOptions(),
                TestConstants.LoggerFactory
            );
            var connectOptions = new ConnectOptions
            {
                BrowserWSEndpoint = browser.WebSocketEndpoint,
                Protocol = ((Browser)browser).Protocol,
            };

            try
            {
                Assert.That(browser.IsConnected, Is.True);
                browser.Disconnect();
                Assert.That(browser.IsConnected, Is.False);

                await using var newBrowser = await Puppeteer.ConnectAsync(
                    connectOptions,
                    TestConstants.LoggerFactory
                );
                Assert.That(newBrowser.IsConnected, Is.True);
                newBrowser.Disconnect();
                Assert.That(newBrowser.IsConnected, Is.False);
            }
            finally
            {
                try
                {
                    IBrowser cleanupBrowser = null;
                    try
                    {
                        cleanupBrowser = await Puppeteer.ConnectAsync(
                            connectOptions,
                            TestConstants.LoggerFactory
                        );
                    }
                    catch
                    {
                        // The browser process is killed below.
                    }

                    if (cleanupBrowser != null)
                    {
                        await cleanupBrowser.CloseAsync();
                    }
                }
                finally
                {
                    var browserProcess = browser.Process;
                    if (browserProcess is { HasExited: false })
                    {
                        browserProcess.Kill();
                        browserProcess.WaitForExit();
                    }

                    await browser.CloseAsync();
                }
            }
        }
    }
}
