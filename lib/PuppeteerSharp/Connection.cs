using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;

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

            Task.Factory.StartNew(GetResponseAsync);
        }

        #region Private Members
        private int _lastId;
        private readonly ConcurrentDictionary<int, MessageTask> _responses = new ConcurrentDictionary<int, MessageTask>();
        private readonly ConcurrentDictionary<string, CDPSession> _sessions = new ConcurrentDictionary<string, CDPSession>();
        private readonly TaskQueue _socketQueue = new TaskQueue();
        private readonly CancellationTokenSource _websocketReaderCancellationSource = new CancellationTokenSource();
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

        internal async Task<dynamic> SendAsync(string method, dynamic args = null)
        {
            if (IsClosed)
            {
                throw new TargetClosedException($"Protocol error({method}): Target closed.");
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
                Method = method
            };
            _responses.TryAdd(id, callback);

            try
            {
                var encoded = Encoding.UTF8.GetBytes(message);
                var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
                await _socketQueue.Enqueue(() => WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, default)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_responses.TryRemove(id, out _))
                {
                    callback.TaskWrapper.TrySetException(ex);
                }
            }

            return await callback.TaskWrapper.Task.ConfigureAwait(false);
        }

        internal async Task<T> SendAsync<T>(string method, dynamic args = null)
        {
            JToken response = await SendAsync(method, args).ConfigureAwait(false);
            return response.ToObject<T>();
        }

        internal async Task<CDPSession> CreateSessionAsync(TargetInfo targetInfo)
        {
            string sessionId = (await SendAsync("Target.attachToTarget", new
            {
                targetId = targetInfo.TargetId
            }).ConfigureAwait(false)).sessionId;
            var session = new CDPSession(this, targetInfo.Type, sessionId);
            _sessions.TryAdd(sessionId, session);
            return session;
        }
        #endregion

        private void OnClose(Exception ex)
        {
            if (IsClosed)
            {
                return;
            }
            IsClosed = true;

            _websocketReaderCancellationSource.Cancel();
            Closed?.Invoke(this, new EventArgs());

            foreach (var entry in _sessions)
            {
                if (_sessions.TryRemove(entry.Key, out _))
                {
                    entry.Value.OnClosed();
                }
            }

            foreach (var entry in _responses)
            {
                if (_responses.TryRemove(entry.Key, out _))
                {
                    entry.Value.TaskWrapper.TrySetException(
                        new TargetClosedException($"Protocol error({entry.Value.Method}): Target closed.", ex));
                }
            }
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
            while (!IsClosed)
            {
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
                    catch (Exception ex)
                    {
                        OnClose(ex);
                        return null;
                    }

                    endOfMessage = result.EndOfMessage;

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        response.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        OnClose(null);
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

            return null;
        }

        private void ProcessResponse(string response)
        {
            dynamic obj = JsonConvert.DeserializeObject(response);
            var objAsJObject = (JObject)obj;

            _logger.LogTrace("◀ Receive {Message}", response);

            var id = (int?)objAsJObject["id"];
            if (id.HasValue)
            {
                if (_responses.TryRemove(id.Value, out var callback))
                {
                    callback.TaskWrapper.TrySetResult(obj.result);
                }
            }
            else
            {
                if (obj.method == "Target.receivedMessageFromTarget")
                {
                    if (_sessions.TryGetValue(objAsJObject["params"]["sessionId"].ToString(), out var session))
                    {
                        session.OnMessage(objAsJObject["params"]["message"].ToString());
                    }
                }
                else if (obj.method == "Target.detachedFromTarget")
                {
                    if (_sessions.TryRemove(objAsJObject["params"]["sessionId"].ToString(), out var session) && !session.IsClosed)
                    {
                        session.OnClosed();
                    }
                }
                else
                {
                    MessageReceived?.Invoke(this, new MessageEventArgs
                    {
                        MessageID = obj.method,
                        MessageData = objAsJObject["params"]
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
            // result.Options.KeepAliveInterval = TimeSpan.FromMilliseconds(options.KeepAliveInterval);
            await result.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
            return result;
        };

        internal static async Task<Connection> Create(string url, IConnectionOptions connectionOptions, ILoggerFactory loggerFactory = null)
        {
            var ws = await (connectionOptions.WebSocketFactory ?? DefaultWebSocketFactory)(new Uri(url), connectionOptions, default);
            return new Connection(url, connectionOptions.SlowMo, ws, loggerFactory);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Connection"/> object.
        /// It will raise the <see cref="Closed"/> event and call <see cref="WebSocket.CloseAsync(WebSocketCloseStatus, string, CancellationToken)"/>.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Connection"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Connection"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="Connection"/> so the garbage collector can reclaim the memory that the
        /// <see cref="Connection"/> was occupying.</remarks>
        public void Dispose()
        {
            OnClose(new ObjectDisposedException($"Connection({Url})"));
            WebSocket.Dispose();
        }
        #endregion

        #region IConnection
        ILoggerFactory IConnection.LoggerFactory => LoggerFactory;
        bool IConnection.IsClosed => IsClosed;
        Task<dynamic> IConnection.SendAsync(string method, dynamic args) => SendAsync(method, args);
        #endregion
    }
}