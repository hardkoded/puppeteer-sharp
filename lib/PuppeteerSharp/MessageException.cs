using System;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    /// <summary>
    /// Exception thrown by <seealso cref="CDPSession.SendAsync(string, dynamic)"/>
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
            var error = obj.SelectToken(Constants.ERROR);
            var message = $"Protocol error ({callback.Method}): {error[Constants.MESSAGE]}";

            if (error[Constants.DATA] != null)
            {
                message += $" {error[Constants.DATA]}";
            }

            return !string.IsNullOrEmpty(error[Constants.MESSAGE].ToString()) ? RewriteErrorMeesage(message) : string.Empty;
        }
    }
}