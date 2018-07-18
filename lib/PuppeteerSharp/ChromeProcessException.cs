using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Chrome process exception thrown by <see cref="Launcher"/>.
    /// </summary>
    public class ChromeProcessException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeProcessException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public ChromeProcessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeProcessException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ChromeProcessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}