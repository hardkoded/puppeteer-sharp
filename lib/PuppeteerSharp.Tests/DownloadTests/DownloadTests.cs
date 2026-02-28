using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DownloadTests
{
    public class DownloadTests : PuppeteerBrowserBaseTest
    {
        [Test, PuppeteerTest("download.spec", "Download Browser.createBrowserContext", "should download to configured location")]
        public async Task ShouldDownloadToConfiguredLocation()
        {
            using var tempDir = new TempDirectory();
            await using var context = await Browser.CreateBrowserContextAsync(new BrowserContextOptions
            {
                DownloadBehavior = new DownloadBehavior
                {
                    Policy = DownloadPolicy.Allow,
                    DownloadPath = tempDir.Path,
                },
            });
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/download.html");
            await page.ClickAsync("#download");
            await WaitForFileExistenceAsync(Path.Combine(tempDir.Path, "download.txt"), 10_000);
        }

        [Test, PuppeteerTest("download.spec", "Download Browser.createBrowserContext", "should not download to location")]
        public async Task ShouldNotDownloadToLocation()
        {
            using var tempDir = new TempDirectory();
            await using var context = await Browser.CreateBrowserContextAsync(new BrowserContextOptions
            {
                DownloadBehavior = new DownloadBehavior
                {
                    Policy = DownloadPolicy.Deny,
                    DownloadPath = "/tmp",
                },
            });
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/download.html");
            await page.ClickAsync("#download");
            Assert.ThrowsAsync<TimeoutException>(() =>
                WaitForFileExistenceAsync(Path.Combine(tempDir.Path, "download.txt"), 1_000));
        }

        private static async Task WaitForFileExistenceAsync(string filePath, int timeout = 5_000)
        {
            if (File.Exists(filePath))
            {
                return;
            }

            using var cts = new CancellationTokenSource(timeout);
            var dir = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);

            using var watcher = new FileSystemWatcher(dir)
            {
                Filter = fileName,
                EnableRaisingEvents = true,
            };

            var tcs = new TaskCompletionSource<bool>();
            cts.Token.Register(() => tcs.TrySetException(
                new TimeoutException($"Exceeded timeout of {timeout} ms for watching {filePath}")));

            watcher.Created += (_, e) =>
            {
                if (e.Name == fileName)
                {
                    tcs.TrySetResult(true);
                }
            };

            // Check again after setting up watcher to avoid race condition
            if (File.Exists(filePath))
            {
                return;
            }

            await tcs.Task.ConfigureAwait(false);
        }
    }
}
