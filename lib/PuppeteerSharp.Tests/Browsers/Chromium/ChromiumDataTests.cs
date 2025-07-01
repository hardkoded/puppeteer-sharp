using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.Chromium
{
    public class ChromiumDataTests
    {
        [Test, PuppeteerTest("chromium-data.spec", "Chromium", "should resolve download URLs")]
        public void ShouldResolveDownloadUrls()
        {
            Assert.That(
                BrowserData.Chromium.ResolveDownloadUrl(Platform.Linux, "1083080", null),
                Is.EqualTo("https://storage.googleapis.com/chromium-browser-snapshots/Linux_x64/1083080/chrome-linux.zip"));
            Assert.That(
                BrowserData.Chromium.ResolveDownloadUrl(Platform.MacOS, "1083080", null),
                Is.EqualTo("https://storage.googleapis.com/chromium-browser-snapshots/Mac/1083080/chrome-mac.zip"));
            Assert.That(
                BrowserData.Chromium.ResolveDownloadUrl(Platform.MacOSArm64, "1083080", null),
                Is.EqualTo("https://storage.googleapis.com/chromium-browser-snapshots/Mac_Arm/1083080/chrome-mac.zip"));
            Assert.That(
                BrowserData.Chromium.ResolveDownloadUrl(Platform.Win32, "1083080", null),
                Is.EqualTo("https://storage.googleapis.com/chromium-browser-snapshots/Win/1083080/chrome-win.zip"));
            Assert.That(
                BrowserData.Chromium.ResolveDownloadUrl(Platform.Win64, "1083080", null),
                Is.EqualTo("https://storage.googleapis.com/chromium-browser-snapshots/Win_x64/1083080/chrome-win.zip"));
        }

        [Test, PuppeteerTest("chromium-data.spec", "Chromium", "should resolve executable paths")]
        public void ShouldResolveExecutablePath()
        {
            Assert.That(
                BrowserData.Chromium.RelativeExecutablePath(Platform.Linux, "12372323"),
                Is.EqualTo(Path.Combine("chrome-linux", "chrome")));

            Assert.That(
              BrowserData.Chromium.RelativeExecutablePath(Platform.MacOS, "12372323"),
              Is.EqualTo(Path.Combine(
                    "chrome-mac",
                    "Chromium.app",
                    "Contents",
                    "MacOS",
                    "Chromium"
                )));

            Assert.That(
              BrowserData.Chromium.RelativeExecutablePath(Platform.MacOSArm64, "12372323"),
              Is.EqualTo(Path.Combine(
                    "chrome-mac",
                    "Chromium.app",
                    "Contents",
                    "MacOS",
                    "Chromium"
                )));

            Assert.That(
              BrowserData.Chromium.RelativeExecutablePath(Platform.Win32, "12372323"),
              Is.EqualTo(Path.Combine("chrome-win", "chrome.exe")));

            Assert.That(
              Path.Combine("chrome-win", "chrome.exe"),
              Is.EqualTo(BrowserData.Chromium.RelativeExecutablePath(Platform.Win64, "12372323")));
        }

        [Test]
        [Retry(2)]
        public async Task ShouldResolveBuildIdFromPlatform()
            => Assert.That(int.TryParse(await BrowserData.Chromium.ResolveBuildIdAsync(Platform.MacOSArm64), out var _), Is.True);
    }
}
