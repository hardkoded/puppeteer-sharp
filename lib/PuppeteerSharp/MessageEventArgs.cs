using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp
{
    /// <summary>
    /// <seealso cref="CDPSession.MessageReceived"/> arguments.
    /// </summary>
    public class MessageEventArgs
    {
        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        public string MessageID { get; internal set; }
        /// <summary>
        /// Gets or sets the message data.
        /// </summary>
        /// <value>The message data.</value>
        public dynamic MessageData { get; internal set; }
    }
}