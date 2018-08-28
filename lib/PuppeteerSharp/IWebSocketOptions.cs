using System;
using System.Net.WebSockets;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="WebSocket"/> creation.
    /// </summary>
    public interface IWebSocketOptions
    {
        /// <summary>
        /// Keep alive value (in milliseconds)
        /// </summary>
        [Obsolete("Chromium doesn't support pings yet (see: https://bugs.chromium.org/p/chromium/issues/detail?id=865002)")]
        int KeepAliveInterval { get; set; }
    }
}