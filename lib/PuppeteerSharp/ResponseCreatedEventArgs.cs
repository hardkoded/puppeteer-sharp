using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="IPage.Response"/> arguments.
    /// </summary>
    public class ResponseCreatedEventArgs(IResponse response) : EventArgs
    {
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>The response.</value>
        public IResponse Response { get; } = response;
    }
}
