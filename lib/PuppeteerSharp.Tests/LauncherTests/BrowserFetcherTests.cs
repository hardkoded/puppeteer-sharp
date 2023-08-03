using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Mono.Unix;
using PuppeteerSharp.Helpers.Linux;
using System.Collections.Generic;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.LauncherTests
{
    public class BrowserFetcherTests : PuppeteerBaseTest
    {
        private readonly string _downloadsFolder;

        public BrowserFetcherTests(): base()
        {
            _downloadsFolder = Path.Combine(Directory.GetCurrentDirectory(), ".test-chromium");
            EnsureDownloadsFolderIsDeleted();
        }

        [PuppeteerTest("launcher.spec.ts", "BrowserFetcher", "should download and extract chrome linux binary")]
        [PuppeteerTimeout]
        public async Task ShouldDownloadAndExtractLinuxBinary()
        {
            using var browserFetcher = Puppeteer.CreateBrowserFetcher(new BrowserFetcherOptions
            {
                Platform = Platform.Linux,
                Path = _downloadsFolder,
                Host = TestConstants.ServerUrl
            });
            var revisionInfo = browserFetcher.RevisionInfo("123456");

            Server.SetRedirect(revisionInfo.Url.Substring(TestConstants.ServerUrl.Length), "/chromium-linux.zip");
            Assert.False(revisionInfo.Local);
            Assert.AreEqual(Platform.Linux, revisionInfo.Platform);
            Assert.False(await browserFetcher.CanDownloadAsync("100000"));
            Assert.True(await browserFetcher.CanDownloadAsync("123456"));

            try
            {
                revisionInfo = await browserFetcher.DownloadAsync("123456");
                Assert.True(revisionInfo.Local);
                Assert.AreEqual("LINUX BINARY\n", File.ReadAllText(revisionInfo.ExecutablePath));

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
#if NETCOREAPP //This will not be run on net4x anyway.
                    Mono.Unix.FileAccessPermissions permissions = ConvertPermissions(LinuxSysCall.ExecutableFilePermissions);

                    Assert.AreEqual(permissions, UnixFileSystemInfo.GetFileSystemEntry(revisionInfo.ExecutablePath).FileAccessPermissions & permissions);
#endif
                }
                Assert.AreEqual(new[] { "123456" }, browserFetcher.LocalRevisions());
                browserFetcher.Remove("123456");
                Assert.IsEmpty(browserFetcher.LocalRevisions());

                //Download should return data from a downloaded version
                //This section is not in the Puppeteer test.
                await browserFetcher.DownloadAsync("123456");
                Server.Reset();
                revisionInfo = await browserFetcher.DownloadAsync("123456");
                Assert.True(revisionInfo.Local);
                Assert.AreEqual("LINUX BINARY\n", File.ReadAllText(revisionInfo.ExecutablePath));
            }
            finally
            {
                EnsureDownloadsFolderIsDeleted();
            }
        }

        [PuppeteerTest("launcher.spec.ts", "BrowserFetcher", "should download and extract firefox linux binary")]
        [Skip(SkipAttribute.Targets.OSX | SkipAttribute.Targets.Linux)]
        public async Task ShouldDownloadAndExtractFirefoxLinuxBinary()
        {
            using var browserFetcher = Puppeteer.CreateBrowserFetcher(new BrowserFetcherOptions
            {
                Platform = Platform.Linux,
                Path = _downloadsFolder,
                Host = TestConstants.ServerUrl,
                Product = Product.Firefox
            });
            var expectedVersion = "75.0a1";
            var revisionInfo = browserFetcher.RevisionInfo(expectedVersion);

            Server.SetRedirect(
                revisionInfo.Url.Substring(TestConstants.ServerUrl.Length),
                "/firefox.zip");
            Assert.False(revisionInfo.Local);
            Assert.AreEqual(Platform.Linux, revisionInfo.Platform);
            Assert.False(await browserFetcher.CanDownloadAsync("100000"));
            Assert.True(await browserFetcher.CanDownloadAsync(expectedVersion));

            try
            {
                revisionInfo = await browserFetcher.DownloadAsync(expectedVersion);
                Assert.True(revisionInfo.Local);
                Assert.AreEqual("FIREFOX LINUX BINARY\n", File.ReadAllText(revisionInfo.ExecutablePath));

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
#if NETCOREAPP //This will not be run on net4x anyway.
                    Mono.Unix.FileAccessPermissions permissions = ConvertPermissions(LinuxSysCall.ExecutableFilePermissions);

                    Assert.AreEqual(permissions, UnixFileSystemInfo.GetFileSystemEntry(revisionInfo.ExecutablePath).FileAccessPermissions & permissions);
#endif
                }
                Assert.AreEqual(new[] { expectedVersion }, browserFetcher.LocalRevisions());
                browserFetcher.Remove(expectedVersion);
                Assert.IsEmpty(browserFetcher.LocalRevisions());

                //Download should return data from a downloaded version
                //This section is not in the Puppeteer test.
                await browserFetcher.DownloadAsync(expectedVersion);
                Server.Reset();
                revisionInfo = await browserFetcher.DownloadAsync(expectedVersion);
                Assert.True(revisionInfo.Local);
                Assert.AreEqual("FIREFOX LINUX BINARY\n", File.ReadAllText(revisionInfo.ExecutablePath));
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

            var map = new Dictionary<Helpers.Linux.FileAccessPermissions, Mono.Unix.FileAccessPermissions>()
            {
                {Helpers.Linux.FileAccessPermissions.OtherExecute, Mono.Unix.FileAccessPermissions.OtherExecute},
                {Helpers.Linux.FileAccessPermissions.OtherWrite, Mono.Unix.FileAccessPermissions.OtherWrite},
                {Helpers.Linux.FileAccessPermissions.OtherRead, Mono.Unix.FileAccessPermissions.OtherRead},
                {Helpers.Linux.FileAccessPermissions.GroupExecute, Mono.Unix.FileAccessPermissions.GroupExecute},
                {Helpers.Linux.FileAccessPermissions.GroupWrite, Mono.Unix.FileAccessPermissions.GroupWrite},
                {Helpers.Linux.FileAccessPermissions.GroupRead, Mono.Unix.FileAccessPermissions.GroupRead},
                {Helpers.Linux.FileAccessPermissions.UserExecute, Mono.Unix.FileAccessPermissions.UserExecute},
                {Helpers.Linux.FileAccessPermissions.UserWrite, Mono.Unix.FileAccessPermissions.UserWrite},
                {Helpers.Linux.FileAccessPermissions.UserRead, Mono.Unix.FileAccessPermissions.UserRead}
            };

            foreach (var item in map.Keys)
            {
                if ((executableFilePermissions & item) == item)
                {
                    output |= map[item];
                }
            }

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
