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

        private static LaunchOptions BrowserWithExtensionOptions() => new()
        {
            Headless = false,
            Args = new[]
            {
                $"--disable-extensions-except={_extensionPath.Quote()}",
                $"--load-extension={_extensionPath.Quote()}"
            }
        };

        [Test, PuppeteerTest("extensions.spec", "extensions", "service_worker target type should be available")]
        public async Task ServiceWorkerTargetTypeShouldBeAvailable()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            var serviceWorkTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.ServiceWorker);
            Assert.That(serviceWorkTarget, Is.Not.Null);
        }

        [Test, PuppeteerTest("extensions.spec", "extensions", "can evaluate in the service worker")]
        public async Task CanEvaluateInTheServiceWorker()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            var serviceWorkerTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.ServiceWorker);
            var worker = await serviceWorkerTarget.WorkerAsync();
            Assert.That(await worker.EvaluateFunctionAsync<int>("() => globalThis.MAGIC"), Is.EqualTo(42));
        }
    }
}
