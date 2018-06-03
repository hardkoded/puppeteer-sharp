using System;
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

namespace PuppeteerSharp
{
    /// <summary>
    /// A connection handles the communication with a Chromium browser
    /// </summary>
    public class Connection : IDisposable
    {
        private readonly ILogger _logger;

        internal Connection(string url, int delay, ClientWebSocket ws, ILoggerFactory loggerFactory = null)
        {
            LoggerFactory = loggerFactory ?? new LoggerFactory();
            Url = url;
            Delay = delay;
            WebSocket = ws;

            _logger = LoggerFactory.CreateLogger<Connection>();
            _socketQueue = new TaskQueue();
            _responses = new Dictionary<int, MessageTask>();
            _sessions = new Dictionary<string, CDPSession>();
            _websocketReaderCancellationSource = new CancellationTokenSource();

            Task task = Task.Factory.StartNew(async () =>
            {
                await GetResponseAsync();
            });
        }

        #region Private Members
        private int _lastId;
        private Dictionary<int, MessageTask> _responses;
        private Dictionary<string, CDPSession> _sessions;
        private TaskQueue _socketQueue;
        private const string CloseMessage = "Browser.close";
        private bool _stopReading;
        private CancellationTokenSource _websocketReaderCancellationSource;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the WebSocket URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; private set; }
        /// <summary>
        /// Gets the sleep time when a message is received.
        /// </summary>
        /// <value>The delay.</value>
        public int Delay { get; private set; }
        /// <summary>
        /// Gets the WebSocket.
        /// </summary>
        /// <value>The web socket.</value>
        public WebSocket WebSocket { get; private set; }
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

        internal ILoggerFactory LoggerFactory { get; }

        #endregion

        #region Public Methods

        internal async Task<dynamic> SendAsync(string method, dynamic args = null)
        {
            var id = ++_lastId;
            var message = JsonConvert.SerializeObject(new Dictionary<string, object>(){
                {"id", id},
                {"method", method},
                {"params", args}
            });

            _logger.LogTrace("Send ► {Id} Method {Method} Params {@Params}", id, method, (object)args);

            _responses[id] = new MessageTask
            {
                TaskWrapper = new TaskCompletionSource<dynamic>(),
                Method = method
            };

            var encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
            await _socketQueue.Enqueue(() => WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, default(CancellationToken)));

            if (method == CloseMessage)
            {
                StopReading();
            }

            return await _responses[id].TaskWrapper.Task;
        }

        internal async Task<CDPSession> CreateSessionAsync(string targetId)
        {
            string sessionId = (await SendAsync("Target.attachToTarget", new { targetId })).sessionId;
            var session = new CDPSession(this, targetId, sessionId);
            _sessions.Add(sessionId, session);
            return session;
        }
        #endregion

        private void OnClose()
        {
            if (!IsClosed)
            {
                _websocketReaderCancellationSource.Cancel();
                Closed?.Invoke(this, new EventArgs());
            }

            foreach (var session in _sessions.Values)
            {
                session.OnClosed();
            }

            foreach (var response in _responses.Values.Where(r => !r.TaskWrapper.Task.IsCompleted))
            {
                response.TaskWrapper.SetException(new TargetClosedException(
                    $"Protocol error({response.Method}): Target closed."
                ));
            }

            _responses.Clear();
            _sessions.Clear();
            IsClosed = true;
        }

        internal void StopReading() => _stopReading = true;

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
                string response = string.Empty;

                while (!endOfMessage)
                {
                    WebSocketReceiveResult result = null;
                    try
                    {
                        result = await WebSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            _websocketReaderCancellationSource.Token);
                    }
                    catch (Exception) when (_stopReading)
                    {
                        return null;
                    }
                    catch (OperationCanceledException)
                    {
                        return null;
                    }
                    catch (Exception)
                    {
                        if (!IsClosed)
                        {
                            OnClose();
                            return null;
                        }
                    }

                    endOfMessage = result.EndOfMessage;

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        response += Encoding.UTF8.GetString(buffer, 0, result.Count);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        OnClose();
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(response))
                {
                    ProcessResponse(response);
                }
            }
        }

        private void ProcessResponse(string response)
        {
            dynamic obj = JsonConvert.DeserializeObject(response);
            var objAsJObject = obj as JObject;

            _logger.LogTrace("◀ Receive {Message}", response);

            if (objAsJObject["id"] != null)
            {
                int id = (int)objAsJObject["id"];

                //If we get the object we are waiting for we return if
                //if not we add this to the list, sooner or later some one will come for it 
                if (!_responses.ContainsKey(id))
                {
                    _responses[id] = new MessageTask { TaskWrapper = new TaskCompletionSource<dynamic>() };
                }

                _responses[id].TaskWrapper.SetResult(obj.result);
            }
            else
            {
                if (obj.method == "Target.receivedMessageFromTarget")
                {
                    var session = _sessions.GetValueOrDefault(objAsJObject["params"]["sessionId"].ToString());
                    if (session != null)
                    {
                        session.OnMessage(objAsJObject["params"]["message"].ToString());
                    }
                }
                else if (obj.method == "Target.detachedFromTarget")
                {
                    var session = _sessions.GetValueOrDefault(objAsJObject["params"]["sessionId"].ToString());
                    if (!(session?.IsClosed ?? true))
                    {
                        session.OnClosed();
                        _sessions.Remove(objAsJObject["params"]["sessionId"].ToString());
                    }
                }
                else
                {
                    MessageReceived?.Invoke(this, new MessageEventArgs
                    {
                        MessageID = obj.method,
                        MessageData = objAsJObject["params"] as dynamic
                    });
                }
            }
        }
        #endregion
        #region Static Methods

        internal static async Task<Connection> Create(string url, int delay = 0, int keepAliveInterval = 60, ILoggerFactory loggerFactory = null)
        {
            var ws = new ClientWebSocket();
            ws.Options.KeepAliveInterval = new TimeSpan(0, 0, keepAliveInterval);
            await ws.ConnectAsync(new Uri(url), default(CancellationToken)).ConfigureAwait(false);
            return new Connection(url, delay, ws, loggerFactory);
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
            OnClose();
            WebSocket.Dispose();
        }

        #endregion
    }
}
