using System;
using System.Net.WebSockets;

namespace PuppeteerSharp
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Options for connecting to an existing browser.
    /// </summary>
    public class ConnectOptions : IBrowserOptions, IConnectionOptions
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
        /// A browser websocket endpoint to connect to.
        /// </summary>
        public string BrowserWSEndpoint { get; set; }

        /// <summary>
        /// Slows down Puppeteer operations by the specified amount of milliseconds. Useful so that you can see what is going on.
        /// </summary>
        public int SlowMo { get; set; }

        /// <summary>
        /// Keep alive value.
        /// </summary>
        [Obsolete("Chromium doesn't support pings yet (see: https://bugs.chromium.org/p/chromium/issues/detail?id=865002)")]
        public int KeepAliveInterval { get; set; } = 0;

        /// <summary>
        /// Optional factory for <see cref="WebSocket"/> implementations.
        /// </summary>
        public Func<Uri, IConnectionOptions, CancellationToken, Task<WebSocket>> WebSocketFactory { get; set; }
    }
}