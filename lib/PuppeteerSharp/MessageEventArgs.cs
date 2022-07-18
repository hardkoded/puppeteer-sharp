using Newtonsoft.Json.Linq;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// <seealso cref="DevToolsConnection.MessageReceived"/> arguments.
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
        public JToken MessageData { get; internal set; }
    }
}
