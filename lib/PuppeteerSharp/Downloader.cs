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

        internal static string CurrentPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "mac";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return IntPtr.Size == 8 ? "win64" : "win32";
            return string.Empty;
        }
    }
}
