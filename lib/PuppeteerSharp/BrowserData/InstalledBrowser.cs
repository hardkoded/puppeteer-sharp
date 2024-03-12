using System;
using System.Collections.Generic;
using System.IO;

namespace PuppeteerSharp.BrowserData
{
    /// <summary>
    /// Installed browser info.
    /// </summary>
    public class InstalledBrowser
    {
        private static readonly Dictionary<SupportedBrowser, Func<Platform, string, string>> _executablePathByBrowser = new()
        {
            [SupportedBrowser.Chrome] = Chrome.RelativeExecutablePath,
            [SupportedBrowser.ChromeHeadlessShell] = ChromeHeadlessShell.RelativeExecutablePath,
            [SupportedBrowser.Chromium] = Chromium.RelativeExecutablePath,
            [SupportedBrowser.Firefox] = Firefox.RelativeExecutablePath,
        };

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
                _executablePathByBrowser[Browser](Platform, BuildId));
        }
    }
}
