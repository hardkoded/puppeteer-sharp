using System.Net.WebSockets;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="Connection"/> creation.
    /// </summary>
    public interface IConnectionOptions : IWebSocketOptions
    {
        /// <summary>
        /// Slows down Puppeteer operations by the specified amount of milliseconds. Useful so that you can see what is going on.
        /// </summary>
        int SlowMo { get; set; }

        /// <summary>
        /// Optional factory for <see cref="WebSocket"/> implementations.
        /// </summary>
        WebSocketFactory WebSocketFactory { get; set; }
    }
}