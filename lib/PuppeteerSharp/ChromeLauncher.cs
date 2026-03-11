using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a Chromium process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public class ChromeLauncher : LauncherBase
    {
        private const string UserDataDirArgument = "--user-data-dir";

        private PipeTransport _pipeTransport;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeLauncher"/> class.
        /// </summary>
        /// <param name="executable">Full path of executable.</param>
        /// <param name="options">Options for launching Chromium.</param>
        public ChromeLauncher(string executable, LaunchOptions options)
            : base(executable, options)
        {
            (var chromiumArgs, TempUserDataDir) = PrepareChromiumArgs(options);

            if (options.Pipe)
            {
                ConfigurePipeProcess(executable, chromiumArgs);
            }
            else
            {
                Process.StartInfo.Arguments = string.Join(" ", chromiumArgs);
            }
        }

        /// <inheritdoc />
        internal override PipeTransport PipeTransport => _pipeTransport;

        /// <inheritdoc />
        public override string ToString() => $"Chromium process; EndPoint={EndPoint}; State={CurrentState}";

        internal static string[] GetDefaultArgs(LaunchOptions options)
        {
            var userDisabledFeatures = GetFeatures("--disable-features", options.Args);
            var args = options.Args ?? [];
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
                "RenderDocument",
                "IPH_ReadingModePageActionLabel",
                "ReadAnythingOmniboxChip",
                "ProcessPerSiteUpToMainFrameThreshold",
                "IsolateSandboxedIframes",
                "PartitionAllocSchedulerLoopQuarantineTaskControlledPurge",
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
                "PdfOopif",
            };

            enabledFeatures.AddRange(userEnabledFeatures);

            var chromiumArguments = new List<string>(
            [
                "--allow-pre-commit-input",
                "--disable-background-networking",
                "--disable-background-timer-throttling",
                "--disable-backgrounding-occluded-windows",
                "--disable-breakpad",
                "--disable-client-side-phishing-detection",
                "--disable-component-extensions-with-background-pages",
                "--disable-default-apps",
                "--disable-dev-shm-usage",
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
            ])
            {
                $"--disable-features={string.Join(",", disabledFeatures)}",
                $"--enable-features={string.Join(",", enabledFeatures)}",
            };

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

            chromiumArguments.Add(
                options.EnableExtensions is { Enabled: true }
                    ? "--enable-unsafe-extension-debugging"
                    : "--disable-extensions");

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

        /// <summary>
        /// Creates the pipe transport after the process has started.
        /// Must be called after <see cref="LauncherBase.StartAsync"/>.
        /// </summary>
        internal void InitializePipeTransport()
        {
            _pipeTransport = new PipeTransport(
                Process.StandardInput.BaseStream,
                Process.StandardOutput.BaseStream);
            _pipeTransport.Start();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pipeTransport?.Dispose();
            }

            base.Dispose(disposing);
        }

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
                if (options.Pipe)
                {
                    chromiumArgs.Add("--remote-debugging-pipe");
                }
                else
                {
                    chromiumArgs.Add("--remote-debugging-port=0");
                }
            }

            var userDataDirOption = chromiumArgs.FirstOrDefault(i => i.StartsWith(UserDataDirArgument, StringComparison.Ordinal));
            if (string.IsNullOrEmpty(userDataDirOption))
            {
                tempUserDataDirectory = new TempDirectory();
                chromiumArgs.Add($"{UserDataDirArgument}={tempUserDataDirectory.Path.Quote()}");
            }

            return (chromiumArgs, tempUserDataDirectory);
        }

        private static string FindShell()
        {
            if (System.IO.File.Exists("/bin/bash"))
            {
                return "/bin/bash";
            }

            if (System.IO.File.Exists("/usr/bin/bash"))
            {
                return "/usr/bin/bash";
            }

            return "/bin/sh";
        }

        private void ConfigurePipeProcess(string executable, List<string> chromiumArgs)
        {
            var arguments = string.Join(" ", chromiumArgs);

            // Redirect stdin/stdout so we can use them as the pipe transport.
            // The shell script remaps stdin→FD3 (browser reads) and stdout→FD4 (browser writes),
            // then redirects the real stdin from /dev/null and stdout to stderr.
            Process.StartInfo.RedirectStandardInput = true;
            Process.StartInfo.RedirectStandardOutput = true;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.StartInfo.Arguments = arguments;
            }
            else
            {
                // On Unix, use a shell wrapper to remap stdin/stdout to FDs 3/4.
                // exec 3<&0 → FD 3 reads from what was stdin (our write pipe)
                // exec 4>&1 → FD 4 writes to what was stdout (our read pipe)
                // 0</dev/null → redirect stdin from /dev/null
                // 1>&2 → redirect stdout to stderr (for DumpIO)
                var shell = FindShell();
                var script = $"exec 3<&0 4>&1 0</dev/null 1>&2; exec \"{executable}\" {arguments}";

                Process.StartInfo.FileName = shell;
                Process.StartInfo.Arguments = string.Empty;
                Process.StartInfo.ArgumentList.Add("-c");
                Process.StartInfo.ArgumentList.Add(script);
            }
        }
    }
}
