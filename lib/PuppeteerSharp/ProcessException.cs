using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// process exception thrown by <see cref="Launcher"/>.
    /// </summary>
    [Serializable]
    public class ProcessException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessException"/> class.
        /// </summary>
        public ProcessException()
        {
        }

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
