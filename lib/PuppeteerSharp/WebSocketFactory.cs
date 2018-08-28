using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Factory method for connected web sockets.
    /// </summary>
    /// <param name="uri">The uri to connect to.</param>
    /// <param name="options">Web socket creation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="WebSocket"/> that is connected to the given <paramref name="uri"/>.</returns>
    public delegate Task<WebSocket> WebSocketFactory(Uri uri, IWebSocketOptions options, CancellationToken cancellationToken);
}