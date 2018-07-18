using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Arguments used by <see cref="Page"/> events.
    /// </summary>
    /// <seealso cref="Page.Request"/>
    /// <seealso cref="Page.RequestFailed"/>
    /// <seealso cref="Page.RequestFinished"/>
    public class RequestEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        public Request Request { get; internal set; }
    }
}