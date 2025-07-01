using System.IO;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.Firefox
{
    public class FirefoxDataTests
    {
        [Test, PuppeteerTest("firefox-data.spec", "Firefox", "should resolve URLs for Nightly")]
        public void ShouldResolveUrlsForNightly()
        {
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Linux, "nightly_111.0a1", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central/firefox-111.0a1.en-US.linux-x86_64.tar.bz2"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOS, "nightly_111.0a1", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central/firefox-111.0a1.en-US.mac.dmg"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOSArm64, "nightly_111.0a1", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central/firefox-111.0a1.en-US.mac.dmg"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win32, "nightly_111.0a1", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central/firefox-111.0a1.en-US.win32.zip"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win64, "nightly_111.0a1", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central/firefox-111.0a1.en-US.win64.zip"));
        }

        [Test, PuppeteerTest("firefox-data.spec", "Firefox", "should resolve URLs for beta")]
        public void ShouldResolveUrlsForBeta()
        {
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Linux, "beta_115.0b8", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/releases/115.0b8/linux-x86_64/en-US/firefox-115.0b8.tar.bz2"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOS, "beta_115.0b8", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/releases/115.0b8/mac/en-US/Firefox 115.0b8.dmg"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOSArm64, "beta_115.0b8", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/releases/115.0b8/mac/en-US/Firefox 115.0b8.dmg"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win32, "beta_115.0b8", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/releases/115.0b8/win32/en-US/Firefox Setup 115.0b8.exe"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win64, "beta_115.0b8", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/releases/115.0b8/win64/en-US/Firefox Setup 115.0b8.exe"));
        }

        [Test, PuppeteerTest("firefox-data.spec", "Firefox", "should resolve URLs for stable")]
        public void ShouldResolveUrlsForStable()
        {
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Linux, "stable_114.0", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/releases/114.0/linux-x86_64/en-US/firefox-114.0.tar.bz2"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOS, "stable_114.0", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/releases/114.0/mac/en-US/Firefox 114.0.dmg"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOSArm64, "stable_114.0", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/releases/114.0/mac/en-US/Firefox 114.0.dmg"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win32, "stable_114.0", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/releases/114.0/win32/en-US/Firefox Setup 114.0.exe"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win64, "stable_114.0", null),
                Is.EqualTo("https://archive.mozilla.org/pub/firefox/releases/114.0/win64/en-US/Firefox Setup 114.0.exe"));
        }

        [Test, PuppeteerTest("firefox-data.spec", "Firefox", "should resolve URLs for devedition")]
        public void ShouldResolveUrlsForDevedition()
        {
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Linux, "devedition_114.0b8", null),
                Is.EqualTo("https://archive.mozilla.org/pub/devedition/releases/114.0b8/linux-x86_64/en-US/firefox-114.0b8.tar.bz2"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOS, "devedition_114.0b8", null),
                Is.EqualTo("https://archive.mozilla.org/pub/devedition/releases/114.0b8/mac/en-US/Firefox 114.0b8.dmg"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOSArm64, "devedition_114.0b8", null),
                Is.EqualTo("https://archive.mozilla.org/pub/devedition/releases/114.0b8/mac/en-US/Firefox 114.0b8.dmg"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win32, "devedition_114.0b8", null),
                Is.EqualTo("https://archive.mozilla.org/pub/devedition/releases/114.0b8/win32/en-US/Firefox Setup 114.0b8.exe"));
            Assert.That(
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win64, "devedition_114.0b8", null),
                Is.EqualTo("https://archive.mozilla.org/pub/devedition/releases/114.0b8/win64/en-US/Firefox Setup 114.0b8.exe"));
        }

        [Test, PuppeteerTest("firefox-data.spec", "Firefox", "should resolve executable paths")]
        public void ShouldResolveExecutablePath()
        {
            Assert.That(
                BrowserData.Firefox.RelativeExecutablePath(Platform.Linux, "111.0a1"),
                Is.EqualTo(Path.Combine("firefox", "firefox")));

            Assert.That(
              BrowserData.Firefox.RelativeExecutablePath(Platform.MacOS, "111.0a1"),
              Is.EqualTo(Path.Combine(
                    "Firefox.app",
                    "Contents",
                    "MacOS",
                    "firefox"
                )));

            Assert.That(
              BrowserData.Firefox.RelativeExecutablePath(Platform.MacOSArm64, "111.0a1"),
              Is.EqualTo(Path.Combine(
                    "Firefox.app",
                    "Contents",
                    "MacOS",
                    "firefox"
                )));

            Assert.That(
                BrowserData.Firefox.RelativeExecutablePath(Platform.MacOSArm64, "stable_111.0a1"),
                Is.EqualTo(Path.Combine(
                    "Firefox.app",
                    "Contents",
                    "MacOS",
                    "firefox"
                )));

            Assert.That(
              BrowserData.Firefox.RelativeExecutablePath(Platform.Win32, "111.0a1"),
              Is.EqualTo(Path.Combine("core", "firefox.exe")));
        }
    }
}
