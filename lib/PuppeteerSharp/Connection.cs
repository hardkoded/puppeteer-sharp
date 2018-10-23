using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// A connection handles the communication with a Chromium browser
    /// </summary>
    public class Connection : IDisposable, IConnection
    {
        private readonly ILogger _logger;

        internal Connection(string url, int delay, WebSocket ws, ILoggerFactory loggerFactory = null)
        {
            LoggerFactory = loggerFactory ?? new LoggerFactory();
            Url = url;
            Delay = delay;
            WebSocket = ws;

            _logger = LoggerFactory.CreateLogger<Connection>();
            _socketQueue = new TaskQueue();
            _callbacks = new ConcurrentDictionary<int, MessageTask>( );
            _sessions = new ConcurrentDictionary<string, CDPSession>();
            _websocketReaderCancellationSource = new CancellationTokenSource();

            Task.Factory.StartNew(GetResponseAsync);
        }

        #region Private Members
        private int _lastId;
        private readonly ConcurrentDictionary<int, MessageTask> _callbacks;
        private readonly ConcurrentDictionary<string, CDPSession> _sessions;
        private readonly TaskQueue _socketQueue;
        private readonly CancellationTokenSource _websocketReaderCancellationSource;
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
        /// Gets the WebSocket.
        /// </summary>
        /// <value>The web socket.</value>
        public WebSocket WebSocket { get; }
        /// <summary>
        /// Occurs when the connection is closed.
        /// </summary>
        public event EventHandler Closed;
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
        /// Gets the logger factory.
        /// </summary>
        /// <value>The logger factory.</value>
        public ILoggerFactory LoggerFactory { get; }

        #endregion

        #region Public Methods

        internal async Task<JObject> SendAsync(string method, dynamic args = null, bool waitForCallback = true)
        {
            if (IsClosed)
            {
                throw new TargetClosedException($"Protocol error({method}): Target closed.");
            }

            var id = Interlocked.Increment(ref _lastId);
            var message = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                { MessageKeys.Id, id },
                { MessageKeys.Method, method },
                { MessageKeys.Params, args }
            });

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

            var encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            await _socketQueue.Enqueue(() => WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, default)).ConfigureAwait(false);

            return waitForCallback ? await callback.TaskWrapper.Task.ConfigureAwait(false) : null;
        }

        internal async Task<T> SendAsync<T>(string method, dynamic args = null)
        {
            JToken response = await SendAsync(method, args).ConfigureAwait(false);
            return response.ToObject<T>();
        }

        internal async Task<CDPSession> CreateSessionAsync(TargetInfo targetInfo)
        {
            var sessionId = (await SendAsync("Target.attachToTarget", new
            {
                targetId = targetInfo.TargetId
            }).ConfigureAwait(false))[MessageKeys.SessionId].AsString();
            var session = new CDPSession(this, targetInfo.Type, sessionId);
            _sessions.TryAdd(sessionId, session);
            return session;
        }

        internal bool HasPendingCallbacks() => _callbacks.Count != 0;
        #endregion

        private void OnClose()
        {
            if (IsClosed)
            {
                return;
            }
            IsClosed = true;

            _websocketReaderCancellationSource.Cancel();
            Closed?.Invoke(this, new EventArgs());

            foreach (var session in _sessions.Values.ToArray())
            {
                session.OnClosed();
            }
            _sessions.Clear();

            foreach (var response in _callbacks.Values.ToArray())
            {
                response.TaskWrapper.TrySetException(new TargetClosedException(
                    $"Protocol error({response.Method}): Target closed."
                ));
            }
            _callbacks.Clear();
        }

        internal static IConnection FromSession(CDPSession session)
        {
            var connection = session.Connection;
            while (connection is CDPSession)
            {
                connection = connection.Connection;
            }
            return connection;
        }
        #region Private Methods

        /// <summary>
        /// Starts listening the socket
        /// </summary>
        /// <returns>The start.</returns>
        private async Task<object> GetResponseAsync()
        {
            var buffer = new byte[2048];

            //If it's not in the list we wait for it
            while (true)
            {
                if (IsClosed)
                {
                    OnClose();
                    return null;
                }

                var endOfMessage = false;
                var response = new StringBuilder();

                while (!endOfMessage)
                {
                    WebSocketReceiveResult result;
                    try
                    {
                        result = await WebSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            _websocketReaderCancellationSource.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        return null;
                    }
                    catch (Exception)
                    {
                        OnClose();
                        return null;
                    }

                    endOfMessage = result.EndOfMessage;

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        response.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        OnClose();
                        return null;
                    }
                }

                if (response.Length > 0)
                {
                    if (Delay > 0)
                    {
                        await Task.Delay(Delay).ConfigureAwait(false);
                    }

                    ProcessResponse(response.ToString());
                }
            }
        }

        private void ProcessResponse(string response)
        {
            JObject obj = null;

            try
            {
                obj = JObject.Parse(response);
            }
            catch (JsonException exc)
            {
                _logger.LogError(exc, "Failed to deserialize response", response);
                return;
            }

            _logger.LogTrace("◀ Receive {Message}", response);

            var id = obj[MessageKeys.Id]?.Value<int>();

            if (id.HasValue)
            {
                //If we get the object we are waiting for we return if
                //if not we add this to the list, sooner or later some one will come for it 
                if (_callbacks.TryRemove(id.Value, out var callback))
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
            }
            else
            {
                var method = obj[MessageKeys.Method].AsString();
                var param = obj[MessageKeys.Params];

                if (method == "Target.receivedMessageFromTarget")
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
                    if (_sessions.TryRemove(sessionId, out var session) && !session.IsClosed)
                    {
                        session.OnClosed();
                    }
                }
                else
                {
                    MessageReceived?.Invoke(this, new MessageEventArgs
                    {
                        MessageID = method,
                        MessageData = param
                    });
                }
            }
        }
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
            var ws = await (connectionOptions.WebSocketFactory ?? DefaultWebSocketFactory)(
                new Uri(url),
                connectionOptions,
                default).ConfigureAwait(false);
            return new Connection(url, connectionOptions.SlowMo, ws, loggerFactory);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Connection"/> object.
        /// It will raise the <see cref="Closed"/> event and dispose <see cref="WebSocket"/>.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Connection"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Connection"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="Connection"/> so the garbage collector can reclaim the memory that the
        /// <see cref="Connection"/> was occupying.</remarks>
        public void Dispose()
        {
            OnClose();
            WebSocket.Dispose();
        }
        #endregion

        #region IConnection
        ILoggerFactory IConnection.LoggerFactory => LoggerFactory;
        bool IConnection.IsClosed => IsClosed;
        Task<JObject> IConnection.SendAsync(string method, dynamic args, bool waitForCallback)
            => SendAsync(method, args, waitForCallback);
        IConnection IConnection.Connection => null;
        #endregion
    }
}