using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// The CDPSession instances are used to talk raw Chrome Devtools Protocol:
    ///  * Protocol methods can be called with <see cref="CDPSession.SendAsync(string, object, bool)"/> method.
    ///  * Protocol events, using the <see cref="CDPSession.MessageReceived"/> event.
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
    public class CDPSession
    {
        private readonly ConcurrentDictionary<int, MessageTask> _callbacks;

        internal CDPSession(Connection connection, TargetType targetType, string sessionId)
        {
            Connection = connection;
            TargetType = targetType;
            Id = sessionId;

            _callbacks = new ConcurrentDictionary<int, MessageTask>();
        }

        /// <summary>
        /// Occurs when message received from Chromium.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Occurs when the connection is closed.
        /// </summary>
        public event EventHandler Disconnected;

        internal event EventHandler<SessionAttachedEventArgs> SessionAttached;

        /// <summary>
        /// Gets the target type.
        /// </summary>
        /// <value>The target type.</value>
        public TargetType TargetType { get; }

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public string Id { get; }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        internal Connection Connection { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CDPSession"/> is closed.
        /// </summary>
        /// <value><c>true</c> if is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed { get; internal set; }

        /// <summary>
        /// Connection close reason.
        /// </summary>
        public string CloseReason { get; private set; }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        /// <value>The logger factory.</value>
        public ILoggerFactory LoggerFactory => Connection.LoggerFactory;

        internal void Send(string method, object args = null)
            => _ = SendAsync(method, args, false);

        /// <summary>
        /// Protocol methods can be called with this method.
        /// </summary>
        /// <param name="method">The method name</param>
        /// <param name="args">The method args</param>
        /// <typeparam name="T">Return type.</typeparam>
        /// <returns>The task.</returns>
        public async Task<T> SendAsync<T>(string method, object args = null)
        {
            var content = await SendAsync(method, args).ConfigureAwait(false);
            return content.ToObject<T>(true);
        }

        /// <summary>
        /// Protocol methods can be called with this method.
        /// </summary>
        /// <param name="method">The method name</param>
        /// <param name="args">The method args</param>
        /// <param name="waitForCallback">
        /// If <c>true</c> the method will return a task to be completed when the message is confirmed by Chromium.
        /// If <c>false</c> the task will be considered complete after sending the message to Chromium.
        /// </param>
        /// <returns>The task.</returns>
        /// <exception cref="PuppeteerSharp.PuppeteerException">If the <see cref="Connection"/> is closed.</exception>
        public async Task<JObject> SendAsync(string method, object args = null, bool waitForCallback = true)
        {
            if (Connection == null)
            {
                throw new PuppeteerException(
                    $"Protocol error ({method}): Session closed. " +
                    $"Most likely the {TargetType} has been closed." +
                    $"Close reason: {CloseReason}");
            }

            var id = Connection.GetMessageID();
            MessageTask callback = null;
            if (waitForCallback)
            {
                callback = new MessageTask
                {
                    TaskWrapper = new TaskCompletionSource<JObject>(TaskCreationOptions.RunContinuationsAsynchronously),
                    Method = method
                };
                _callbacks[id] = callback;
            }

            try
            {
                await Connection.RawSendASync(id, method, args, Id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (waitForCallback && _callbacks.TryRemove(id, out _))
                {
                    callback.TaskWrapper.TrySetException(new MessageException(ex.Message, ex));
                }
            }

            return waitForCallback ? await callback.TaskWrapper.Task.ConfigureAwait(false) : null;
        }

        /// <summary>
        /// Detaches session from target. Once detached, session won't emit any events and can't be used to send messages.
        /// </summary>
        /// <returns>Task</returns>
        /// <exception cref="T:PuppeteerSharp.PuppeteerException">If the <see cref="Connection"/> is closed.</exception>
        public Task DetachAsync()
        {
            if (Connection == null)
            {
                throw new PuppeteerException($"Session already detached.Most likely the {TargetType} has been closed.");
            }

            return Connection.SendAsync("Target.detachFromTarget", new TargetDetachFromTargetRequest
            {
                SessionId = Id
            });
        }

        internal bool HasPendingCallbacks() => _callbacks.Count != 0;

        internal void OnMessage(ConnectionResponse obj)
        {
            var id = obj.Id;

            if (id.HasValue && _callbacks.TryRemove(id.Value, out var callback))
            {
                Connection.MessageQueue.Enqueue(callback, obj);
            }
            else
            {
                var method = obj.Method;
                MessageReceived?.Invoke(this, new MessageEventArgs
                {
                    MessageID = method,
                    MessageData = obj.Params
                });
            }
        }

        internal void Close(string closeReason)
        {
            if (IsClosed)
            {
                return;
            }

            CloseReason = closeReason;
            IsClosed = true;

            foreach (var callback in _callbacks.Values.ToArray())
            {
                callback.TaskWrapper.TrySetException(new TargetClosedException(
                    $"Protocol error({callback.Method}): Target closed.",
                    closeReason));
            }

            _callbacks.Clear();
            Disconnected?.Invoke(this, EventArgs.Empty);
            Connection = null;
        }

        internal void OnSessionAttached(CDPSession session)
            => SessionAttached?.Invoke(this, new SessionAttachedEventArgs { Session = session });
    }
}
