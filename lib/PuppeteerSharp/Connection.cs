using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp
{
    /// <summary>
    /// A connection handles the communication with a Chromium browser
    /// </summary>
    public class Connection : IDisposable
    {
        private readonly ILogger _logger;

        internal Connection(string url, int delay, IConnectionTransport transport, ILoggerFactory loggerFactory = null)
        {
            LoggerFactory = loggerFactory ?? new LoggerFactory();
            Url = url;
            Delay = delay;
            Transport = transport;

            _logger = LoggerFactory.CreateLogger<Connection>();

            Transport.MessageReceived += Transport_MessageReceived;
            Transport.Closed += Transport_Closed;
            _callbacks = new ConcurrentDictionary<int, MessageTask>();
            _sessions = new ConcurrentDictionary<string, CDPSession>();
            _asyncSessions = new AsyncDictionaryHelper<string, CDPSession>(_sessions, "Session {0} not found");
        }

        #region Private Members
        private int _lastId;
        private readonly ConcurrentDictionary<int, MessageTask> _callbacks;
        private readonly ConcurrentDictionary<string, CDPSession> _sessions;
        private readonly AsyncDictionaryHelper<string, CDPSession> _asyncSessions;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the WebSocket URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; }
        /// <summary>
        /// Gets the sleep time when a message is received.
        /// </summary>
        /// <value>The delay.</value>
        public int Delay { get; }
        /// <summary>
        /// Gets the Connection transport.
        /// </summary>
        /// <value>Connection transport.</value>
        public IConnectionTransport Transport { get; }
        /// <summary>
        /// Occurs when the connection is closed.
        /// </summary>
        public event EventHandler Disconnected;
        /// <summary>
        /// Occurs when a message from chromium is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Connection"/> is closed.
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

        internal int GetMessageID() => Interlocked.Increment(ref _lastId);
        internal Task RawSendASync(int id, string method, object args, string sessionId = null)
            => Transport.SendAsync(JsonConvert.SerializeObject(new ConnectionRequest
            {
                Id = id,
                Method = method,
                Params = args,
                SessionId = sessionId
            }, JsonHelper.DefaultJsonSerializerSettings));

        internal async Task<JObject> SendAsync(string method, object args = null, bool waitForCallback = true)
        {
            if (IsClosed)
            {
                throw new TargetClosedException($"Protocol error({method}): Target closed.", CloseReason);
            }

            var id = GetMessageID();
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

            await RawSendASync(id, method, args).ConfigureAwait(false);
            return waitForCallback ? await callback.TaskWrapper.Task.ConfigureAwait(false) : null;
        }

        internal async Task<T> SendAsync<T>(string method, object args = null)
        {
            var response = await SendAsync(method, args).ConfigureAwait(false);
            return response.ToObject<T>(true);
        }

        internal async Task<CDPSession> CreateSessionAsync(TargetInfo targetInfo)
        {
            var sessionId = (await SendAsync<TargetAttachToTargetResponse>("Target.attachToTarget", new TargetAttachToTargetRequest
            {
                TargetId = targetInfo.TargetId,
                Flatten = true
            }).ConfigureAwait(false)).SessionId;
            return await GetSessionAsync(sessionId).ConfigureAwait(false);
        }

        internal bool HasPendingCallbacks() => _callbacks.Count != 0;
        #endregion

        internal void Close(string closeReason)
        {
            if (IsClosed)
            {
                return;
            }
            IsClosed = true;
            CloseReason = closeReason;

            Transport.StopReading();
            Disconnected?.Invoke(this, new EventArgs());

            foreach (var session in _sessions.Values.ToArray())
            {
                session.Close(closeReason);
            }
            _sessions.Clear();

            foreach (var response in _callbacks.Values.ToArray())
            {
                response.TaskWrapper.TrySetException(new TargetClosedException(
                    $"Protocol error({response.Method}): Target closed.",
                    closeReason
                ));
            }
            _callbacks.Clear();
        }

        internal static Connection FromSession(CDPSession session) => session.Connection;
        internal CDPSession GetSession(string sessionId) => _sessions.GetValueOrDefault(sessionId);
        internal Task<CDPSession> GetSessionAsync(string sessionId) => _asyncSessions.GetItemAsync(sessionId);

        #region Private Methods

        private async void Transport_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var response = e.Message;
                ConnectionResponse obj = null;

                if (response.Length > 0 && Delay > 0)
                {
                    await Task.Delay(Delay).ConfigureAwait(false);
                }

                try
                {
                    obj = JsonConvert.DeserializeObject<ConnectionResponse>(response, JsonHelper.DefaultJsonSerializerSettings);
                }
                catch (JsonException exc)
                {
                    _logger.LogError(exc, "Failed to deserialize response", response);
                    return;
                }

                _logger.LogTrace("◀ Receive {Message}", response);
                ProcessIncomingMessage(obj);
            }
            catch (Exception ex)
            {
                var message = $"Connection failed to process {e.Message}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                Close(message);
            }
        }

        private void ProcessIncomingMessage(ConnectionResponse obj)
        {
            var method = obj.Method;
            var param = obj.Params?.ToObject<ConnectionResponseParams>();

            if (method == "Target.attachedToTarget")
            {
                var sessionId = param.SessionId;
                var session = new CDPSession(this, param.TargetInfo.Type, sessionId);
                _asyncSessions.AddItem(sessionId, session);
            }
            else if (method == "Target.detachedFromTarget")
            {
                var sessionId = param.SessionId;
                if (_sessions.TryRemove(sessionId, out var session) && !session.IsClosed)
                {
                    session.Close("Target.detachedFromTarget");
                }
            }

            if (!string.IsNullOrEmpty(obj.SessionId))
            {
                var session = GetSession(obj.SessionId);
                session.OnMessage(obj);
            }
            else if (obj.Id.HasValue)
            {
                //If we get the object we are waiting for we return if
                //if not we add this to the list, sooner or later some one will come for it 
                if (_callbacks.TryRemove(obj.Id.Value, out var callback))
                {
                    if (obj.Error != null)
                    {
                        callback.TaskWrapper.TrySetException(new MessageException(callback, obj.Error));
                    }
                    else
                    {
                        callback.TaskWrapper.TrySetResult(obj.Result);
                    }
                }
            }
            else
            {
                MessageReceived?.Invoke(this, new MessageEventArgs
                {
                    MessageID = method,
                    MessageData = obj.Params
                });
            }
        }

        void Transport_Closed(object sender, TransportClosedEventArgs e) => Close(e.CloseReason);

        #endregion

        #region Static Methods

        /// <summary>
        /// Gets default web socket factory implementation.
        /// </summary>
        public static readonly Func<Uri, IConnectionOptions, CancellationToken, Task<WebSocket>> DefaultWebSocketFactory = async (uri, options, cancellationToken) =>
        {
            var result = new ClientWebSocket();
            result.Options.KeepAliveInterval = TimeSpan.Zero;
            await result.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
            return result;
        };

        internal static async Task<Connection> Create(string url, IConnectionOptions connectionOptions, ILoggerFactory loggerFactory = null)
        {
            var transport = connectionOptions.Transport ?? new WebSocketTransport();
            connectionOptions.WebSocketFactory = connectionOptions.WebSocketFactory ?? DefaultWebSocketFactory;

            await transport.InitializeAsync(url, connectionOptions).ConfigureAwait(false);

            return new Connection(url, connectionOptions.SlowMo, transport, loggerFactory);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Connection"/> object.
        /// It will raise the <see cref="Disconnected"/> event and dispose <see cref="Transport"/>.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Connection"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Connection"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="Connection"/> so the garbage collector can reclaim the memory that the
        /// <see cref="Connection"/> was occupying.</remarks>
        public void Dispose()
        {
            Close("Connection disposed");
            Transport.Dispose();
        }
        #endregion
    }
}