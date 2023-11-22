using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PuppeteerSharp.BrowserData
{
    internal class Cache
    {
        private readonly string _rootDir;

        public Cache() => _rootDir = BrowserFetcher.GetBrowsersLocation();

        public Cache(string rootDir) => _rootDir = rootDir;

        public string GetBrowserRoot(SupportedBrowser browser) => Path.Combine(_rootDir, browser.ToString());

        public string GetInstallationDir(SupportedBrowser browser, Platform platform, string buildId)
            => Path.Combine(GetBrowserRoot(browser), $"{platform}-{buildId}");

        public IEnumerable<InstalledBrowser> GetInstalledBrowsers()
        {
            var rootInfo = new DirectoryInfo(_rootDir);

            if (!rootInfo.Exists)
            {
                return Array.Empty<InstalledBrowser>();
            }

            var browserNames = Enum.GetNames(typeof(SupportedBrowser)).Select(browser => browser.ToUpperInvariant());
            var browsers = rootInfo.GetDirectories().Where(browser => browserNames.Contains(browser.Name.ToUpperInvariant()));

            return browsers.SelectMany(browser =>
            {
                var browserEnum = (SupportedBrowser)Enum.Parse(typeof(SupportedBrowser), browser.Name, ignoreCase: true);
                var dirInfo = new DirectoryInfo(GetBrowserRoot(browserEnum));
                var dirs = dirInfo.GetDirectories();

                return dirs.Select(dir =>
                {
                    var result = ParseFolderPath(dir);

                    if (result == null)
                    {
                        return null;
                    }

                    var platformEnum = (Platform)Enum.Parse(typeof(Platform), result.Value.Platform, ignoreCase: true);
                    return new InstalledBrowser(this, browserEnum, result.Value.BuildId, platformEnum);
                })
                .Where(item => item != null);
            });
        }

        public void Uninstall(SupportedBrowser browser, Platform platform, string buildId)
        {
            var dir = new DirectoryInfo(GetInstallationDir(browser, platform, buildId));
            if (dir.Exists)
            {
                dir.Delete(true);
            }
        }

        public void Clear() => new DirectoryInfo(_rootDir).Delete(true);

        private (string Platform, string BuildId)? ParseFolderPath(DirectoryInfo directory)
        {
            var name = directory.Name;
            var splits = name.Split('-');

            if (splits.Length != 2)
            {
                return null;
            }

            return (splits[0], splits[1]);
        }
    }
}
