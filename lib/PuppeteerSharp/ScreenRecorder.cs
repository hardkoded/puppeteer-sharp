using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a screen recording session.
    /// </summary>
    public class ScreenRecorder : IAsyncDisposable
    {
        private const int DefaultFps = 30;
        private const int DefaultCrf = 30;
        private const int DefaultColors = 256;

        private readonly Page _page;
        private readonly int _fps;
        private readonly Process _ffmpegProcess;
        private readonly CancellationTokenSource _cts = new();
        private readonly TaskCompletionSource<(byte[] Buffer, double WallTime)> _lastFrameTcs =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        private readonly Task _frameProcessingTask;

        private bool _stopped;

        internal ScreenRecorder(Page page, int width, int height, ScreencastOptions options)
        {
            _page = page;

            var ffmpegPath = options.FfmpegPath ?? "ffmpeg";
            var fps = options.Fps ?? DefaultFps;
            _fps = fps;

            // Validate ffmpeg exists.
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

            var format = options.Format ?? "webm";
            var loop = options.Loop.HasValue ? (int)options.Loop.Value : -1;
            var delay = options.Delay ?? -1;
            var quality = options.Quality ?? DefaultCrf;
            var colors = options.Colors ?? DefaultColors;
            var overwrite = options.Overwrite ?? true;

            var filters = new List<string>
            {
                FormattableString.Invariant($"crop='min({width},iw):min({height},ih):0:0'"),
                FormattableString.Invariant($"pad={width}:{height}:0:0"),
            };

            if (options.Speed.HasValue)
            {
                filters.Add(FormattableString.Invariant($"setpts={1m / options.Speed.Value}*PTS"));
            }

            if (options.Crop != null)
            {
                var crop = options.Crop;
                filters.Add(FormattableString.Invariant($"crop={crop.Width}:{crop.Height}:{crop.X}:{crop.Y}"));
            }

            if (options.Scale.HasValue)
            {
                filters.Add(FormattableString.Invariant($"scale=iw*{options.Scale.Value}:-1:flags=lanczos"));
            }

            var formatArgs = GetFormatArgs(format, fps, loop, delay, quality, colors, filters);

            // Ensure the output directory exists.
            if (options.Path != null)
            {
                var dir = Path.GetDirectoryName(Path.GetFullPath(options.Path));
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            // Build ffmpeg argument list as a single string for compatibility with netstandard2.0.
            var argParts = new List<string>
            {
                "-loglevel", "error",

                // Reduces general buffering.
                "-avioflags", "direct",

                // Reduces initial buffering while analyzing input fps and other stats.
                "-fpsprobesize", "0",
                "-probesize", "32",
                "-analyzeduration", "0",
                "-fflags", "nobuffer",

                // `-framerate` is an input option and must appear before `-i`;
                // otherwise ffmpeg ignores it and the image2pipe demuxer falls back to
                // its default 25fps, stretching the output timeline relative to the
                // frames we feed it at `fps`.
                "-framerate", fps.ToString(CultureInfo.InvariantCulture),

                // Forces input to be read from standard input, and forces png input image format.
                "-f", "image2pipe", "-vcodec", "png", "-i", "pipe:0",

                // No audio.
                "-an",

                // This drastically reduces stalling when cpu is overbooked.
                "-threads", "1",

                // Disable bitrate.
                "-b:v", "0",
            };

            argParts.AddRange(formatArgs);
            argParts.AddRange(new[] { "-vf", string.Join(",", filters) });

            // Overwrite output, or exit immediately if file already exists.
            argParts.Add(overwrite ? "-y" : "-n");

            // Output to stdout.
            argParts.Add("pipe:1");

            var startInfo = new ProcessStartInfo(ffmpegPath, string.Join(" ", argParts.Select(QuoteArgument)))
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            _ffmpegProcess = Process.Start(startInfo)
                ?? throw new PuppeteerException("Failed to start ffmpeg process.");

            // If a path is specified, pipe output to file.
            if (options.Path != null)
            {
                var outputPath = options.Path;
                _ = Task.Run(async () =>
                {
                    using var fileStream = File.OpenWrite(outputPath);
                    await _ffmpegProcess.StandardOutput.BaseStream
                        .CopyToAsync(fileStream)
                        .ConfigureAwait(false);
                });
            }

            _ffmpegProcess.ErrorDataReceived += (_, e) =>
            {
                if (e.Data != null)
                {
                    Debug.WriteLine($"[ffmpeg] {e.Data}");
                }
            };
            _ffmpegProcess.BeginErrorReadLine();

            _frameProcessingTask = ProcessFramesAsync();
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

            // Stop sending more frames.
            await _page.StopScreencastAsync().ConfigureAwait(false);

            // Signal frame processing to stop accepting new frames.
            _cts.Cancel();

            // Wait for frame processing task to finish.
            try
            {
                await _frameProcessingTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected.
            }

            // Pad the end with the last frame to fill any remaining time.
            if (_lastFrameTcs.Task.Status == TaskStatus.RanToCompletion)
            {
                var (buffer, lastWallTime) = await _lastFrameTcs.Task.ConfigureAwait(false);
                if (buffer.Length > 0)
                {
                    var elapsed = (Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency) - lastWallTime;
                    var padFrames = Math.Max(1, (int)Math.Round(_fps * elapsed));
                    for (var i = 0; i < padFrames; i++)
                    {
                        await WriteFrameAsync(buffer).ConfigureAwait(false);
                    }
                }
            }

            // Close stdin to signal ffmpeg we are done.
            _ffmpegProcess.StandardInput.Close();

            // Wait for ffmpeg to finish.
            await Task.Run(() => _ffmpegProcess.WaitForExit()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await StopAsync().ConfigureAwait(false);
            _cts.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Computes how many encoder frames to emit for a captured frame that spans
        /// [previousTimestamp, timestamp], so that the cumulative number of emitted
        /// frames tracks a constant-fps grid anchored at startTimestamp.
        ///
        /// Counting each interval independently with
        /// Math.Round(fps * (timestamp - previousTimestamp)) is wrong when frames
        /// are captured faster than fps: every sub-1/fps interval still rounds up to
        /// a whole frame, so the emitted frame count grows with the capture rate
        /// instead of staying at fps * duration, which stretches playback.
        /// Differencing the rounded cumulative position keeps the total at
        /// Math.Round(fps * (lastTimestamp - startTimestamp)), independent of the
        /// capture rate.
        /// </summary>
        /// <param name="startTimestamp">Timestamp of the first captured frame (seconds).</param>
        /// <param name="previousTimestamp">Timestamp of the preceding captured frame (seconds).</param>
        /// <param name="timestamp">Timestamp of the current captured frame (seconds).</param>
        /// <param name="fps">Target output frames per second.</param>
        /// <returns>Number of encoder frames to emit for this interval.</returns>
        internal static int CountFrames(double startTimestamp, double previousTimestamp, double timestamp, int fps)
        {
            var end = (int)Math.Round((timestamp - startTimestamp) * fps);
            var start = (int)Math.Round((previousTimestamp - startTimestamp) * fps);
            return Math.Max(0, end - start);
        }

        private static List<string> GetFormatArgs(
            string format,
            int fps,
            int loop,
            int delay,
            int quality,
            int colors,
            List<string> filters)
        {
            var libvpxArgs = new List<string>
            {
                "-vcodec", "vp9",
                "-crf", quality.ToString(CultureInfo.InvariantCulture),
                "-deadline", "realtime",
                "-cpu-used", Math.Min(Environment.ProcessorCount / 2, 8).ToString(CultureInfo.InvariantCulture),
            };

            switch (format)
            {
                case "webm":
                    return new List<string>(libvpxArgs) { "-f", "webm" };

                case "gif":
                    var gifFps = fps == DefaultFps ? 20 : fps;
                    var gifLoop = loop == int.MaxValue ? 0 : loop;
                    var gifDelay = delay != -1 ? delay / 10 : delay;

                    filters.Add(FormattableString.Invariant(
                        $"fps={gifFps},split[s0][s1];[s0]palettegen=stats_mode=diff:max_colors={colors}[p];[s1][p]paletteuse=dither=bayer"));

                    return new List<string>
                    {
                        "-loop", gifLoop.ToString(CultureInfo.InvariantCulture),
                        "-final_delay", gifDelay.ToString(CultureInfo.InvariantCulture),
                        "-f", "gif",
                    };

                case "mp4":
                    return new List<string>(libvpxArgs)
                    {
                        "-movflags", "hybrid_fragmented",
                        "-f", "mp4",
                    };

                default:
                    throw new PuppeteerException($"Unsupported format: {format}. Supported formats are: webm, gif, mp4.");
            }
        }

        private static string QuoteArgument(string arg)
        {
            if (arg.IndexOf(' ') >= 0 || arg.IndexOf('"') >= 0)
            {
                return "\"" + arg.Replace("\"", "\\\"") + "\"";
            }

            return arg;
        }

        private async Task ProcessFramesAsync()
        {
            byte[] previousBuffer = null;
            double previousTimestamp = 0;
            double? startTimestamp = null;

            var frameChannel = Channel.CreateUnbounded<(byte[] Buffer, double Timestamp)>(
                new UnboundedChannelOptions { SingleReader = true });

            void OnScreencastFrame(object sender, MessageEventArgs e)
            {
                if (e.MessageID != "Page.screencastFrame")
                {
                    return;
                }

                // Acknowledge the frame immediately so the browser can send the next one.
                var sessionId = e.MessageData.GetProperty("sessionId").GetInt32();
                _ = _page.Client.SendAsync("Page.screencastFrameAck", new { sessionId });

                if (!e.MessageData.TryGetProperty("metadata", out var metadata) ||
                    !metadata.TryGetProperty("timestamp", out var timestampElement))
                {
                    return;
                }

                var timestamp = timestampElement.GetDouble();
                var data = e.MessageData.GetProperty("data").GetString();
                var buffer = Convert.FromBase64String(data);

                frameChannel.Writer.TryWrite((buffer, timestamp));
            }

            _page.Client.MessageReceived += OnScreencastFrame;

            try
            {
                await foreach (var (buffer, timestamp) in frameChannel.Reader.ReadAllAsync(_cts.Token).ConfigureAwait(false))
                {
                    if (previousBuffer != null)
                    {
                        startTimestamp ??= previousTimestamp;
                        var framesToWrite = CountFrames(startTimestamp.Value, previousTimestamp, timestamp, _fps);
                        for (var i = 0; i < framesToWrite; i++)
                        {
                            await WriteFrameAsync(previousBuffer).ConfigureAwait(false);
                        }
                    }

                    previousBuffer = buffer;
                    previousTimestamp = timestamp;
                }
            }
            finally
            {
                _page.Client.MessageReceived -= OnScreencastFrame;
                frameChannel.Writer.Complete();

                // Set the last frame for tail-padding in StopAsync.
                var now = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                _lastFrameTcs.TrySetResult((previousBuffer ?? Array.Empty<byte>(), now));
            }
        }

        private async Task WriteFrameAsync(byte[] buffer)
        {
            try
            {
                await _ffmpegProcess.StandardInput.BaseStream
                    .WriteAsync(buffer, 0, buffer.Length)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ffmpeg] Failed to write frame: {ex.Message}");
            }
        }
    }
}
