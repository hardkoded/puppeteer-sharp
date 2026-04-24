using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ExtensionsTests
{
    public class ExtensionsTests : PuppeteerBaseTest
    {
        private static readonly string _extensionPath = Path.Combine(AppContext.BaseDirectory, "Assets", "simple-extension");
        private static readonly string _extensionWithPagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "extension-with-page");

        private static LaunchOptions BrowserWithExtensionOptions() => new()
        {
            EnableExtensions = true,
            Pipe = true,
        };

        [Test, PuppeteerTest("extensions.spec", "extensions", "service_worker target type should be available")]
        public async Task ServiceWorkerTargetTypeShouldBeAvailable()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            await browserWithExtension.InstallExtensionAsync(_extensionPath);
            var serviceWorkTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.ServiceWorker);
            Assert.That(serviceWorkTarget, Is.Not.Null);
        }

        [Test, PuppeteerTest("extensions.spec", "extensions", "can evaluate in the service worker")]
        public async Task CanEvaluateInTheServiceWorker()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            await browserWithExtension.InstallExtensionAsync(_extensionPath);
            var serviceWorkerTarget = await browserWithExtension.WaitForTargetAsync(t => t.Type == TargetType.ServiceWorker);
            var worker = await serviceWorkerTarget.WorkerAsync();
            Assert.That(await worker.EvaluateFunctionAsync<int>("() => globalThis.MAGIC"), Is.EqualTo(42));
        }

        [Test, PuppeteerTest("extensions.spec", "extensions", "should list extensions and their properties")]
        public async Task ShouldListExtensionsAndTheirProperties()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);

            var extensionId = await browserWithExtension.InstallExtensionAsync(_extensionPath);
            var extensions = await browserWithExtension.ExtensionsAsync();

            var extension = extensions[extensionId];

            Assert.That(extension, Is.Not.Null);
            Assert.That(extension.Name, Is.EqualTo("Simple extension"));
            Assert.That(extension.Version, Is.EqualTo("0.1"));
            Assert.That(extension.Id, Is.EqualTo(extensionId));
        }

        [Test, PuppeteerTest("extensions.spec", "extensions", "should list extension workers")]
        public async Task ShouldListExtensionWorkers()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);

            var extensionId = await browserWithExtension.InstallExtensionAsync(_extensionPath);
            var extension = (await browserWithExtension.ExtensionsAsync())[extensionId];

            var page = await browserWithExtension.NewPageAsync();
            await extension.TriggerActionAsync(page);

            await browserWithExtension.WaitForTargetAsync(target =>
                target.Url.Contains(extensionId) && target.Type == TargetType.ServiceWorker);

            var workers = await extension.WorkersAsync();
            Assert.That(workers.Count, Is.GreaterThan(0));
        }

        [Test, PuppeteerTest("extensions.spec", "extensions", "should trigger extension action")]
        public async Task ShouldTriggerExtensionAction()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);

            var page = await browserWithExtension.NewPageAsync();
            var extensionId = await browserWithExtension.InstallExtensionAsync(_extensionPath);
            var extensions = await browserWithExtension.ExtensionsAsync();
            var extension = extensions[extensionId];

            await page.TriggerExtensionActionAsync(extension);
            // If it doesn't throw, we consider it successful for this level of testing.
        }

        [Test, PuppeteerTest("extensions.spec", "extensions", "should list extension pages")]
        public async Task ShouldListExtensionPages()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);

            var extensionId = await browserWithExtension.InstallExtensionAsync(_extensionWithPagePath);
            var extensions = await browserWithExtension.ExtensionsAsync();
            var extension = extensions[extensionId];

            var page = await browserWithExtension.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);

            await extension.TriggerActionAsync(page);

            await browserWithExtension.WaitForTargetAsync(target =>
                target.Url.Contains("popup.html") && target.Url.Contains(extensionId));

            var pages = await extension.PagesAsync();
            Assert.That(pages.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(pages.Any(p => p.Url.Contains("popup.html")), Is.True);
        }

        [Test, PuppeteerTest("extensions.spec", "extensions", "should capture console logs from extension pages")]
        public async Task ShouldCaptureConsoleLogsFromExtensionPages()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);

            var extensionId = await browserWithExtension.InstallExtensionAsync(_extensionWithPagePath);
            var extensions = await browserWithExtension.ExtensionsAsync();
            var extension = extensions[extensionId];

            var page = await browserWithExtension.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);

            await page.TriggerExtensionActionAsync(extension);

            var popupTarget = await browserWithExtension.WaitForTargetAsync(target =>
                target.Url.Contains("popup.html") && target.Url.Contains(extensionId));

            var extPage = await popupTarget.AsPageAsync();

            var messageTask = new TaskCompletionSource<string>();
            extPage.Console += (sender, e) => messageTask.TrySetResult(e.Message.Text);

            await extPage.EvaluateExpressionAsync("console.log('hello from extension page')");

            var message = await messageTask.Task.WithTimeout(5000);
            Assert.That(message, Is.EqualTo("hello from extension page"));
        }

        [Test, PuppeteerTest("extensions.spec", "extensions", "should capture console logs from extension workers")]
        public async Task ShouldCaptureConsoleLogsFromExtensionWorkers()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);

            var extensionId = await browserWithExtension.InstallExtensionAsync(_extensionWithPagePath);
            var extensions = await browserWithExtension.ExtensionsAsync();
            var extension = extensions[extensionId];

            var page = await browserWithExtension.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            await extension.TriggerActionAsync(page);

            var workerTarget = await browserWithExtension.WaitForTargetAsync(target =>
                target.Url.Contains(extensionId) && target.Type == TargetType.ServiceWorker);

            var worker = await workerTarget.WorkerAsync();
            var messageToLog = "hello from extension worker";

            var messageTask = new TaskCompletionSource<string>();
            worker.Console += (sender, e) =>
            {
                if (e.Message.Text == messageToLog)
                {
                    messageTask.TrySetResult(e.Message.Text);
                }
            };

            await worker.EvaluateFunctionAsync("msg => console.log(msg)", messageToLog);

            var message = await messageTask.Task.WithTimeout(5000);
            Assert.That(message, Is.EqualTo(messageToLog));
        }

        [Test, PuppeteerTest("extensions.spec", "extensions", "should remove extension from list after uninstall")]
        public async Task ShouldRemoveExtensionFromListAfterUninstall()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);

            var id = await browserWithExtension.InstallExtensionAsync(_extensionPath);
            var extensions = await browserWithExtension.ExtensionsAsync();
            Assert.That(extensions.ContainsKey(id), Is.True);

            await browserWithExtension.UninstallExtensionAsync(id);

            extensions = await browserWithExtension.ExtensionsAsync();
            Assert.That(extensions.ContainsKey(id), Is.False);
        }
    }
}
