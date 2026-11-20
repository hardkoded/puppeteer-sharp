using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.Cli
{
    /// <summary>
    /// Puppeteer sharp doesn't have a CLI per se. These tests cover the retry option on the fetcher.
    /// </summary>
    public class CliTests
    {
        private string _tmpDir;

        [SetUp]
        public void CreateDir()
        {
            _tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tmpDir);
        }

        [TearDown]
        public void DeleteDir() => Directory.Delete(_tmpDir, true);

        [
            Test,
            PuppeteerTest(
                "CLI.test",
                "CLI",
                "proves that --retry reaches the download (partial: BrowserFetcher, not the CLI)"
            )
        ]
        public void Partial_ProvesThatRetryReachesTheDownload()
        {
            using var server = new TestHttpServer(_ => 500);
            var fetcher = new BrowserFetcher(
                new BrowserFetcherOptions
                {
                    Browser = SupportedBrowser.Chrome,
                    Path = _tmpDir,
                    Platform = Platform.Linux,
                    SkipChromeHeadlessShellDownload = true,
                    Retry = 1,
                }
            )
            {
                BaseUrl = server.Url.Replace("/test", string.Empty),
            };

            Assert.ThrowsAsync<PuppeteerException>(() =>
                fetcher.DownloadAsync(BrowserData.Chrome.DefaultBuildId)
            );
            Assert.That(server.Requests, Is.EqualTo(2));
        }

        [
            Test,
            PuppeteerTest(
                "CLI.test",
                "CLI",
                "rejects invalid --retry option values (partial: negative values only)"
            )
        ]
        public void Partial_RejectsInvalidRetryOptionValues() =>
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new BrowserFetcher(new BrowserFetcherOptions { Retry = -1 })
            );
    }
}
