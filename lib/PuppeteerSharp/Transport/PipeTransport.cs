using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IO;

namespace PuppeteerSharp.Transport
{
    /// <summary>
    /// Transport implementation using pipes for browser communication.
    /// Used with Chrome's <c>--remote-debugging-pipe</c> flag, which communicates
    /// via file descriptors 3 (browser reads) and 4 (browser writes).
    /// Messages are null-terminated (<c>\0</c>) JSON strings.
    /// </summary>
    public class PipeTransport : IConnectionTransport
    {
        private static readonly RecyclableMemoryStreamManager _memoryStreamManager = new();
        private readonly Stream _pipeWrite;
        private readonly Stream _pipeRead;
        private readonly SemaphoreSlim _sendSemaphore = new(1, 1);

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "Disposed in StopReading/Dispose.")]
        private CancellationTokenSource _readCts = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeTransport"/> class.
        /// </summary>
        /// <param name="pipeWrite">The stream to write messages to (browser reads from this).</param>
        /// <param name="pipeRead">The stream to read messages from (browser writes to this).</param>
        public PipeTransport(Stream pipeWrite, Stream pipeRead)
        {
            _pipeWrite = pipeWrite ?? throw new ArgumentNullException(nameof(pipeWrite));
            _pipeRead = pipeRead ?? throw new ArgumentNullException(nameof(pipeRead));
        }

        /// <inheritdoc/>
        public event EventHandler<TransportClosedEventArgs> Closed;

        /// <inheritdoc/>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <inheritdoc/>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Starts the background receive loop.
        /// </summary>
        public void Start()
        {
            Task.Factory.StartNew(
                () => ReceiveLoopAsync(_readCts.Token),
                _readCts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        /// <inheritdoc/>
        public async Task SendAsync(byte[] message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (IsClosed)
            {
                return;
            }

            await _sendSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (IsClosed)
                {
                    return;
                }

                await _pipeWrite.WriteAsync(message, 0, message.Length).ConfigureAwait(false);
                await _pipeWrite.WriteAsync(new byte[] { 0 }, 0, 1).ConfigureAwait(false);
                await _pipeWrite.FlushAsync().ConfigureAwait(false);
            }
            catch (IOException)
            {
                OnClose("Pipe write error");
            }
            catch (ObjectDisposedException)
            {
                OnClose("Pipe disposed");
            }
            finally
            {
                _sendSemaphore.Release();
            }
        }

        /// <inheritdoc/>
        public void StopReading()
        {
            var cts = Interlocked.CompareExchange(ref _readCts, null, _readCts);
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="PipeTransport"/>.
        /// </summary>
        /// <param name="disposing">Whether disposal was initiated by <see cref="Dispose()"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopReading();
                _sendSemaphore?.Dispose();
            }
        }

        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            var messageBuffer = _memoryStreamManager.GetStream();
            try
            {
                var readBuffer = new byte[32 * 1024];

                while (!cancellationToken.IsCancellationRequested)
                {
                    int bytesRead;
                    try
                    {
                        bytesRead = await _pipeRead.ReadAsync(readBuffer, 0, readBuffer.Length, cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (IOException)
                    {
                        OnClose("Pipe read error");
                        return;
                    }
                    catch (ObjectDisposedException)
                    {
                        OnClose("Pipe disposed");
                        return;
                    }

                    if (bytesRead == 0)
                    {
                        OnClose("Pipe closed by remote end");
                        return;
                    }

                    // Scan for null terminators to split messages
                    var startIndex = 0;
                    for (var i = 0; i < bytesRead; i++)
                    {
                        if (readBuffer[i] == 0)
                        {
                            if (i > startIndex)
                            {
                                messageBuffer.Write(readBuffer, startIndex, i - startIndex);
                            }

                            if (messageBuffer.Length > 0)
                            {
                                var messageData = messageBuffer.ToArray();
                                messageBuffer.SetLength(0);
                                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(messageData));
                            }

                            startIndex = i + 1;
                        }
                    }

                    // Buffer remaining partial data
                    if (startIndex < bytesRead)
                    {
                        messageBuffer.Write(readBuffer, startIndex, bytesRead - startIndex);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Normal on shutdown
            }
            finally
            {
                messageBuffer.Dispose();
            }
        }

        private void OnClose(string reason)
        {
            if (!IsClosed)
            {
                IsClosed = true;
                StopReading();
                Closed?.Invoke(this, new TransportClosedEventArgs(reason));
            }
        }
    }
}
