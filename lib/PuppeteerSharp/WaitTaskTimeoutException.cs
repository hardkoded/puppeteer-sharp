using System;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    /// <summary>
    /// Timeout exception that might be thrown by <c>WaitFor</c> methods in <see cref="Frame"/>.
    /// </summary>
    [Serializable]
    public class WaitTaskTimeoutException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitTaskTimeoutException"/> class.
        /// </summary>
        public WaitTaskTimeoutException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitTaskTimeoutException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public WaitTaskTimeoutException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitTaskTimeoutException"/> class.
        /// </summary>
        /// <param name="timeout">Timeout.</param>
        public WaitTaskTimeoutException(int timeout) : base($"Waiting failed: {timeout}ms exceeded")
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitTaskTimeoutException"/> class.
        /// </summary>
        /// <param name="timeout">Timeout.</param>
        /// <param name="elementType">Element type.</param>
        public WaitTaskTimeoutException(int timeout, string elementType) : base($"waiting for {elementType} failed: timeout {timeout}ms exceeded")
        {
            Timeout = timeout;
            ElementType = elementType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitTaskTimeoutException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public WaitTaskTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitTaskTimeoutException"/> class.
        /// </summary>
        /// <param name="info">Info.</param>
        /// <param name="context">Context.</param>
        protected WaitTaskTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Timeout that caused the exception.
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout { get; }

        /// <summary>
        /// Element type the WaitTask was waiting for.
        /// </summary>
        /// <value>The element.</value>
        public string ElementType { get; }
    }
}
