using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PuppeteerSharp.BrowserData
{
    internal class Chromium
    {
        internal static Task<string> ResolveBuildIdAsync(Platform platform)
            => JsonUtils.GetTextAsync($"https://storage.googleapis.com/chromium-browser-snapshots/{GetFolder(platform)}/LAST_CHANGE");

        internal static string ResolveDownloadUrl(Platform platform, string buildId, string baseUrl)
            => $"{baseUrl ?? "https://storage.googleapis.com/chromium-browser-snapshots"}/{string.Join("/", ResolveDownloadPath(platform, buildId))}";

        internal static string RelativeExecutablePath(Platform platform, string builId)
            => platform switch
            {
                Platform.MacOS or Platform.MacOSArm64 => Path.Combine(
                    "chrome-mac",
                    "Chromium.app",
                    "Contents",
                    "MacOS",
                    "Chromium"),
                Platform.Linux => Path.Combine("chrome-linux", "chrome"),
                Platform.Win32 or Platform.Win64 => Path.Combine("chrome-win", "chrome.exe"),
                _ => throw new ArgumentException("Invalid platform", nameof(platform)),
            };

        private static string[] ResolveDownloadPath(Platform platform, string buildId)
            => new string[]
            {
                GetFolder(platform),
                buildId,
                $"{GetArchive(platform, buildId)}.zip",
            };

        private static string GetArchive(Platform platform, string buildId)
            => platform switch
            {
                Platform.Linux => "chrome-linux",
                Platform.MacOS or Platform.MacOSArm64 => "chrome-mac",

                // Windows archive name changed at r591479.
                Platform.Win32 or Platform.Win64 => int.TryParse(buildId, out var revValue) && revValue > 591479 ? "chrome-win" : "chrome-win32",
                _ => throw new PuppeteerException($"Unknown platform: {platform}"),
            };

        private static string GetFolder(Platform platform)
            => platform switch
            {
                Platform.Linux => "Linux_x64",
                Platform.MacOSArm64 => "Mac_Arm",
                Platform.MacOS => "Mac",
                Platform.Win32 => "Win",
                Platform.Win64 => "Win_x64",
                _ => throw new PuppeteerException($"Unknown platform: {platform}"),
            };
    }
}
