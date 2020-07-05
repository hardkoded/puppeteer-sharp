using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Chromium process exception thrown by <see cref="Launcher"/>.
    /// </summary>
    public class ProcessException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public ProcessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ProcessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}