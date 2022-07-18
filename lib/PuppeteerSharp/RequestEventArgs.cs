using System;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// Arguments used by <see cref="IDevToolsContext"/> events.
    /// </summary>
    /// <seealso cref="IDevToolsContext.Request"/>
    /// <seealso cref="IDevToolsContext.RequestFailed"/>
    /// <seealso cref="IDevToolsContext.RequestFinished"/>
    public class RequestEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        public Request Request { get; internal set; }
    }
}
