using System;
using System.IO;

namespace PuppeteerSharp.BrowserData
{
    /// <summary>
    /// Chrome info.
    /// </summary>
    public static class ChromeHeadlessShell
    {
        internal static string ResolveDownloadUrl(Platform platform, string buildId, string baseUrl)
            => $"{baseUrl ?? "https://storage.googleapis.com/chrome-for-testing-public"}/{string.Join("/", ResolveDownloadPath(platform, buildId))}";

        internal static string RelativeExecutablePath(Platform platform, string buildId)
            => platform switch
            {
                Platform.MacOS or Platform.MacOSArm64 => Path.Combine(
                    "chrome-headless-shell-" + GetFolder(platform),
                    "chrome-headless-shell"),
                Platform.Linux => Path.Combine("chrome-headless-shell-linux64", "chrome-headless-shell"),
                Platform.Win32 or Platform.Win64 => Path.Combine("chrome-headless-shell-" + GetFolder(platform), "chrome-headless-shell.exe"),
                _ => throw new ArgumentException("Invalid platform", nameof(platform)),
            };

        private static string[] ResolveDownloadPath(Platform platform, string buildId)
            => new string[]
            {
                buildId,
                GetFolder(platform),
                $"chrome-headless-shell-{GetFolder(platform)}.zip",
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
