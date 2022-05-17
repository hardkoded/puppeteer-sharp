using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Arguments used by <see cref="IPage"/> events.
    /// </summary>
    /// <seealso cref="IPage.Request"/>
    /// <seealso cref="IPage.RequestFailed"/>
    /// <seealso cref="IPage.RequestFinished"/>
    public class RequestEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>The request.</value>
        public Request Request { get; internal set; }
    }
}
