using System;
using System.Collections.Generic;
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
        private readonly object _messageReceivedLock = new();
        private readonly Queue<MessageEventArgs> _bufferedMessages = new();
        private EventHandler<MessageEventArgs> _messageReceived;
        private bool _earlyMessagesFlushed;

        /// <inheritdoc/>
        public event EventHandler<MessageEventArgs> MessageReceived
        {
            add
            {
                if (value is null)
                {
                    return;
                }

                lock (_messageReceivedLock)
                {
                    _messageReceived += value;
                }
            }

            remove
            {
                lock (_messageReceivedLock)
                {
                    _messageReceived -= value;
                }
            }
        }

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
        public abstract bool Detached { get; }

        /// <inheritdoc/>
        public virtual ILoggerFactory LoggerFactory => Connection.LoggerFactory;

        internal Connection Connection { get; set; }

        internal CdpTarget Target { get; set; }

        internal abstract CDPSession ParentSession { get; }

        /// <summary>
        /// When <c>true</c>, method-events are buffered (instead of dispatched live) until
        /// <see cref="FlushEarlyMessages"/> is called, then replayed to every subscriber.
        /// Enabled for worker sessions: their consumers (the worker and its isolated world)
        /// subscribe only after the session has attached, so without buffering the init events
        /// Chrome replays on attach (Inspector.workerScriptLoaded, Runtime.executionContextCreated)
        /// race the subscriptions and can be dropped. This gives .NET the run-to-completion
        /// ordering that single-threaded environments get for free.
        /// </summary>
        private protected virtual bool BufferEarlyMessages => false;

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

        /// <inheritdoc/>
        public abstract void Close(string closeReason);

        internal void OnSessionReady(CDPSession session) => Ready?.Invoke(this, new SessionEventArgs(session));

        internal void OnSessionAttached(CDPSession session)
            => SessionAttached?.Invoke(this, new SessionEventArgs(session));

        internal void OnSessionDetached(CDPSession session)
            => SessionDetached?.Invoke(this, new SessionEventArgs(session));

        internal void OnSwapped(CDPSession session) => Swapped?.Invoke(this, new SessionEventArgs(session));

        /// <summary>
        /// Replays any buffered method-events to all current subscribers and switches the session to
        /// live dispatch. Called once the consumer has finished wiring up its listeners (so every
        /// listener receives the early events exactly once, in order).
        /// </summary>
        internal void FlushEarlyMessages()
        {
            lock (_messageReceivedLock)
            {
                if (_earlyMessagesFlushed)
                {
                    return;
                }

                _earlyMessagesFlushed = true;

                while (_bufferedMessages.Count > 0)
                {
                    // Dequeue unconditionally before the null-conditional invoke: `a?.M(Dequeue())` would
                    // short-circuit the argument evaluation (and never dequeue) when nobody has subscribed
                    // yet, spinning this loop forever.
                    var message = _bufferedMessages.Dequeue();
                    _messageReceived?.Invoke(this, message);
                }
            }
        }

        /// <summary>
        /// Discards any buffered method-events that were never replayed (e.g. a worker session that
        /// closed before its consumer flushed).
        /// </summary>
        internal void ClearBufferedMessages()
        {
            lock (_messageReceivedLock)
            {
                _bufferedMessages.Clear();
            }
        }

        /// <summary>
        /// Emits <see cref="MessageReceived"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected void OnMessageReceived(MessageEventArgs e)
        {
            if (BufferEarlyMessages)
            {
                lock (_messageReceivedLock)
                {
                    if (!_earlyMessagesFlushed)
                    {
                        _bufferedMessages.Enqueue(e);
                        return;
                    }
                }
            }

            _messageReceived?.Invoke(this, e);
        }

        /// <summary>
        /// Emits <see cref="Disconnected"/> event.
        /// </summary>
        protected void OnDisconnected() => Disconnected?.Invoke(this, EventArgs.Empty);
    }
}
