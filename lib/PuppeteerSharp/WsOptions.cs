using System.Collections.Generic;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options for the WebSocket connection to the browser.
    /// </summary>
    public class WsOptions
    {
        /// <summary>
        /// Default ping period in milliseconds when <see cref="KeepAlive"/> is enabled.
        /// </summary>
        public const int DefaultKeepAliveIntervalMs = 30_000;

        /// <summary>
        /// Headers to use for the web socket connection.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Whether to send WebSocket pings and drop the connection when a pong does
        /// not come back within the same interval. Detects a connection that died
        /// without a close frame, which otherwise leaves calls hanging until
        /// <see cref="IConnectionOptions.ProtocolTimeout"/>.
        /// </summary>
        /// <remarks>
        /// Dead-connection detection requires .NET 9 or later. On earlier runtimes,
        /// enabling keep-alive only configures the WebSocket keep-alive interval.
        /// </remarks>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// Ping period in milliseconds. Only used when <see cref="KeepAlive"/> is set.
        /// Defaults to <see cref="DefaultKeepAliveIntervalMs"/>.
        /// </summary>
        public int? KeepAliveIntervalMs { get; set; }
    }
}
