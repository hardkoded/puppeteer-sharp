using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    /// A connection handles the communication with a Chromium browser.
    /// </summary>
    public class Connection : IDisposable, ICDPConnection
    {
        private readonly ILogger _logger;
        private readonly TaskQueue _callbackQueue = new();

        private readonly ConcurrentDictionary<int, MessageTask> _callbacks = new();
        private readonly ConcurrentDictionary<string, CDPSession> _sessions = new();
        private readonly AsyncDictionaryHelper<string, CDPSession> _asyncSessions;
        private readonly List<string> _manuallyAttached = new();
        private int _lastId;

        internal Connection(string url, int delay, bool enqueueAsyncMessages, IConnectionTransport transport, ILoggerFactory loggerFactory = null)
        {
            LoggerFactory = loggerFactory ?? new LoggerFactory();
            Url = url;
            Delay = delay;
            Transport = transport;

            _logger = LoggerFactory.CreateLogger<Connection>();

            MessageQueue = new AsyncMessageQueue(enqueueAsyncMessages, _logger);
            _asyncSessions = new AsyncDictionaryHelper<string, CDPSession>(_sessions, "Session {0} not found");

            Transport.MessageReceived += Transport_MessageReceived;
            Transport.Closed += Transport_Closed;
        }

        /// <summary>
        /// Occurs when the connection is closed.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Occurs when a message from chromium is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;

        internal event EventHandler<SessionEventArgs> SessionAttached;

        internal event EventHandler<SessionEventArgs> SessionDetached;

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
        /// Gets a value indicating whether this <see cref="Connection"/> is closed.
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

        internal AsyncMessageQueue MessageQueue { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async Task<JObject> SendAsync(string method, object args = null, bool waitForCallback = true)
        {
            if (IsClosed)
            {
                throw new TargetClosedException($"Protocol error({method}): Target closed.", CloseReason);
            }

            var id = GetMessageID();
            var message = GetMessage(id, method, args);

            MessageTask callback = null;
            if (waitForCallback)
            {
                callback = new MessageTask
                {
                    TaskWrapper = new TaskCompletionSource<JObject>(TaskCreationOptions.RunContinuationsAsynchronously),
                    Method = method,
                    Message = message,
                };
                _callbacks[id] = callback;
            }

            await RawSendAsync(message).ConfigureAwait(false);
            return waitForCallback ? await callback.TaskWrapper.Task.ConfigureAwait(false) : null;
        }

        /// <inheritdoc/>
        public async Task<T> SendAsync<T>(string method, object args = null)
        {
            var response = await SendAsync(method, args).ConfigureAwait(false);
            return response.ToObject<T>(true);
        }

        internal static async Task<Connection> Create(string url, IConnectionOptions connectionOptions, ILoggerFactory loggerFactory = null, CancellationToken cancellationToken = default)
        {
#pragma warning disable 618
            var transport = connectionOptions.Transport;
#pragma warning restore 618
            if (transport == null)
            {
                var transportFactory = connectionOptions.TransportFactory ?? WebSocketTransport.DefaultTransportFactory;
                transport = await transportFactory(new Uri(url), connectionOptions, cancellationToken).ConfigureAwait(false);
            }

            return new Connection(url, connectionOptions.SlowMo, connectionOptions.EnqueueAsyncMessages, transport, loggerFactory);
        }

        internal static Connection FromSession(CDPSession session) => session.Connection;

        internal int GetMessageID() => Interlocked.Increment(ref _lastId);

        internal Task RawSendAsync(string message)
        {
            _logger.LogTrace("Send ► {Message}", message);
            return Transport.SendAsync(message);
        }

        internal string GetMessage(int id, string method, object args, string sessionId = null)
            => JsonConvert.SerializeObject(
                new ConnectionRequest { Id = id, Method = method, Params = args, SessionId = sessionId },
                JsonHelper.DefaultJsonSerializerSettings);

        internal bool IsAutoAttached(string targetId)
            => !_manuallyAttached.Contains(targetId);

        internal async Task<CDPSession> CreateSessionAsync(TargetInfo targetInfo, bool isAutoAttachEmulated)
        {
            if (!isAutoAttachEmulated)
            {
                _manuallyAttached.Add(targetInfo.TargetId);
            }

            var sessionId = (await SendAsync<TargetAttachToTargetResponse>(
                "Target.attachToTarget",
                new TargetAttachToTargetRequest
                {
                    TargetId = targetInfo.TargetId,
                    Flatten = true,
                }).ConfigureAwait(false)).SessionId;
            _manuallyAttached.Remove(targetInfo.TargetId);
            return await GetSessionAsync(sessionId).ConfigureAwait(false);
        }

        internal bool HasPendingCallbacks() => !_callbacks.IsEmpty;

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
                    closeReason));
            }

            _callbacks.Clear();
            MessageQueue.Dispose();
        }

        internal CDPSession GetSession(string sessionId) => _sessions.GetValueOrDefault(sessionId);

        internal Task<CDPSession> GetSessionAsync(string sessionId) => _asyncSessions.GetItemAsync(sessionId);

        /// <summary>
        /// Releases all resource used by the <see cref="Connection"/> object.
        /// It will raise the <see cref="Disconnected"/> event and dispose <see cref="Transport"/>.
        /// </summary>
        /// <remarks>Call <see cref="Dispose()"/> when you are finished using the <see cref="Connection"/>. The
        /// <see cref="Dispose()"/> method leaves the <see cref="Connection"/> in an unusable state.
        /// After calling <see cref="Dispose()"/>, you must release all references to the
        /// <see cref="Connection"/> so the garbage collector can reclaim the memory that the
        /// <see cref="Connection"/> was occupying.</remarks>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing)
        {
            Close("Connection disposed");
            Transport.MessageReceived -= Transport_MessageReceived;
            Transport.Closed -= Transport_Closed;
            Transport.Dispose();
            _callbackQueue.Dispose();
        }

        private async void Transport_MessageReceived(object sender, MessageReceivedEventArgs e)
            => await _callbackQueue.Enqueue(() => ProcessMessage(e)).ConfigureAwait(false);

        private async Task ProcessMessage(MessageReceivedEventArgs e)
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

                SessionAttached?.Invoke(this, new SessionEventArgs { Session = session });

                if (obj.SessionId != null && _sessions.TryGetValue(obj.SessionId, out var parentSession))
                {
                    parentSession.OnSessionAttached(session);
                }
            }
            else if (method == "Target.detachedFromTarget")
            {
                var sessionId = param.SessionId;
                if (_sessions.TryRemove(sessionId, out var session) && !session.IsClosed)
                {
                    session.Close("Target.detachedFromTarget");
                    SessionDetached?.Invoke(this, new SessionEventArgs() { Session = session });
                }
            }

            if (!string.IsNullOrEmpty(obj.SessionId))
            {
                var session = GetSession(obj.SessionId);
                session?.OnMessage(obj);
            }
            else if (obj.Id.HasValue)
            {
                // If we get the object we are waiting for we return if
                // if not we add this to the list, sooner or later some one will come for it
                if (_callbacks.TryRemove(obj.Id.Value, out var callback))
                {
                    MessageQueue.Enqueue(callback, obj);
                }
            }
            else
            {
                MessageReceived?.Invoke(this, new MessageEventArgs
                {
                    MessageID = method,
                    MessageData = obj.Params,
                });
            }
        }

        private void Transport_Closed(object sender, TransportClosedEventArgs e) => Close(e.CloseReason);
    }
}
