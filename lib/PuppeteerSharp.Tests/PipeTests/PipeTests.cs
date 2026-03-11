using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PipeTests
{
    public class PipeTests : PuppeteerBaseTest
    {
        [Test, PuppeteerTest("pipe.spec", "Puppeteer.launch", "should support the pipe option")]
        public async Task ShouldSupportThePipeOption()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Ignore("Pipe transport is not implemented on Windows yet");
            }

            var options = TestConstants.DefaultBrowserOptions();
            options.Pipe = true;

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            Assert.That(await browser.PagesAsync(), Has.Exactly(1).Items);
            Assert.That(browser.WebSocketEndpoint, Is.EqualTo(string.Empty));
            var page = await browser.NewPageAsync();
            Assert.That(await page.EvaluateExpressionAsync<int>("11 * 11"), Is.EqualTo(121));
            await page.CloseAsync();
        }

        [Test, PuppeteerTest("pipe.spec", "Puppeteer.launch", "should support the pipe argument")]
        public async Task ShouldSupportThePipeArgument()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Ignore("Pipe transport is not implemented on Windows yet");
            }

            var options = TestConstants.DefaultBrowserOptions();
            options.Args = ["--remote-debugging-pipe", .. options.Args];
            options.Pipe = true;

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            Assert.That(browser.WebSocketEndpoint, Is.EqualTo(string.Empty));
            var page = await browser.NewPageAsync();
            Assert.That(await page.EvaluateExpressionAsync<int>("11 * 11"), Is.EqualTo(121));
            await page.CloseAsync();
        }

        [Test, PuppeteerTest("pipe.spec", "Puppeteer.launch", "should fire \"disconnected\" when closing with pipe")]
        public async Task ShouldFireDisconnectedWhenClosingWithPipe()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Ignore("Pipe transport is not implemented on Windows yet");
            }

            var options = TestConstants.DefaultBrowserOptions();
            options.Pipe = true;

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            var disconnectedTask = WaitForBrowserDisconnect(browser);
            // Emulate user exiting browser.
            browser.Process.Kill();
            await disconnectedTask;
        }
    }
}
