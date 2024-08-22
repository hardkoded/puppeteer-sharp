using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Base exception used to identify any exception thrown by PuppeteerSharp.
    /// </summary>
    [Serializable]
    public class PuppeteerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerException"/> class.
        /// </summary>
        public PuppeteerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public PuppeteerException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public PuppeteerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal static string RewriteErrorMeesage(string message)
            => message.Contains("Cannot find context with specified id") || message.Contains("Inspected target navigated or close")
                ? "Execution context was destroyed, most likely because of a navigation."
                : message;
    }
}
