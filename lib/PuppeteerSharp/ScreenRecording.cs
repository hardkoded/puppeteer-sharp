using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a video stream-based screen recording session.
    /// </summary>
    public abstract class ScreenRecording : IAsyncEnumerable<byte[]>, IAsyncDisposable
    {
        private readonly Channel<byte[]> _channel = Channel.CreateUnbounded<byte[]>(
            new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = true,
            });

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenRecording"/> class.
        /// </summary>
        /// <param name="page">The page being recorded.</param>
        /// <param name="options">Recording options.</param>
        /// <param name="logger">Logger instance.</param>
        protected ScreenRecording(Page page, RecordOptions options, ILogger logger)
        {
            Page = page;
            Options = options ?? new RecordOptions();
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the page being recorded.
        /// </summary>
        protected Page Page { get; }

        /// <summary>
        /// Gets the recording options.
        /// </summary>
        protected RecordOptions Options { get; }

        /// <summary>
        /// Gets the logger instance.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the destinations the recording is piped to.
        /// </summary>
        protected HashSet<IWritableDestination> Destinations { get; } = new();

        /// <summary>
        /// Gets a value indicating whether the recording has been stopped.
        /// </summary>
        protected bool Stopped { get; set; }

        /// <summary>
        /// Stops the screen recording.
        /// </summary>
        /// <returns>A task that completes when recording has stopped.</returns>
        public abstract Task StopAsync();

        /// <summary>
        /// Pipes the recorded stream to a destination stream.
        /// </summary>
        /// <param name="destination">The destination stream.</param>
        /// <returns>A task that completes when piping finishes.</returns>
        public Task PipeAsync(Stream destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            return PipeAsync(new StreamWritableDestination(destination));
        }

        /// <summary>
        /// Pipes the recorded stream to a writable destination and returns it for chaining.
        /// </summary>
        /// <param name="destination">Writable target for MP4 chunks.</param>
        /// <returns>The same destination instance.</returns>
        public IWritableDestination Pipe(IWritableDestination destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            Destinations.Add(destination);
            return destination;
        }

        /// <summary>
        /// Registers a writable destination to receive recording chunks asynchronously.
        /// </summary>
        /// <param name="destination">The destination that will receive chunks.</param>
        /// <returns>A task that completes when the destination has been registered.</returns>
        public Task PipeAsync(IWritableDestination destination)
        {
            Pipe(destination);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public IAsyncEnumerator<byte[]> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => _channel.Reader.ReadAllAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await StopAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts the screen recording.
        /// </summary>
        /// <returns>A task that completes when recording has started.</returns>
        internal abstract Task StartAsync();

        /// <summary>
        /// Enqueues a chunk to stream consumers and destinations.
        /// </summary>
        /// <param name="buffer">The chunk to enqueue.</param>
        protected void EnqueueChunk(byte[] buffer)
        {
            _channel.Writer.TryWrite(buffer);

            foreach (var destination in Destinations)
            {
                destination.Write(buffer);
            }
        }

        /// <summary>
        /// Closes all destinations and completes the readable stream.
        /// </summary>
        /// <returns>A task that completes when destinations are closed.</returns>
        protected Task CloseDestinationsAsync()
        {
            _channel.Writer.TryComplete();

            foreach (var destination in Destinations)
            {
                destination.End();
            }

            Destinations.Clear();
            return Task.CompletedTask;
        }

        private sealed class StreamWritableDestination : IWritableDestination
        {
            private readonly Stream _stream;

            internal StreamWritableDestination(Stream stream) => _stream = stream;

            public bool Write(ReadOnlySpan<byte> chunk)
            {
#if NETSTANDARD2_0
                _stream.Write(chunk.ToArray(), 0, chunk.Length);
#else
                _stream.Write(chunk);
#endif
                return true;
            }

            public void End()
            {
                _stream.Flush();
            }
        }
    }
}
