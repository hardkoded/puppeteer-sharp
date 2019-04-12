using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Mono.Unix;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Helpers.Linux;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class BrowserFetcherTests : PuppeteerBaseTest
    {
        private readonly string _downloadsFolder;

        public BrowserFetcherTests(ITestOutputHelper output) : base(output)
        {
            _downloadsFolder = Path.Combine(Directory.GetCurrentDirectory(), ".test-chromium");
            EnsureDownloadsFolderIsDeleted();
        }

        [Fact]
        public async Task ShouldDownloadAndExtractLinuxBinary()
        {
            var browserFetcher = Puppeteer.CreateBrowserFetcher(new BrowserFetcherOptions
            {
                Platform = Platform.Linux,
                Path = _downloadsFolder,
                Host = TestConstants.ServerUrl
            });
            var revisionInfo = browserFetcher.RevisionInfo(123456);

            Server.SetRedirect(revisionInfo.Url.Substring(TestConstants.ServerUrl.Length), "/chromium-linux.zip");
            Assert.False(revisionInfo.Local);
            Assert.Equal(Platform.Linux, revisionInfo.Platform);
            Assert.False(await browserFetcher.CanDownloadAsync(100000));
            Assert.True(await browserFetcher.CanDownloadAsync(123456));

            try
            {
                revisionInfo = await browserFetcher.DownloadAsync(123456);
                Assert.True(revisionInfo.Local);
                Assert.Equal("LINUX BINARY\n", File.ReadAllText(revisionInfo.ExecutablePath));

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
#if NETCOREAPP //This will not be run on net4x anyway.
                    Mono.Unix.FileAccessPermissions permissions = ConvertPermissions(LinuxSysCall.ExecutableFilePermissions);

                    Assert.Equal(permissions,UnixFileSystemInfo.GetFileSystemEntry(revisionInfo.ExecutablePath).FileAccessPermissions & permissions);
#endif
                }
                Assert.Equal(new[] { 123456 }, browserFetcher.LocalRevisions());
                browserFetcher.Remove(123456);
                Assert.Empty(browserFetcher.LocalRevisions());

                //Download should return data from a downloaded version
                //This section is not in the Puppeteer test.
                await browserFetcher.DownloadAsync(123456);
                Server.Reset();
                revisionInfo = await browserFetcher.DownloadAsync(123456);
                Assert.True(revisionInfo.Local);
                Assert.Equal("LINUX BINARY\n", File.ReadAllText(revisionInfo.ExecutablePath));
            }
            finally
            {
                EnsureDownloadsFolderIsDeleted();
            }
        }

#if NETCOREAPP
        private Mono.Unix.FileAccessPermissions ConvertPermissions(Helpers.Linux.FileAccessPermissions executableFilePermissions)
        {
            Mono.Unix.FileAccessPermissions output = 0;

            void AddIfThere(Helpers.Linux.FileAccessPermissions source, Mono.Unix.FileAccessPermissions dest)
            {
                if ((executableFilePermissions & source) == source)
                {
                    output |= dest;
                }
            }

            AddIfThere(Helpers.Linux.FileAccessPermissions.OtherExecute, Mono.Unix.FileAccessPermissions.OtherExecute);
            AddIfThere(Helpers.Linux.FileAccessPermissions.OtherWrite, Mono.Unix.FileAccessPermissions.OtherWrite);
            AddIfThere(Helpers.Linux.FileAccessPermissions.OtherRead, Mono.Unix.FileAccessPermissions.OtherRead);
            AddIfThere(Helpers.Linux.FileAccessPermissions.GroupExecute, Mono.Unix.FileAccessPermissions.GroupExecute);
            AddIfThere(Helpers.Linux.FileAccessPermissions.GroupWrite, Mono.Unix.FileAccessPermissions.GroupWrite);
            AddIfThere(Helpers.Linux.FileAccessPermissions.GroupRead, Mono.Unix.FileAccessPermissions.GroupRead);
            AddIfThere(Helpers.Linux.FileAccessPermissions.UserExecute, Mono.Unix.FileAccessPermissions.UserExecute);
            AddIfThere(Helpers.Linux.FileAccessPermissions.UserWrite, Mono.Unix.FileAccessPermissions.UserWrite);
            AddIfThere(Helpers.Linux.FileAccessPermissions.UserRead, Mono.Unix.FileAccessPermissions.UserRead);

            return output;
        }
#endif

        private void EnsureDownloadsFolderIsDeleted()
        {
            if (Directory.Exists(_downloadsFolder))
            {
                Directory.Delete(_downloadsFolder, true);
            }
        }
    }
}