using System;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp;

namespace PuppeteerSharp
{
    /// <summary>
    /// An ICDPConnection is an object able to send and receive messages from the browser.
    /// </summary>
    public interface ICDPConnection
    {
        /// <summary>
        /// Occurs when message received from Chromium.
        /// </summary>
        event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Protocol methods can be called with this method.
        /// </summary>
        /// <param name="method">The method name.</param>
        /// <param name="args">The method args.</param>
        /// <param name="options">The options.</param>
        /// <typeparam name="T">Return type.</typeparam>
        /// <returns>The task.</returns>
        Task<T> SendAsync<T>(string method, object args = null, CommandOptions options = null);
    }
}
