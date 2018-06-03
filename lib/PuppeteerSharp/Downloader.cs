using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Downloader class used to download a chromium version from Google.
    /// </summary>
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

        /// <summary>
        /// Default chromiumg revision.
        /// </summary>
        public const int DefaultRevision = 526987;

        /// <summary>
        /// Initializes a new instance of the <see cref="Downloader"/> class.
        /// </summary>
        /// <param name="downloadsFolder">Downloads folder.</param>
        public Downloader(string downloadsFolder)
        {
            _downloadsFolder = downloadsFolder;
            _downloadHost = DefaultDownloadHost;
        }

        #region Public Methods

        /// <summary>
        /// Creates a <see cref="Downloader"/> class specifing a default download folder.
        /// </summary>
        /// <returns>A new downloader.</returns>
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
                {
                    return Platform.MacOS;
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Platform.Linux;
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return IntPtr.Size == 8 ? Platform.Win64 : Platform.Win32;
                }

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

        /// <summary>
        /// Downloads the revision.
        /// </summary>
        /// <returns>Task which resolves to the completed download.</returns>
        /// <param name="revision">Revision.</param>
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

        /// <summary>
        /// Gets the executable path for a revision.
        /// </summary>
        /// <returns>The executable path.</returns>
        /// <param name="revision">Revision.</param>
        public string GetExecutablePath(int revision)
        {
            return GetExecutablePath(CurrentPlatform, GetFolderPath(CurrentPlatform, revision));
        }

        /// <summary>
        /// Gets the executable path.
        /// </summary>
        /// <returns>The executable path.</returns>
        /// <param name="platform">Platform.</param>
        /// <param name="folderPath">Folder path.</param>
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
