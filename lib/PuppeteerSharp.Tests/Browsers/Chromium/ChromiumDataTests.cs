using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.Chromium
{
    public class ChromiumDataTests
    {
        [Test, Retry(2), PuppeteerTest("chromium-data.spec", "Chromium", "should resolve download URLs")]
        public void ShouldResolveDownloadUrls()
        {
            Assert.AreEqual(
                "https://storage.googleapis.com/chromium-browser-snapshots/Linux_x64/1083080/chrome-linux.zip",
                BrowserData.Chromium.ResolveDownloadUrl(Platform.Linux, "1083080", null));
            Assert.AreEqual(
                "https://storage.googleapis.com/chromium-browser-snapshots/Mac/1083080/chrome-mac.zip",
                BrowserData.Chromium.ResolveDownloadUrl(Platform.MacOS, "1083080", null));
            Assert.AreEqual(
                "https://storage.googleapis.com/chromium-browser-snapshots/Mac_Arm/1083080/chrome-mac.zip",
                BrowserData.Chromium.ResolveDownloadUrl(Platform.MacOSArm64, "1083080", null));
            Assert.AreEqual(
                "https://storage.googleapis.com/chromium-browser-snapshots/Win/1083080/chrome-win.zip",
                BrowserData.Chromium.ResolveDownloadUrl(Platform.Win32, "1083080", null));
            Assert.AreEqual(
                "https://storage.googleapis.com/chromium-browser-snapshots/Win_x64/1083080/chrome-win.zip",
                BrowserData.Chromium.ResolveDownloadUrl(Platform.Win64, "1083080", null));
        }

        [Test, Retry(2), PuppeteerTest("chromium-data.spec", "Chromium", "should resolve executable paths")]
        public void ShouldResolveExecutablePath()
        {
            Assert.AreEqual(
                Path.Combine("chrome-linux", "chrome"),
                BrowserData.Chromium.RelativeExecutablePath(Platform.Linux, "12372323"));

            Assert.AreEqual(
                Path.Combine(
                    "chrome-mac",
                    "Chromium.app",
                    "Contents",
                    "MacOS",
                    "Chromium"
                ),
              BrowserData.Chromium.RelativeExecutablePath(Platform.MacOS, "12372323"));

            Assert.AreEqual(
                Path.Combine(
                    "chrome-mac",
                    "Chromium.app",
                    "Contents",
                    "MacOS",
                    "Chromium"
                ),
              BrowserData.Chromium.RelativeExecutablePath(Platform.MacOSArm64, "12372323"));

            Assert.AreEqual(
              Path.Combine("chrome-win", "chrome.exe"),
              BrowserData.Chromium.RelativeExecutablePath(Platform.Win32, "12372323"));

            Assert.AreEqual(
              BrowserData.Chromium.RelativeExecutablePath(Platform.Win64, "12372323"),
              Path.Combine("chrome-win", "chrome.exe"));
        }

        [Test]
        [Retry(2)]
        public async Task ShouldResolveBuildIdFromPlatform()
            => Assert.True(int.TryParse(await BrowserData.Chromium.ResolveBuildIdAsync(Platform.MacOSArm64), out var _));
    }
}
