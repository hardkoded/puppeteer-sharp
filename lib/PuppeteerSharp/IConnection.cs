using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Connection interface implemented by <see cref="Connection"/> and <see cref="CDPSession"/>
    /// </summary>
    internal interface IConnection
    {
        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        /// <value>The logger factory.</value>
        ILoggerFactory LoggerFactory { get; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="T:PuppeteerSharp.IConnection"/> is closed.
        /// </summary>
        /// <value><c>true</c> if is closed; otherwise, <c>false</c>.</value>
        bool IsClosed { get; }
        /// <summary>
        /// Sends a message to chromium.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="method">Method to call.</param>
        /// <param name="args">Method arguments.</param>
        Task<dynamic> SendAsync(string method, dynamic args = null);
    }
}