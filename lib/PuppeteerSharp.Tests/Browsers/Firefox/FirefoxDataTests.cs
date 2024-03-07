using System;
using System.IO;
using NUnit.Framework;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.Firefox
{
    public class FirefoxDataTests
    {
        [Test, Retry(2), PuppeteerTest("firefox-data.spec", "Firefox", "should resolve download URLs")]
        public void ShouldResolveDownloadUrls()
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

        [Test, Retry(2), PuppeteerTest("firefox-data.spec", "Firefox", "should resolve executable paths")]
        public void ShouldResolveExecutablePath()
        {
            Assert.AreEqual(
                Path.Combine("firefox", "firefox"),
                BrowserData.Firefox.RelativeExecutablePath(Platform.Linux, "111.0a1"));

            Assert.AreEqual(
                Path.Combine(
                    "Firefox Nightly.app",
                    "Contents",
                    "MacOS",
                    "firefox"
                ),
              BrowserData.Firefox.RelativeExecutablePath(Platform.MacOS, "111.0a1"));

            Assert.AreEqual(
                Path.Combine(
                    "Firefox Nightly.app",
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
