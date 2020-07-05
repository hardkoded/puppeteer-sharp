using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a Firefox process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public class FirefoxProcess : ProcessBase
    {
        #region Constants

        internal static readonly string[] DefaultArgs = {
          "--remote-debugging-port=0",
          "--no-remote",
          "--foreground"
        };

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="FirefoxProcess"/> instance.
        /// </summary>
        /// <param name="executable">Full path of executable.</param>
        /// <param name="options">Options for launching Firefox.</param>
        /// <param name="loggerFactory">Logger factory</param>
        public FirefoxProcess(string executable, LaunchOptions options, ILoggerFactory loggerFactory)
            : base(executable, options, loggerFactory)
        {
            List<string> firefoxArgs;
            (firefoxArgs, TempUserDataDir) = PrepareFirefoxArgs(options);

            Process.StartInfo.Arguments = string.Join(" ", firefoxArgs);
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public override string ToString() => $"Firefox process; EndPoint={EndPoint}; State={CurrentState}";

        #endregion

        #region Private methods

        private static (List<string> firefoxArgs, TempDirectory tempUserDataDirectory) PrepareFirefoxArgs(LaunchOptions options)
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

            TempDirectory tempUserDataDirectory = null;

            if (!firefoxArgs.Contains("-profile") && !firefoxArgs.Contains("--profile"))
            {
                tempUserDataDirectory = new TempDirectory();
                firefoxArgs.Add("--profile");
                firefoxArgs.Add($"{tempUserDataDirectory.Path.Quote()}");
            }

            return (firefoxArgs, tempUserDataDirectory);
        }

        internal static string[] GetDefaultArgs(LaunchOptions options)
        {
            var firefoxArguments = new List<string>(DefaultArgs);

            if (!string.IsNullOrEmpty(options.UserDataDir))
            {
                firefoxArguments.Add("--profile");
                firefoxArguments.Add($"{options.UserDataDir.Quote()}");
            }

            if (options.Headless)
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

        #endregion
    }
}
