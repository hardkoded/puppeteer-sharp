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
using Newtonsoft.Json;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Linux;
using static PuppeteerSharp.BrowserFetcherOptions;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public class BrowserFetcher : IBrowserFetcher
    {
        /// <summary>
        /// Default chromium revision.
        /// </summary>
        public const string DefaultChromiumRevision = "1069273";

        private static readonly Dictionary<Product, string> _hosts = new Dictionary<Product, string>
        {
            [Product.Chrome] = "https://storage.googleapis.com",
            [Product.Firefox] = "https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central",
        };

        private static readonly Dictionary<(Product Product, Platform Platform), string> _downloadUrls = new()
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
        private readonly CustomFileDownloadAction _customFileDownload;
        private bool _isDisposed;

        /// <inheritdoc cref="BrowserFetcher"/>
        public BrowserFetcher()
        {
            DownloadsFolder = Path.Combine(GetExecutablePath(), ".local-chromium");
            DownloadHost = _hosts[Product.Chrome];
            Platform = GetCurrentPlatform();
            Product = Product.Chrome;
            _customFileDownload = _webClient.DownloadFileTaskAsync;
        }

        /// <inheritdoc cref="BrowserFetcher"/>
        public BrowserFetcher(Product product) : this(new BrowserFetcherOptions { Product = product })
        {
        }

        /// <inheritdoc cref="BrowserFetcher"/>
        public BrowserFetcher(BrowserFetcherOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            DownloadsFolder = string.IsNullOrEmpty(options.Path) ?
               Path.Combine(GetExecutablePath(), options.Product == Product.Chrome ? ".local-chromium" : ".local-firefox") :
               options.Path;
            DownloadHost = string.IsNullOrEmpty(options.Host) ? _hosts[options.Product] : options.Host;
            Platform = options.Platform ?? GetCurrentPlatform();
            Product = options.Product;
            _customFileDownload = options.CustomFileDownload ?? _webClient.DownloadFileTaskAsync;
        }

        /// <inheritdoc/>
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        /// <inheritdoc/>
        public string DefaultFirefoxRevision { get; private set; } = "latest";

        /// <inheritdoc/>
        public string DownloadsFolder { get; }

        /// <inheritdoc/>
        public string DownloadHost { get; }

        /// <inheritdoc/>
        public Platform Platform { get; }

        /// <inheritdoc/>
        public Product Product { get; }

        /// <inheritdoc/>
        public IWebProxy WebProxy
        {
            get => _webClient.Proxy;
            set => _webClient.Proxy = value;
        }

        /// <summary>
        /// Get executable path.
        /// </summary>
        /// <param name="product"><see cref="Product"/>.</param>
        /// <param name="platform"><see cref="Platform"/>.</param>
        /// <param name="revision">chromium revision.</param>
        /// <param name="folderPath">folder path.</param>
        /// <returns>executable path.</returns>
        /// <exception cref="ArgumentException">For not supported <see cref="Platform"/>.</exception>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public IEnumerable<string> LocalRevisions()
        {
            var directoryInfo = new DirectoryInfo(DownloadsFolder);

            if (directoryInfo.Exists)
            {
                return directoryInfo.GetDirectories().Select(d => GetRevisionFromPath(d.Name));
            }

            return Array.Empty<string>();
        }

        /// <inheritdoc/>
        public void Remove(string revision)
        {
            var directory = new DirectoryInfo(GetFolderPath(revision));
            if (directory.Exists)
            {
                directory.Delete(true);
            }
        }

        /// <inheritdoc/>
        public async Task<RevisionInfo> GetRevisionInfoAsync()
            => RevisionInfo(Product == Product.Chrome ? DefaultChromiumRevision : await GetDefaultFirefoxRevisionAsync().ConfigureAwait(false));

        /// <inheritdoc/>
        public RevisionInfo RevisionInfo(string revision)
        {
            var result = new RevisionInfo
            {
                FolderPath = GetFolderPath(revision),
                Url = GetDownloadURL(Product, Platform, DownloadHost, revision),
                Revision = revision,
                Platform = Platform,
            };
            result.ExecutablePath = GetExecutablePath(Product, Platform, revision, result.FolderPath);
            result.Local = new DirectoryInfo(result.FolderPath).Exists;

            return result;
        }

        /// <inheritdoc/>
        public async Task<RevisionInfo> DownloadAsync()
            => await DownloadAsync(
                Product == Product.Chrome
                ? DefaultChromiumRevision
                : await GetDefaultFirefoxRevisionAsync().ConfigureAwait(false)).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task<RevisionInfo> DownloadAsync(string revision)
        {
            var url = GetDownloadURL(Product, Platform, DownloadHost, revision);
            var filePath = Path.Combine(DownloadsFolder, url.Split('/').Last());
            var folderPath = GetFolderPath(revision);
            var archiveName = GetArchiveName(Product, Platform, revision);

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

            await _customFileDownload(url, filePath).ConfigureAwait(false);

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

            if (GetCurrentPlatform() == Platform.Linux)
            {
                var executables = new string[]
                {
                    "chrome",
                    "chrome_crashpad_handler",
                    "chrome-management-service",
                    "chrome_sandbox", // setuid
                    "crashpad_handler",
                    "google-chrome",
                    "libvulkan.so.1",
                    "nacl_helper",
                    "nacl_helper_bootstrap",
                    "xdg-mime",
                    "xdg-settings",
                    "cron/google-chrome",
                };

                foreach (var executable in executables)
                {
                    var execPath = Path.Combine(folderPath, archiveName, executable);

                    if (File.Exists(execPath))
                    {
                        var code = LinuxSysCall.Chmod(execPath, LinuxSysCall.ExecutableFilePermissions);

                        if (code != 0)
                        {
                            throw new Exception("Chmod operation failed");
                        }
                    }
                }
            }

            return RevisionInfo(revision);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public string GetExecutablePath(string revision)
            => GetExecutablePath(Product, Platform, revision, GetFolderPath(revision));

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

        internal static string GetExecutablePath()
        {
            DirectoryInfo assemblyDirectory = new(AppContext.BaseDirectory);
            if (!assemblyDirectory.Exists || !File.Exists(Path.Combine(assemblyDirectory.FullName, "PuppeteerSharp.dll")))
            {
                string assemblyLocation;
                var assembly = typeof(Puppeteer).Assembly;
#pragma warning disable SYSLIB0012 // 'Assembly.CodeBase' is obsolete: 'Assembly.CodeBase and Assembly.EscapedCodeBase are only included for .NET Framework compatibility.
                if (Uri.TryCreate(assembly.CodeBase, UriKind.Absolute, out var codeBase) && codeBase.IsFile)
#pragma warning restore SYSLIB0012 // 'Assembly.CodeBase' is obsolete: 'Assembly.CodeBase and Assembly.EscapedCodeBase are only included for .NET Framework compatibility.
                {
                    assemblyLocation = codeBase.LocalPath;
                }
                else
                {
                    assemblyLocation = assembly.Location;
                }

                assemblyDirectory = new FileInfo(assemblyLocation).Directory;
            }

            if (!assemblyDirectory.Exists || !File.Exists(Path.Combine(assemblyDirectory.FullName, "PuppeteerSharp.dll")))
            {
                assemblyDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            }

            return assemblyDirectory.FullName;
        }

        /// <summary>
        /// Dispose <see cref="WebClient"/>.
        /// </summary>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _webClient.Dispose();
            }

            _isDisposed = true;
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

        private static void ExtractTar(string zipPath, string folderPath)
        {
            new DirectoryInfo(folderPath).Create();
            using var process = new Process();
            process.StartInfo.FileName = "tar";
            process.StartInfo.Arguments = $"-xvjf \"{zipPath}\" -C \"{folderPath}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
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
                    EnableRaisingEvents = true,
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
                process.StartInfo.Arguments = $"detach \"{dmgPath}\" -quiet";
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit();
            }
            catch
            {
                // swallow
            }
        }
    }
}
