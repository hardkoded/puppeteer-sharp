using System;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// <see cref="IDevToolsContext.Response"/> arguments.
    /// </summary>
    public class ResponseCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>The response.</value>
        public Response Response { get; internal set; }
    }
}
