using System.Runtime.Serialization;
using PuppeteerSharp.Cdp;

namespace PuppeteerSharp
{
    /// <summary>
    /// Exception thrown by the <see cref="Connection"/> when it detects that the target was closed.
    /// </summary>
    [System.Serializable]
    public class TargetClosedException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetClosedException"/> class.
        /// </summary>
        public TargetClosedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetClosedException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public TargetClosedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetClosedException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public TargetClosedException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetClosedException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="closeReason">Close reason.</param>
        public TargetClosedException(string message, string closeReason) : base($"{message} ({closeReason})")
            => CloseReason = closeReason;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetClosedException"/> class.
        /// </summary>
        /// <param name="info">The serialization collection for custom serializations.</param>
        /// <param name="context">Provides additional caller-provided context.</param>
        protected TargetClosedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Close Reason.
        /// </summary>
        /// <value>The close reason.</value>
        public string CloseReason { get; }
    }
}
