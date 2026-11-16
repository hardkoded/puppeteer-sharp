using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        };

        private static void AssertNoServiceWorkerReported(ITarget[] targets, string id)
        {
            var target = targets.FirstOrDefault(t => t.Url.Contains(id) && t.Type == TargetType.ServiceWorker);
            Assert.That(target, Is.Null);
        }

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

        [Test, PuppeteerTest("extensions.spec", "extensions", "should drop the previous service_worker target when re-installing an extension")]
        public async Task ShouldDropThePreviousServiceWorkerTargetWhenReinstallingAnExtension()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            var extensionId = await browserWithExtension.InstallExtensionAsync(_extensionPath);
            var firstWorkerTarget = await browserWithExtension.WaitForTargetAsync(
                t => t.Url.Contains(extensionId) && t.Type == TargetType.ServiceWorker);
            Assert.That(firstWorkerTarget, Is.Not.Null);

            var destroyedTcs = new TaskCompletionSource<ITarget>(TaskCreationOptions.RunContinuationsAsynchronously);
            browserWithExtension.TargetDestroyed += (_, e) => destroyedTcs.TrySetResult(e.Target);

            var reinstalledId = await browserWithExtension.InstallExtensionAsync(_extensionPath);
            Assert.That(reinstalledId, Is.EqualTo(extensionId));

            var destroyedTarget = await destroyedTcs.Task.WithTimeout(5000);
            Assert.That(destroyedTarget, Is.SameAs(firstWorkerTarget));

            var secondWorkerTarget = await browserWithExtension.WaitForTargetAsync(
                t => t.Url.Contains(extensionId) && t.Type == TargetType.ServiceWorker && t != firstWorkerTarget);
            Assert.That(secondWorkerTarget, Is.Not.Null);

            var serviceWorkerTargets = browserWithExtension.Targets()
                .Where(t => t.Url.Contains(extensionId) && t.Type == TargetType.ServiceWorker)
                .ToArray();
            Assert.That(serviceWorkerTargets, Is.EqualTo(new[] { secondWorkerTarget }));

            await browserWithExtension.UninstallExtensionAsync(extensionId);
            AssertNoServiceWorkerReported(browserWithExtension.Targets(), extensionId);
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
            Assert.That(extension.Path, Is.EqualTo(_extensionPath));
            Assert.That(extension.Enabled, Is.True);
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

        [Test, PuppeteerTest("extensions.spec", "extensions", "should be available in Incognito profiles if enabledInIncognito is true")]
        public async Task ShouldBeAvailableInIncognitoProfilesIfEnabledInIncognitoIsTrue()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);

            var extensionId = await browserWithExtension.InstallExtensionAsync(
                _extensionPath,
                new ExtensionInstallOptions { EnabledInIncognito = true });

            var context = await browserWithExtension.CreateBrowserContextAsync();
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);

            var target = await browserWithExtension.WaitForTargetAsync(t =>
                t.Url.Contains(extensionId) && t.Type == TargetType.ServiceWorker);
            Assert.That(target, Is.Not.Null);

            var realms = page.ExtensionRealms();

            Realm contentScriptRealm = null;
            foreach (var realm in realms)
            {
                var extension = await realm.ExtensionAsync();
                if (extension != null && extension.Id == extensionId)
                {
                    contentScriptRealm = realm;
                    break;
                }
            }

            Assert.That(contentScriptRealm, Is.Not.Null, "realm should be defined");

            var isContentScript = await contentScriptRealm.EvaluateFunctionAsync<bool>("() => globalThis.thisIsTheContentScript");
            Assert.That(isContentScript, Is.True);

            await browserWithExtension.UninstallExtensionAsync(extensionId);
            await context.CloseAsync();
            var targets = browserWithExtension.Targets();
            AssertNoServiceWorkerReported(targets, extensionId);
        }

        [Test, PuppeteerTest("extensions.spec", "extensions service worker network events", "should report network requests on the service worker")]
        public async Task ShouldReportNetworkRequestsOnTheServiceWorker()
        {
            // The service worker runs on a chrome-extension:// origin.
            Server.SetRoute("/cors.json", async context =>
            {
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                context.Response.Headers["Content-Type"] = "application/json";
                await context.Response.WriteAsync("{}");
            });
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            var worker = await InstallAndGetWorkerAsync(browserWithExtension);
            var url = TestConstants.ServerUrl + "/cors.json";

            var requestTcs = new TaskCompletionSource<IRequest>(TaskCreationOptions.RunContinuationsAsynchronously);
            var responseTcs = new TaskCompletionSource<IResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
            var finishedTcs = new TaskCompletionSource<IRequest>(TaskCreationOptions.RunContinuationsAsynchronously);
            worker.Request += (_, e) =>
            {
                if (e.Request.Url == url)
                {
                    requestTcs.TrySetResult(e.Request);
                }
            };
            worker.Response += (_, e) =>
            {
                if (e.Response.Url == url)
                {
                    responseTcs.TrySetResult(e.Response);
                }
            };
            worker.RequestFinished += (_, e) =>
            {
                if (e.Request.Url == url)
                {
                    finishedTcs.TrySetResult(e.Request);
                }
            };
            await worker.EvaluateFunctionAsync("async url => { await (await fetch(url)).text(); }", url);
            var request = await requestTcs.Task.WithTimeout();
            var response = await responseTcs.Task.WithTimeout();
            var finished = await finishedTcs.Task.WithTimeout();

            Assert.That(request.Worker, Is.SameAs(worker));
            Assert.That(request.Frame, Is.Null);
            Assert.That(request.Page, Is.Null);
            Assert.That(response.Request, Is.SameAs(request));
            Assert.That(response.Status, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.That(finished, Is.SameAs(request));
        }

        [Test, PuppeteerTest("extensions.spec", "extensions service worker network events", "should report failed network requests on the service worker")]
        public async Task ShouldReportFailedNetworkRequestsOnTheServiceWorker()
        {
            Server.SetRoute("/fail", context =>
            {
                context.Abort();
                return Task.CompletedTask;
            });
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            var worker = await InstallAndGetWorkerAsync(browserWithExtension);
            var url = TestConstants.ServerUrl + "/fail";

            var failedTcs = new TaskCompletionSource<IRequest>(TaskCreationOptions.RunContinuationsAsynchronously);
            worker.RequestFailed += (_, e) =>
            {
                if (e.Request.Url == url)
                {
                    failedTcs.TrySetResult(e.Request);
                }
            };
            await worker.EvaluateFunctionAsync("async url => { await fetch(url).catch(() => {}); }", url);
            var failed = await failedTcs.Task.WithTimeout();

            Assert.That(failed.Worker, Is.SameAs(worker));
            Assert.That(failed.FailureText, Is.Not.Empty);
        }

        [Test, PuppeteerTest("extensions.spec", "extensions service worker network events", "should not allow intercepting service worker requests")]
        public async Task ShouldNotAllowInterceptingServiceWorkerRequests()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);
            var worker = await InstallAndGetWorkerAsync(browserWithExtension);

            var requestTcs = new TaskCompletionSource<IRequest>(TaskCreationOptions.RunContinuationsAsynchronously);
            worker.Request += (_, e) =>
            {
                if (e.Request.Url == TestConstants.EmptyPage)
                {
                    requestTcs.TrySetResult(e.Request);
                }
            };
            await worker.EvaluateFunctionAsync("async url => { await fetch(url).catch(() => {}); }", TestConstants.EmptyPage);
            var request = await requestTcs.Task.WithTimeout();

            var exception = Assert.ThrowsAsync<PuppeteerException>(() => request.ContinueAsync());
            Assert.That(exception!.Message, Does.Contain("Request Interception is not enabled!"));
        }

        private static async Task<WebWorker> InstallAndGetWorkerAsync(IBrowser browser)
        {
            var id = await browser.InstallExtensionAsync(_extensionPath);
            var target = await browser.WaitForTargetAsync(t => t.Url.Contains(id) && t.Type == TargetType.ServiceWorker);
            return await target.WorkerAsync();
        }
    }
}
