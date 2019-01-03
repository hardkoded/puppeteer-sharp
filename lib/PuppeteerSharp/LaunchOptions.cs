using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options for launching the Chrome/ium browser.
    /// </summary>
    public class LaunchOptions : IBrowserOptions, IConnectionOptions
    {
        private string[] _ignoredDefaultArgs;
        private bool _devtools;

        /// <summary>
        /// Whether to ignore HTTPS errors during navigation. Defaults to false.
        /// </summary>
        public bool IgnoreHTTPSErrors { get; set; }

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
        public int Timeout { get; set; } = Puppeteer.DefaultTimeout;

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
        public bool Devtools
        {
            get => _devtools;
            set
            {
                _devtools = value;
                if (value)
                {
                    Headless = false;
                }
            }
        }

        /// <summary>
        /// Keep alive value.
        /// </summary>
        [Obsolete("Chromium doesn't support pings yet (see: https://bugs.chromium.org/p/chromium/issues/detail?id=865002)")]
        public int KeepAliveInterval { get; set; } = 0;

        /// <summary>
        /// Logs process counts after launching chrome and after exiting.
        /// </summary>
        public bool LogProcess { get; set; }

        /// <summary>
        /// If <c>true</c>, then do not use <see cref="Puppeteer.DefaultArgs"/>.
        /// Dangerous option; use with care. Defaults to <c>false</c>.
        /// </summary>
        public bool IgnoreDefaultArgs { get; set; }

        /// <summary>
        /// if <see cref="IgnoreDefaultArgs"/> is set to <c>false</c> this list will be used to filter <see cref="Puppeteer.DefaultArgs"/>
        /// </summary>
        public string[] IgnoredDefaultArgs
        {
            get => _ignoredDefaultArgs;
            set
            {
                IgnoreDefaultArgs = true;
                _ignoredDefaultArgs = value;
            }
        }

        /// <summary>
        /// Optional factory for <see cref="WebSocket"/> implementations.
        /// If <see cref="Transport"/> is set this property will be ignored.
        /// </summary>
        public Func<Uri, IConnectionOptions, CancellationToken, Task<WebSocket>> WebSocketFactory { get; set; }

        /// <summary>
        /// Optional connection transport.
        /// </summary>
        public IConnectionTransport Transport { get; set; }

        /// <summary>
        /// Gets or sets the default Viewport.
        /// </summary>
        /// <value>The default Viewport.</value>
        public ViewPortOptions DefaultViewport { get; set; } = ViewPortOptions.Default;

        /// <summary>
        /// If not <see cref="Transport"/> is set this will be use to determine is the default <see cref="WebSocketTransport"/> will enqueue messages.
        /// </summary>
        /// <remarks>
        /// It's set to <c>true</c> by default because it's the safest way to send commands to Chromium.
        /// Setting this to <c>false</c> proved to work in .NET Core but it tends to fail on .NET Framework.
        /// </remarks>
        public bool EnqueueTransportMessages { get; set; } = true;
    }
}