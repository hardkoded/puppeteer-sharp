using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a screen recording session.
    /// </summary>
    public class ScreenRecorder : IAsyncDisposable
    {
        private readonly Page _page;
        private bool _stopped;

        internal ScreenRecorder(Page page, ScreencastOptions options)
        {
            _page = page;

            var ffmpegPath = options.FfmpegPath ?? "ffmpeg";

            // Test if ffmpeg exists.
            try
            {
                var processInfo = new ProcessStartInfo(ffmpegPath)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };
                using var process = Process.Start(processInfo);
                process?.Kill();
            }
            catch (Exception ex)
            {
                throw new PuppeteerException($"Failed to launch ffmpeg: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Stops the recorder.
        /// </summary>
        /// <returns>A <see cref="Task"/> that completes when the recorder has stopped.</returns>
        public async Task StopAsync()
        {
            if (_stopped)
            {
                return;
            }

            _stopped = true;
            await _page.StopScreencastAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await StopAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
    }
}
