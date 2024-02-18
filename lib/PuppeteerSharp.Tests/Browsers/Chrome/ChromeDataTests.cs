using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.Chrome
{
    public class ChromeDataTests
    {
        public async Task Usage()
        {
            #region CustomVersionsExample
            Console.WriteLine("Downloading browsers");

            using var browserFetcher = new BrowserFetcher(SupportedBrowser.Chrome);
            var chrome118 = await browserFetcher.DownloadAsync("118.0.5993.70");
            var chrome119 = await browserFetcher.DownloadAsync("119.0.5997.0");

            Console.WriteLine("Navigating");
            await using (var browser = await Puppeteer.LaunchAsync(new()
            {
                ExecutablePath = chrome118.GetExecutablePath(),
            }))
            {
                await using var page = await browser.NewPageAsync();
                await page.GoToAsync("https://www.whatismybrowser.com/");

                Console.WriteLine("Generating PDF");
                await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "118.pdf"));

                Console.WriteLine("Export completed");
            }

            await using (var browser = await Puppeteer.LaunchAsync(new()
            {
                ExecutablePath = chrome119.GetExecutablePath(),
            }))
            {
                await using var page = await browser.NewPageAsync();
                await page.GoToAsync("https://www.whatismybrowser.com/");

                Console.WriteLine("Generating PDF");
                await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "119.pdf"));

                Console.WriteLine("Export completed");
            }
            #endregion
        }

        [Test, Retry(2), PuppeteerTest("chrome-data.spec", "Chrome", "should resolve download URLs")]
        public void ShouldResolveDownloadUrls()
        {
            Assert.AreEqual(
                "https://storage.googleapis.com/chrome-for-testing-public/113.0.5672.0/linux64/chrome-linux64.zip",
                BrowserData.Chrome.ResolveDownloadUrl(Platform.Linux, "113.0.5672.0", null));
            Assert.AreEqual(
                "https://storage.googleapis.com/chrome-for-testing-public/113.0.5672.0/mac-x64/chrome-mac-x64.zip",
                BrowserData.Chrome.ResolveDownloadUrl(Platform.MacOS, "113.0.5672.0", null));
            Assert.AreEqual(
                "https://storage.googleapis.com/chrome-for-testing-public/113.0.5672.0/mac-arm64/chrome-mac-arm64.zip",
                BrowserData.Chrome.ResolveDownloadUrl(Platform.MacOSArm64, "113.0.5672.0", null));
            Assert.AreEqual(
                "https://storage.googleapis.com/chrome-for-testing-public/113.0.5672.0/win32/chrome-win32.zip",
                BrowserData.Chrome.ResolveDownloadUrl(Platform.Win32, "113.0.5672.0", null));
            Assert.AreEqual(
                "https://storage.googleapis.com/chrome-for-testing-public/113.0.5672.0/win64/chrome-win64.zip",
                BrowserData.Chrome.ResolveDownloadUrl(Platform.Win64, "113.0.5672.0", null));
        }

        [Test, Retry(2), PuppeteerTest("chrome-data.spec", "Chrome", "should resolve executable paths")]
        public void ShouldResolveExecutablePath()
        {
            Assert.AreEqual(
                Path.Combine("chrome-linux64", "chrome"),
                BrowserData.Chrome.RelativeExecutablePath(Platform.Linux, "12372323"));

            Assert.AreEqual(
                Path.Combine(
                    "chrome-mac-x64",
                    "Google Chrome for Testing.app",
                    "Contents",
                    "MacOS",
                    "Google Chrome for Testing"
                ),
              BrowserData.Chrome.RelativeExecutablePath(Platform.MacOS, "12372323"));

            Assert.AreEqual(
                Path.Combine(
                    "chrome-mac-arm64",
                    "Google Chrome for Testing.app",
                    "Contents",
                    "MacOS",
                    "Google Chrome for Testing"
                ),
              BrowserData.Chrome.RelativeExecutablePath(Platform.MacOSArm64, "12372323"));

            Assert.AreEqual(
              Path.Combine("chrome-win32", "chrome.exe"),
              BrowserData.Chrome.RelativeExecutablePath(Platform.Win32, "12372323"));

            Assert.AreEqual(
              BrowserData.Chrome.RelativeExecutablePath(Platform.Win64, "12372323"),
              Path.Combine("chrome-win64", "chrome.exe"));
        }

        // This has a custom name
        [Test, Retry(2), PuppeteerTest("chrome-data.spec", "Chrome", "should resolve system executable path (windows)")]
        public void ShouldResolveSystemExecutablePathWindows()
        {
            Assert.AreEqual(
                "C:\\Program Files\\Google\\Chrome Dev\\Application\\chrome.exe",
                BrowserData.Chrome.ResolveSystemExecutablePath(
                    Platform.Win32,
                    ChromeReleaseChannel.Dev));
        }

        [Test, Retry(2), PuppeteerTest("chrome-data.spec", "Chrome", "should resolve system executable path")]
        public void ShouldResolveSystemExecutablePath()
        {
            Assert.AreEqual(
                "/Applications/Google Chrome Beta.app/Contents/MacOS/Google Chrome Beta",
                BrowserData.Chrome.ResolveSystemExecutablePath(
                    Platform.MacOS,
                    ChromeReleaseChannel.Beta));

            var ex = Assert.Throws<PuppeteerException>(() =>
            {
                BrowserData.Chrome.ResolveSystemExecutablePath(
                    Platform.Linux,
                    ChromeReleaseChannel.Canary);
            });

            Assert.AreEqual("Canary is not supported", ex.Message);
        }

        [Test]
        [Retry(2)]
        public async Task ShouldReturnLatestVersion()
            => await BrowserData.Chrome.ResolveBuildIdAsync(ChromeReleaseChannel.Stable);
    }
}
