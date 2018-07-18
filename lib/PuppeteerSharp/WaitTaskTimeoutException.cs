using System;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    [Serializable]
    internal class WaitTaskTimeoutException : PuppeteerException
    {
        /// <summary>
        /// Timeout that caused the exception
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout { get; }
        /// <summary>
        /// Element type the WaitTask was waiting for
        /// </summary>
        /// <value>The element.</value>
        public string ElementType { get; }

        public WaitTaskTimeoutException()
        {
        }

        public WaitTaskTimeoutException(string message) : base(message)
        {
        }

        public WaitTaskTimeoutException(int timeout, string elementType) :
            base($"waiting for {elementType} failed: timeout {timeout}ms exceeded")
        {
            Timeout = timeout;
            ElementType = elementType;
        }

        public WaitTaskTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WaitTaskTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}