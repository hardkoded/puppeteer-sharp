using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DownloadTests
{
    public class DownloadTests : PuppeteerBrowserBaseTest
    {
        private string _tempDir;

        [SetUp]
        public void CreateTempDir()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "downloads-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        [TearDown]
        public void DeleteTempDir()
        {
            try
            {
                if (Directory.Exists(_tempDir))
                {
                    Directory.Delete(_tempDir, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        [Test, PuppeteerTest("download.spec", "Download Browser.createBrowserContext", "should download to configured location")]
        public async Task ShouldDownloadToConfiguredLocation()
        {
            await using var context = await Browser.CreateBrowserContextAsync(new BrowserContextOptions
            {
                DownloadBehavior = new DownloadBehavior
                {
                    Policy = DownloadPolicy.Allow,
                    DownloadPath = _tempDir,
                },
            });
            var page = await context.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/download.html");
            await page.ClickAsync("#download");
            await WaitForFileExistenceAsync(Path.Combine(_tempDir, "download.txt"));
        }

        [Test, PuppeteerTest("download.spec", "Download Browser.createBrowserContext", "should not download to location")]
        public async Task ShouldNotDownloadToLocation()
        {
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
            Assert.ThrowsAsync<TimeoutException>(async () =>
                await WaitForFileExistenceAsync(Path.Combine(_tempDir, "download.txt")));
        }

        private static async Task WaitForFileExistenceAsync(string filePath, int timeout = 1000)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(timeout);
            while (DateTime.UtcNow < deadline)
            {
                if (File.Exists(filePath))
                {
                    return;
                }

                await Task.Delay(50).ConfigureAwait(false);
            }

            throw new TimeoutException($"Exceeded timeout of {timeout} ms for watching {filePath}");
        }
    }
}
