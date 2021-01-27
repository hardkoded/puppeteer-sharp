using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a Chromium process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public class ChromiumLauncher : LauncherBase
    {
        #region Constants

        internal static readonly string[] DefaultArgs = {
            "--disable-background-networking",
            "--enable-features=NetworkService,NetworkServiceInProcess",
            "--disable-background-timer-throttling",
            "--disable-backgrounding-occluded-windows",
            "--disable-breakpad",
            "--disable-client-side-phishing-detection",
            "--disable-component-extensions-with-background-pages",
            "--disable-default-apps",
            "--disable-dev-shm-usage",
            "--disable-extensions",
            "--disable-features=TranslateUI",
            "--disable-hang-monitor",
            "--disable-ipc-flooding-protection",
            "--disable-popup-blocking",
            "--disable-prompt-on-repost",
            "--disable-renderer-backgrounding",
            "--disable-sync",
            "--force-color-profile=srgb",
            "--metrics-recording-only",
            "--no-first-run",
            "--enable-automation",
            "--password-store=basic",
            "--use-mock-keychain"
        };

        private const string UserDataDirArgument = "--user-data-dir";

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="ChromiumLauncher"/> instance.
        /// </summary>
        /// <param name="executable">Full path of executable.</param>
        /// <param name="options">Options for launching Chromium.</param>
        /// <param name="loggerFactory">Logger factory</param>
        public ChromiumLauncher(string executable, LaunchOptions options, ILoggerFactory loggerFactory)
            : base(executable, options, loggerFactory)
        {
            List<string> chromiumArgs;
            (chromiumArgs, TempUserDataDir) = PrepareChromiumArgs(options);

            Process.StartInfo.Arguments = string.Join(" ", chromiumArgs);
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public override string ToString() => $"Chromium process; EndPoint={EndPoint}; State={CurrentState}";

        #endregion

        #region Private methods

        private static (List<string> chromiumArgs, TempDirectory tempUserDataDirectory) PrepareChromiumArgs(LaunchOptions options)
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

            string userDataDirOption = chromiumArgs.FirstOrDefault(i => i.StartsWith(UserDataDirArgument, StringComparison.Ordinal));
            if (string.IsNullOrEmpty(userDataDirOption))
            {
                tempUserDataDirectory = new TempDirectory();
                chromiumArgs.Add($"{UserDataDirArgument}={tempUserDataDirectory.Path.Quote()}");
            }

            return (chromiumArgs, tempUserDataDirectory);
        }

        internal static string[] GetDefaultArgs(LaunchOptions options)
        {
            var chromiumArguments = new List<string>(DefaultArgs);

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
                chromiumArguments.AddRange(new[] {
                    "--headless",
                    "--hide-scrollbars",
                    "--mute-audio"
                });
            }

            if (options.Args.All(arg => arg.StartsWith("-", StringComparison.Ordinal)))
            {
                chromiumArguments.Add("about:blank");
            }

            chromiumArguments.AddRange(options.Args);
            return chromiumArguments.ToArray();
        }

        #endregion
    }
}
