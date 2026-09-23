using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// Registers a synchronous fallback for removing a temporary profile when the
    /// host process exits before the browser process can run its async cleanup.
    /// </summary>
    /// <remarks>
    /// Port of upstream <c>registerProcessExitCleanup</c> from BrowserLauncher.ts (#15441).
    /// </remarks>
    internal static class ProcessExitCleanup
    {
        private static readonly object Gate = new();
        private static readonly Dictionary<IProcessExitEmitter, CleanupState> ProcessExitCleanupEntries = new();

        /// <summary>
        /// Registers synchronous cleanup of <paramref name="userDataDir"/> on process exit.
        /// </summary>
        /// <param name="userDataDir">Temporary user data directory to remove.</param>
        /// <param name="logError">Optional error logger; defaults to <see cref="Debug.WriteLine(object)"/>.</param>
        /// <param name="processEmitter">Process-exit source; defaults to <see cref="AppDomainProcessExitEmitter.Instance"/>.</param>
        /// <returns>An action that unregisters this cleanup entry.</returns>
        public static Action Register(
            string userDataDir,
            Action<Exception> logError = null,
            IProcessExitEmitter processEmitter = null)
        {
            if (string.IsNullOrEmpty(userDataDir))
            {
                throw new ArgumentException("Path must be specified", nameof(userDataDir));
            }

            processEmitter ??= AppDomainProcessExitEmitter.Instance;
            logError ??= static ex => Debug.WriteLine(ex);

            lock (Gate)
            {
                if (!ProcessExitCleanupEntries.TryGetValue(processEmitter, out var cleanup))
                {
                    var entries = new HashSet<CleanupEntry>();
                    void OnExit()
                    {
                        CleanupEntry[] snapshot;
                        lock (Gate)
                        {
                            snapshot = new CleanupEntry[entries.Count];
                            entries.CopyTo(snapshot);
                        }

                        foreach (var entry in snapshot)
                        {
                            try
                            {
                                DeleteDirectorySync(entry.UserDataDir);
                            }
                            catch (Exception error)
                            {
                                entry.LogError(error);
                            }
                        }
                    }

                    cleanup = new CleanupState(entries, OnExit);
                    ProcessExitCleanupEntries[processEmitter] = cleanup;
                    processEmitter.Once(OnExit);
                }

                var entry = new CleanupEntry(userDataDir, logError);
                cleanup.Entries.Add(entry);

                return () =>
                {
                    lock (Gate)
                    {
                        if (!cleanup.Entries.Remove(entry) || cleanup.Entries.Count > 0)
                        {
                            return;
                        }

                        processEmitter.Off(cleanup.OnExit);
                        ProcessExitCleanupEntries.Remove(processEmitter);
                    }
                };
            }
        }

        private static void DeleteDirectorySync(string path)
        {
            const int maxRetries = 3;
            const int retryDelayMs = 100;

            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, recursive: true);
                    }

                    return;
                }
                catch (Exception) when (attempt < maxRetries - 1)
                {
                    Thread.Sleep(retryDelayMs);
                }
            }
        }

        private sealed class CleanupEntry(string userDataDir, Action<Exception> logError)
        {
            public string UserDataDir { get; } = userDataDir;

            public Action<Exception> LogError { get; } = logError;
        }

        private sealed class CleanupState(HashSet<CleanupEntry> entries, Action onExit)
        {
            public HashSet<CleanupEntry> Entries { get; } = entries;

            public Action OnExit { get; } = onExit;
        }
    }
}
