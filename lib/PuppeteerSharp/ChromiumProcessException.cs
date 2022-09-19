using System;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    /// <summary>
    /// Chromium process exception thrown by <see cref="Launcher"/>.
    /// </summary>
    [Obsolete("ProcessException will be thrown")]
    [Serializable]
    public class ChromiumProcessException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromiumProcessException"/> class.
        /// </summary>
        public ChromiumProcessException()
        {
        }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromiumProcessException"/> class.
        /// </summary>
        /// <param name="info">The serialization collection for custom serializations.</param>
        /// <param name="context">Provides additional caller-provided context.</param>
        protected ChromiumProcessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
