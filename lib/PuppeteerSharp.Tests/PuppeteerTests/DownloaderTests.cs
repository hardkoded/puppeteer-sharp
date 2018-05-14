using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    public class DownloaderTests
    {
        [Fact(Skip = "Long run")]
        public async Task ShouldDownloadChromium()
        {
            var downloadsFolder = Path.Combine(Directory.GetCurrentDirectory(), ".test-chromium");
            var dirInfo = new DirectoryInfo(downloadsFolder);
            var downloader = new Downloader(downloadsFolder);

            if (dirInfo.Exists)
            {
                dirInfo.Delete(true);
            }

            await downloader.DownloadRevisionAsync(TestConstants.ChromiumRevision);
            Assert.True(new FileInfo(downloader.GetExecutablePath(TestConstants.ChromiumRevision)).Exists);
        }
    }
}
