using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// The CDPSession instances are used to talk raw Chrome Devtools Protocol:
    ///  * Protocol methods can be called with <see cref="CDPSession.SendAsync(string, dynamic, bool)"/> method.
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
    public class CDPSession : IConnection
    {
        internal CDPSession(IConnection connection, TargetType targetType, string sessionId, ILoggerFactory loggerFactory = null)
        {
            LoggerFactory = loggerFactory ?? new LoggerFactory();
            Connection = connection;
            TargetType = targetType;
            SessionId = sessionId;

            _callbacks = new ConcurrentDictionary<int, MessageTask>();
            _logger = Connection.LoggerFactory.CreateLogger<CDPSession>();
            _sessions = new ConcurrentDictionary<string, CDPSession>();
        }

        #region Private Members
        private int _lastId;
        private readonly ConcurrentDictionary<int, MessageTask> _callbacks;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, CDPSession> _sessions;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the target type.
        /// </summary>
        /// <value>The target type.</value>
        public TargetType TargetType { get; }
        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public string SessionId { get; }
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        internal IConnection Connection { get; private set; }
        /// <summary>
        /// Occurs when message received from Chromium.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;
        /// <summary>
        /// Occurs when tracing is completed.
        /// </summary>
        public event EventHandler<TracingCompleteEventArgs> TracingComplete;
        /// <summary>
        /// Occurs when the connection is closed.
        /// </summary>
        public event EventHandler Closed;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CDPSession"/> is closed.
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
        public ILoggerFactory LoggerFactory { get; }
        #endregion

        #region Public Methods

        internal void Send(string method, dynamic args = null)
            => _ = SendAsync(method, args, false);

        /// <summary>
        /// Protocol methods can be called with this method.
        /// </summary>
        /// <param name="method">The method name</param>
        /// <param name="args">The method args</param>
        /// <returns>The task.</returns>
        public async Task<T> SendAsync<T>(string method, dynamic args = null)
        {
            JObject content = await SendAsync(method, args).ConfigureAwait(false);

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
        /// <exception cref="PuppeteerSharp.PuppeteerException"></exception>
        public async Task<JObject> SendAsync(string method, dynamic args = null, bool waitForCallback = true)
        {
            if (Connection == null)
            {
                throw new PuppeteerException(
                    $"Protocol error ({method}): Session closed. " +
                    $"Most likely the {TargetType} has been closed." +
                    $"Close reason: {CloseReason}");
            }
            var id = Interlocked.Increment(ref _lastId);
            var message = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                { MessageKeys.Id, id },
                { MessageKeys.Method, method },
                { MessageKeys.Params, args }
            }, JsonHelper.DefaultJsonSerializerSettings);

            _logger.LogTrace("Send ► {Id} Method {Method} Params {@Params}", id, method, (object)args);

            MessageTask callback = null;
            if (waitForCallback)
            {
                callback = new MessageTask
                {
                    TaskWrapper = new TaskCompletionSource<JObject>(),
                    Method = method
                };
                _callbacks[id] = callback;
            }

            try
            {
                await Connection.SendAsync(
                    "Target.sendMessageToTarget", new Dictionary<string, object>
                    {
                        {"sessionId", SessionId},
                        {"message", message}
                    },
                    waitForCallback).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (waitForCallback && _callbacks.TryRemove(id, out _))
                {
                    callback.TaskWrapper.SetException(new MessageException(ex.Message, ex));
                }
            }

            return waitForCallback ? await callback.TaskWrapper.Task.ConfigureAwait(false) : null;
        }

        /// <summary>
        /// Detaches session from target. Once detached, session won't emit any events and can't be used to send messages.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="T:PuppeteerSharp.PuppeteerException"></exception>
        public Task DetachAsync()
        {
            if (Connection == null)
            {
                throw new PuppeteerException($"Session already detached.Most likely the { TargetType } has been closed.");
            }
            return Connection.SendAsync("Target.detachFromTarget", new { sessionId = SessionId });
        }

        internal bool HasPendingCallbacks() => _callbacks.Count != 0;

        #endregion

        #region Private Methods

        internal void OnMessage(string message)
        {
            _logger.LogTrace("◀ Receive {Message}", message);

            JObject obj = null;

            try
            {
                obj = JObject.Parse(message);
            }
            catch (JsonException exc)
            {
                _logger.LogError(exc, "Failed to deserialize message", message);
                return;
            }

            var id = obj[MessageKeys.Id]?.Value<int>();

            if (id.HasValue && _callbacks.TryRemove(id.Value, out var callback))
            {
                if (obj[MessageKeys.Error] != null)
                {
                    callback.TaskWrapper.TrySetException(new MessageException(callback, obj));
                }
                else
                {
                    callback.TaskWrapper.TrySetResult(obj[MessageKeys.Result].Value<JObject>());
                }
            }
            else
            {
                var method = obj[MessageKeys.Method].AsString();
                var param = obj[MessageKeys.Params];

                if (method == "Tracing.tracingComplete")
                {
                    TracingComplete?.Invoke(this, new TracingCompleteEventArgs
                    {
                        Stream = param[MessageKeys.Stream].AsString()
                    });
                }
                else if (method == "Target.receivedMessageFromTarget")
                {
                    var sessionId = param[MessageKeys.SessionId].AsString();

                    if (_sessions.TryGetValue(sessionId, out var session))
                    {
                        session.OnMessage(param[MessageKeys.Message].AsString());
                    }
                }
                else if (method == "Target.detachedFromTarget")
                {
                    var sessionId = param[MessageKeys.SessionId].AsString();

                    if (_sessions.TryRemove(sessionId, out var session))
                    {
                        session.Close("Target.detachedFromTarget");
                    }
                }

                MessageReceived?.Invoke(this, new MessageEventArgs
                {
                    MessageID = method,
                    MessageData = param
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

            foreach (var session in _sessions.Values.ToArray())
            {
                session.Close(closeReason);
            }
            _sessions.Clear();

            foreach (var callback in _callbacks.Values.ToArray())
            {
                callback.TaskWrapper.TrySetException(new TargetClosedException(
                    $"Protocol error({callback.Method}): Target closed.",
                    closeReason
                ));
            }
            _callbacks.Clear();
            Closed?.Invoke(this, EventArgs.Empty);
            Connection = null;
        }

        internal CDPSession CreateSession(TargetType targetType, string sessionId)
        {
            var session = new CDPSession(this, targetType, sessionId);
            _sessions[sessionId] = session;
            return session;
        }
        #endregion

        #region IConnection
        ILoggerFactory IConnection.LoggerFactory => LoggerFactory;
        bool IConnection.IsClosed => IsClosed;
        Task<JObject> IConnection.SendAsync(string method, dynamic args, bool waitForCallback)
            => SendAsync(method, args, waitForCallback);
        IConnection IConnection.Connection => Connection;
        void IConnection.Close(string closeReason) => Close(closeReason);
        #endregion
    }
}