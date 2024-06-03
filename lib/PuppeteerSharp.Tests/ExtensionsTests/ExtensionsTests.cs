using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ExtensionsTests
{
    public class ExtensionsTests : PuppeteerBaseTest
    {
        private static readonly string _extensionPath = Path.Combine(AppContext.BaseDirectory, "Assets", "simple-extension");
        private static readonly string _serviceWorkerExtensionPath = Path.Combine(AppContext.BaseDirectory, "Assets", "service-worker-extension");

        private static LaunchOptions BrowserWithExtensionOptions() => new()
        {
            Headless = false,
            Args = new[]
            {
                $"--disable-extensions-except={_extensionPath.Quote()}",
                $"--load-extension={_extensionPath.Quote()}"
            }
        };

        private static LaunchOptions BrowserWithServiceWorkerExtensionOptions() => new()
        {
            Headless = false,
            Args = new[]
            {
                $"--disable-extensions-except={_serviceWorkerExtensionPath.Quote()}",
                $"--load-extension={_serviceWorkerExtensionPath.Quote()}"
            }
        };

        [Test, Retry(2), PuppeteerTest("extensions.spec", "extensions", "background_page target type should be available")]
        public async Task BackgroundPageTargetTypeShouldBeAvailable()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            await using (await browserWithExtension.NewPageAsync())
            {
                var backgroundPageTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.BackgroundPage);
                Assert.NotNull(backgroundPageTarget);
            }
        }

        [Test, Retry(2), PuppeteerTest("extensions.spec", "extensions", "service_worker target type should be available")]
        public async Task ServiceWorkerTargetTypeShouldBeAvailable()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithServiceWorkerExtensionOptions(),
                TestConstants.LoggerFactory);
            var serviceWorkTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.ServiceWorker);
            await using var page = await browserWithExtension.NewPageAsync();
            Assert.NotNull(serviceWorkTarget);

        }

        [Test, Retry(2), PuppeteerTest("extensions.spec", "extensions", "target.page() should return a background_page")]
        public async Task TargetPageShouldReturnABackgroundPage()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            var backgroundPageTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.BackgroundPage);
            await using var page = await backgroundPageTarget.PageAsync();
            Assert.AreEqual(6, await page.EvaluateFunctionAsync<int>("() => 2 * 3"));
            Assert.AreEqual(42, await page.EvaluateFunctionAsync<int>("() => window.MAGIC"));
        }
    }
}
