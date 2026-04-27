using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
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

        private AnonymousPipeServerStream _pipeToProcess;
        private AnonymousPipeServerStream _pipeFromProcess;
        private PipeTransport _pipeTransport;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeLauncher"/> class.
        /// </summary>
        /// <param name="executable">Full path of executable.</param>
        /// <param name="options">Options for launching Chromium.</param>
        public ChromeLauncher(string executable, LaunchOptions options)
            : base(executable ?? throw new ArgumentNullException(nameof(executable)), options)
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

            if (string.Equals(
                    Environment.GetEnvironmentVariable("PUPPETEER_DANGEROUS_NO_SANDBOX"),
                    "true",
                    StringComparison.Ordinal) &&
                !args.Contains("--no-sandbox"))
            {
                chromiumArguments.Add("--no-sandbox");
            }

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
            // Dispose the local copy of the client handles now that the child process
            // has inherited them. This prevents handle leaks in the parent process.
            _pipeToProcess.DisposeLocalCopyOfClientHandle();
            _pipeFromProcess.DisposeLocalCopyOfClientHandle();

            _pipeTransport = new PipeTransport(_pipeToProcess, _pipeFromProcess);
            _pipeTransport.Start();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pipeTransport?.Dispose();
                _pipeToProcess?.Dispose();
                _pipeFromProcess?.Dispose();
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
            if (File.Exists("/bin/bash"))
            {
                return "/bin/bash";
            }

            if (File.Exists("/usr/bin/bash"))
            {
                return "/usr/bin/bash";
            }

            return "/bin/sh";
        }

        private void ConfigurePipeProcess(string executable, List<string> chromiumArgs)
        {
            var arguments = string.Join(" ", chromiumArgs);

            // Create anonymous pipes with inheritable handles so the child process
            // can communicate via these pipes instead of WebSocket.
            _pipeToProcess = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
            _pipeFromProcess = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);

            var readHandle = _pipeToProcess.GetClientHandleAsString();
            var writeHandle = _pipeFromProcess.GetClientHandleAsString();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // On Windows, pass pipe handles directly via --remote-debugging-io-pipes.
                Process.StartInfo.Arguments = $"{arguments} --remote-debugging-io-pipes={readHandle},{writeHandle}";
            }
            else
            {
                // On Unix, use a shell wrapper to remap the inherited pipe FDs to FDs 3/4.
                // exec 3<&{fd} → FD 3 reads from our write pipe (browser reads commands)
                // exec 4>&{fd} → FD 4 writes to our read pipe (browser writes responses)
                // {fd}<&- / {fd}>&- → close the original pipe FDs to avoid leaking them
                var shell = FindShell();

                var escapedExecutable = executable.Replace("\"", "\\\"");
                var escapedArguments = arguments.Replace("\"", "\\\"");
                var script = $"exec 3<&{readHandle} 4>&{writeHandle} {readHandle}<&- {writeHandle}>&-; exec \\\"{escapedExecutable}\\\" {escapedArguments}";

                Process.StartInfo.FileName = shell;
                Process.StartInfo.Arguments = $"-c \"{script}\"";
            }
        }
    }
}
