using System;
using System.Text.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// <seealso cref="CDPSession.MessageReceived"/> arguments.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        public string MessageID { get; internal set; }

        /// <summary>
        /// Gets the message data.
        /// </summary>
        /// <value>The message data.</value>
        public JsonElement MessageData { get; internal set; }
    }
}
