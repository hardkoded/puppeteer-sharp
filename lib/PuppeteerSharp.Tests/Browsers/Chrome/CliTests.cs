using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.Chrome
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

        [Test, PuppeteerTest("CLI.spec", "Chrome CLI", "should download Chrome binaries")]
        public async Task ShouldDownloadChromeBinaries()
        {
            var fetcher = new BrowserFetcher(SupportedBrowser.Chrome)
            {
                CacheDir = _cacheDir,
                Platform = Platform.Linux
            };
            await fetcher.DownloadAsync(BrowserData.Chrome.DefaultBuildId);

            Assert.That(new FileInfo(Path.Combine(
                _cacheDir,
                "Chrome",
                $"Linux-{BrowserData.Chrome.DefaultBuildId}",
                "chrome-linux64",
                "chrome")).Exists, Is.True);
        }

        [Test]
        public async Task ShouldDownloadChromeLinuxArm64Binaries()
        {
            var fetcher = new BrowserFetcher(SupportedBrowser.Chrome)
            {
                CacheDir = _cacheDir,
                Platform = Platform.LinuxArm64
            };
            await fetcher.DownloadAsync(BrowserData.Chrome.DefaultBuildId);

            var executable = new FileInfo(Path.Combine(
                _cacheDir,
                "Chrome",
                $"LinuxArm64-{BrowserData.Chrome.DefaultBuildId}",
                "chrome-linux-arm64",
                "chrome"));
            Assert.That(executable.Exists, Is.True);

            // e_machine is a little-endian ushort at offset 18 of the ELF header; 0xB7 is EM_AARCH64.
            using var stream = executable.OpenRead();
            var header = new byte[20];
            Assert.That(stream.Read(header, 0, header.Length), Is.EqualTo(header.Length));
            Assert.That(header[18] | (header[19] << 8), Is.EqualTo(0xB7));
        }
    }
}
