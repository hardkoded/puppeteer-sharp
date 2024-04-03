using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp.BrowserData
{
    /// <summary>
    /// Chrome info.
    /// </summary>
    public static class Chrome
    {
        /// <summary>
        /// Default chrome build.
        /// </summary>
        public static string DefaultBuildId => "123.0.6312.86";

        internal static async Task<string> ResolveBuildIdAsync(ChromeReleaseChannel channel)
            => (await GetLastKnownGoodReleaseForChannel(channel).ConfigureAwait(false)).Version;

        internal static string ResolveDownloadUrl(Platform platform, string buildId, string baseUrl)
            => $"{baseUrl ?? "https://storage.googleapis.com/chrome-for-testing-public"}/{string.Join("/", ResolveDownloadPath(platform, buildId))}";

        internal static string RelativeExecutablePath(Platform platform, string builId)
            => platform switch
            {
                Platform.MacOS or Platform.MacOSArm64 => Path.Combine(
                    "chrome-" + GetFolder(platform),
                    "Google Chrome for Testing.app",
                    "Contents",
                    "MacOS",
                    "Google Chrome for Testing"),
                Platform.Linux => Path.Combine("chrome-linux64", "chrome"),
                Platform.Win32 or Platform.Win64 => Path.Combine("chrome-" + GetFolder(platform), "chrome.exe"),
                _ => throw new ArgumentException("Invalid platform", nameof(platform)),
            };

        internal static string ResolveSystemExecutablePath(Platform platform, ChromeReleaseChannel channel)
        {
            switch (platform)
            {
                case Platform.Win64:
                case Platform.Win32:
                    var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    return channel switch
                    {
                        ChromeReleaseChannel.Stable => $"{programFilesPath}\\Google\\Chrome\\Application\\chrome.exe",
                        ChromeReleaseChannel.Beta => $"{programFilesPath}\\Google\\Chrome Beta\\Application\\chrome.exe",
                        ChromeReleaseChannel.Canary => $"{programFilesPath}\\Google\\Chrome SxS\\Application\\chrome.exe",
                        ChromeReleaseChannel.Dev => $"{programFilesPath}\\Google\\Chrome Dev\\Application\\chrome.exe",
                        _ => throw new PuppeteerException($"{channel} is not supported"),
                    };
                case Platform.MacOS:
                case Platform.MacOSArm64:
                    return channel switch
                    {
                        ChromeReleaseChannel.Stable => $"/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
                        ChromeReleaseChannel.Beta => $"/Applications/Google Chrome Beta.app/Contents/MacOS/Google Chrome Beta",
                        ChromeReleaseChannel.Canary => $"/Applications/Google Chrome Canary.app/Contents/MacOS/Google Chrome Canary",
                        ChromeReleaseChannel.Dev => $"/Applications/Google Chrome Dev.app/Contents/MacOS/Google Chrome Dev",
                        _ => throw new PuppeteerException($"{channel} is not supported"),
                    };
                case Platform.Linux:
                    return channel switch
                    {
                        ChromeReleaseChannel.Stable => $"/opt/google/chrome/chrome",
                        ChromeReleaseChannel.Beta => $"/opt/google/chrome-beta/chrome",
                        ChromeReleaseChannel.Dev => $"/opt/google/chrome-unstable/chrome",
                        _ => throw new PuppeteerException($"{channel} is not supported"),
                    };
                default:
                    throw new PuppeteerException($"{platform} is not supported");
            }
        }

        private static async Task<ChromeGoodVersionsResult.ChromeGoodVersionsResultVersion> GetLastKnownGoodReleaseForChannel(ChromeReleaseChannel channel)
        {
            var data = await JsonUtils.GetAsync<ChromeGoodVersionsResult>("https://googlechromelabs.github.io/chrome-for-testing/last-known-good-versions.json").ConfigureAwait(false);

            foreach (var channelKey in data.Channels.Keys.ToArray())
            {
                data.Channels[channelKey.ToUpperInvariant()] = data.Channels[channelKey]!;
            }

            return data.Channels[channel.ToString().ToUpperInvariant()];
        }

        private static string[] ResolveDownloadPath(Platform platform, string buildId)
            => new string[]
            {
                buildId,
                GetFolder(platform),
                $"chrome-{GetFolder(platform)}.zip",
            };

        private static string GetFolder(Platform platform)
            => platform switch
            {
                Platform.Linux => "linux64",
                Platform.MacOSArm64 => "mac-arm64",
                Platform.MacOS => "mac-x64",
                Platform.Win32 => "win32",
                Platform.Win64 => "win64",
                _ => throw new PuppeteerException($"Unknown platform: {platform}"),
            };
    }
}
