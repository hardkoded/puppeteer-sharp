using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="IPage.Response"/> arguments.
    /// </summary>
    public class ResponseCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>The response.</value>
        public Response Response { get; internal set; }
    }
}
