using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Net;
using System.IO.Compression;

namespace PuppeteerSharp
{
    /// <summary>
    /// BrowserFetcher can download and manage different versions of Chromium.
    /// BrowserFetcher operates on revision strings that specify a precise version of Chromium, e.g. 533271. Revision strings can be obtained from omahaproxy.appspot.com.
    /// </summary>
    /// <example>
    /// Example on how to use BrowserFetcher to download a specific version of Chromium and run Puppeteer against it:
    /// <code>
    /// var browserFetcher = Puppeteer.CreateBrowserFetcher();
    /// var revisionInfo = await browserFetcher.DownloadAsync(533271);
    /// var browser = await await Puppeteer.LaunchAsync(new LaunchOptions { ExecutablePath = revisionInfo.ExecutablePath});
    /// </code>
    /// </example>
    public class BrowserFetcher
    {
        private const string DefaultDownloadHost = "https://storage.googleapis.com";
        private static readonly Dictionary<Platform, string> _downloadUrls = new Dictionary<Platform, string> {
            {Platform.Linux, "{0}/chromium-browser-snapshots/Linux_x64/{1}/chrome-linux.zip"},
            {Platform.MacOS, "{0}/chromium-browser-snapshots/Mac/{1}/chrome-mac.zip"},
            {Platform.Win32, "{0}/chromium-browser-snapshots/Win/{1}/chrome-win32.zip"},
            {Platform.Win64, "{0}/chromium-browser-snapshots/Win_x64/{1}/chrome-win32.zip"}
        };

        /// <summary>
        /// Default chromiumg revision.
        /// </summary>
        public const int DefaultRevision = 571375;

        /// <summary>
        /// Gets the downloads folder.
        /// </summary>
        /// <value>The downloads folder.</value>
        public string DownloadsFolder { get; }
        /// <summary>
        /// A download host to be used. Defaults to https://storage.googleapis.com.
        /// </summary>
        /// <value>The download host.</value>
        public string DownloadHost { get; }
        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <value>The platform.</value>
        public Platform Platform { get; }
        /// <summary>
        /// Occurs when download progress in <see cref="DownloadAsync(int)"/> changes.
        /// </summary>
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserFetcher"/> class.
        /// </summary>
        public BrowserFetcher()
        {
            DownloadsFolder = Path.Combine(Directory.GetCurrentDirectory(), ".local-chromium");
            DownloadHost = DefaultDownloadHost;
            Platform = GetCurrentPlatform();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserFetcher"/> class.
        /// </summary>
        /// <param name="options">Fetch options.</param>
        public BrowserFetcher(BrowserFetcherOptions options)
        {
            DownloadsFolder = string.IsNullOrEmpty(options.Path) ?
               Path.Combine(Directory.GetCurrentDirectory(), ".local-chromium") :
               options.Path;
            DownloadHost = string.IsNullOrEmpty(options.Host) ? DefaultDownloadHost : options.Host;
            Platform = options.Platform ?? GetCurrentPlatform();
        }

        #region Public Methods

        /// <summary>
        /// The method initiates a HEAD request to check if the revision is available.
        /// </summary>
        /// <returns>Whether the version is available or not.</returns>
        /// <param name="revision">A revision to check availability.</param>
        public async Task<bool> CanDownloadAsync(int revision)
        {
            var url = string.Format(_downloadUrls[Platform], DownloadHost, revision);

            var client = new HttpClient();
            var response = await client.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Head
            }).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// A list of all revisions available locally on disk.
        /// </summary>
        /// <returns>The available revisions.</returns>
        public IEnumerable<int> LocalRevisions()
        {
            var directoryInfo = new DirectoryInfo(DownloadsFolder);

            if (directoryInfo.Exists)
            {
                return directoryInfo.GetDirectories().Select(d => GetRevisionFromPath(d.Name)).Where(v => v > 0);
            }

            return new int[] { };
        }

        /// <summary>
        /// Removes a downloaded revision.
        /// </summary>
        /// <param name="revision">Revision to remove.</param>
        public void Remove(int revision)
        {
            var directory = new DirectoryInfo(GetFolderPath(revision));
            if (directory.Exists)
            {
                directory.Delete(true);
            }
        }

        /// <summary>
        /// Gets the revision info.
        /// </summary>
        /// <returns>Revision info.</returns>
        /// <param name="revision">A revision to get info for.</param>
        public RevisionInfo RevisionInfo(int revision)
        {
            var result = new RevisionInfo
            {
                FolderPath = GetFolderPath(revision),
                Url = string.Format(_downloadUrls[Platform], DownloadHost, revision),
                Revision = revision,
                Platform = Platform
            };
            result.ExecutablePath = GetExecutablePath(Platform, result.FolderPath);
            result.Local = new DirectoryInfo(result.FolderPath).Exists;

            return result;
        }

        /// <summary>
        /// Downloads the revision.
        /// </summary>
        /// <returns>Task which resolves to the completed download.</returns>
        /// <param name="revision">Revision.</param>
        public async Task<RevisionInfo> DownloadAsync(int revision)
        {
            var url = string.Format(_downloadUrls[Platform], DownloadHost, revision);
            var zipPath = Path.Combine(DownloadsFolder, $"download-{Platform.ToString()}-{revision}.zip");
            var folderPath = GetFolderPath(revision);

            if (new DirectoryInfo(folderPath).Exists)
            {
                return RevisionInfo(revision);
            }

            var downloadFolder = new DirectoryInfo(DownloadsFolder);
            if (!downloadFolder.Exists)
            {
                downloadFolder.Create();
            }

            var webClient = new WebClient();

            if (DownloadProgressChanged != null)
            {
                webClient.DownloadProgressChanged += DownloadProgressChanged;
            }
            await webClient.DownloadFileTaskAsync(new Uri(url), zipPath).ConfigureAwait(false);

            if (Platform == Platform.MacOS)
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

            return RevisionInfo(revision);
        }

        /// <summary>
        /// Gets the executable path for a revision.
        /// </summary>
        /// <returns>The executable path.</returns>
        /// <param name="revision">Revision.</param>
        public string GetExecutablePath(int revision)
        {
            return GetExecutablePath(Platform, GetFolderPath(revision));
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

        private static Platform GetCurrentPlatform()
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
                return RuntimeInformation.OSArchitecture == Architecture.X64 ? Platform.Win64 : Platform.Win32;
            }

            return Platform.Unknown;
        }
        private string GetFolderPath(int revision)
        {
            return Path.Combine(DownloadsFolder, $"{Platform.ToString()}-{revision}");
        }

        private void NativeExtractToDirectory(string zipPath, string folderPath)
        {
            var process = new Process();
            process.StartInfo.FileName = "unzip";
            process.StartInfo.Arguments = $"{zipPath} -d {folderPath}";
            process.Start();
            process.WaitForExit();
        }

        private int GetRevisionFromPath(string folderName)
        {
            var splits = folderName.Split('-');
            if (splits.Length != 2)
            {
                return 0;
            }
            Platform platform;
            if (!Enum.TryParse<Platform>(splits[0], out platform))
            {
                platform = Platform.Unknown;
            }
            if (!_downloadUrls.Keys.Contains(platform))
            {
                return 0;
            }
            int.TryParse(splits[1], out var revision);
            return revision;
        }

        #endregion
    }
}