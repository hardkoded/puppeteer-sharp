using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a writable destination for a <see cref="ScreenRecording"/>.
    /// </summary>
    public interface IWritableDestination
    {
        /// <summary>
        /// Writes a chunk of data to the destination.
        /// </summary>
        /// <param name="chunk">The chunk to write.</param>
        /// <returns><c>true</c> if the chunk was written.</returns>
        bool Write(ReadOnlySpan<byte> chunk);

        /// <summary>
        /// Ends the destination stream.
        /// </summary>
        void End();
    }
}
