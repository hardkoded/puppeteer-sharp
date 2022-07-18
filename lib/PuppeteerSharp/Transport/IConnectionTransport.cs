using System;
using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom.Transport
{
    /// <summary>
    /// Connection transport abstraction.
    /// </summary>
    public interface IConnectionTransport : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="Transport.IConnectionTransport"/> is closed.
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
        /// Occurs when a message is received.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        /// <summary>
        /// Occurs when an error occurs processing the message.
        /// </summary>
        event EventHandler<MessageErrorEventArgs> MessageError;
    }
}
