using System;
using System.Buffers;
using System.Buffers.Text;
using System.IO;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Helpers
{
    internal static class ProtocolStreamReader
    {
        internal static async Task<string> ReadProtocolStreamStringAsync(CDPSession client, string handle, Stream outputStream)
        {
            var result = new StringBuilder();

            var eof = false;

            while (!eof)
            {
                var response = await client.SendAsync<IOReadResponse>("IO.read", new IOReadRequest
                {
                    Handle = handle,
                }).ConfigureAwait(false);

                eof = response.Eof;

                result.Append(response.Data);

                if (outputStream != null)
                {
#if NET10_0_OR_GREATER
                    await WriteStringToStreamAsync(outputStream, response.Data).ConfigureAwait(false);
#else
                    var data = Encoding.UTF8.GetBytes(response.Data);
                    await outputStream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
#endif
                }
            }

            await client.SendAsync("IO.close", new IOCloseRequest
            {
                Handle = handle,
            }).ConfigureAwait(false);

            return result.ToString();
        }

        internal static async Task ReadProtocolBase64StreamByteAsync(CDPSession client, string handle, Stream outputStream)
        {
            var eof = false;

            while (!eof)
            {
                var response = await client.SendAsync<IOReadResponse>("IO.read", new IOReadRequest
                {
                    Handle = handle,
                }).ConfigureAwait(false);

                eof = response.Eof;
                await DecodeStringInChunksAsync(response.Data, outputStream).ConfigureAwait(false);
            }

            await client.SendAsync("IO.close", new IOCloseRequest
            {
                Handle = handle,
            }).ConfigureAwait(false);
        }

        internal static async Task DecodeStringInChunksAsync(string base64String, Stream outputStream)
        {
            // Chunk size must be a multiple of 4.
            const int charChunkSize = 32 * 1024;

            // Since Base64 is ASCII, 1 char = 1 byte.
            var utf8Buffer = ArrayPool<byte>.Shared.Rent(charChunkSize);
            var decodeBuffer = ArrayPool<byte>.Shared.Rent(charChunkSize); // Over-allocated for safety

#if NET10_0_OR_GREATER
            var decodeBufferMemory = decodeBuffer.AsMemory();
#endif

            try
            {
                var length = base64String.Length;

                for (var i = 0; i < length; i += charChunkSize)
                {
                    var readOnlySpan = base64String.AsSpan();
                    var remaining = readOnlySpan.Length - i;
                    var currentChunkSize = Math.Min(charChunkSize, remaining);

                    var utf8ByteCount = Encoding.UTF8.GetBytes(base64String, i, currentChunkSize, utf8Buffer, 0);
                    ReadOnlySpan<byte> utf8Slice = utf8Buffer.AsSpan().Slice(0, utf8ByteCount);

                    var status = Base64.DecodeFromUtf8(
                        utf8Slice,
                        decodeBuffer,
                        out _,
                        out var written);

                    if (status is OperationStatus.Done or OperationStatus.DestinationTooSmall)
                    {
#if NET10_0_OR_GREATER
                        await outputStream.WriteAsync(decodeBufferMemory.Slice(0, written)).ConfigureAwait(false);
#else
                        await outputStream.WriteAsync(decodeBuffer, 0, written).ConfigureAwait(false);
#endif
                    }
                    else if (status == OperationStatus.NeedMoreData && i + currentChunkSize >= length)
                    {
                        // Handle potential partial final block if string isn't perfectly padded
                        break;
                    }
                    else
                    {
                        throw new FormatException("Invalid Base64 sequence.");
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(utf8Buffer);
                ArrayPool<byte>.Shared.Return(decodeBuffer);
            }
        }

#if NET10_0_OR_GREATER
        private static async Task WriteStringToStreamAsync(Stream destination, string text)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(32 * 1024);

            try
            {
                var totalCharsRead = 0;
                while (totalCharsRead < text.Length)
                {
                    var source = text.AsSpan();

                    var status = Utf8.FromUtf16(
                        source.Slice(totalCharsRead),
                        buffer,
                        out var charsRead,
                        out var bytesWritten);

                    await destination.WriteAsync(buffer.AsMemory(0, bytesWritten)).ConfigureAwait(false);

                    totalCharsRead += charsRead;

                    if (status == OperationStatus.DestinationTooSmall)
                    {
                        // This is expected; it just means we filled our 8KB buffer
                        // and need to loop to do the next chunk.
                        continue;
                    }

                    if (status != OperationStatus.Done)
                    {
                        throw new InvalidOperationException($"Encoding failed: {status}");
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
#endif
    }
}
