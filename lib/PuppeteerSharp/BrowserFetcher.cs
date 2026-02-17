using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Linux;
using static PuppeteerSharp.BrowserFetcherOptions;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public sealed class BrowserFetcher : IBrowserFetcher
    {
        private const string PublishSingleFileLocalApplicationDataFolderName = "PuppeteerSharp";

        private readonly CustomFileDownloadAction _customFileDownload;
        private readonly ILogger<BrowserFetcher> _logger;

        /// <inheritdoc cref="BrowserFetcher"/>
        public BrowserFetcher()
        {
            CacheDir = GetBrowsersLocation();
            Platform = GetCurrentPlatform();
            Browser = SupportedBrowser.Chrome;
            _customFileDownload = DownloadFileUsingHttpClientTaskAsync;
        }

        /// <inheritdoc cref="BrowserFetcher"/>
        public BrowserFetcher(SupportedBrowser browser, ILoggerFactory loggerFactory = null)
            : this(new BrowserFetcherOptions { Browser = browser }, loggerFactory)
        {
        }

        /// <inheritdoc cref="BrowserFetcher"/>
        public BrowserFetcher(BrowserFetcherOptions options, ILoggerFactory loggerFactory = null)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Browser = options.Browser;
            CacheDir = string.IsNullOrEmpty(options.Path) ? GetBrowsersLocation() : options.Path;
            Platform = options.Platform ?? GetCurrentPlatform();
            _customFileDownload = options.CustomFileDownload ?? DownloadFileUsingHttpClientTaskAsync;
            _logger = loggerFactory?.CreateLogger<BrowserFetcher>();
        }

        /// <inheritdoc/>
        public string CacheDir { get; set; }

        /// <inheritdoc/>
        public string BaseUrl { get; set; }

        /// <inheritdoc/>
        public Platform Platform { get; set; }

        /// <inheritdoc/>
        public SupportedBrowser Browser { get; set; }

        /// <inheritdoc/>
        public IWebProxy WebProxy { get; set; }

        /// <inheritdoc/>
        public async Task<bool> CanDownloadAsync(string revision)
        {
            try
            {
                var url = GetDownloadURL(Browser, Platform, BaseUrl, revision);

                using var handler = new HttpClientHandler();
                if (WebProxy != null)
                {
                    handler.Proxy = WebProxy;
                    handler.UseProxy = true;
                }

                using var client = new HttpClient(handler);
                using var requestMessage = new HttpRequestMessage(HttpMethod.Head, url);
                using var response = await client.SendAsync(requestMessage).ConfigureAwait(false);
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, $"Failed to check download {Browser} for {Platform} from {BaseUrl}");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<InstalledBrowser> DownloadAsync()
        {
            var buildId = Browser switch
            {
                SupportedBrowser.Firefox => await Firefox.GetDefaultBuildIdAsync().ConfigureAwait(false),
                SupportedBrowser.Chrome or SupportedBrowser.ChromeHeadlessShell => Chrome.DefaultBuildId,
                SupportedBrowser.Chromium => await Chromium.ResolveBuildIdAsync(Platform).ConfigureAwait(false),
                _ => throw new PuppeteerException($"{Browser} not supported."),
            };

            return await DownloadAsync(buildId).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<InstalledBrowser> DownloadAsync(BrowserTag tag)
        {
            var revision = await ResolveBuildIdAsync(tag).ConfigureAwait(false);
            return await DownloadAsync(revision).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public IEnumerable<InstalledBrowser> GetInstalledBrowsers()
            => new Cache(CacheDir).GetInstalledBrowsers();

        /// <inheritdoc/>
        public void Uninstall(string buildId)
            => new Cache(CacheDir).Uninstall(Browser, Platform, buildId);

        /// <inheritdoc/>
        public async Task<InstalledBrowser> DownloadAsync(string buildId)
        {
            var installedBrowser = await DownloadAsync(Browser, buildId).ConfigureAwait(false);

            if (Browser == SupportedBrowser.Chrome)
            {
                await DownloadAsync(SupportedBrowser.ChromeHeadlessShell, buildId).ConfigureAwait(false);
            }

            return installedBrowser;
        }

        /// <inheritdoc/>
        public string GetExecutablePath(string buildId)
            => new InstalledBrowser(
                new Cache(CacheDir),
                Browser,
                buildId,
                Platform).GetExecutablePath();

        internal static Platform GetCurrentPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return RuntimeInformation.OSArchitecture == Architecture.Arm64 ? Platform.MacOSArm64 : Platform.MacOS;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Platform.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return RuntimeInformation.OSArchitecture == Architecture.X64 ||
                       (RuntimeInformation.OSArchitecture == Architecture.Arm64 && IsWindows11()) ? Platform.Win64 : Platform.Win32;
            }

            return Platform.Unknown;
        }

        internal static bool IsWindows11()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Build >= 22000;

        internal static string GetBrowsersLocation()
        {
            var assembly = typeof(Puppeteer).Assembly;
            var assemblyName = assembly.GetName().Name + ".dll";
            DirectoryInfo assemblyDirectory = new(AppContext.BaseDirectory);

            if (!assemblyDirectory.Exists || !File.Exists(Path.Combine(assemblyDirectory.FullName, assemblyName)))
            {
                var assemblyLocation = assembly.Location;

                if (string.IsNullOrEmpty(assemblyLocation))
                {
                    var singleFilePublishFilePathForBrowserExecutables = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PublishSingleFileLocalApplicationDataFolderName);
                    if (!Directory.Exists(singleFilePublishFilePathForBrowserExecutables))
                    {
                        Directory.CreateDirectory(singleFilePublishFilePathForBrowserExecutables);
                    }

                    return singleFilePublishFilePathForBrowserExecutables;
                }

                assemblyDirectory = new FileInfo(assemblyLocation).Directory;
            }

            if (!assemblyDirectory!.Exists || !File.Exists(Path.Combine(assemblyDirectory.FullName, assemblyName)))
            {
                assemblyDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            }

            return assemblyDirectory.FullName;
        }

        private static string GetDownloadURL(SupportedBrowser browser, Platform platform, string baseUrl, string buildId) => browser switch
        {
            SupportedBrowser.Chrome => Chrome.ResolveDownloadUrl(platform, buildId, baseUrl),
            SupportedBrowser.ChromeHeadlessShell => ChromeHeadlessShell.ResolveDownloadUrl(platform, buildId, baseUrl),
            SupportedBrowser.Chromium => Chromium.ResolveDownloadUrl(platform, buildId, baseUrl),
            SupportedBrowser.Firefox => Firefox.ResolveDownloadUrl(platform, buildId, baseUrl),
            _ => throw new NotSupportedException(),
        };

        private static void ExtractTar(string zipPath, string folderPath)
        {
            var compression = zipPath.EndsWith("xz", StringComparison.InvariantCulture) ? "J" : "j";
            new DirectoryInfo(folderPath).Create();
            using var process = new Process();
            process.StartInfo.FileName = "tar";
            process.StartInfo.Arguments = $"-xv{compression}f \"{zipPath}\" -C \"{folderPath}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
        }

        private static void ExecuteSetup(string exePath, string folderPath)
        {
            new DirectoryInfo(folderPath).Create();
            using var process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.Arguments = $"/ExtractDir={folderPath}";
            process.StartInfo.EnvironmentVariables.Add("__compat_layer", "RuAsInvoker");
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
        }

        private async Task<InstalledBrowser> DownloadAsync(SupportedBrowser browser, string buildId)
        {
            var url = GetDownloadURL(browser, Platform, BaseUrl, buildId);
            var fileName = url.Split('/').Last();
            var cache = new Cache(CacheDir);
            var archivePath = Path.Combine(CacheDir, fileName);
            var downloadFolder = new DirectoryInfo(CacheDir);

            if (!downloadFolder.Exists)
            {
                downloadFolder.Create();
            }

            var outputPath = cache.GetInstallationDir(browser, Platform, buildId);

            var installedBrowserCandidate = new InstalledBrowser(cache, browser, buildId, Platform);
            if (new FileInfo(installedBrowserCandidate.GetExecutablePath()).Exists)
            {
                installedBrowserCandidate.PermissionsFixed = RunSetup(installedBrowserCandidate);
                return installedBrowserCandidate;
            }

            try
            {
                await _customFileDownload(url, archivePath).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PuppeteerException($"Failed to download {browser} for {Platform} from {url}", ex);
            }

            await UnpackArchiveAsync(archivePath, outputPath, fileName).ConfigureAwait(false);
            new FileInfo(archivePath).Delete();

            var installedBrowser = new InstalledBrowser(cache, browser, buildId, Platform);
            installedBrowser.PermissionsFixed = RunSetup(installedBrowser);
            return installedBrowser;
        }

        private bool? RunSetup(InstalledBrowser installedBrowser)
        {
            // On Windows for Chrome invoke setup.exe to configure sandboxes.
            if (
                installedBrowser.Platform is Platform.Win32 or Platform.Win64 &&
                installedBrowser.Browser == SupportedBrowser.Chrome && installedBrowser.Platform == GetCurrentPlatform())
            {
                try
                {
                    var browserDir = new FileInfo(installedBrowser.GetExecutablePath()).Directory;
                    var setupExePath = Path.Combine(browserDir!.FullName, "setup.exe");

                    if (!File.Exists(setupExePath))
                    {
                        return false;
                    }

                    using var process = new Process();
                    process.StartInfo.FileName = setupExePath;
                    process.StartInfo.Arguments = $"--configure-browser-in-directory=\"{browserDir.FullName}\"";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    process.WaitForExit();

                    return true;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to run setup.exe");
                    return false;
                }
            }

            return null;
        }

        private async Task InstallDmgAsync(string dmgPath, string folderPath)
        {
            try
            {
                var destinationDirectoryInfo = new DirectoryInfo(folderPath);

                if (!destinationDirectoryInfo.Exists)
                {
                    destinationDirectoryInfo.Create();
                }

                var mountAndCopyTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                using var process = new Process();
                process.EnableRaisingEvents = true;
                process.StartInfo.FileName = "hdiutil";
                process.StartInfo.Arguments = $"attach -nobrowse -noautoopen \"{dmgPath}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.OutputDataReceived += (_, e) =>
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

                    using var copyProcess = new Process();
                    copyProcess.StartInfo.FileName = "cp";
                    copyProcess.StartInfo.Arguments = $"-R \"{appFile.FullName}\" \"{folderPath}\"";
                    copyProcess.Start();
                    copyProcess.WaitForExit();
                    mountAndCopyTcs.TrySetResult(true);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                await mountAndCopyTcs.Task.WithTimeout(Puppeteer.DefaultTimeout).ConfigureAwait(false);
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
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to unmount dmg");
            }
        }

        private Task<string> ResolveBuildIdAsync(BrowserTag tag)
        {
            switch (Browser)
            {
                case SupportedBrowser.Firefox:
                    return tag switch
                    {
                        BrowserTag.Latest => Firefox.ResolveBuildIdAsync(FirefoxChannel.Nightly),
                        BrowserTag.Beta => Firefox.ResolveBuildIdAsync(FirefoxChannel.Beta),
                        BrowserTag.Nightly => Firefox.ResolveBuildIdAsync(FirefoxChannel.Nightly),
                        BrowserTag.DevEdition => Firefox.ResolveBuildIdAsync(FirefoxChannel.DevEdition),
                        BrowserTag.Stable => Firefox.ResolveBuildIdAsync(FirefoxChannel.Stable),
                        BrowserTag.Esr => Firefox.ResolveBuildIdAsync(FirefoxChannel.Esr),
                        _ => throw new PuppeteerException($"{tag} is not supported for {Browser}. Use 'latest' instead."),
                    };
                case SupportedBrowser.Chrome:
                    return tag switch
                    {
                        BrowserTag.Latest => Chrome.ResolveBuildIdAsync(ChromeReleaseChannel.Canary),
                        BrowserTag.Beta => Chrome.ResolveBuildIdAsync(ChromeReleaseChannel.Beta),
                        BrowserTag.Canary => Chrome.ResolveBuildIdAsync(ChromeReleaseChannel.Canary),
                        BrowserTag.Dev => Chrome.ResolveBuildIdAsync(ChromeReleaseChannel.Dev),
                        BrowserTag.Stable => Chrome.ResolveBuildIdAsync(ChromeReleaseChannel.Stable),
                        _ => throw new PuppeteerException($"{tag} is not supported for {Browser}."),
                    };
                case SupportedBrowser.Chromium:
                    return tag switch
                    {
                        BrowserTag.Latest => Chromium.ResolveBuildIdAsync(Platform),
                        _ => throw new PuppeteerException($"{tag} is not supported for {Browser}. Use 'latest' instead."),
                    };
                default:
                    throw new PuppeteerException($"{Browser} not supported.");
            }
        }

        private async Task UnpackArchiveAsync(string archivePath, string outputPath, string archiveName)
        {
            if (archivePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                ZipFile.ExtractToDirectory(archivePath, outputPath);
            }
            else if (archivePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                ExecuteSetup(archivePath, outputPath);
            }
            else if (archivePath.Contains(".tar."))
            {
                ExtractTar(archivePath, outputPath);
            }
            else
            {
                await InstallDmgAsync(archivePath, outputPath).ConfigureAwait(false);
            }

            if (GetCurrentPlatform() == Platform.Linux)
            {
                var executables = new[]
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
                    var execPath = Path.Combine(outputPath, archiveName, executable);

                    if (File.Exists(execPath))
                    {
                        var code = LinuxSysCall.Chmod(execPath, LinuxSysCall.ExecutableFilePermissions);

                        if (code != 0)
                        {
                            throw new PuppeteerException("Chmod operation failed");
                        }
                    }
                }
            }
        }

        private async Task DownloadFileUsingHttpClientTaskAsync(string address, string filename)
        {
            using var handler = new HttpClientHandler();
            if (WebProxy != null)
            {
                handler.Proxy = WebProxy;
                handler.UseProxy = true;
            }

            using var client = new HttpClient(handler);

            // Send the GET request and retrieve the response as a stream
            using var downloadStream = await client.GetStreamAsync(address).ConfigureAwait(false);

            // Write the stream to a file
            using var fileStream = File.Create(filename);
            await downloadStream.CopyToAsync(fileStream).ConfigureAwait(false);
        }
    }
}
