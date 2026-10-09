using System.IO;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.ChromeHeadlessShell
{
    public class ChromeHeadlessShellDataTests
    {
        [Test, PuppeteerTest("chrome-headless-shell-data.spec", "chrome-headless-shell", "should resolve download URLs")]
        public void ShouldResolveDownloadUrls()
        {
            Assert.That(
                BrowserData.ChromeHeadlessShell.ResolveDownloadUrl(Platform.Linux, "118.0.5950.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/118.0.5950.0/linux64/chrome-headless-shell-linux64.zip"));
            Assert.That(
                BrowserData.ChromeHeadlessShell.ResolveDownloadUrl(Platform.LinuxArm64, "153.0.8001.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/153.0.8001.0/linux-arm64/chrome-headless-shell-linux-arm64.zip"));
            Assert.That(
                BrowserData.ChromeHeadlessShell.ResolveDownloadUrl(Platform.MacOS, "118.0.5950.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/118.0.5950.0/mac-x64/chrome-headless-shell-mac-x64.zip"));
            Assert.That(
                BrowserData.ChromeHeadlessShell.ResolveDownloadUrl(Platform.MacOSArm64, "118.0.5950.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/118.0.5950.0/mac-arm64/chrome-headless-shell-mac-arm64.zip"));
            Assert.That(
                BrowserData.ChromeHeadlessShell.ResolveDownloadUrl(Platform.Win32, "118.0.5950.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/118.0.5950.0/win32/chrome-headless-shell-win32.zip"));
            Assert.That(
                BrowserData.ChromeHeadlessShell.ResolveDownloadUrl(Platform.Win64, "118.0.5950.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/118.0.5950.0/win64/chrome-headless-shell-win64.zip"));
        }

        [Test, PuppeteerTest("chrome-headless-shell-data.spec", "chrome-headless-shell", "should resolve executable paths")]
        public void ShouldResolveExecutablePath()
        {
            Assert.That(
                BrowserData.ChromeHeadlessShell.RelativeExecutablePath(Platform.Linux, "12372323"),
                Is.EqualTo(Path.Combine("chrome-headless-shell-linux64", "chrome-headless-shell")));
            Assert.That(
                BrowserData.ChromeHeadlessShell.RelativeExecutablePath(Platform.LinuxArm64, "12372323"),
                Is.EqualTo(Path.Combine("chrome-headless-shell-linux-arm64", "chrome-headless-shell")));
            Assert.That(
                BrowserData.ChromeHeadlessShell.RelativeExecutablePath(Platform.MacOS, "12372323"),
                Is.EqualTo(Path.Combine("chrome-headless-shell-mac-x64", "chrome-headless-shell")));
            Assert.That(
                BrowserData.ChromeHeadlessShell.RelativeExecutablePath(Platform.MacOSArm64, "12372323"),
                Is.EqualTo(Path.Combine("chrome-headless-shell-mac-arm64", "chrome-headless-shell")));
            Assert.That(
                BrowserData.ChromeHeadlessShell.RelativeExecutablePath(Platform.Win32, "12372323"),
                Is.EqualTo(Path.Combine("chrome-headless-shell-win32", "chrome-headless-shell.exe")));
            Assert.That(
                BrowserData.ChromeHeadlessShell.RelativeExecutablePath(Platform.Win64, "12372323"),
                Is.EqualTo(Path.Combine("chrome-headless-shell-win64", "chrome-headless-shell.exe")));
        }

        [Test]
        public void ShouldFallBackToLinux64ForLinuxArm64BuildsOlderThan153()
        {
            Assert.That(
                BrowserData.ChromeHeadlessShell.ResolveDownloadUrl(Platform.LinuxArm64, "153.0.8000.0", null),
                Is.EqualTo("https://storage.googleapis.com/chrome-for-testing-public/153.0.8000.0/linux64/chrome-headless-shell-linux64.zip"));
            Assert.That(
                BrowserData.ChromeHeadlessShell.RelativeExecutablePath(Platform.LinuxArm64, "153.0.8000.0"),
                Is.EqualTo(Path.Combine("chrome-headless-shell-linux64", "chrome-headless-shell")));
        }
    }
}
