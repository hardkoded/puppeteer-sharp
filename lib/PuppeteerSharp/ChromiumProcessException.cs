using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Chromium process exception thrown by <see cref="Launcher"/>.
    /// </summary>
    public class ChromiumProcessException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromiumProcessException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public ChromiumProcessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromiumProcessException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ChromiumProcessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}