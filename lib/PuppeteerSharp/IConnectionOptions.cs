using System;
using System.Net.WebSockets;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Transport;

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
        /// Optional factory for <see cref="WebSocket"/> implementations.
        /// If <see cref="Transport"/> is set this property will be ignored.
        /// </summary>
        WebSocketFactory WebSocketFactory { get; set; }

        /// <summary>
        /// Optional factory for <see cref="IConnectionTransport"/> implementations.
        /// </summary>
        TransportFactory TransportFactory { get; set; }

        /// <summary>
        /// If not <see cref="Transport"/> is set this will be use to determine is the default <see cref="WebSocketTransport"/> will enqueue messages.
        /// </summary>
        bool EnqueueTransportMessages { get; set; }

        /// <summary>
        /// Affects how responses to <see cref="CDPSession.SendAsync"/> are returned to the caller.
        /// </summary>
        bool EnqueueAsyncMessages { get; set; }

        /// <summary>
        /// Callback to decide if Puppeteer should connect to a given target or not.
        /// </summary>
        public Func<Target, bool> TargetFilter { get; set; }

        /// <summary>
        /// Timeout setting for individual protocol (CDP) calls.
        /// Defaults to 180_000.
        /// </summary>
        public int ProtocolTimeout { get; set; }
    }
}
