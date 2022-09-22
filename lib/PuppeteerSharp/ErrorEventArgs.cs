using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="IPage.Error"/> arguments.
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEventArgs"/> class.
        /// </summary>
        /// <param name="error">Error.</param>
        public ErrorEventArgs(string error)
        {
            Error = error;
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error.</value>
        public string Error { get; }
    }
}
