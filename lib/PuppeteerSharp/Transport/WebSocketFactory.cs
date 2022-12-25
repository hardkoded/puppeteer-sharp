namespace PuppeteerSharp.Transport
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Delegate for creation of <see cref="WebSocket"/> instances.
    /// </summary>
    /// <param name="url">Chromium URL.</param>
    /// <param name="options">Connection options.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task{WebSocket}"/> instance for the asynchronous socket create and connect operation.</returns>
    public delegate Task<WebSocket> WebSocketFactory(Uri url, IConnectionOptions options, CancellationToken cancellationToken);
}
