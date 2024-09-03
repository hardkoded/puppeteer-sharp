using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public abstract class CDPSession : ICDPSession
    {
        /// <inheritdoc/>
        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <inheritdoc/>
        public event EventHandler Disconnected;

        /// <inheritdoc/>
        public event EventHandler<SessionEventArgs> SessionAttached;

        /// <inheritdoc/>
        public event EventHandler<SessionEventArgs> SessionDetached;

        internal event EventHandler<SessionEventArgs> Ready;

        internal event EventHandler<SessionEventArgs> Swapped;

        /// <inheritdoc/>
        public string Id { get; init; }

        /// <inheritdoc/>
        public string CloseReason { get; protected set; }

        /// <inheritdoc/>
        public ILoggerFactory LoggerFactory => Connection.LoggerFactory;

        internal Connection Connection { get; set; }

        internal Target Target { get; set; }

        internal abstract CDPSession ParentSession { get; }

        /// <inheritdoc/>
        public async Task<T> SendAsync<T>(string method, object args = null, CommandOptions options = null)
        {
            var content = await SendAsync(method, args, true, options).ConfigureAwait(false);
            Debug.Assert(content != null, nameof(content) + " != null");
            return content.Value.ToObject<T>();
        }

        /// <inheritdoc/>
        public abstract Task<JsonElement?> SendAsync(string method, object args = null, bool waitForCallback = true, CommandOptions options = null);

        /// <inheritdoc/>
        public abstract Task DetachAsync();

        internal void OnSessionReady(CDPSession session) => Ready?.Invoke(this, new SessionEventArgs(session));

        internal abstract void Close(string closeReason);

        internal void OnSessionAttached(CDPSession session)
            => SessionAttached?.Invoke(this, new SessionEventArgs(session));

        internal void OnSessionDetached(CDPSession session)
            => SessionDetached?.Invoke(this, new SessionEventArgs(session));

        internal void OnSwapped(CDPSession session) => Swapped?.Invoke(this, new SessionEventArgs(session));

        /// <summary>
        /// Emits <see cref="MessageReceived"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected void OnMessageReceived(MessageEventArgs e) => MessageReceived?.Invoke(this, e);

        /// <summary>
        /// Emits <see cref="Disconnected"/> event.
        /// </summary>
        protected void OnDisconnected() => Disconnected?.Invoke(this, EventArgs.Empty);
    }
}
