using System;

namespace PuppeteerSharp.Browsers
{
    /// <summary>
    /// A <c>browser[@tagOrBuildId]</c> argument, e.g. <c>chrome@stable</c> or <c>chrome@116.0.5793.0</c>.
    /// </summary>
    internal sealed class BrowserSpec
    {
        private BrowserSpec(SupportedBrowser browser, string version)
        {
            Browser = browser;
            Version = version;
        }

        public SupportedBrowser Browser { get; }

        /// <summary>
        /// Gets the raw tag or build id. Null when none was given.
        /// </summary>
        public string Version { get; }

        public static BrowserSpec Parse(string value)
        {
            var parts = value.Split(['@'], 2);
            var browser = parts[0].ToLowerInvariant() switch
            {
                "chrome" => SupportedBrowser.Chrome,
                "chrome-headless-shell" or "chromeheadlessshell" => SupportedBrowser.ChromeHeadlessShell,
                "chromium" => SupportedBrowser.Chromium,
                "firefox" => SupportedBrowser.Firefox,
                _ => throw new ArgumentException($"Unknown browser '{parts[0]}'. Supported: chrome, chrome-headless-shell, chromium, firefox."),
            };

            return new BrowserSpec(browser, parts.Length > 1 && parts[1].Length > 0 ? parts[1] : null);
        }

        public bool TryGetTag(out BrowserTag tag)
        {
            tag = BrowserTag.Latest;
            return Version != null && Enum.TryParse(Version.Replace("-", string.Empty), true, out tag) && !int.TryParse(Version, out _);
        }
    }
}
