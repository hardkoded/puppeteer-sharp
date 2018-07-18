using System;
using System.IO;
namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="CDPSession.TracingComplete"/> arguments.
    /// </summary>
    public class TracingCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public string Stream { get; internal set; }
    }
}