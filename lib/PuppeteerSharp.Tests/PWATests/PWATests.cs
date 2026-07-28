using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PWATests
{
    public class PWATests : PuppeteerBaseTest
    {
        // The `PWA` CDP domain is only available over a pipe connection.
        private static LaunchOptions PipeBrowserOptions()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Pipe = true;
            return options;
        }

        [Test, PuppeteerTest("pwa.test", "PWA", "installs and uninstalls a PWA")]
        public async Task InstallsAndUninstallsAPwa()
        {
            await using var browser = await Puppeteer.LaunchAsync(PipeBrowserOptions(), TestConstants.LoggerFactory);
            var manifestId = $"{TestConstants.ServerUrl}/pwa/";
            var startUrl = $"{TestConstants.ServerUrl}/pwa/index.html";

            var returnedId = await browser.InstallPWAAsync(new InstallPWAOptions
            {
                ManifestId = manifestId,
                InstallUrlOrBundleUrl = startUrl,
            });
            Assert.That(returnedId, Is.EqualTo(manifestId));

            var installedState = await browser.GetPWAStateAsync(new GetPWAStateOptions { ManifestId = manifestId });
            Assert.That(installedState.BadgeCount, Is.EqualTo(0));
            Assert.That(installedState.FileHandlers, Is.Not.Null);

            await browser.UninstallPWAAsync(new UninstallPWAOptions { ManifestId = manifestId });

            Assert.ThrowsAsync<MessageException>(() => browser.GetPWAStateAsync(new GetPWAStateOptions { ManifestId = manifestId }));
        }

        [Test, PuppeteerTest("pwa.test", "PWA", "launches an installed PWA and returns its Page")]
        public async Task LaunchesAnInstalledPwaAndReturnsItsPage()
        {
            await using var browser = await Puppeteer.LaunchAsync(PipeBrowserOptions(), TestConstants.LoggerFactory);
            var manifestId = $"{TestConstants.ServerUrl}/pwa/";
            var startUrl = $"{TestConstants.ServerUrl}/pwa/index.html";

            await browser.InstallPWAAsync(new InstallPWAOptions
            {
                ManifestId = manifestId,
                InstallUrlOrBundleUrl = startUrl,
                DisplayMode = PWADisplayMode.Standalone,
            });

            var page = await browser.LaunchPWAAsync(new LaunchPWAOptions { ManifestId = manifestId });
            try
            {
                Assert.That(page.Url, Is.EqualTo(startUrl));
                var isStandalone = await page.EvaluateFunctionAsync<bool>(
                    "() => matchMedia('(display-mode: standalone)').matches");
                Assert.That(isStandalone, Is.True);
            }
            finally
            {
                await page.CloseAsync();
                await browser.UninstallPWAAsync(new UninstallPWAOptions { ManifestId = manifestId });
            }
        }

        [Test, PuppeteerTest("pwa.test", "PWA", "launches an installed PWA at an explicit url")]
        public async Task LaunchesAnInstalledPwaAtAnExplicitUrl()
        {
            await using var browser = await Puppeteer.LaunchAsync(PipeBrowserOptions(), TestConstants.LoggerFactory);
            var manifestId = $"{TestConstants.ServerUrl}/pwa/";
            var startUrl = $"{TestConstants.ServerUrl}/pwa/index.html";

            await browser.InstallPWAAsync(new InstallPWAOptions
            {
                ManifestId = manifestId,
                InstallUrlOrBundleUrl = startUrl,
                DisplayMode = PWADisplayMode.Standalone,
            });

            var page = await browser.LaunchPWAAsync(new LaunchPWAOptions { ManifestId = manifestId, Url = startUrl });
            try
            {
                Assert.That(page.Url, Is.EqualTo(startUrl));
            }
            finally
            {
                await page.CloseAsync();
                await browser.UninstallPWAAsync(new UninstallPWAOptions { ManifestId = manifestId });
            }
        }

        [Test, PuppeteerTest("pwa.test", "PWA", "installs a PWA with a standalone display mode")]
        public async Task InstallsAPwaWithAStandaloneDisplayMode()
        {
            await using var browser = await Puppeteer.LaunchAsync(PipeBrowserOptions(), TestConstants.LoggerFactory);
            var manifestId = $"{TestConstants.ServerUrl}/pwa/";
            var startUrl = $"{TestConstants.ServerUrl}/pwa/index.html";

            await browser.InstallPWAAsync(new InstallPWAOptions
            {
                ManifestId = manifestId,
                InstallUrlOrBundleUrl = startUrl,
                DisplayMode = PWADisplayMode.Standalone,
            });

            var page = await browser.LaunchPWAAsync(new LaunchPWAOptions { ManifestId = manifestId });
            try
            {
                var isStandalone = await page.EvaluateFunctionAsync<bool>(
                    "() => matchMedia('(display-mode: standalone)').matches");
                Assert.That(isStandalone, Is.True);
            }
            finally
            {
                await page.CloseAsync();
                await browser.UninstallPWAAsync(new UninstallPWAOptions { ManifestId = manifestId });
            }
        }
    }
}
