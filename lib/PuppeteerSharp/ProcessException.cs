using System;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    /// <summary>
    /// process exception thrown by <see cref="Launcher"/>.
    /// </summary>
#pragma warning disable 612, 618
    [Serializable]
    public class ProcessException : ChromiumProcessException
#pragma warning restore 612, 618
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessException"/> class.
        /// </summary>
        /// <param name="info">The serialization collection for custom serializations.</param>
        /// <param name="context">Provides additional caller-provided context.</param>
        protected ProcessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
