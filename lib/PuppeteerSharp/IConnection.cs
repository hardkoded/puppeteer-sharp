using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

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
        /// Connection close reason.
        /// </summary>
        string CloseReason { get; }
        /// <summary>
        /// Sends a message to chromium.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="method">Method to call.</param>
        /// <param name="args">Method arguments.</param>
        /// <param name="waitForCallback">
        /// If <c>true</c> the method will return a task to be completed when the message is confirmed by Chromium.
        /// If <c>false</c> the task will be considered complete after sending the message to Chromium.
        /// </param>
        Task<JObject> SendAsync(string method, dynamic args = null, bool waitForCallback = true);
        /// <summary>
        /// Gets the parent connection
        /// </summary>
        IConnection Connection { get; }
        /// <summary>
        /// Occurs when the connection is closed.
        /// </summary>
        event EventHandler Closed;
        /// <summary>
        /// Close the connection.
        /// </summary>
        /// <param name="closeReason">Close reason.</param>
        void Close(string closeReason);
    }
}