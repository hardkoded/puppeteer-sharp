using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace PuppeteerSharp.Tests
{
    public class ScreenRecordingTests
    {
        [TestCase(null)]
        [TestCase(true)]
        public async Task ShouldPreserveBufferedChunksForLaterEnumeration(bool? bufferForEnumeration)
        {
            await using var recording = new TestScreenRecording(new RecordOptions { BufferForEnumeration = bufferForEnumeration });
            byte[] first = { 1, 2 };
            byte[] second = { 3, 4 };
            recording.Emit(first);
            recording.Emit(second);
            await recording.StopAsync();

            var chunks = new List<byte[]>();
            await foreach (var chunk in recording)
            {
                chunks.Add(chunk);
            }

            Assert.That(chunks, Is.EqualTo(new[] { first, second }));
        }

        [Test]
        public async Task ShouldPipeAndEndDestinationWithoutEnumerationBuffering()
        {
            await using var recording = new TestScreenRecording(new RecordOptions { BufferForEnumeration = false });
            using var stream = new MemoryStream();
            var destination = new TestDestination(stream);
            recording.Pipe(destination);
            recording.Emit(new byte[] { 1, 2 });
            recording.Emit(new byte[] { 3, 4 });
            await recording.StopAsync();

            Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
            Assert.That(destination.Ended, Is.True);
            Assert.Throws<InvalidOperationException>(() => recording.GetAsyncEnumerator());
        }

        [Test]
        public async Task ShouldRejectEnumerationBeforeReceivingChunksWhenBufferingIsDisabled()
        {
            await using var recording = new TestScreenRecording(new RecordOptions { BufferForEnumeration = false });
            Assert.Throws<InvalidOperationException>(() => recording.GetAsyncEnumerator());
        }

        [Test]
        public async Task ShouldPropagateDestinationWriteFailuresWithoutEnumerationBuffering()
        {
            await using var recording = new TestScreenRecording(new RecordOptions { BufferForEnumeration = false });
            using var stream = new MemoryStream();
            stream.Dispose();
            recording.Pipe(new TestDestination(stream));

            Assert.Throws<ObjectDisposedException>(() => recording.Emit(new byte[] { 1 }));
        }

        private sealed class TestScreenRecording : ScreenRecording
        {
            public TestScreenRecording(RecordOptions options)
                : base(null, options, NullLogger.Instance)
            {
            }

            public override Task StopAsync() => CloseDestinationsAsync();

            public void Emit(byte[] chunk) => EnqueueChunk(chunk);

            internal override Task StartAsync() => Task.CompletedTask;
        }

        private sealed class TestDestination : IWritableDestination
        {
            private readonly Stream _stream;

            public TestDestination(Stream stream)
            {
                _stream = stream;
            }

            public bool Ended { get; private set; }

            public bool Write(ReadOnlySpan<byte> chunk)
            {
                _stream.Write(chunk);
                return true;
            }

            public void End() => Ended = true;
        }
    }
}
