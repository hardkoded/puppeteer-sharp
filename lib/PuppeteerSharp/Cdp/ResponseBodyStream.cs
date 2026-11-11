using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp
{
    /// <summary>
    /// Streams the body of a response that is still loading, using <c>Network.streamResourceContent</c>
    /// for the data received so far and <c>Network.dataReceived</c> events for the rest.
    /// </summary>
    internal sealed class ResponseBodyStream : Stream
    {
        private readonly CDPSession _client;
        private readonly string _requestId;
        private readonly object _lock = new();
        private readonly Queue<byte[]> _chunks = new();
        private byte[] _current;
        private int _currentOffset;

        // Reads wait until the buffered data has been queued, so events received while the command is pending stay behind it.
        private bool _ready;
        private bool _finished;
        private Exception _error;
        private bool _disposed;
        private TaskCompletionSource<bool> _waiter;

        private ResponseBodyStream(CDPSession client, string requestId)
        {
            _client = client;
            _requestId = requestId;
        }

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override long Length => throw new NotSupportedException();

        /// <inheritdoc/>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a stream for the body of the request with the given id.
        /// </summary>
        /// <param name="client">The session that receives the request events.</param>
        /// <param name="requestId">The request id.</param>
        /// <returns>A stream of the body chunks as they arrive.</returns>
        public static async Task<Stream> CreateAsync(CDPSession client, string requestId)
        {
            var stream = new ResponseBodyStream(client, requestId);
            client.MessageReceived += stream.OnMessageReceived;
            client.Disconnected += stream.OnDisconnected;

            NetworkStreamResourceContentResponse response;
            try
            {
                response = await client.SendAsync<NetworkStreamResourceContentResponse>(
                    "Network.streamResourceContent",
                    new NetworkStreamResourceContentRequest { RequestId = requestId }).ConfigureAwait(false);
            }
            catch
            {
                stream.Dispose();
                throw;
            }

            stream.MarkReady(response.BufferedData);
            return stream;
        }

        /// <inheritdoc/>
        public override void Flush()
        {
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
            => ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();

        /// <inheritdoc/>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0 || count < 0 || buffer.Length - offset < count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            while (true)
            {
                TaskCompletionSource<bool> waiter;
                lock (_lock)
                {
                    ThrowIfDisposed();
                    if (_ready)
                    {
                        if (_current == null && _chunks.Count > 0)
                        {
                            _current = _chunks.Dequeue();
                            _currentOffset = 0;
                        }

                        if (_current != null)
                        {
                            var read = Math.Min(count, _current.Length - _currentOffset);
                            Buffer.BlockCopy(_current, _currentOffset, buffer, offset, read);
                            _currentOffset += read;
                            if (_currentOffset == _current.Length)
                            {
                                _current = null;
                            }

                            return read;
                        }

                        if (_error != null)
                        {
                            throw _error;
                        }

                        if (_finished)
                        {
                            return 0;
                        }
                    }

                    waiter = _waiter = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                }

                using (cancellationToken.Register(() => waiter.TrySetCanceled(cancellationToken)))
                {
                    await waiter.Task.ConfigureAwait(false);
                }
            }
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_lock)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    _disposed = true;
                    _chunks.Clear();
                    _current = null;
                    _finished = true;
                    SignalWaiter();
                }

                Unsubscribe();
            }

            base.Dispose(disposing);
        }

        private void MarkReady(string bufferedData)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(bufferedData) && _error == null)
                {
                    // The buffered data precedes anything that arrived while the command was pending.
                    var pending = _chunks.ToArray();
                    _chunks.Clear();
                    _chunks.Enqueue(Convert.FromBase64String(bufferedData));
                    foreach (var chunk in pending)
                    {
                        _chunks.Enqueue(chunk);
                    }
                }

                _ready = true;
                SignalWaiter();
            }
        }

        private void Enqueue(byte[] chunk)
        {
            lock (_lock)
            {
                if (_finished)
                {
                    return;
                }

                _chunks.Enqueue(chunk);
                SignalWaiter();
            }
        }

        private void Complete(Exception error)
        {
            lock (_lock)
            {
                if (_finished)
                {
                    return;
                }

                _finished = true;
                if (error != null)
                {
                    _error = error;
                    _chunks.Clear();
                }

                SignalWaiter();
            }

            Unsubscribe();
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Network.dataReceived":
                    var dataReceived = e.MessageData.ToObject<NetworkDataReceivedEventResponse>();
                    if (dataReceived.RequestId == _requestId && !string.IsNullOrEmpty(dataReceived.Data))
                    {
                        Enqueue(Convert.FromBase64String(dataReceived.Data));
                    }

                    break;
                case "Network.loadingFinished":
                    if (e.MessageData.ToObject<LoadingFinishedEventResponse>().RequestId == _requestId)
                    {
                        Complete(null);
                    }

                    break;
                case "Network.loadingFailed":
                    var loadingFailed = e.MessageData.ToObject<LoadingFailedEventResponse>();
                    if (loadingFailed.RequestId == _requestId)
                    {
                        var errorText = !string.IsNullOrEmpty(loadingFailed.ErrorText) ? loadingFailed.ErrorText : loadingFailed.BlockedReason;
                        Complete(new PuppeteerException($"Loading failed: {errorText}"));
                    }

                    break;
            }
        }

        private void OnDisconnected(object sender, EventArgs e)
            => Complete(new TargetClosedException("Session closed."));

        private void Unsubscribe()
        {
            _client.MessageReceived -= OnMessageReceived;
            _client.Disconnected -= OnDisconnected;
        }

        private void SignalWaiter()
        {
            var waiter = _waiter;
            _waiter = null;
            waiter?.TrySetResult(true);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ResponseBodyStream));
            }
        }
    }
}
