using System;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// Arguments used by <see cref="DevToolsContext"/> events.
    /// </summary>
    /// <seealso cref="DevToolsContext.Request"/>
    /// <seealso cref="DevToolsContext.RequestFailed"/>
    /// <seealso cref="DevToolsContext.RequestFinished"/>
    public class RequestEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        public Request Request { get; internal set; }
    }
}