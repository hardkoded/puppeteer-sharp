using System;
using System.IO;

namespace PuppeteerSharp.BrowserData
{
    /// <summary>
    /// Installed browser info.
    /// </summary>
    public class InstalledBrowser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledBrowser"/> class.
        /// </summary>
        /// <param name="cache">Cache.</param>
        /// <param name="browser">Browser.</param>
        /// <param name="buildId">BuildId.</param>
        /// <param name="platform">Platform.</param>
        internal InstalledBrowser(Cache cache, SupportedBrowser browser, string buildId, Platform platform)
        {
            Cache = cache;
            Browser = browser;
            BuildId = buildId;
            Platform = platform;
        }

        /// <summary>
        /// Browser.
        /// </summary>
        public SupportedBrowser Browser { get; set; }

        /// <summary>
        /// Gets or sets the buildID.
        /// </summary>
        public string BuildId { get; set; }

        /// <summary>
        /// Revision platform.
        /// </summary>
        public Platform Platform { get; set; }

        /// <summary>
        /// Whether the permissions have been fixed in the browser.
        /// If Puppeteer executed the command to fix the permissions, this will be true.
        /// If Puppeteer failed to fix the permissions, this will be false.
        /// If the platform does not require permissions to be fixed, this will be null.
        /// </summary>
        public bool? PermissionsFixed { get; internal set; }

        /// <summary>
        /// Revision platform.
        /// </summary>
        internal Cache Cache { get; set; }

        /// <summary>
        /// Get executable path.
        /// </summary>
        /// <returns>executable path.</returns>
        /// <exception cref="ArgumentException">For not supported <see cref="Platform"/>.</exception>
        public string GetExecutablePath()
        {
            var installationDir = Cache.GetInstallationDir(Browser, Platform, BuildId);
            return Path.Combine(
                installationDir,
                GetExecutablePath(Browser, Platform, BuildId));
        }

        private static string GetExecutablePath(SupportedBrowser browser, Platform platform, string buildId) => browser switch
        {
            SupportedBrowser.Chrome => Chrome.RelativeExecutablePath(platform, buildId),
            SupportedBrowser.ChromeHeadlessShell => ChromeHeadlessShell.RelativeExecutablePath(platform, buildId),
            SupportedBrowser.Chromium => Chromium.RelativeExecutablePath(platform, buildId),
            SupportedBrowser.Firefox => Firefox.RelativeExecutablePath(platform, buildId),
            _ => throw new NotSupportedException(),
        };
    }
}
