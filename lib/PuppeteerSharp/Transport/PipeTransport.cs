using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Transport
{
    /// <summary>
    /// Transport implementation using anonymous pipes for browser communication.
    /// Used with Chrome's <c>--remote-debugging-pipe</c> flag, which communicates
    /// via file descriptors 3 (browser reads) and 4 (browser writes) on Unix,
    /// or via <c>--remote-debugging-io-pipes</c> on Windows.
    /// </summary>
    public class PipeTransport : IConnectionTransport
    {
        private readonly AnonymousPipeServerStream _pipeWrite;
        private readonly AnonymousPipeServerStream _pipeRead;
        private readonly SemaphoreSlim _sendSemaphore = new(1, 1);

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "Disposed in StopReading/Dispose.")]
        private CancellationTokenSource _readCts = new();

        private bool _clientHandlesDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeTransport"/> class.
        /// Creates two anonymous pipe server streams for bidirectional communication.
        /// </summary>
        public PipeTransport()
        {
            // PipeDirection.Out: we write to this pipe, browser reads from FD 3
            _pipeWrite = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);

            // PipeDirection.In: we read from this pipe, browser writes to FD 4
            _pipeRead = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
        }

        /// <inheritdoc/>
        public event EventHandler<TransportClosedEventArgs> Closed;

        /// <inheritdoc/>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <inheritdoc/>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the client handle string for the pipe the browser reads from (FD 3).
        /// Must be called before <see cref="Start"/>.
        /// </summary>
        public string ReadPipeHandle => _clientHandlesDisposed ? string.Empty : _pipeWrite.GetClientHandleAsString();

        /// <summary>
        /// Gets the client handle string for the pipe the browser writes to (FD 4).
        /// Must be called before <see cref="Start"/>.
        /// </summary>
        public string WritePipeHandle => _clientHandlesDisposed ? string.Empty : _pipeRead.GetClientHandleAsString();

        /// <summary>
        /// Starts the pipe transport after the browser process has been launched.
        /// Disposes local copies of client handles and begins the receive loop.
        /// </summary>
        public void Start()
        {
            if (!_clientHandlesDisposed)
            {
                _pipeWrite.DisposeLocalCopyOfClientHandle();
                _pipeRead.DisposeLocalCopyOfClientHandle();
                _clientHandlesDisposed = true;
            }

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
                _pipeWrite?.Dispose();
                _pipeRead?.Dispose();
                _sendSemaphore?.Dispose();
            }
        }

        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            var messageBuffer = new MemoryStream();
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
