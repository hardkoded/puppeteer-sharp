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
        public async Task ReuseChromeExample()
        {
            #region reusechrome_example
            var downloadPath = "/Users/dario/chrome";
            var browserFetcherOptions = new BrowserFetcherOptions { Path = downloadPath };
            var browserFetcher = new BrowserFetcher(browserFetcherOptions);
            var installedBrowser = await browserFetcher.DownloadAsync();
            #endregion
        }

        public async Task Usage()
        {
            #region customversions_example
            Console.WriteLine("Downloading browsers");

            var browserFetcher = new BrowserFetcher(SupportedBrowser.Chrome);
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

        [Test, PuppeteerTest("chrome-data.spec", "Chrome", "should resolve download URLs")]
        public void ShouldResolveDownloadUrls()
        {
            Assert.That(
                BrowserData.Chrome.ResolveDownloadUrl(Platform.Linux, "113.0.5672.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/113.0.5672.0/linux64/chrome-linux64.zip"));
            Assert.That(
                BrowserData.Chrome.ResolveDownloadUrl(Platform.MacOS, "113.0.5672.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/113.0.5672.0/mac-x64/chrome-mac-x64.zip"));
            Assert.That(
                BrowserData.Chrome.ResolveDownloadUrl(Platform.MacOSArm64, "113.0.5672.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/113.0.5672.0/mac-arm64/chrome-mac-arm64.zip"));
            Assert.That(
                BrowserData.Chrome.ResolveDownloadUrl(Platform.Win32, "113.0.5672.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/113.0.5672.0/win32/chrome-win32.zip"));
            Assert.That(
                BrowserData.Chrome.ResolveDownloadUrl(Platform.Win64, "113.0.5672.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/113.0.5672.0/win64/chrome-win64.zip"));
        }

        [Test, PuppeteerTest("chrome-data.spec", "Chrome", "should resolve executable paths")]
        public void ShouldResolveExecutablePath()
        {
            Assert.That(
                BrowserData.Chrome.RelativeExecutablePath(Platform.Linux, "12372323"),
                Is.EqualTo(Path.Combine("chrome-linux64", "chrome")));

            Assert.That(
                BrowserData.Chrome.RelativeExecutablePath(Platform.MacOS, "12372323"),
                Is.EqualTo(Path.Combine(
                    "chrome-mac-x64",
                    "Google Chrome for Testing.app",
                    "Contents",
                    "MacOS",
                    "Google Chrome for Testing"
                )));

            Assert.That(
                BrowserData.Chrome.RelativeExecutablePath(Platform.MacOSArm64, "12372323"),
                Is.EqualTo(Path.Combine(
                    "chrome-mac-arm64",
                    "Google Chrome for Testing.app",
                    "Contents",
                    "MacOS",
                    "Google Chrome for Testing"
                )));

            Assert.That(
                BrowserData.Chrome.RelativeExecutablePath(Platform.Win32, "12372323"),
                Is.EqualTo(Path.Combine("chrome-win32", "chrome.exe")));

            Assert.That(
                Path.Combine("chrome-win64", "chrome.exe"),
                Is.EqualTo(BrowserData.Chrome.RelativeExecutablePath(Platform.Win64, "12372323")));
        }

        // This has a custom name
        [Test, PuppeteerTest("chrome-data.spec", "Chrome", "should resolve system executable path (windows)")]
        public void ShouldResolveSystemExecutablePathWindows()
        {
            Assert.That(
                BrowserData.Chrome.ResolveSystemExecutablePath(Platform.Win32, ChromeReleaseChannel.Dev),
                Is.EqualTo("C:\\Program Files\\Google\\Chrome Dev\\Application\\chrome.exe"));
        }

        [Test, PuppeteerTest("chrome-data.spec", "Chrome", "should resolve system executable path")]
        public void ShouldResolveSystemExecutablePath()
        {
            Assert.That(
                BrowserData.Chrome.ResolveSystemExecutablePath(Platform.MacOS, ChromeReleaseChannel.Beta),
                Is.EqualTo("/Applications/Google Chrome Beta.app/Contents/MacOS/Google Chrome Beta"));

            var ex = Assert.Throws<PuppeteerException>(() =>
            {
                BrowserData.Chrome.ResolveSystemExecutablePath(
                    Platform.Linux,
                    ChromeReleaseChannel.Canary);
            });

            Assert.That(ex.Message, Is.EqualTo("Canary is not supported"));
        }

        [Test]
        [Retry(2)]
        public async Task ShouldReturnLatestVersion()
            => await BrowserData.Chrome.ResolveBuildIdAsync(ChromeReleaseChannel.Stable);
    }
}
