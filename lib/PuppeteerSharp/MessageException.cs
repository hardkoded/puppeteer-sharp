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

        internal MessageException(MessageTask callback, JObject obj) : base(GetCallbackMessage(callback, obj))
        {
        }

        internal static string GetCallbackMessage(MessageTask callback, JObject obj)
        {
            var error = obj.SelectToken(MessageKeys.Error);
            var message = $"Protocol error ({callback.Method}): {error[MessageKeys.Message]}";

            if (error[MessageKeys.Data] != null)
            {
                message += $" {error[MessageKeys.Data]}";
            }

            return !string.IsNullOrEmpty(error[MessageKeys.Message].ToString()) ? RewriteErrorMeesage(message) : string.Empty;
        }
    }
}