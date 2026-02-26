using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
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

            // Diagnostic: verify CDP response to setDownloadBehavior
            var session = await page.CreateCDPSessionAsync();
            var response = await session.SendAsync("Browser.setDownloadBehavior", new
            {
                behavior = "allow",
                downloadPath = _tempDir,
                eventsEnabled = true,
            });
            TestContext.Out.WriteLine($"CDP setDownloadBehavior response: {response}");
            TestContext.Out.WriteLine($"Download dir: {_tempDir}");
            TestContext.Out.WriteLine($"Download dir exists: {Directory.Exists(_tempDir)}");
            TestContext.Out.WriteLine($"Browser version: {await Browser.GetVersionAsync()}");

            // Listen for download events
            session.MessageReceived += (sender, e) =>
            {
                if (e.MessageID.Contains("download", StringComparison.OrdinalIgnoreCase))
                {
                    TestContext.Out.WriteLine($"Download event: {e.MessageID} => {e.MessageData}");
                }
            };

            await page.GoToAsync(TestConstants.ServerUrl + "/download.html");
            await page.ClickAsync("#download");

            // Give extra time and check
            await Task.Delay(3_000);

            // List files in download dir
            if (Directory.Exists(_tempDir))
            {
                var files = Directory.GetFiles(_tempDir, "*", SearchOption.AllDirectories);
                TestContext.Out.WriteLine($"Files in download dir ({files.Length}):");
                foreach (var f in files)
                {
                    TestContext.Out.WriteLine($"  {f} ({new FileInfo(f).Length} bytes)");
                }
            }

            // Check default user Downloads folder
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var defaultDownloads = Path.Combine(userProfile, "Downloads");
            if (Directory.Exists(defaultDownloads))
            {
                var defaultFiles = Directory.GetFiles(defaultDownloads, "download*", SearchOption.TopDirectoryOnly);
                if (defaultFiles.Length > 0)
                {
                    TestContext.Out.WriteLine($"Files matching 'download*' in {defaultDownloads} ({defaultFiles.Length}):");
                    foreach (var f in defaultFiles.Take(5))
                    {
                        TestContext.Out.WriteLine($"  {f} ({new FileInfo(f).Length} bytes)");
                    }
                }
            }

            await WaitForFileExistenceAsync(Path.Combine(_tempDir, "download.txt"), 10_000);
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
            Assert.ThrowsAsync<TimeoutException>(() =>
                WaitForFileExistenceAsync(Path.Combine(_tempDir, "download.txt"), 1_000));
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
