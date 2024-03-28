using System;
using System.Runtime.Serialization;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Exception thrown by. <seealso cref="CDPSession.SendAsync{T}(string, object, CommandOptions)"/>
    /// </summary>
    [Serializable]
    public class MessageException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageException"/> class.
        /// </summary>
        public MessageException()
        {
        }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageException"/> class.
        /// </summary>
        /// <param name="info">The serialization collection for custom serializations.</param>
        /// <param name="context">Provides additional caller-provided context.</param>
        protected MessageException(SerializationInfo info, StreamingContext context) : base(info, context)
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
