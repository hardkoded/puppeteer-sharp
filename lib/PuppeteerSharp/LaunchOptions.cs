using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options for launching the Chrome/ium browser.
    /// </summary>
    public class LaunchOptions : IBrowserOptions
    {
        /// <summary>
        /// Whether to ignore HTTPS errors during navigation. Defaults to false.
        /// </summary>
        public bool IgnoreHTTPSErrors { get; set; }

        /// <summary>
        /// If set to true, sets Headless = false, otherwise, enables automation.
        /// </summary>
        public bool AppMode { get; set; }

        /// <summary>
        /// Whether to run browser in headless mode. Defaults to true unless the devtools option is true.
        /// </summary>
        public bool Headless { get; set; } = true;

        /// <summary>
        /// Path to a Chromium or Chrome executable to run instead of bundled Chromium. If executablePath is a relative path, then it is resolved relative to current working directory.
        /// </summary>
        public string ExecutablePath { get; set; }

        /// <summary>
        /// Slows down Puppeteer operations by the specified amount of milliseconds. Useful so that you can see what is going on.
        /// </summary>
        public int SlowMo { get; set; }

        /// <summary>
        /// Additional arguments to pass to the browser instance. List of Chromium flags can be found <a href="http://peter.sh/experiments/chromium-command-line-switches/">here</a>.
        /// </summary>
        public string[] Args { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Maximum time in milliseconds to wait for the browser instance to start. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
        /// </summary>
        public int Timeout { get; set; } = 30_000;

        /// <summary>
        ///  Whether to pipe browser process stdout and stderr into process.stdout and process.stderr. Defaults to false.
        /// </summary>
        public bool DumpIO { get; set; }

        /// <summary>
        /// Path to a User Data Directory.
        /// </summary>
        public string UserDataDir { get; set; }

        /// <summary>
        /// Specify environment variables that will be visible to browser. Defaults to Environment variables.
        /// </summary>
        public IDictionary<string, string> Env { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Whether to auto-open DevTools panel for each tab. If this option is true, the headless option will be set false.
        /// </summary>
        public bool Devtools { get; set; }

        /// <summary>
        /// Keep alive value.
        /// </summary>
        public int KeepAliveInterval { get; set; } = 30;

        /// <summary>
        /// Logs process counts after launching chrome and after exiting.
        /// </summary>
        public bool LogProcess { get; set; }
    }
}
