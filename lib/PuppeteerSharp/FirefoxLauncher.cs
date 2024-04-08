using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a Firefox process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public class FirefoxLauncher : LauncherBase
    {
        private static readonly string[] _defaultArgs =
        [
            "--no-remote"
        ];

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxLauncher"/> class.
        /// </summary>
        /// <param name="executable">Full path of executable.</param>
        /// <param name="options">Options for launching Firefox.</param>
        public FirefoxLauncher(string executable, LaunchOptions options)
            : base(executable, options)
        {
            (var firefoxArgs, TempUserDataDir) = PrepareFirefoxArgs(options);

            Process.StartInfo.Arguments = string.Join(" ", firefoxArgs);
        }

        /// <inheritdoc />
        public override Task<string> GetDefaultBuildIdAsync() => Firefox.GetDefaultBuildIdAsync();

        /// <inheritdoc />
        public override string ToString() => $"Firefox process; EndPoint={EndPoint}; State={CurrentState}";

        internal static string[] GetDefaultArgs(LaunchOptions options)
        {
            var firefoxArguments = new List<string>(_defaultArgs);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                firefoxArguments.Add("--foreground");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                firefoxArguments.Add("--wait-for-browser");
            }

            if (!string.IsNullOrEmpty(options.UserDataDir))
            {
                firefoxArguments.Add("--profile");
                firefoxArguments.Add($"{options.UserDataDir.Quote()}");
            }

            if (options.HeadlessMode == HeadlessMode.True)
            {
                firefoxArguments.Add("--headless");
            }

            if (options.Devtools)
            {
                firefoxArguments.Add("--devtools");
            }

            if (options.Args.All(arg => arg.StartsWith("-", StringComparison.Ordinal)))
            {
                firefoxArguments.Add("about:blank");
            }

            firefoxArguments.AddRange(options.Args);
            return firefoxArguments.ToArray();
        }

        private static (List<string> FirefoxArgs, TempDirectory TempUserDataDirectory) PrepareFirefoxArgs(LaunchOptions options)
        {
            var firefoxArgs = new List<string>();

            if (!options.IgnoreDefaultArgs)
            {
                firefoxArgs.AddRange(GetDefaultArgs(options));
            }
            else if (options.IgnoredDefaultArgs?.Length > 0)
            {
                firefoxArgs.AddRange(GetDefaultArgs(options).Except(options.IgnoredDefaultArgs));
            }
            else
            {
                firefoxArgs.AddRange(options.Args);
            }

            if (!firefoxArgs.Any(a => a.StartsWith("-remote-debugging", StringComparison.OrdinalIgnoreCase)))
            {
                firefoxArgs.Add("--remote-debugging-port=0");
            }

            TempDirectory tempUserDataDirectory = null;

            if (!firefoxArgs.Contains("-profile") && !firefoxArgs.Contains("--profile"))
            {
                tempUserDataDirectory = new TempDirectory();
                Firefox.CreateProfile(tempUserDataDirectory.Path, GetPreferences(options.ExtraPrefsFirefox));
                firefoxArgs.Add("--profile");
                firefoxArgs.Add($"{tempUserDataDirectory.Path.Quote()}");
            }

            return (firefoxArgs, tempUserDataDirectory);
        }

        private static Dictionary<string, object> GetPreferences(Dictionary<string, object> optionsExtraPreferencesFirefox)
        {
            var result = optionsExtraPreferencesFirefox ?? [];
            result["browser.tabs.closeWindowWithLastTab"] = false;
            result["network.cookie.cookieBehavior"] = 0;
            result["fission.bfcacheInParent"] = false;
            result["remote.active-protocols"] = 2;
            result["fission.webContentIsolationStrategy"] = 0;

            return result;
        }
    }
}
