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
        private const string ExpectedId = "mbljndkcfjhaffohbnmoedabegpolpmd";

        [Test, PuppeteerTest("webExtension.spec", "webExtension", "can install and uninstall an extension")]
        public async Task CanInstallAndUninstallAnExtension()
        {
            var options = TestConstants.DefaultBrowserOptions();

            if (TestConstants.IsChrome)
            {
                options.IgnoredDefaultArgs = new[] { "--disable-extensions" };
                options.Args = new[] { "--enable-unsafe-extension-debugging" }
                    .Concat(options.Args ?? Array.Empty<string>())
                    .ToArray();
            }

            await using var browser = await Puppeteer.LaunchAsync(
                options,
                TestConstants.LoggerFactory);

            // Install an extension. Since the `key` field is present in the
            // manifest, this should always have the same ID.
            Assert.That(await browser.InstallExtensionAsync(_extensionPath), Is.EqualTo(ExpectedId));

            // Check we can uninstall the extension.
            await browser.UninstallExtensionAsync(ExpectedId);
        }
    }
}
