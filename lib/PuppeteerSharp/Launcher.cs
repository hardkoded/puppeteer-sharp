﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;

namespace PuppeteerSharp
{
    public class Launcher
    {
        private static string[] _defaultArgs = {
            "--disable-background-networking",
            "--disable-background-timer-throttling",
            "--disable-client-side-phishing-detection",
            "--disable-default-apps",
            "--disable-extensions",
            "--disable-hang-monitor",
            "--disable-popup-blocking",
            "--disable-prompt-on-repost",
            "--disable-sync",
            "--disable-translate",
            "--metrics-recording-only",
            "--no-first-run",
            "--remote-debugging-port=0",
            "--safebrowsing-disable-auto-update",
        };

        public static string[] _automationArgs = {
            "--enable-automation",
            "--password-store=basic",
            "--use-mock-keychain"
        };

        private bool _chromeClosed;
        private Process _chromeProcess;
        private string _temporaryUserDataDir = null;
        private Connection _connection = null;
        private Timer _timer = null;

        internal async Task<Browser> LaunchAsync(Dictionary<string, object> options, int chromiumRevision)
        {
            var chromeArguments = new List<string>(_defaultArgs);

            if (options.ContainsKey("appMode"))
            {
                options["headless"] = false;
            }
            else
            {
                chromeArguments.AddRange(_automationArgs);
            }

            if (options.ContainsKey("args") &&
               ((string[])options["args"]).Any(i => i.StartsWith("--user-data-dir", StringComparison.Ordinal)))
            {
                if (!options.ContainsKey("userDataDir"))
                {
                    _temporaryUserDataDir = GetTemporaryDirectory();
                    chromeArguments.Add($"--user-data-dir=${_temporaryUserDataDir}");
                }
                else
                {
                    chromeArguments.Add($"--user-data-dir=${options["userDataDir"]}");
                }
            }

            if (options.TryGetValue("devtools", out var hasDevTools) && (bool)hasDevTools)
            {
                chromeArguments.Add("--auto-open-devtools-for-tabs");
                options["headless"] = false;
            }

            if (options.TryGetValue("headless", out var isHeadless) && (bool)isHeadless)
            {
                chromeArguments.AddRange(new[]{
                    "--headless",
                    "--disable-gpu",
                    "--hide-scrollbars",
                    "--mute-audio"
                });
            }

            var chromeExecutable = (options.GetValueOrDefault("executablePath") ?? "").ToString();

            if (string.IsNullOrEmpty(chromeExecutable))
            {
                var downloader = Downloader.CreateDefault();
                var revisionInfo = downloader.RevisionInfo(Downloader.CurrentPlatform, chromiumRevision);
                chromeExecutable = revisionInfo.ExecutablePath;
            }

            if (options.ContainsKey("args"))
            {
                chromeArguments.AddRange((string[])options["args"]);
            }

            _chromeProcess = new Process();
            _chromeProcess.StartInfo.FileName = chromeExecutable;
            _chromeProcess.StartInfo.Arguments = string.Join(" ", chromeArguments);

            SetEnvVariables(_chromeProcess.StartInfo.Environment,
                            options.ContainsKey("env") ? (IDictionary<string, string>)options["env"] : null,
                            (IDictionary)Environment.GetEnvironmentVariables());

            if (!options.ContainsKey("dumpio"))
            {
                _chromeProcess.StartInfo.RedirectStandardOutput = false;
                _chromeProcess.StartInfo.RedirectStandardError = false;
            }

            _chromeProcess.Exited += async (sender, e) =>
            {
                _chromeClosed = true;
                await KillChrome();
            };

            try
            {
                var connectionDelay = (int)(options.GetValueOrDefault("slowMo") ?? 0);
                var browserWSEndpoint = await WaitForEndpoint(_chromeProcess,
                                                              (int)(options.GetValueOrDefault("timeout") ?? 30 * 100),
                                                              options.ContainsKey("dumpio"));
                var keepAliveInterval = options.ContainsKey("keepAliveInterval") ? Convert.ToInt32(options["keepAliveInterval"]) : 30;

                _connection = await Connection.Create(browserWSEndpoint, connectionDelay, keepAliveInterval);
                return await Browser.CreateAsync(_connection, options, KillChrome);
            }
            catch (Exception ex)
            {
                await ForceKillChrome();
                throw new Exception("Failed to create connection", ex);
            }

        }

        private Task<string> WaitForEndpoint(Process chromeProcess, int timeout, bool dumpio)
        {
            var taskWrapper = new TaskCompletionSource<string>();
            var output = string.Empty;

            chromeProcess.StartInfo.RedirectStandardOutput = true;
            chromeProcess.StartInfo.RedirectStandardError = true;

            chromeProcess.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    output += e.Data + "\n";
                    var match = Regex.Match(e.Data, "^DevTools listening on (ws:\\/\\/.*)");

                    if (!match.Success)
                    {
                        return;
                    }

                    CleanUp();
                    taskWrapper.SetResult(match.Groups[1].Value);

                    //Restore defaults for Redirects
                    if (!dumpio)
                    {
                        chromeProcess.StartInfo.RedirectStandardOutput = false;
                        chromeProcess.StartInfo.RedirectStandardError = false;
                    }
                }
            };

            chromeProcess.Exited += (sender, e) =>
            {
                CleanUp();

                var error = chromeProcess.StandardError.ReadToEnd();
                taskWrapper.SetException(new ChromeProcessException($"Failed to launch chrome! {error}"));
            };

            if (timeout > 0)
            {
                //We have to declare timer before initializing it because if we don't do this 
                //we can't dispose it in the action created in the constructor
                _timer = new Timer((state) =>
                {
                    taskWrapper.SetException(
                        new ChromeProcessException($"Timed out after {timeout} ms while trying to connect to Chrome! "));
                    _timer.Dispose();
                }, null, timeout, 0);

            }

            chromeProcess.Start();
            chromeProcess.BeginErrorReadLine();
            return taskWrapper.Task;
        }

        private void CleanUp()
        {
            _timer?.Dispose();
            _timer = null;
            _chromeProcess?.RemoveExitedEvent();
        }

        private async Task KillChrome()
        {
            if (!string.IsNullOrEmpty(_temporaryUserDataDir))
            {
                await ForceKillChrome();
            }
            else if (_connection != null)
            {
                await _connection.SendAsync("Browser.close", null);
            }
        }

        private async Task ForceKillChrome()
        {
            if (_chromeProcess.Id != 0 && Process.GetProcessById(_chromeProcess.Id) != null)
            {
                _chromeProcess.Kill();
            }

            if (_temporaryUserDataDir != null)
            {
                await Task.Factory.StartNew(path => Directory.Delete((string)path, true), _temporaryUserDataDir);
            }
        }

        private static void SetEnvVariables(IDictionary<string, string> environment, IDictionary<string, string> customEnv,
                                            IDictionary realEnv)
        {
            foreach (DictionaryEntry item in realEnv)
            {
                environment[item.Key.ToString()] = item.Value.ToString();
            }

            if (customEnv != null)
            {
                foreach (var item in customEnv)
                {
                    environment[item.Key] = item.Value;
                }
            }
        }

        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
