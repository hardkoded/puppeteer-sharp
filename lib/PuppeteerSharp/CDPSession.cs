using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace PuppeteerSharp
{
    /// <summary>
    /// The CDPSession instances are used to talk raw Chrome Devtools Protocol:
    ///  * Protocol methods can be called with <see cref="CDPSession.SendAsync(string, bool, dynamic)"/> method.
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
    /// dynamic response = await client.SendAsync("Animation.getPlaybackRate");
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

            _callbacks = new Dictionary<int, MessageTask>();
            _logger = Connection.LoggerFactory.CreateLogger<CDPSession>();
            _sessions = new Dictionary<string, CDPSession>();
        }

        #region Private Members
        private int _lastId;
        private readonly Dictionary<int, MessageTask> _callbacks;
        private readonly ILogger _logger;
        private readonly Dictionary<string, CDPSession> _sessions;
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
        /// Gets or sets a value indicating whether this <see cref="CDPSession"/> is closed.
        /// </summary>
        /// <value><c>true</c> if is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed { get; internal set; }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        /// <value>The logger factory.</value>
        public ILoggerFactory LoggerFactory { get; }
        #endregion

        #region Public Methods

        internal async Task<T> SendAsync<T>(string method, dynamic args = null)
        {
            var content = await SendAsync(method, true, args).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(content);
        }

        internal Task<dynamic> SendAsync(string method, dynamic args = null)
        {
            return SendAsync(method, false, args ?? new { });
        }

        internal async Task<dynamic> SendAsync(string method, bool rawContent, dynamic args = null)
        {
            if (Connection == null)
            {
                throw new Exception($"Protocol error ({method}): Session closed. Most likely the {TargetType} has been closed.");
            }
            var id = ++_lastId;
            var message = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"id", id},
                {"method", method},
                {"params", args}
            });
            _logger.LogTrace("Send ► {Id} Method {Method} Params {@Params}", id, method, (object)args);

            var callback = new MessageTask
            {
                TaskWrapper = new TaskCompletionSource<dynamic>(),
                Method = method,
                RawContent = rawContent
            };
            _callbacks[id] = callback;

            try
            {
                await Connection.SendAsync("Target.sendMessageToTarget", new Dictionary<string, object>
                {
                    {"sessionId", SessionId},
                    {"message", message}
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_callbacks.ContainsKey(id))
                {
                    _callbacks.Remove(id);
                    callback.TaskWrapper.SetException(new MessageException(ex.Message, ex));
                }
            }

            return await callback.TaskWrapper.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Detaches session from target. Once detached, session won't emit any events and can't be used to send messages.
        /// </summary>
        /// <returns></returns>
        public Task DetachAsync()
            => Connection.SendAsync("Target.detachFromTarget", new { sessionId = SessionId });

        #endregion

        #region Private Methods

        internal void OnMessage(string message)
        {
            dynamic obj = JsonConvert.DeserializeObject(message);
            var objAsJObject = (JObject)obj;

            _logger.LogTrace("◀ Receive {Message}", message);

            var id = (int?)objAsJObject["id"];
            if (id.HasValue && _callbacks.TryGetValue(id.Value, out var callback) && _callbacks.Remove(id.Value))
            {
                if (objAsJObject["error"] != null)
                {
                    callback.TaskWrapper.TrySetException(new MessageException(callback, obj));
                }
                else
                {
                    if (callback.RawContent)
                    {
                        callback.TaskWrapper.TrySetResult(JsonConvert.SerializeObject(obj.result));
                    }
                    else
                    {
                        callback.TaskWrapper.TrySetResult(obj.result);
                    }
                }
            }
            else
            {
                if (obj.method == "Tracing.tracingComplete")
                {
                    TracingComplete?.Invoke(this, new TracingCompleteEventArgs
                    {
                        Stream = objAsJObject["params"].Value<string>("stream")
                    });
                }
                else if (obj.method == "Target.receivedMessageFromTarget")
                {
                    var sessionId = objAsJObject["params"]["sessionId"].ToString();
                    if (_sessions.TryGetValue(sessionId, out var session))
                    {
                        session.OnMessage(objAsJObject["params"]["message"].ToString());
                    }
                }
                else if (obj.method == "Target.detachedFromTarget")
                {
                    var sessionId = objAsJObject["params"]["sessionId"].ToString();
                    if (_sessions.TryGetValue(sessionId, out var session) && _sessions.Remove(sessionId))
                    {
                        session.OnClosed();
                    }
                }

                MessageReceived?.Invoke(this, new MessageEventArgs
                {
                    MessageID = obj.method,
                    MessageData = objAsJObject["params"]
                });
            }
        }

        internal void OnClosed()
        {
            if (IsClosed)
            {
                return;
            }
            IsClosed = true;

            foreach (var session in _sessions.Values.ToArray())
            {
                session.OnClosed();
            }
            _sessions.Clear();

            foreach (var callback in _callbacks.Values.ToArray())
            {
                callback.TaskWrapper.TrySetException(new TargetClosedException(
                    $"Protocol error({callback.Method}): Target closed."
                ));
            }
            _callbacks.Clear();

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
        Task<dynamic> IConnection.SendAsync(string method, dynamic args) => SendAsync(method, args);
        #endregion
    }
}