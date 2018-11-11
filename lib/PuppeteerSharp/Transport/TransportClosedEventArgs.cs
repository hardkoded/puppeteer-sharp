using System;
namespace PuppeteerSharp.Transport
{
    /// <summary>
    /// <see cref="IConnectionTransport.Closed"/>
    /// </summary>
    public class TransportClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the close reason.
        /// </summary>
        public string CloseReason { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerSharp.Transport.TransportClosedEventArgs"/> class.
        /// </summary>
        /// <param name="closeReason">Close reason.</param>
        public TransportClosedEventArgs(string closeReason) => CloseReason = closeReason;
    }
}
