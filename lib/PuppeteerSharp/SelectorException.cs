using System;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    /// <summary>
    /// Exception thrown when an element selector returns null.
    /// </summary>
    /// <seealso cref="PuppeteerHandleExtensions.EvaluateFunctionAsync{T}(System.Threading.Tasks.Task{ElementHandle}, string, object[])"/>
    /// <seealso cref="Frame.SelectAsync(string, string[])"/>
    /// <seealso cref="Page.ClickAsync(string, Input.ClickOptions)"/>
    /// <seealso cref="Page.TapAsync(string)"/>
    /// <seealso cref="Page.HoverAsync(string)"/>
    /// <seealso cref="Page.FocusAsync(string)"/>
    /// <seealso cref="Page.SelectAsync(string, string[])"/>
    [Serializable]
    public class SelectorException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorException"/> class.
        /// </summary>
        public SelectorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public SelectorException(string message) : base(message)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="selector">Selector.</param>
        public SelectorException(string message, string selector) : base(message)
        {
            Selector = selector;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public SelectorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorException"/> class.
        /// </summary>
        /// <param name="info">Serialization Info.</param>
        /// <param name="context">Streaming Context.</param>
        protected SelectorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Gets the selector.
        /// </summary>
        /// <value>The selector.</value>
        public string Selector { get; }
    }
}
