using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.Firefox
{
    /// <summary>
    /// Puppeteer sharp doesn't have a CLI per se. But it matches the test we have upstream.
    /// </summary>
    public class CliTests
    {
        private readonly string _cacheDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        [SetUp]
        public void CreateDir()
            => new DirectoryInfo(_cacheDir).Create();

        [TearDown]
        public void DeleteDir()
            => new Cache(_cacheDir).Clear();

        [Test, Retry(2), PuppeteerTest("CLI.spec", "Chrome CLI", "should download Chrome binaries")]
        public async Task ShouldDownloadChromeBinaries()
        {
            using var fetcher = new BrowserFetcher(SupportedBrowser.Chrome)
            {
                CacheDir = _cacheDir,
                Platform = Platform.Linux
            };
            await fetcher.DownloadAsync(BrowserData.Chrome.DefaultBuildId);

            Assert.True(new FileInfo(Path.Combine(
                _cacheDir,
                "Chrome",
                $"Linux-{BrowserData.Chrome.DefaultBuildId}",
                "chrome-linux64",
                "chrome")).Exists);
        }
    }
}
