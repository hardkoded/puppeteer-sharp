using System;
using System.Collections.Generic;
using System.IO;
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

        private static readonly string[] _profileCommandLineArguments = ["-profile", "--profile"];
        private readonly string _userDataDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxLauncher"/> class.
        /// </summary>
        /// <param name="executable">Full path of executable.</param>
        /// <param name="options">Options for launching Firefox.</param>
        public FirefoxLauncher(string executable, LaunchOptions options)
            : base(executable, options)
        {
            (var firefoxArgs, TempUserDataDir, _userDataDir) = PrepareFirefoxArgs(options);

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

        internal override void OnExit()
        {
            // If TempUserDataDir is null it means that the user provided their own userDataDir
            if (TempUserDataDir is null)
            {
                var backupSuffix = ".puppeteer";
                string[] backupFiles = ["prefs.js", "user.js"];
                var basePath = _userDataDir.Unquote();
                foreach (var backupFile in backupFiles)
                {
                    var backupPath = Path.Combine(basePath, backupFile + backupSuffix);
                    var originalPath = Path.Combine(basePath, backupFile);
                    if (File.Exists(backupPath))
                    {
                        // We don't have the overwrite parameter in netstandard
                        if (File.Exists(originalPath))
                        {
                            File.Delete(originalPath);
                        }

                        File.Move(backupPath, Path.Combine(basePath, backupFile));
                    }
                }
            }

            base.OnExit();
        }

        private static (List<string> FirefoxArgs, TempDirectory TempUserDataDirectory, string UserDataDir) PrepareFirefoxArgs(LaunchOptions options)
        {
            var firefoxArguments = new List<string>();

            if (!options.IgnoreDefaultArgs)
            {
                firefoxArguments.AddRange(GetDefaultArgs(options));
            }
            else if (options.IgnoredDefaultArgs?.Length > 0)
            {
                firefoxArguments.AddRange(GetDefaultArgs(options).Except(options.IgnoredDefaultArgs));
            }
            else
            {
                firefoxArguments.AddRange(options.Args);
            }

            if (!firefoxArguments.Any(a => a.StartsWith("-remote-debugging", StringComparison.OrdinalIgnoreCase)))
            {
                firefoxArguments.Add("--remote-debugging-port=0");
            }

            // Check for the profile argument, which will always be set even
            // with a custom directory specified via the userDataDir option.
            var profileArgIndex = firefoxArguments.FindIndex(arg => _profileCommandLineArguments.Contains(arg));
            string userDataDir;
            TempDirectory tempUserDataDirectory = null;

            if (profileArgIndex != -1)
            {
                userDataDir = firefoxArguments[profileArgIndex + 1];
                if (userDataDir == null)
                {
                    throw new PuppeteerException("Missing value for profile command line argument");
                }
            }
            else
            {
                tempUserDataDirectory = new TempDirectory();
                userDataDir = tempUserDataDirectory.Path;

                firefoxArguments.Add("--profile");
                firefoxArguments.Add($"{userDataDir.Quote()}");
            }

            Firefox.CreateProfile(userDataDir, GetPreferences(options.ExtraPrefsFirefox));

            return (firefoxArguments, tempUserDataDirectory, userDataDir);
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
