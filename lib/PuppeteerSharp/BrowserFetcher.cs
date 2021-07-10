using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;
using Newtonsoft.Json;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Linux;

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
        private static readonly Dictionary<Product, string> _hosts = new Dictionary<Product, string>
        {
            [Product.Chrome] = "https://storage.googleapis.com",
            [Product.Firefox] = "https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central",
        };

        private static readonly Dictionary<(Product product, Platform platform), string> _downloadUrls = new Dictionary<(Product product, Platform platform), string>
        {
            [(Product.Chrome, Platform.Linux)] = "{0}/chromium-browser-snapshots/Linux_x64/{1}/{2}.zip",
            [(Product.Chrome, Platform.MacOS)] = "{0}/chromium-browser-snapshots/Mac/{1}/{2}.zip",
            [(Product.Chrome, Platform.Win32)] = "{0}/chromium-browser-snapshots/Win/{1}/{2}.zip",
            [(Product.Chrome, Platform.Win64)] = "{0}/chromium-browser-snapshots/Win_x64/{1}/{2}.zip",
            [(Product.Firefox, Platform.Linux)] = "{0}/firefox-{1}.en-US.{2}-x86_64.tar.bz2",
            [(Product.Firefox, Platform.MacOS)] = "{0}/firefox-{1}.en-US.{2}.dmg",
            [(Product.Firefox, Platform.Win32)] = "{0}/firefox-{1}.en-US.{2}.zip",
            [(Product.Firefox, Platform.Win64)] = "{0}/firefox-{1}.en-US.{2}.zip",
        };

        private readonly WebClient _webClient = new WebClient();

        /// <summary>
        /// Default Chromium revision.
        /// </summary>
        [Obsolete("Use DefaultChromiumRevision instead")]
        public static int DefaultRevision { get; } = int.Parse(DefaultChromiumRevision, CultureInfo.CurrentCulture.NumberFormat);

        /// <summary>
        /// Default Chromium revision.
        /// </summary>
        public const string DefaultChromiumRevision = "884014";

        /// <summary>
        /// Default Chromium revision.
        /// </summary>
        public string DefaultFirefoxRevision { get; private set; } = "latest";

        /// <summary>
        /// Gets the downloads folder.
        /// </summary>
        public string DownloadsFolder { get; }

        /// <summary>
        /// A download host to be used. Defaults to https://storage.googleapis.com.
        /// </summary>
        public string DownloadHost { get; }

        /// <summary>
        /// Gets the platform.
        /// </summary>
        public Platform Platform { get; }

        /// <summary>
        /// Gets the product.
        /// </summary>
        public Product Product { get; }

        /// <summary>
        /// Proxy used by the WebClient in <see cref="DownloadAsync(int)"/> and <see cref="CanDownloadAsync(int)"/>
        /// </summary>
        public IWebProxy WebProxy
        {
            get => _webClient.Proxy;
            set => _webClient.Proxy = value;
        }

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
            DownloadHost = _hosts[Product.Chrome];
            Platform = GetCurrentPlatform();
            Product = Product.Chrome;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserFetcher"/> class.
        /// </summary>
        /// <param name="product">Product.</param>
        public BrowserFetcher(Product product) : this(new BrowserFetcherOptions { Product = product })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserFetcher"/> class.
        /// </summary>
        /// <param name="options">Fetch options.</param>
        public BrowserFetcher(BrowserFetcherOptions options)
        {
            DownloadsFolder = string.IsNullOrEmpty(options.Path) ?
               Path.Combine(Directory.GetCurrentDirectory(), options.Product == Product.Chrome ? ".local-chromium" : ".local-firefox") :
               options.Path;
            DownloadHost = string.IsNullOrEmpty(options.Host) ? _hosts[options.Product] : options.Host;
            Platform = options.Platform ?? GetCurrentPlatform();
            Product = options.Product;
        }

        /// <summary>
        /// The method initiates a HEAD request to check if the revision is available.
        /// </summary>
        /// <returns>Whether the version is available or not.</returns>
        /// <param name="revision">A revision to check availability.</param>
        [Obsolete("Use CanDownloadAsync(string revision) instead")]
        public Task<bool> CanDownloadAsync(int revision) => CanDownloadAsync(revision.ToString(CultureInfo.CurrentCulture.NumberFormat));

        /// <summary>
        /// The method initiates a HEAD request to check if the revision is available.
        /// </summary>
        /// <returns>Whether the version is available or not.</returns>
        /// <param name="revision">A revision to check availability.</param>
        public async Task<bool> CanDownloadAsync(string revision)
        {
            try
            {
                var url = GetDownloadURL(Product, Platform, DownloadHost, revision);

                var client = WebRequest.Create(url);
                client.Proxy = _webClient.Proxy;
                client.Method = "HEAD";
                using (var response = (HttpWebResponse)await client.GetResponseAsync().ConfigureAwait(false))
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }

        /// <summary>
        /// A list of all revisions available locally on disk.
        /// </summary>
        /// <returns>The available revisions.</returns>
        public IEnumerable<string> LocalRevisions()
        {
            var directoryInfo = new DirectoryInfo(DownloadsFolder);

            if (directoryInfo.Exists)
            {
                return directoryInfo.GetDirectories().Select(d => GetRevisionFromPath(d.Name));
            }

            return Array.Empty<string>();
        }

        /// <summary>
        /// Removes a downloaded revision.
        /// </summary>
        /// <param name="revision">Revision to remove.</param>
        [Obsolete("Use remove(string revision) instead")]
        public void Remove(int revision) => Remove(revision.ToString(CultureInfo.CurrentCulture.NumberFormat));

        /// <summary>
        /// Removes a downloaded revision.
        /// </summary>
        /// <param name="revision">Revision to remove.</param>
        public void Remove(string revision)
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
        [Obsolete("Use RevisionInfo(string revision) instead")]
        public RevisionInfo RevisionInfo(int revision) => RevisionInfo(revision.ToString(CultureInfo.CurrentCulture.NumberFormat));

        /// <summary>
        /// Gets the revision info.
        /// </summary>
        /// <returns>Revision info.</returns>
        public async Task<RevisionInfo> GetRevisionInfoAsync()
            => RevisionInfo(Product == Product.Chrome ? DefaultChromiumRevision : await GetDefaultFirefoxRevisionAsync().ConfigureAwait(false));

        /// <summary>
        /// Gets the revision info.
        /// </summary>
        /// <returns>Revision info.</returns>
        /// <param name="revision">A revision to get info for.</param>
        public RevisionInfo RevisionInfo(string revision)
        {
            var result = new RevisionInfo
            {
                FolderPath = GetFolderPath(revision),
                Url = GetDownloadURL(Product, Platform, DownloadHost, revision),
                Revision = revision,
                Platform = Platform
            };
            result.ExecutablePath = GetExecutablePath(Product, Platform, revision, result.FolderPath);
            result.Local = new DirectoryInfo(result.FolderPath).Exists;

            return result;
        }

        /// <summary>
        /// Downloads the revision.
        /// </summary>
        /// <returns>Task which resolves to the completed download.</returns>
        /// <param name="revision">Revision.</param>
        [Obsolete("Use DownloadAsync(string revision) instead")]
        public Task<RevisionInfo> DownloadAsync(int revision) => DownloadAsync(revision.ToString(CultureInfo.CurrentCulture.NumberFormat));

        /// <summary>
        /// Downloads the revision.
        /// </summary>
        /// <returns>Task which resolves to the completed download.</returns>
        public async Task<RevisionInfo> DownloadAsync()
            => await DownloadAsync(
                Product == Product.Chrome
                ? DefaultChromiumRevision
                : await GetDefaultFirefoxRevisionAsync().ConfigureAwait(false)).ConfigureAwait(false);

        /// <summary>
        /// Downloads the revision.
        /// </summary>
        /// <returns>Task which resolves to the completed download.</returns>
        /// <param name="revision">Revision.</param>
        public async Task<RevisionInfo> DownloadAsync(string revision)
        {
            var url = GetDownloadURL(Product, Platform, DownloadHost, revision);
            var filePath = Path.Combine(DownloadsFolder, url.Split('/').Last());
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

            if (DownloadProgressChanged != null)
            {
                _webClient.DownloadProgressChanged += DownloadProgressChanged;
            }

            await _webClient.DownloadFileTaskAsync(url, filePath).ConfigureAwait(false);

            if (filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                if (Platform == Platform.MacOS)
                {
                    NativeExtractToDirectory(filePath, folderPath);
                }
                else
                {
                    ZipFile.ExtractToDirectory(filePath, folderPath);
                }
            }
            else if (filePath.EndsWith(".tar.bz2", StringComparison.OrdinalIgnoreCase))
            {
                ExtractTar(filePath, folderPath);
            }
            else
            {
                await InstallDMGAsync(filePath, folderPath).ConfigureAwait(false);
            }

            new FileInfo(filePath).Delete();

            var revisionInfo = RevisionInfo(revision);

            if (revisionInfo != null && GetCurrentPlatform() == Platform.Linux)
            {
                var execPath = revisionInfo.ExecutablePath;
                var dirName = Path.GetDirectoryName(execPath);

                var code = LinuxSysCall.Chmod(execPath, LinuxSysCall.ExecutableFilePermissions);
                if (code != 0)
                {
                    throw new Exception("Chmod operation failed");
                }

                var naclPath = $"{dirName}/nacl_helper";
                if (File.Exists(naclPath))
                {
                    code = LinuxSysCall.Chmod(naclPath, LinuxSysCall.ExecutableFilePermissions);
                    if (code != 0)
                    {
                        throw new Exception("Chmod operation failed");
                    }
                }
            }

            return revisionInfo;
        }

        private Task InstallDMGAsync(string dmgPath, string folderPath)
        {
            try
            {
                var destinationDirectoryInfo = new DirectoryInfo(folderPath);

                if (!destinationDirectoryInfo.Exists)
                {
                    destinationDirectoryInfo.Create();
                }

                var mountAndCopyTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                using var process = new Process
                {
                    EnableRaisingEvents = true
                };

                process.StartInfo.FileName = "hdiutil";
                process.StartInfo.Arguments = $"attach -nobrowse -noautoopen \"{dmgPath}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null || mountAndCopyTcs.Task.IsCompleted)
                    {
                        return;
                    }

                    var volumes = new Regex("\\/Volumes\\/(.*)").Match(e.Data);

                    if (!volumes.Success)
                    {
                        return;
                    }

                    var mountPath = volumes.Captures[0];
                    var appFile = new DirectoryInfo(mountPath.Value).GetDirectories("*.app").FirstOrDefault();

                    if (appFile == null)
                    {
                        mountAndCopyTcs.TrySetException(new PuppeteerException($"Cannot find app in {mountPath.Value}"));
                        return;
                    }

                    using var process = new Process();
                    process.StartInfo.FileName = "cp";
                    process.StartInfo.Arguments = $"-R \"{appFile.FullName}\" \"{folderPath}\"";
                    process.Start();
                    process.WaitForExit();
                    mountAndCopyTcs.TrySetResult(true);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                return mountAndCopyTcs.Task.WithTimeout(Puppeteer.DefaultTimeout);
            }
            finally
            {
                UnmountDmg(dmgPath);
            }
        }

        private void UnmountDmg(string dmgPath)
        {
            try
            {
                using var process = new Process();
                process.StartInfo.FileName = "hdiutil";
                process.StartInfo.Arguments = $"detach \"{ dmgPath}\" -quiet";
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit();
            }
            catch
            {
                // swallow
            }
        }

        private static void ExtractTar(string zipPath, string folderPath)
        {
            new DirectoryInfo(folderPath).Create();
            using var process = new Process();
            process.StartInfo.FileName = "tar";
            process.StartInfo.Arguments = $"-xvjf \"{zipPath}\" -C \"{folderPath}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// Gets the executable path for a revision.
        /// </summary>
        /// <returns>The executable path.</returns>
        /// <param name="revision">Revision.</param>
        [Obsolete("Use GetExecutablePath(string revision) instead")]
        public string GetExecutablePath(int revision) => GetExecutablePath(revision.ToString(CultureInfo.CurrentCulture.NumberFormat));

        /// <summary>
        /// Gets the executable path for a revision.
        /// </summary>
        /// <returns>The executable path.</returns>
        /// <param name="revision">Revision.</param>
        public string GetExecutablePath(string revision)
            => GetExecutablePath(Product, Platform, revision, GetFolderPath(revision));

        /// <summary>
        /// Gets the executable path.
        /// </summary>
        /// <returns>The executable path.</returns>
        /// <param name="product">Product.</param>
        /// <param name="platform">Platform.</param>
        /// <param name="revision">Revision.</param>
        /// <param name="folderPath">Folder path.</param>
        [Obsolete("Use GetExecutablePath(string product, Platform platform, string revision, string folderPath) instead")]
        public static string GetExecutablePath(Product product, Platform platform, int revision, string folderPath)
            => GetExecutablePath(product, platform, revision.ToString(CultureInfo.CurrentCulture.NumberFormat), folderPath);

        /// <summary>
        /// Gets the executable path.
        /// </summary>
        /// <returns>The executable path.</returns>
        /// <param name="product">Product.</param>
        /// <param name="platform">Platform.</param>
        /// <param name="revision">Revision.</param>
        /// <param name="folderPath">Folder path.</param>
        public static string GetExecutablePath(Product product, Platform platform, string revision, string folderPath)
        {
            if (product == Product.Chrome)
            {
                switch (platform)
                {
                    case Platform.MacOS:
                        return Path.Combine(
                            folderPath,
                            GetArchiveName(product, platform, revision),
                            "Chromium.app",
                            "Contents",
                            "MacOS",
                            "Chromium");
                    case Platform.Linux:
                        return Path.Combine(folderPath, GetArchiveName(product, platform, revision), "chrome");
                    case Platform.Win32:
                    case Platform.Win64:
                        return Path.Combine(folderPath, GetArchiveName(product, platform, revision), "chrome.exe");
                    default:
                        throw new ArgumentException("Invalid platform", nameof(platform));
                }
            }
            else
            {
                switch (platform)
                {
                    case Platform.MacOS:
                        return Path.Combine(
                            folderPath,
                            "Firefox Nightly.app",
                            "Contents",
                            "MacOS",
                            "firefox");
                    case Platform.Linux:
                        return Path.Combine(folderPath, "firefox", "firefox");
                    case Platform.Win32:
                    case Platform.Win64:
                        return Path.Combine(folderPath, "firefox", "firefox.exe");
                    default:
                        throw new ArgumentException("Invalid platform", nameof(platform));
                }
            }
        }

        internal static Platform GetCurrentPlatform()
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

        private string GetFolderPath(string revision)
            => Path.Combine(DownloadsFolder, $"{Platform}-{revision}");

        private void NativeExtractToDirectory(string zipPath, string folderPath)
        {
            using var process = new Process();
            process.StartInfo.FileName = "unzip";
            process.StartInfo.Arguments = $"\"{zipPath}\" -d \"{folderPath}\"";
            process.Start();
            process.WaitForExit();
        }

        private string GetRevisionFromPath(string folderName)
        {
            var splits = folderName.Split('-');
            if (splits.Length != 2)
            {
                return "0";
            }

            if (!Enum.TryParse<Platform>(splits[0], out var platform))
            {
                platform = Platform.Unknown;
            }

            if (!_downloadUrls.Keys.Contains((Product, platform)))
            {
                return "0";
            }

            return splits[1];
        }

        private async Task<string> GetDefaultFirefoxRevisionAsync()
        {
            if (DefaultFirefoxRevision == "latest")
            {
                using var client = new HttpClient();
                var response = await client.GetStringAsync("https://product-details.mozilla.org/1.0/firefox_versions.json").ConfigureAwait(false);
                var version = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
                DefaultFirefoxRevision = version["FIREFOX_NIGHTLY"];
            }

            return DefaultFirefoxRevision;
        }

        private static string GetArchiveName(Product product, Platform platform, string revision)
        {
            if (product == Product.Chrome)
            {
                switch (platform)
                {
                    case Platform.Linux:
                        return "chrome-linux";
                    case Platform.MacOS:
                        return "chrome-mac";
                    case Platform.Win32:
                    case Platform.Win64:
                        return int.TryParse(revision, out var revValue) && revValue > 591479 ? "chrome-win" : "chrome-win32";
                    default:
                        throw new ArgumentException("Invalid platform", nameof(platform));
                }
            }
            else
            {
                switch (platform)
                {
                    case Platform.Linux:
                        return "linux";
                    case Platform.MacOS:
                        return "mac";
                    case Platform.Win32:
                        return "win32";
                    case Platform.Win64:
                        return "win64";
                    default:
                        throw new ArgumentException("Invalid platform", nameof(platform));
                }
            }
        }

        private static string GetDownloadURL(Product product, Platform platform, string host, string revision)
            => string.Format(CultureInfo.CurrentCulture, _downloadUrls[(product, platform)], host, revision, GetArchiveName(product, platform, revision));
    }
}
