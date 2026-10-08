using System;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using PuppeteerSharp.Bidi;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LauncherTests
{
    public class BrowserCloseTests : PuppeteerBaseTest
    {
        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Browser.close", "should terminate network waiters")]
        public async Task ShouldTerminateNetworkWaiters()
        {
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            await using var remote = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = browser.WebSocketEndpoint,
                Protocol = ((Browser)browser).Protocol,
            });

            var newPage = await remote.NewPageAsync();
            var requestTask = newPage.WaitForRequestAsync(TestConstants.EmptyPage);
            var responseTask = newPage.WaitForResponseAsync(TestConstants.EmptyPage);

            await browser.CloseAsync();

            var exception = Assert.ThrowsAsync<TargetClosedException>(() => requestTask);
            Assert.That(exception.Message, Does.Contain("Target closed"));
            Assert.That(exception.Message, Does.Not.Contain("Timeout"));

            exception = Assert.ThrowsAsync<TargetClosedException>(() => responseTask);
            Assert.That(exception.Message, Does.Contain("Target closed"));
            Assert.That(exception.Message, Does.Not.Contain("Timeout"));
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Browser.close", "should stop the browser process when the close command fails")]
        public async Task ShouldStopTheBrowserProcessWhenTheCloseCommandFails()
        {
            if (PuppeteerTestAttribute.IsCdp)
            {
                Assert.Ignore("There is no BiDi connection to break on CDP.");
            }

            var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var launcher = ((Browser)browser).Launcher;
            try
            {
                await ((BidiBrowser)browser).Driver.StopAsync();

                await browser.CloseAsync();

                Assert.That(await launcher.WaitForExitAsync(TimeSpan.FromSeconds(10)), Is.True);
            }
            finally
            {
                await launcher.KillAsync();
            }
        }

        [Test, PuppeteerTest("launcher.spec", "PuppeteerSharp", "delete temp user data dir when disposing browser")]
        public async Task DeleteTempUserDataDirWhenDisposingBrowser()
        {
            var options = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher(TestConstants.LoggerFactory);
            await using var browser = await launcher.LaunchAsync(options);

            var tempUserDataDir = ((Browser)browser).Launcher.TempUserDataDir;
            Assert.That(tempUserDataDir.Path, Does.Exist);

            await browser.DisposeAsync();
            Assert.That(tempUserDataDir.Path, Does.Not.Exist);
        }
    }
}
