using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom.Helpers;
using CefSharp.DevTools.Dom.Helpers.Json;
using CefSharp.DevTools.Dom.Messaging;
using CefSharp.DevTools.Dom.Transport;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// A connection handles the communication with a Chromium browser
    /// </summary>
    public class DevToolsConnection : IDisposable
    {
        private readonly ILogger _logger;

        internal DevToolsConnection(bool enqueueAsyncMessages, IConnectionTransport transport, ILoggerFactory loggerFactory = null)
        {
            LoggerFactory = loggerFactory ?? new LoggerFactory();
            Transport = transport;

            _logger = LoggerFactory.CreateLogger<DevToolsConnection>();

            Transport.MessageReceived += OnTransportMessageReceived;
            Transport.MessageError += OnTransportErrorReceived;
            _callbacks = new ConcurrentDictionary<int, MessageTask>();
            MessageQueue = new AsyncMessageQueue(enqueueAsyncMessages, _logger);
        }

        private readonly ConcurrentDictionary<int, MessageTask> _callbacks;
        private int _lastId;

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
        /// Gets or sets a value indicating whether this <see cref="DevToolsConnection"/> is closed.
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

        internal int GetMessageID() => Interlocked.Increment(ref _lastId);

        internal Task RawSendASync(int id, string method, object args)
        {
            var message = JsonConvert.SerializeObject(
                new ConnectionRequest { Id = id, Method = method, Params = args },
                JsonHelper.DefaultJsonSerializerSettings);
            _logger.LogTrace("Send ► {Message}", message);

            return Transport.SendAsync(message);
        }

        private async Task<JObject> SendAsyncInternal(string method, object args = null)
        {
            if (IsClosed)
            {
                throw new TargetClosedException($"Protocol error({method}): Target closed.", CloseReason);
            }

            var id = GetMessageID();

            var callback = new MessageTask(method);
            _callbacks[id] = callback;

            await RawSendASync(id, method, args).ConfigureAwait(false);

            return await callback.TaskWrapper.Task.ConfigureAwait(false);
        }

        internal async Task<T> SendAsync<T>(string method, object args = null)
        {
            var response = await SendAsyncInternal(method, args).ConfigureAwait(false);
            return response.ToObject<T>(true);
        }

        /// <summary>
        /// Has pending callbacks
        /// </summary>
        /// <returns>returns true if there are pending callbacks, otherwise false</returns>
        public bool HasPendingCallbacks() => _callbacks.Count != 0;

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

            foreach (var response in _callbacks.Values.ToArray())
            {
                response.TaskWrapper.TrySetException(new TargetClosedException(
                    $"Protocol error({response.Method}): Target closed.",
                    closeReason));
            }

            _callbacks.Clear();
            MessageQueue.Dispose();
        }

        private void OnTransportErrorReceived(object sender, MessageErrorEventArgs e)
        {
            var message = $"Connection failed to process {e.Exception.Message}. {e.Exception.Message}. {e.Exception.StackTrace}";
            _logger.LogError(e.Exception, message);
            Close(message);
        }

        private void OnTransportMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var response = e.Message;
                ConnectionResponse obj = null;

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
            if (obj.Id.HasValue)
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
                    MessageID = obj.Method,
                    MessageData = obj.Params
                });
            }
        }

        /// <summary>
        /// Attach to an existing embedded Browser instance
        /// </summary>
        /// <param name="connectionTransport">connection transport</param>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A connection to the browser</returns>
        public static DevToolsConnection Attach(IConnectionTransport connectionTransport, ILoggerFactory loggerFactory = null, CancellationToken cancellationToken = default)
        {
            if (connectionTransport == null)
            {
                throw new ArgumentNullException(nameof(connectionTransport));
            }
            var connection = new DevToolsConnection(false, connectionTransport, loggerFactory);

            return connection;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="DevToolsConnection"/> object.
        /// It will raise the <see cref="Disconnected"/> event and dispose <see cref="Transport"/>.
        /// </summary>
        /// <remarks>Call <see cref="Dispose()"/> when you are finished using the <see cref="DevToolsConnection"/>. The
        /// <see cref="Dispose()"/> method leaves the <see cref="DevToolsConnection"/> in an unusable state.
        /// After calling <see cref="Dispose()"/>, you must release all references to the
        /// <see cref="DevToolsConnection"/> so the garbage collector can reclaim the memory that the
        /// <see cref="DevToolsConnection"/> was occupying.</remarks>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing)
        {
            Close("Connection disposed");
            Transport.MessageReceived -= OnTransportMessageReceived;
            Transport.MessageError -= OnTransportErrorReceived;
            Transport.Dispose();
        }

        internal void Send(string method, object args = null)
            => _ = SendAsync(method, args, false);

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
        public async Task<JObject> SendAsync(string method, object args = null, bool waitForCallback = true)
        {
            var id = GetMessageID();
            MessageTask callback = null;
            if (waitForCallback)
            {
                callback = new MessageTask(method);
                _callbacks[id] = callback;
            }

            try
            {
                await RawSendASync(id, method, args).ConfigureAwait(false);
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
    }
}
