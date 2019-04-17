using System;

namespace PuppeteerSharp.Transport
{
    /// <summary>
    /// Message received event arguments.
    /// <see cref="IConnectionTransport.MessageReceived"/>
    /// </summary>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.Input.ClickOptions class instead")]
    public class MessageReceivedEventArgs : Abstractions.Transport.MessageReceivedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerSharp.Transport.MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public MessageReceivedEventArgs(string message)
            : base(message)
        {
        }
    }
}