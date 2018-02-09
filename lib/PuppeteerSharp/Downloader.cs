using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Downloader
    {
        private readonly string _downloadsFolder;
        private const string DefaultDownloadHost = "https://storage.googleapis.com";
        private static readonly Dictionary<Platform, string> _downloadUrls = new Dictionary<Platform, string> {
            {Platform.Linux, "{0}/chromium-browser-snapshots/Linux_x64/{1}/chrome-linux.zip"},
            {Platform.MacOS, "{0}/chromium-browser-snapshots/Mac/{1}/chrome-mac.zip"},
            {Platform.Win32, "{0}/chromium-browser-snapshots/Win/{1}/chrome-win32.zip"},
            {Platform.Win64, "{0}/chromium-browser-snapshots/Win_x64/{1}/chrome-win32.zip"}
        };

        private string _downloadHost;

        public Downloader(string downloadsFolder)
        {
            _downloadsFolder = downloadsFolder;
            _downloadHost = DefaultDownloadHost;
        }

        #region Public Methods
        public static Downloader CreateDefault()
        {
            var downloadsFolder = Path.Combine(Directory.GetCurrentDirectory(), ".local-chromium");
            return new Downloader(downloadsFolder);
        }

        internal static Platform CurrentPlatform
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return Platform.MacOS;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    return Platform.Linux;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return IntPtr.Size == 8 ? Platform.Win64 : Platform.Win32;
                return Platform.Unknown;
            }
        }

        internal RevisionInfo RevisionInfo(Platform platform, int revision)
        {
            var result = new RevisionInfo
            {
                FolderPath = GetFolderPath(platform, revision),
                Revision = revision
            };
            result.ExecutablePath = GetExecutablePath(platform, result.FolderPath);
            return result;
        }

        public async Task DownloadRevisionAsync(int revision)
        {
            var url = string.Format(_downloadUrls[CurrentPlatform], _downloadHost, revision);
            var zipPath = Path.Combine(_downloadsFolder, $"download-{CurrentPlatform.ToString()}-{revision}.zip");
            var folderPath = GetFolderPath(CurrentPlatform, revision);

            if (new DirectoryInfo(folderPath).Exists)
            {
                return;
            }

            var downloadFolder = new DirectoryInfo(_downloadsFolder);
            if (!downloadFolder.Exists)
            {
                downloadFolder.Create();
            }

            await new WebClient().DownloadFileTaskAsync(new Uri(url), zipPath);

            if (CurrentPlatform == Platform.MacOS)
            {
                //ZipFile and many others unzip libraries have issues extracting .app files
                //Until we have a clear solution we'll call the native unzip tool
                //https://github.com/dotnet/corefx/issues/15516
                NativeExtractToDirectory(zipPath, folderPath);
            }
            else
            {
                ZipFile.ExtractToDirectory(zipPath, folderPath);
            }

            new FileInfo(zipPath).Delete();

        }

        public string GetExecutablePath(int revision)
        {
            return GetExecutablePath(CurrentPlatform, GetFolderPath(CurrentPlatform, revision));
        }


        public static string GetExecutablePath(Platform platform, string folderPath)
        {
            switch (platform)
            {
                case Platform.MacOS:
                    return Path.Combine(folderPath, "chrome-mac", "Chromium.app", "Contents",
                                                         "MacOS", "Chromium");
                case Platform.Linux:
                    return Path.Combine(folderPath, "chrome-linux", "chrome");
                case Platform.Win32:
                case Platform.Win64:
                    return Path.Combine(folderPath, "chrome-win32", "chrome.exe");
                default:
                    throw new ArgumentException("Invalid platform", nameof(platform));
            }
        }


        #endregion

        #region Private Methods

        private string GetFolderPath(Platform platform, int revision)
        {
            return Path.Combine(_downloadsFolder, $"{platform.ToString()}-{revision}");
        }

        private void NativeExtractToDirectory(string zipPath, string folderPath)
        {
            var process = new Process();
            process.StartInfo.FileName = "unzip";
            process.StartInfo.Arguments = $"{zipPath} -d {folderPath}";
            process.Start();
            process.WaitForExit();
        }

        #endregion
    }
}
