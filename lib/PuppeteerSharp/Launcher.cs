using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using System.Linq;
using System.IO;

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

        public Launcher()
        {
        }

        internal static void Launch(Dictionary<string, object> options, PuppeteerOptions puppeteerOptions)
        {
            string temporaryUserDataDir;
            var chromeArguments = new List<string>(_defaultArgs);

            if(options.ContainsKey("appMode"))
            {
                options["headless"] = false;
            }
            else
            {
                chromeArguments.AddRange(_automationArgs);
            }

            if(options.ContainsKey("args") && 
               ((string[])options["args"]).Any(i => i.StartsWith("--user-data-dir", StringComparison.Ordinal)))
            {
                if(!options.ContainsKey("userDataDir"))
                {
                    temporaryUserDataDir = GetTemporaryDirectory();
                    chromeArguments.Add($"--user-data-dir=${temporaryUserDataDir}");
                }
                else
                {
                    chromeArguments.Add($"--user-data-dir=${options["userDataDir"]}");
                }
            }

            if((bool)options.GetValueOrDefault("devtools"))
            {
                chromeArguments.Add("--auto-open-devtools-for-tabs");
                options["headless"] = false;
            }

            if ((bool)options.GetValueOrDefault("headless"))
            {
                chromeArguments.AddRange(new []{
                    "--headless",
                    "--disable-gpu",
                    "--hide-scrollbars",
                    "--mute-audio"
                });
            }

            var chromeExecutable = (options.GetValueOrDefault("executablePath") ?? "").ToString();

            if(!string.IsNullOrEmpty(chromeExecutable))
            {
                var downloader = Downloader.CreateDefault();-
                var revisionInfo = downloader.RevisionInfo(Downloader.CurrentPlatform(), 
                                                           puppeteerOptions.ChromiumRevision);
            }
            /*
            if (typeof chromeExecutable !== 'string') {
              const downloader = Downloader.createDefault();
              const revisionInfo = downloader.revisionInfo(downloader.currentPlatform(), ChromiumRevision);
              console.assert(revisionInfo.downloaded, `Chromium revision is not downloaded. Run "npm install"`);
              chromeExecutable = revisionInfo.executablePath;
            }
            if (Array.isArray(options.args))
              chromeArguments.push(...options.args);

            const chromeProcess = childProcess.spawn(
                chromeExecutable,
                chromeArguments,
                {
                  detached: true,
                  env: options.env || process.env
                }
            );
            if (options.dumpio) {
              chromeProcess.stdout.pipe(process.stdout);
              chromeProcess.stderr.pipe(process.stderr);
            }

            let chromeClosed = false;
            const waitForChromeToClose = new Promise((fulfill, reject) => {
              chromeProcess.once('close', () => {
                chromeClosed = true;
                // Cleanup as processes exit.
                if (temporaryUserDataDir) {
                  removeFolderAsync(temporaryUserDataDir)
                      .then(() => fulfill())
                      .catch(err => console.error(err));
                } else {
                  fulfill();
                }
              });
            });

            const listeners = [ helper.addEventListener(process, 'exit', forceKillChrome) ];
            if (options.handleSIGINT !== false)
              listeners.push(helper.addEventListener(process, 'SIGINT', forceKillChrome));
            if (options.handleSIGTERM !== false)
              listeners.push(helper.addEventListener(process, 'SIGTERM', killChrome));
            if (options.handleSIGHUP !== false)
              listeners.push(helper.addEventListener(process, 'SIGHUP', killChrome));
            @type {?Connection} 
            let connection = null;
            try
            {
                const connectionDelay = options.slowMo || 0;
                const browserWSEndpoint = await waitForWSEndpoint(chromeProcess, options.timeout || 30 * 1000);
                connection = await Connection.create(browserWSEndpoint, connectionDelay);
                return Browser.create(connection, options, killChrome);
            }
            catch (e)
            {
                forceKillChrome();
                throw e;
            }
            */
        }

        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
