using System;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Exception thrown by <seealso cref="CDPSession.SendAsync(string, object)"/>
    /// </summary>
    public class MessageException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public MessageException(string message) : base(message)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public MessageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal MessageException(MessageTask callback, ConnectionError error) : base(GetCallbackMessage(callback, error))
        {
        }

        internal static string GetCallbackMessage(MessageTask callback, ConnectionError connectionError)
        {
            var message = $"Protocol error ({callback.Method}): {connectionError.Message}";

            if (!string.IsNullOrEmpty(connectionError.Data))
            {
                message += $" {connectionError.Data}";
            }

            return !string.IsNullOrEmpty(connectionError.Message) ? RewriteErrorMeesage(message) : string.Empty;
        }
    }
}