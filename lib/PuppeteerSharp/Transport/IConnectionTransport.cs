using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PuppeteerSharp.Transport
{
    /// <summary>
    /// Connection transport abstraction.
    /// </summary>
    public interface IConnectionTransport : IDisposable
    {
        /// <summary>
        /// Initialize the Transport
        /// </summary>
        /// <param name="url">Chromium URL</param>
        /// <param name="connectionOptions">Connection options</param>
        /// <param name="loggerFactory">Logger factory</param>
        Task InitializeAsync(string url, IConnectionOptions connectionOptions, ILoggerFactory loggerFactory = null);

        /// <summary>
        /// Gets a value indicating whether this <see cref="PuppeteerSharp.Transport.IConnectionTransport"/> is closed.
        /// </summary>
        bool IsClosed { get; }
        /// <summary>
        /// Stops reading incoming data.
        /// </summary>
        void StopReading();
        /// <summary>
        /// Sends a message using the transport.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="message">Message to send.</param>
        Task SendAsync(string message);
        /// <summary>
        /// Occurs when the transport is closed.
        /// </summary>
        event EventHandler<TransportClosedEventArgs> Closed;
        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
