using System;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LauncherTests
{
    public class BrowserLauncherTests : PuppeteerBaseTest
    {
        [
            Test,
            PuppeteerTest(
                "BrowserLauncher.test",
                "BrowserLauncher.launch",
                "rejects targetFilter for Chrome with WebDriver BiDi before launching"
            )
        ]
        public void RejectsTargetFilterForChromeWithWebDriverBiDiBeforeLaunching()
        {
            var launcher = new Launcher(TestConstants.LoggerFactory);
            var options = new LaunchOptions
            {
                Browser = SupportedBrowser.Chrome,
                Protocol = ProtocolType.WebdriverBiDi,
                TargetFilter = _ => true,
            };

            Assert.ThrowsAsync<NotSupportedException>(() => launcher.LaunchAsync(options));
            Assert.That(launcher.Process, Is.Null);
        }

        [
            Test,
            PuppeteerTest(
                "BrowserLauncher.test",
                "BrowserLauncher.launch",
                "rejects targetFilter for Firefox defaulting to WebDriver BiDi before launching"
            )
        ]
        public void RejectsTargetFilterForFirefoxDefaultingToWebDriverBiDiBeforeLaunching()
        {
            var launcher = new Launcher(TestConstants.LoggerFactory);
            var options = new LaunchOptions
            {
                Browser = SupportedBrowser.Firefox,
                TargetFilter = _ => true,
            };

            Assert.ThrowsAsync<NotSupportedException>(() => launcher.LaunchAsync(options));
            Assert.That(launcher.Process, Is.Null);
        }
    }
}
