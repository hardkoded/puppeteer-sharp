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
            EnableExtensions = true,
            Args = new[]
            {
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

        [Test, PuppeteerTest("extensions.spec", "extensions", "should list extensions and their properties")]
        public async Task ShouldListExtensionsAndTheirProperties()
        {
            var options = new LaunchOptions
            {
                Headless = false,
                EnableExtensions = true,
                Pipe = true,
            };

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);

            var extensionId = await browser.InstallExtensionAsync(_extensionPath);

            await browser.WaitForTargetAsync(t =>
                t.Url.Contains(extensionId) && t.Type == TargetType.ServiceWorker);

            var extensions = await browser.GetExtensionsAsync();
            var extension = extensions[extensionId];

            Assert.That(extension, Is.Not.Null);
            Assert.That(extension.Name, Is.EqualTo("Simple extension"));
            Assert.That(extension.Version, Is.EqualTo("0.1"));
            Assert.That(extension.Path, Is.EqualTo(_extensionPath));
            Assert.That(extension.Enabled, Is.True);
            Assert.That(extension.Id, Is.EqualTo(extensionId));

            await browser.UninstallExtensionAsync(extensionId);
        }
    }
}
