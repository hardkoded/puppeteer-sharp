using System.IO;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.Firefox
{
    public class FirefoxDataTests
    {
        [Test, Retry(2), PuppeteerTest("firefox-data.spec", "Firefox", "should resolve download URLs")]
        public void ShouldResolveDownloadUrls()
        {
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/111.0a1/linux-x86_64/en-US/firefox-111.0a1.tar.bz2",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Linux, "111.0a1", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/111.0a1/mac/en-US/Firefox 111.0a1.dmg",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOS, "111.0a1", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/111.0a1/mac/en-US/Firefox 111.0a1.dmg",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.MacOSArm64, "111.0a1", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/111.0a1/win32/en-US/Firefox Setup 111.0a1.exe",
                BrowserData.Firefox.ResolveDownloadUrl(Platform.Win32, "111.0a1", null));
            Assert.AreEqual(
                "https://archive.mozilla.org/pub/firefox/releases/111.0a1/win64/en-US/Firefox Setup 111.0a1.exe",
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
