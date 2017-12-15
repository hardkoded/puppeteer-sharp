using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

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

        private static bool _chromeClosed;
        private static Process _chromeProcess;
        private static string _temporaryUserDataDir = null;
        private static Connection _connection = null;

        public Launcher()
        {
        }

        internal static async Task<Browser> Launch(Dictionary<string, object> options, PuppeteerOptions puppeteerOptions)
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

            if ((bool)options.GetValueOrDefault("devtools"))
            {
                chromeArguments.Add("--auto-open-devtools-for-tabs");
                options["headless"] = false;
            }

            if ((bool)options.GetValueOrDefault("headless"))
            {
                chromeArguments.AddRange(new[]{
                    "--headless",
                    "--disable-gpu",
                    "--hide-scrollbars",
                    "--mute-audio"
                });
            }

            var chromeExecutable = (options.GetValueOrDefault("executablePath") ?? "").ToString();

            if (!string.IsNullOrEmpty(chromeExecutable))
            {
                var downloader = Downloader.CreateDefault();
                var revisionInfo = downloader.RevisionInfo(Downloader.CurrentPlatform(),
                                                           puppeteerOptions.ChromiumRevision);
                chromeExecutable = revisionInfo.ExecutablePath;
            }

            if (options.ContainsKey("args"))
            {
                chromeArguments.AddRange((string[])options["args"]);
            }

            _chromeProcess = new Process();
            _chromeProcess.StartInfo.FileName = chromeExecutable;
            _chromeProcess.StartInfo.Arguments = string.Join(" ", chromeArguments);

            SetEnvVariables(_chromeProcess.StartInfo.Environment, options.ContainsKey("env") ?
                            (IDictionary<string, string>)options["env"] :
                            (IDictionary<string, string>)Environment.GetEnvironmentVariables());

            if (!options.ContainsKey("dumpio"))
            {
                _chromeProcess.StartInfo.RedirectStandardOutput = false;
                _chromeProcess.StartInfo.RedirectStandardError = false;
            }

            _chromeProcess.Exited += async (sender, e) => {
                _chromeClosed = true;
                await KillChrome();
            };

            try 
            {
                var connectionDelay = (int)(options.GetValueOrDefault("slowMo") ?? 0);
                var browserWSEndpoint = await WaitForEndpoint(_chromeProcess, 
                                                              (int)(options.GetValueOrDefault("timeout") ?? 30 * 100));

                _connection = await Connection.Create(browserWSEndpoint, connectionDelay);
                return await Browser.CreateAsync(_connection, options, KillChrome);
            }
            catch(Exception ex)
            {
                await ForceKillChrome();
                throw new Exception("Failed to create connection", ex);
            }

        }

        private static Task<string> WaitForEndpoint(Process chromeProcess, int timeout)
        {
            var taskWrapper = new TaskCompletionSource<string>();

            var output = string.Empty;

            chromeProcess.OutputDataReceived += (sender, e) =>
            {
                output += e.Data + "\n";
                var match = Regex.Match(e.Data, "^DevTools listening on (ws:\\/\\/.*)");

                if(!match.Success)
                {
                    return;
                }

                CleanUp();
                taskWrapper.SetResult(match.Value);
            };

            chromeProcess.Exited += (sender, e) =>
            {
                CleanUp();

                var error = chromeProcess.StandardError.ReadToEnd();
                taskWrapper.SetException(new ChromeProcessException($"Failed to launch chrome! {error}"));
            };

            if(timeout > 0)
            {
                //We have to declare timer before initializing it because if we don't do this 
                //we can't dispose it in the action created in the constructor
                Timer timer = null;

                timer = new Timer((state) => {
                    taskWrapper.SetException(
                        new ChromeProcessException($"Timed out after ${timeout} ms while trying to connect to Chrome! " +
                                                   "The only Chrome revision guaranteed to work is r${ChromiumRevision}"));
                    timer.Dispose();
                }, null, timeout, 0);

            }

            return taskWrapper.Task;
        }

        private static void CleanUp()
        {
            throw new NotImplementedException();
        }

        private static async Task KillChrome()
        {
            if (!string.IsNullOrEmpty(_temporaryUserDataDir))
            {
                await ForceKillChrome(); 
            }
            else if(_connection != null)
            {
                await _connection.SendAsync("Browser.close", null);
            }
        }

        private static async Task ForceKillChrome()
        {
            if (_chromeProcess.Id != 0 && Process.GetProcessById(_chromeProcess.Id) != null)
            {
                _chromeProcess.Kill();
            }

            await Task.Factory.StartNew(path => Directory.Delete((string)path, true), _temporaryUserDataDir);
        }

        private static void SetEnvVariables(IDictionary<string, string> environment, IDictionary<string, string> dictionary)
        {
            foreach(var item in dictionary)
            {
                environment[item.Key] = item.Value;
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
