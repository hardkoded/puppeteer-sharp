using System;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    /// <summary>
    /// Exception thrown when a <see cref="IPage"/> fails to navigate an URL.
    /// </summary>
    [Serializable]
    public class NavigationException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationException"/> class.
        /// </summary>
        public NavigationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public NavigationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="url">Url.</param>
        public NavigationException(string message, string url) : base(message)
        {
            Url = url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public NavigationException(string message, Exception innerException) : base(message, innerException)
            => Url = (innerException as NavigationException)?.Url;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationException"/> class.
        /// </summary>
        /// <param name="info">Info.</param>
        /// <param name="context">Context.</param>
        protected NavigationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc/>
        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(Url))
                {
                    return base.Message;
                }

                return $"{base.Message} at {Url}";
            }
        }

        /// <summary>
        /// Url that caused the exception.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; }
    }
}
