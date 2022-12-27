using System;

namespace PuppeteerSharp.Transport
{
    /// <summary>
    /// Message received event arguments.
    /// <see cref="IConnectionTransport.MessageReceived"/>.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerSharp.Transport.MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public MessageReceivedEventArgs(string message) => Message = message;

        /// <summary>
        /// Transport message.
        /// </summary>
        public string Message { get; }
    }
}
