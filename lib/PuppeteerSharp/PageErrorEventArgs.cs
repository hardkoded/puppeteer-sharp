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
        public PageErrorEventArgs(string message) => Message = message;

        /// <summary>
        /// Error Message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }
    }
}
