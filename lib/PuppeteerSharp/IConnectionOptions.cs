using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="Connection"/> creation.
    /// </summary>
    public interface IConnectionOptions
    {
        /// <summary>
        /// Slows down Puppeteer operations by the specified amount of milliseconds. Useful so that you can see what is going on.
        /// </summary>
        int SlowMo { get; set; }

        /// <summary>
        /// Keep alive value (in milliseconds)
        /// </summary>
        [Obsolete("Chromium doesn't support pings yet (see: https://bugs.chromium.org/p/chromium/issues/detail?id=865002)")]
        int KeepAliveInterval { get; set; }

        /// <summary>
        /// Optional factory for <see cref="WebSocket"/> implementations.
        /// </summary>
        Func<Uri, IConnectionOptions, CancellationToken, Task<WebSocket>> WebSocketFactory { get; set; }
    }
}