using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Exception thrown when an operation is not supported.
    /// </summary>
    [Serializable]
    public class UnsupportedOperationException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedOperationException"/> class.
        /// </summary>
        public UnsupportedOperationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedOperationException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public UnsupportedOperationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedOperationException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public UnsupportedOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
