using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Transport
{
    /// <summary>
    /// Default web socket transport.
    /// </summary>
    public class WebSocketTransport : IConnectionTransport
    {
        /// <summary>
        /// Gets the default <see cref="WebSocketFactory"/>. This factory does not support Windows 7.
        /// </summary>
        public static readonly TransportFactory DefaultTransportFactory = CreateDefaultTransport;

        /// <summary>
        /// Gets the default <see cref="TransportFactory"/>.
        /// </summary>
        public static readonly WebSocketFactory DefaultWebSocketFactory = CreateDefaultWebSocket;

        /// <summary>
        /// Gets the default <see cref="TransportTaskScheduler"/>.
        /// </summary>
        public static readonly TransportTaskScheduler DefaultTransportScheduler = ScheduleTransportTask;

        private readonly WebSocket _client;
        private readonly bool _queueRequests;
        private readonly TaskQueue _socketQueue = new();

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "False positive, as it is disposed in StopReading() method.")]
        private CancellationTokenSource _readerCancellationSource = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketTransport"/> class.
        /// </summary>
        /// <param name="client">The web socket.</param>
        /// <param name="scheduler">The scheduler to use for long-running tasks.</param>
        /// <param name="queueRequests">Indicates whether requests should be queued.</param>
        private WebSocketTransport(WebSocket client, TransportTaskScheduler scheduler, bool queueRequests)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            _client = client ?? throw new ArgumentNullException(nameof(client));
            _queueRequests = queueRequests;
            scheduler(GetResponseAsync, _readerCancellationSource.Token);
        }

        /// <summary>
        /// Occurs when the transport is closed.
        /// </summary>
        public event EventHandler<TransportClosedEventArgs> Closed;

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Gets a value indicating whether this <see cref="PuppeteerSharp.Transport.IConnectionTransport"/> is closed.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <inheritdoc />
        public Task SendAsync(string message)
        {
            var encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            Task SendCoreAsync() => _client.SendAsync(buffer, WebSocketMessageType.Text, true, default);

            return _queueRequests ? _socketQueue.Enqueue(SendCoreAsync) : SendCoreAsync();
        }

        /// <summary>
        /// Stops reading incoming data.
        /// </summary>
        public void StopReading()
        {
            var readerCts = Interlocked.CompareExchange(ref _readerCancellationSource, null, _readerCancellationSource);
            if (readerCts != null)
            {
                // Asynchronous read operations may still be in progress, so cancel it first and then dispose
                // the associated CancellationTokenSource.
                readerCts.Cancel();
                readerCts.Dispose();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Close the WebSocketTransport.
        /// </summary>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Make sure any outstanding asynchronous read operation is cancelled.
            StopReading();
            _client?.Dispose();
            _socketQueue.Dispose();
        }

        private static async Task<WebSocket> CreateDefaultWebSocket(Uri url, IConnectionOptions options, CancellationToken cancellationToken)
        {
            var result = new ClientWebSocket();
            result.Options.KeepAliveInterval = TimeSpan.Zero;
            await result.ConnectAsync(url, cancellationToken).ConfigureAwait(false);
            return result;
        }

        private static async Task<IConnectionTransport> CreateDefaultTransport(Uri url, IConnectionOptions connectionOptions, CancellationToken cancellationToken)
        {
            var webSocketFactory = connectionOptions.WebSocketFactory ?? DefaultWebSocketFactory;
            var webSocket = await webSocketFactory(url, connectionOptions, cancellationToken).ConfigureAwait(false);
            return new WebSocketTransport(webSocket, DefaultTransportScheduler, connectionOptions.EnqueueTransportMessages);
        }

        private static void ScheduleTransportTask(Func<CancellationToken, Task> taskFactory, CancellationToken cancellationToken)
            => Task.Factory.StartNew(
                () => taskFactory(cancellationToken),
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

        /// <summary>
        /// Starts listening the socket.
        /// </summary>
        /// <returns>The start.</returns>
        private async Task<object> GetResponseAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[2048];

            while (!IsClosed)
            {
                var endOfMessage = false;
                var response = new StringBuilder();

                while (!endOfMessage)
                {
                    WebSocketReceiveResult result;
                    try
                    {
                        result = await _client.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            cancellationToken).ConfigureAwait(false);
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

            return null;
        }

        private void OnClose(string closeReason)
        {
            if (!IsClosed)
            {
                IsClosed = true;
                StopReading();
                Closed?.Invoke(this, new TransportClosedEventArgs(closeReason));
            }
        }
    }
}
