using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a Chromium process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public class ChromeLauncher : LauncherBase
    {
        private const string UserDataDirArgument = "--user-data-dir";

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeLauncher"/> class.
        /// </summary>
        /// <param name="executable">Full path of executable.</param>
        /// <param name="options">Options for launching Chromium.</param>
        public ChromeLauncher(string executable, LaunchOptions options)
            : base(executable, options)
        {
            List<string> chromiumArgs;
            (chromiumArgs, TempUserDataDir) = PrepareChromiumArgs(options);

            Process.StartInfo.Arguments = string.Join(" ", chromiumArgs);
        }

        /// <inheritdoc />
        public override Task<string> GetDefaultBuildIdAsync() => Task.FromResult(Chrome.DefaultBuildId);

        /// <inheritdoc />
        public override string ToString() => $"Chromium process; EndPoint={EndPoint}; State={CurrentState}";

        internal static string[] GetDefaultArgs(LaunchOptions options)
        {
            var userDisabledFeatures = GetFeatures("--disable-features", options.Args);
            var args = options.Args;
            if (args is not null && userDisabledFeatures.Length > 0)
            {
                args = RemoveMatchingFlags(options.Args, "--disable-features");
            }

            // Merge default disabled features with user-provided ones, if any.
            var disabledFeatures = new List<string>
            {
                "Translate",
                "AcceptCHFrame",
                "MediaRouter",
                "OptimizationHints",
                "ProcessPerSiteUpToMainFrameThreshold",
            };

            disabledFeatures.AddRange(userDisabledFeatures);

            var userEnabledFeatures = GetFeatures("--enable-features", options.Args);
            if (args != null && userEnabledFeatures.Length > 0)
            {
                args = RemoveMatchingFlags(options.Args, "--enable-features");
            }

            // Merge default enabled features with user-provided ones, if any.
            var enabledFeatures = new List<string>
            {
                "NetworkServiceInProcess2",
            };

            disabledFeatures.AddRange(userEnabledFeatures);

            var chromiumArguments = new List<string>(
                new string[]
                {
                    "--allow-pre-commit-input",
                    "--disable-background-networking",
                    "--disable-background-timer-throttling",
                    "--disable-backgrounding-occluded-windows",
                    "--disable-breakpad",
                    "--disable-client-side-phishing-detection",
                    "--disable-component-extensions-with-background-pages",
                    "--disable-component-update",
                    "--disable-default-apps",
                    "--disable-dev-shm-usage",
                    "--disable-extensions",
                    "--disable-field-trial-config",
                    "--disable-hang-monitor",
                    "--disable-infobars",
                    "--disable-ipc-flooding-protection",
                    "--disable-popup-blocking",
                    "--disable-prompt-on-repost",
                    "--disable-renderer-backgrounding",
                    "--disable-search-engine-choice-screen",
                    "--disable-sync",
                    "--enable-automation",
                    "--enable-blink-features=IdleDetection",
                    "--export-tagged-pdf",
                    "--generate-pdf-document-outline",
                    "--force-color-profile=srgb",
                    "--metrics-recording-only",
                    "--no-first-run",
                    "--password-store=basic",
                    "--use-mock-keychain",
                });

            chromiumArguments.Add($"--disable-features={string.Join(",", disabledFeatures)}");
            chromiumArguments.Add($"--enable-features={string.Join(",", enabledFeatures)}");

            if (!string.IsNullOrEmpty(options.UserDataDir))
            {
                chromiumArguments.Add($"{UserDataDirArgument}={options.UserDataDir.Quote()}");
            }

            if (options.Devtools)
            {
                chromiumArguments.Add("--auto-open-devtools-for-tabs");
            }

            if (options.Headless)
            {
                chromiumArguments.AddRange(new[]
                {
                    options.HeadlessMode == HeadlessMode.True ? "--headless=new" : "--headless",
                    "--hide-scrollbars",
                    "--mute-audio",
                });
            }

            if (args.All(arg => arg.StartsWith("-", StringComparison.Ordinal)))
            {
                chromiumArguments.Add("about:blank");
            }

            chromiumArguments.AddRange(args);
            return chromiumArguments.ToArray();
        }

        internal static string[] GetFeatures(string flag, string[] options)
            => options
                .Where(s => s.StartsWith($"{flag}=", StringComparison.InvariantCultureIgnoreCase))
                .Select(s => s.Substring(flag.Length + 1))
                .Where(s => !string.IsNullOrEmpty(s)).ToArray();

        internal static string[] RemoveMatchingFlags(string[] array, string flag)
            => array.Where(arg => !arg.StartsWith(flag, StringComparison.InvariantCultureIgnoreCase)).ToArray();

        private static (List<string> ChromiumArgs, TempDirectory TempUserDataDirectory) PrepareChromiumArgs(LaunchOptions options)
        {
            var chromiumArgs = new List<string>();

            if (!options.IgnoreDefaultArgs)
            {
                chromiumArgs.AddRange(GetDefaultArgs(options));
            }
            else if (options.IgnoredDefaultArgs?.Length > 0)
            {
                chromiumArgs.AddRange(GetDefaultArgs(options).Except(options.IgnoredDefaultArgs));
            }
            else
            {
                chromiumArgs.AddRange(options.Args);
            }

            TempDirectory tempUserDataDirectory = null;

            if (!chromiumArgs.Any(argument => argument.StartsWith("--remote-debugging-", StringComparison.Ordinal)))
            {
                chromiumArgs.Add("--remote-debugging-port=0");
            }

            var userDataDirOption = chromiumArgs.FirstOrDefault(i => i.StartsWith(UserDataDirArgument, StringComparison.Ordinal));
            if (string.IsNullOrEmpty(userDataDirOption))
            {
                tempUserDataDirectory = new TempDirectory();
                chromiumArgs.Add($"{UserDataDirArgument}={tempUserDataDirectory.Path.Quote()}");
            }

            return (chromiumArgs, tempUserDataDirectory);
        }
    }
}
