using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.WebExtensionTests
{
    public class WebExtensionTests : PuppeteerBaseTest
    {
        private static readonly string _extensionPath = Path.Combine(AppContext.BaseDirectory, "Assets", "simple-extension");
        private static readonly string _extensionFirefoxPath = Path.Combine(AppContext.BaseDirectory, "Assets", "simple-extension-firefox");
        private const string ExpectedId = "mbljndkcfjhaffohbnmoedabegpolpmd";

        [Test, PuppeteerTest("webExtension.spec", "webExtension", "can install and uninstall an extension")]
        public async Task CanInstallAndUninstallAnExtension()
        {
            var options = TestConstants.DefaultBrowserOptions();
            var extensionPath = TestConstants.IsChrome ? _extensionPath : _extensionFirefoxPath;
            var expectedId = TestConstants.IsChrome ? ExpectedId : null;

            if (TestConstants.IsChrome)
            {
                options.EnableExtensions = true;
                options.Pipe = true;
            }

            await using var browser = await Puppeteer.LaunchAsync(
                options,
                TestConstants.LoggerFactory);

            var id = await browser.InstallExtensionAsync(extensionPath);

            if (expectedId != null)
            {
                // For Chrome, since the `key` field is present in the
                // manifest, this should always have the same ID.
                Assert.That(id, Is.EqualTo(expectedId));
            }
            else
            {
                // Firefox uses temporary addon IDs
                Assert.That(id, Does.Contain("temporary-addon"));
            }

            // Check we can uninstall the extension.
            await browser.UninstallExtensionAsync(id);
        }
    }
}
