using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PuppeteerSharp
{
    public class Downloader
    {
        private string _downloadsFolder;
        private const string DefaultDownloadHost = "https://storage.googleapis.com";
        private string _downloadHost;

        public Downloader(string downloadsFolder)
        {
            _downloadsFolder = downloadsFolder;
            _downloadHost = DefaultDownloadHost;
        }

        public static Downloader CreateDefault()
        {
            var downloadsFolder = Path.Combine(Directory.GetCurrentDirectory(), ".local-chromium");
            return new Downloader(downloadsFolder);
        }

        internal static Platform CurrentPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return Platform.MacOS;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return Platform.Linux;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return IntPtr.Size == 8 ? Platform.Win64 : Platform.Win32;
            return Platform.Unknown;
        }

        internal RevisionInfo RevisionInfo(Platform platform, string revision)
        {
            var result = new RevisionInfo
            {
                FolderPath = GetFolderPath(platform, revision),
                Revision = revision
            };
            result.ExecutablePath = GetExecutablePath(platform, result.FolderPath);
            return result;
        }

        private static string GetExecutablePath(Platform platform, string folderPath)
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

        private string GetFolderPath(Platform platform, string revision)
        {
            return Path.Combine(_downloadsFolder, $"{platform.ToString()}-{revision}");
        }
    }
}
