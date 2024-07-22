using System.IO;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.Firefox
{
    public class FirefoxDataTests
    {
        [Test, Retry(2), PuppeteerTest("firefox-data.spec", "Firefox", "should resolve URLs for Nightly")]
        public void ShouldResolveUrlsForNightly()
        {
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central/firefox-111.0a1.en-US.linux-x86_64.tar.bz2",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Linux, "111.0a1", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central/firefox-111.0a1.en-US.mac.dmg",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOS, "111.0a1", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central/firefox-111.0a1.en-US.mac.dmg",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOSArm64, "111.0a1", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central/firefox-111.0a1.en-US.win32.zip",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win32, "111.0a1", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central/firefox-111.0a1.en-US.win64.zip",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win64, "111.0a1", null));
        }

        [Test, Retry(2), PuppeteerTest("firefox-data.spec", "Firefox", "should resolve URLs for beta")]
        public void ShouldResolveUrlsForBeta()
        {
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/115.0b8/linux-x86_64/en-US/firefox-115.0b8.tar.bz2",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Linux, "beta_115.0b8", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/115.0b8/mac/en-US/Firefox 115.0b8.dmg",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOS, "beta_115.0b8", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/115.0b8/mac/en-US/Firefox 115.0b8.dmg",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOSArm64, "beta_115.0b8", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/115.0b8/win32/en-US/Firefox Setup 115.0b8.exe",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win32, "beta_115.0b8", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/115.0b8/win64/en-US/Firefox Setup 115.0b8.exe",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win64, "beta_115.0b8", null));
        }

        [Test, Retry(2), PuppeteerTest("firefox-data.spec", "Firefox", "should resolve URLs for stable")]
        public void ShouldResolveUrlsForStable()
        {
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/114.0/linux-x86_64/en-US/firefox-114.0.tar.bz2",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Linux, "stable_114.0", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/114.0/mac/en-US/Firefox 114.0.dmg",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOS, "stable_114.0", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/114.0/mac/en-US/Firefox 114.0.dmg",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOSArm64, "stable_114.0", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/114.0/win32/en-US/Firefox Setup 114.0.exe",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win32, "stable_114.0", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/114.0/win64/en-US/Firefox Setup 114.0.exe",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win64, "stable_114.0", null));
        }

        [Test, Retry(2), PuppeteerTest("firefox-data.spec", "Firefox", "should resolve URLs for devedition")]
        public void ShouldResolveUrlsForDevedition()
        {
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/devedition/releases/114.0b8/linux-x86_64/en-US/firefox-114.0b8.tar.bz2",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Linux, "devedition_114.0b8", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/devedition/releases/114.0b8/mac/en-US/Firefox 114.0b8.dmg",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOS, "devedition_114.0b8", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/devedition/releases/114.0b8/mac/en-US/Firefox 114.0b8.dmg",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOSArm64, "devedition_114.0b8", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/devedition/releases/114.0b8/win32/en-US/Firefox Setup 114.0b8.exe",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win32, "devedition_114.0b8", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/devedition/releases/114.0b8/win64/en-US/Firefox Setup 114.0b8.exe",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win64, "devedition_114.0b8", null));
        }

        [Test, Retry(2), PuppeteerTest("firefox-data.spec", "Firefox", "should resolve executable paths")]
        public void ShouldResolveExecutablePath()
        {
            Assert.AreEqual(
                Path.Combine("firefox", "firefox"),
                BrowserData.Firefox.RelativeExecutablePath(Platform.Linux, "111.0a1"));

            Assert.AreEqual(
                Path.Combine(
                    "Firefox.app",
                    "Contents",
                    "MacOS",
                    "firefox"
                ),
              BrowserData.Firefox.RelativeExecutablePath(Platform.MacOS, "111.0a1"));

            Assert.AreEqual(
                Path.Combine(
                    "Firefox.app",
                    "Contents",
                    "MacOS",
                    "firefox"
                ),
              BrowserData.Firefox.RelativeExecutablePath(Platform.MacOSArm64, "111.0a1"));

            Assert.AreEqual(
              Path.Combine("firefox", "firefox.exe"),
              BrowserData.Firefox.RelativeExecutablePath(Platform.Win32, "111.0a1"));

            Assert.AreEqual(
              BrowserData.Firefox.RelativeExecutablePath(Platform.Win64, "111.0a1"),
              Path.Combine("firefox", "firefox.exe"));
        }
    }
}
