using System;

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
    }
}