using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    /// <summary>
    /// The CDPSession instances are used to talk raw Chrome Devtools Protocol:
    ///  * Protocol methods can be called with <see cref="ICDPConnection.SendAsync(string, object, bool)"/> method.
    ///  * Protocol events, using the <see cref="ICDPConnection.MessageReceived"/> event.
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
    public interface ICDPSession : ICDPConnection
    {
        /// <summary>
        /// Occurs when the connection is closed.
        /// </summary>
        event EventHandler Disconnected;

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
    }
}
