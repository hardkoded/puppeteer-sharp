using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PuppeteerSharp
{
    /// <summary>
    /// The CDPSession instances are used to talk raw Chrome Devtools Protocol:
    ///  * Protocol methods can be called with <see cref="ICDPSession.SendAsync(string, object, bool)"/> method.
    ///  * Protocol events, using the <see cref="ICDPSession.MessageReceived"/> event.
    ///
    /// Documentation on DevTools Protocol can be found here: <see href="https://chromedevtools.github.io/devtools-protocol/"/>.
    ///
    /// <code>
    /// <![CDATA[
    /// var client = await Page.Target.CreateCDPSessionAsync();
    /// await client.SendAsync("Animation.enable");
    /// client.MessageReceived += (sender, e) =>
    /// {
    ///      if (e.MessageID == "Animation.animationCreated")
    ///      {
    ///          Console.WriteLine("Animation created!");
    ///      }
    /// };
    /// JObject response = await client.SendAsync("Animation.getPlaybackRate");
    /// Console.WriteLine("playback rate is " + response.playbackRate);
    /// await client.SendAsync("Animation.setPlaybackRate", new
    /// {
    ///     playbackRate = Convert.ToInt32(response.playbackRate / 2)
    /// });
    /// ]]></code>
    /// </summary>
    public interface ICDPSession
    {
        /// <summary>
        /// Occurs when the connection is closed.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        /// Occurs when message received from Chromium.
        /// </summary>
        event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Connection close reason.
        /// </summary>
        string CloseReason { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ICDPSession"/> is closed.
        /// </summary>
        /// <value><c>true</c> if is closed; otherwise, <c>false</c>.</value>
        bool IsClosed { get; }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        /// <value>The logger factory.</value>
        ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        string Id { get; }

        /// <summary>
        /// Gets the target type.
        /// </summary>
        /// <value>The target type.</value>
        TargetType TargetType { get; }

        /// <summary>
        /// Detaches session from target. Once detached, session won't emit any events and can't be used to send messages.
        /// </summary>
        /// <returns>A Task that when awaited detaches from the session target.</returns>
        /// <exception cref="T:PuppeteerSharp.PuppeteerException">If the <see cref="Connection"/> is closed.</exception>
        Task DetachAsync();

        /// <summary>
        /// Protocol methods can be called with this method.
        /// </summary>
        /// <param name="method">The method name.</param>
        /// <param name="args">The method args.</param>
        /// <param name="waitForCallback">
        /// If <c>true</c> the method will return a task to be completed when the message is confirmed by Chromium.
        /// If <c>false</c> the task will be considered complete after sending the message to Chromium.
        /// </param>
        /// <returns>The task.</returns>
        /// <exception cref="PuppeteerSharp.PuppeteerException">If the <see cref="Connection"/> is closed.</exception>
        Task<JsonObject> SendAsync(string method, object args = null, bool waitForCallback = true);

        /// <summary>
        /// Protocol methods can be called with this method.
        /// </summary>
        /// <param name="method">The method name.</param>
        /// <param name="args">The method args.</param>
        /// <typeparam name="T">Return type.</typeparam>
        /// <returns>The task.</returns>
        Task<T> SendAsync<T>(string method, object args = null);
    }
}
