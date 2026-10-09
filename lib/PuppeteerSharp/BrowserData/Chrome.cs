using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        public static string DefaultBuildId => "154.0.8037.57";

        internal static async Task<string> ResolveBuildIdAsync(ChromeReleaseChannel channel)
            => (await GetLastKnownGoodReleaseForChannel(channel).ConfigureAwait(false)).Version;

        internal static string ResolveDownloadUrl(Platform platform, string buildId, string baseUrl)
            => $"{baseUrl ?? "https://storage.googleapis.com/chrome-for-testing-public"}/{string.Join("/", ResolveDownloadPath(platform, buildId))}";

        internal static string RelativeExecutablePath(Platform platform, string buildId)
            => platform switch
            {
                Platform.MacOS or Platform.MacOSArm64 => Path.Combine(
                    "chrome-" + GetFolder(platform, buildId),
                    "Google Chrome for Testing.app",
                    "Contents",
                    "MacOS",
                    "Google Chrome for Testing"),
                Platform.Linux or Platform.LinuxArm64 => Path.Combine("chrome-" + GetFolder(platform, buildId), "chrome"),
                Platform.Win32 or Platform.Win64 => Path.Combine("chrome-" + GetFolder(platform, buildId), "chrome.exe"),
                _ => throw new ArgumentException("Invalid platform", nameof(platform)),
            };

        internal static string[] ResolveSystemExecutablePaths(Platform platform, ChromeReleaseChannel channel)
        {
            switch (platform)
            {
                case Platform.Win64:
                case Platform.Win32:
                    return GetChromeWindowsLocations(channel);
                case Platform.MacOS:
                case Platform.MacOSArm64:
                    return channel switch
                    {
                        ChromeReleaseChannel.Stable => ["/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"],
                        ChromeReleaseChannel.Beta => ["/Applications/Google Chrome Beta.app/Contents/MacOS/Google Chrome Beta"],
                        ChromeReleaseChannel.Canary => ["/Applications/Google Chrome Canary.app/Contents/MacOS/Google Chrome Canary"],
                        ChromeReleaseChannel.Dev => ["/Applications/Google Chrome Dev.app/Contents/MacOS/Google Chrome Dev"],
                        _ => throw new PuppeteerException($"{channel} is not supported"),
                    };
                case Platform.LinuxArm64:
                case Platform.Linux:
                    return channel switch
                    {
                        ChromeReleaseChannel.Stable => ["/opt/google/chrome/chrome"],
                        ChromeReleaseChannel.Beta => ["/opt/google/chrome-beta/chrome"],
                        ChromeReleaseChannel.Canary => ["/opt/google/chrome-canary/chrome"],
                        ChromeReleaseChannel.Dev => ["/opt/google/chrome-unstable/chrome"],
                        _ => throw new PuppeteerException($"{channel} is not supported"),
                    };
                default:
                    throw new PuppeteerException($"{platform} is not supported");
            }
        }

        internal static string GetFolder(Platform platform, string buildId)
            => platform switch
            {
                // Chrome for Testing started publishing linux-arm64 builds in 153.0.8001.0.
                Platform.LinuxArm64 => buildId != null && CompareVersions(buildId, "153.0.8001.0") < 0 ? "linux64" : "linux-arm64",
                Platform.Linux => "linux64",
                Platform.MacOSArm64 => "mac-arm64",
                Platform.MacOS => "mac-x64",
                Platform.Win32 => "win32",
                Platform.Win64 => "win64",
                _ => throw new PuppeteerException($"Unknown platform: {platform}"),
            };

        internal static int CompareVersions(string a, string b)
        {
            var aParts = ParseVersion(a);
            var bParts = ParseVersion(b);

            for (var i = 0; i < 4; i++)
            {
                var aPart = i < aParts.Length ? aParts[i] : 0;
                var bPart = i < bParts.Length ? bParts[i] : 0;

                if (aPart > bPart)
                {
                    return 1;
                }

                if (aPart < bPart)
                {
                    return -1;
                }
            }

            return 0;
        }

        private static int[] ParseVersion(string version)
        {
            var clean = version.Trim();

            if (!Regex.IsMatch(clean, @"^[0-9]+(?:\.[0-9]+){0,3}$"))
            {
                throw new PuppeteerException($"Version {version} is not a valid Chrome version");
            }

            return clean.Split('.').Select(part => int.Parse(part, CultureInfo.InvariantCulture)).ToArray();
        }

        private static string[] GetChromeWindowsLocations(ChromeReleaseChannel channel)
        {
            var suffix = channel switch
            {
                ChromeReleaseChannel.Stable => Path.Combine("Google", "Chrome", "Application", "chrome.exe"),
                ChromeReleaseChannel.Beta => Path.Combine("Google", "Chrome Beta", "Application", "chrome.exe"),
                ChromeReleaseChannel.Canary => Path.Combine("Google", "Chrome SxS", "Application", "chrome.exe"),
                ChromeReleaseChannel.Dev => Path.Combine("Google", "Chrome Dev", "Application", "chrome.exe"),
                _ => throw new PuppeteerException($"{channel} is not supported"),
            };

            var prefixes = new HashSet<string>();
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            if (!string.IsNullOrEmpty(programFiles))
            {
                prefixes.Add(programFiles);
            }

            var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            if (!string.IsNullOrEmpty(programFilesX86))
            {
                prefixes.Add(programFilesX86);
            }

            // Also check the ProgramW6432 environment variable for the native Program Files
            // on 64-bit systems (useful when running as a 32-bit process on a 64-bit OS).
            var programW6432 = Environment.GetEnvironmentVariable("ProgramW6432");
            if (!string.IsNullOrEmpty(programW6432))
            {
                prefixes.Add(programW6432);
            }

            // https://source.chromium.org/chromium/chromium/src/+/main:chrome/installer/mini_installer/README.md
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrEmpty(localAppData))
            {
                prefixes.Add(localAppData);
            }

            // Fallbacks in case env vars are misconfigured.
            prefixes.Add(@"C:\Program Files");
            prefixes.Add(@"C:\Program Files (x86)");
            prefixes.Add(@"D:\Program Files");
            prefixes.Add(@"D:\Program Files (x86)");

            return prefixes.Select(prefix => Path.Combine(prefix, suffix)).ToArray();
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
            =>
            [
                buildId,
                GetFolder(platform, buildId),
                $"chrome-{GetFolder(platform, buildId)}.zip"
            ];
    }
}
