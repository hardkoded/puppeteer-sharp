using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Transport
{
    /// <summary>
    /// Default web socket transport.
    /// </summary>
    public class WebSocketTransport : IConnectionTransport
    {
        private WebSocket _client;
        private bool _queueRequests;
        private readonly TaskQueue _socketQueue;
        private readonly bool _startReading;

        private CancellationTokenSource _readerCancellationSource { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PuppeteerSharp.Transport.IConnectionTransport"/> is closed.
        /// </summary>
        public bool IsClosed { get; private set; }
        /// <summary>
        /// Occurs when the transport is closed.
        /// </summary>
        public event EventHandler<TransportClosedEventArgs> Closed;
        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerSharp.Transport.WebSocketTransport"/> class.
        /// </summary>
        /// <param name="startReading">If set to <c>true</c> the Transport will start reading as soon as it's initialized.</param>
        public WebSocketTransport(bool startReading = true)
        {
            _startReading = startReading;
            _socketQueue = new TaskQueue();
            _readerCancellationSource = new CancellationTokenSource();

        }

        /// <summary>
        /// Initialize the Transport
        /// </summary>
        /// <param name="url">Chromium URL</param>
        /// <param name="connectionOptions">Connection options</param>
        /// <param name="loggerFactory">Logger factory</param>
        public virtual async Task InitializeAsync(string url, IConnectionOptions connectionOptions, ILoggerFactory loggerFactory = null)
        {
            _client = await connectionOptions.WebSocketFactory(
                new Uri(url),
                connectionOptions,
                default).ConfigureAwait(false);

            _queueRequests = connectionOptions.EnqueueTransportMessages;

            if (_startReading)
            {
                StartReading();
            }
        }

        private void StartReading() => Task.Factory.StartNew(GetResponseAsync, TaskCreationOptions.LongRunning);

        /// <summary>
        /// Sends a message using the transport.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="message">Message to send.</param>
        public Task SendAsync(string message)
        {
            var encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            Task sendTask() => _client.SendAsync(buffer, WebSocketMessageType.Text, true, default);

            return _queueRequests ? _socketQueue.Enqueue(sendTask) : sendTask();
        }

        /// <summary>
        /// Stops reading incoming data.
        /// </summary>
        public void StopReading() => _readerCancellationSource.Cancel();

        /// <summary>
        /// Starts listening the socket
        /// </summary>
        /// <returns>The start.</returns>
        protected async Task<object> GetResponseAsync()
        {
            var buffer = new byte[2048];

            //If it's not in the list we wait for it
            while (true)
            {
                if (IsClosed)
                {
                    OnClose("WebSocket is closed");
                    return null;
                }

                var endOfMessage = false;
                var response = new StringBuilder();

                while (!endOfMessage)
                {
                    WebSocketReceiveResult result;
                    try
                    {
                        result = await _client.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            _readerCancellationSource.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        return null;
                    }
                    catch (Exception ex)
                    {
                        OnClose(ex.Message);
                        return null;
                    }

                    endOfMessage = result.EndOfMessage;

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        response.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        OnClose("WebSocket closed");
                        return null;
                    }
                }

                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(response.ToString()));
            }
        }

        private void OnClose(string closeReason)
        {
            Closed?.Invoke(this, new TransportClosedEventArgs(closeReason));
            IsClosed = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _client?.Dispose();
            _readerCancellationSource?.Dispose();
        }
    }
}
