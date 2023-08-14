using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PuppeteerSharp.BrowserData
{
    /// <summary>
    /// Chrome info.
    /// </summary>
    public static class Firefox
    {
        /// <summary>
        /// Default firefoxbuild.
        /// </summary>
        public const string DefaultBuildId = "FIREFOX_NIGHTLY";

        private static readonly Dictionary<string, string> _cachedBuildIds = new();

        internal static Task<string> GetDefaultBuildIdAsync() => ResolveBuildIdAsync(DefaultBuildId);

        internal static string ResolveDownloadUrl(Platform platform, string buildId, string baseUrl)
            => $"{baseUrl ?? "https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central"}/{string.Join("/", ResolveDownloadPath(platform, buildId))}";

        internal static async Task<string> ResolveBuildIdAsync(string channel)
        {
            if (_cachedBuildIds.ContainsKey(channel))
            {
                return _cachedBuildIds[channel];
            }

            var version = await JsonUtils.GetAsync<Dictionary<string, string>>("https://product-details.mozilla.org/1.0/firefox_versions.json").ConfigureAwait(false);

            if (!version.ContainsKey(channel))
            {
                throw new PuppeteerException($"Channel {channel} not found.");
            }

            _cachedBuildIds[channel] = version[channel];
            return version[channel];
        }

        internal static string RelativeExecutablePath(Platform platform, string builId)
            => platform switch
            {
                Platform.MacOS or Platform.MacOSArm64 => Path.Combine(
                    "Firefox Nightly.app",
                    "Contents",
                    "MacOS",
                    "firefox"),
                Platform.Linux => Path.Combine("firefox", "firefox"),
                Platform.Win32 or Platform.Win64 => Path.Combine("firefox", "firefox.exe"),
                _ => throw new ArgumentException("Invalid platform", nameof(platform)),
            };

        private static string[] ResolveDownloadPath(Platform platform, string buildId)
            => new string[]
            {
                GetArchive(platform, buildId),
            };

        private static string GetArchive(Platform platform, string buildId)
            => platform switch
            {
                Platform.Linux => $"firefox-{buildId}.en-US.{platform.ToString().ToLowerInvariant()}-x86_64.tar.bz2",
                Platform.MacOS or Platform.MacOSArm64 => $"firefox-{buildId}.en-US.mac.dmg",

                // Windows archive name changed at r591479.
                Platform.Win32 or Platform.Win64 => $"firefox-{buildId}.en-US.{platform.ToString().ToLowerInvariant()}.zip",
                _ => throw new PuppeteerException($"Unknown platform: {platform}"),
            };
    }
}
