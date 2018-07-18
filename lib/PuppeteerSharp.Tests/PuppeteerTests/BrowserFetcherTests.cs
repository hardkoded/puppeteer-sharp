using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class BrowserFetcherTests : PuppeteerBaseTest
    {
        public BrowserFetcherTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task ShouldDownloadAndExtractLinuxBinary()
        {
            var downloadsFolder = Path.Combine(Directory.GetCurrentDirectory(), ".test-chromium");
            var browserFetcher = Puppeteer.CreateBrowserFetcher(new BrowserFetcherOptions
            {
                Platform = Platform.Linux,
                Path = downloadsFolder,
                Host = TestConstants.ServerUrl
            });
            var revisionInfo = browserFetcher.RevisionInfo(123456);

            Server.SetRedirect(revisionInfo.Url.Substring(TestConstants.ServerUrl.Length), "/chromium-linux.zip");
            Assert.False(revisionInfo.Local);
            Assert.Equal(Platform.Linux, revisionInfo.Platform);
            Assert.False(await browserFetcher.CanDownloadAsync(100000));
            Assert.True(await browserFetcher.CanDownloadAsync(123456));

            revisionInfo = await browserFetcher.DownloadAsync(123456);
            Assert.True(revisionInfo.Local);
            Assert.Equal("LINUX BINARY\n", File.ReadAllText(revisionInfo.ExecutablePath));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Mono.Unix.Native.Syscall.stat(revisionInfo.ExecutablePath, out var stat);
                Assert.Equal(BrowserFetcher.BrowserPermissionsInLinux, stat.st_mode & BrowserFetcher.BrowserPermissionsInLinux);
            }
            Assert.Equal(new[] { 123456 }, browserFetcher.LocalRevisions());
            browserFetcher.Remove(123456);
            Assert.Empty(browserFetcher.LocalRevisions());

            new DirectoryInfo(downloadsFolder).Delete(true);
        }
    }
}