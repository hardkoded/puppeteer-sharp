using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Page error event arguments.
    /// </summary>
    public class PageErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageErrorEventArgs"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public PageErrorEventArgs(string message)
        {
            Message = message;
            Error = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageErrorEventArgs"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="error">Raw error value.</param>
        public PageErrorEventArgs(string message, object error)
        {
            Message = message;
            Error = error;
        }

        /// <summary>
        /// Error Message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets the raw error value. This can be an <see cref="string"/> for standard errors,
        /// or any other type (including <c>null</c>) when the page throws a primitive value.
        /// </summary>
        public object Error { get; }
    }
}
