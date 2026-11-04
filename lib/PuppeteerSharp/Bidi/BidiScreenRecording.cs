#if !CDP_ONLY

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Bidi.Core;

namespace PuppeteerSharp.Bidi;

internal sealed class BidiScreenRecording : ScreenRecording
{
    private readonly BidiPage _page;
    private string _screencastId;
    private string _path;

    internal BidiScreenRecording(BidiPage page, RecordOptions options, ILogger logger)
        : base(page, options, logger)
    {
        _page = page;
        _page.BidiMainFrame.BrowsingContext.Closed += OnBrowsingContextClosed;
    }

    /// <inheritdoc/>
    public override async Task StopAsync()
    {
        if (Stopped)
        {
            return;
        }

        Stopped = true;
        _page.BidiMainFrame.BrowsingContext.Closed -= OnBrowsingContextClosed;

        try
        {
            if (string.IsNullOrEmpty(_screencastId))
            {
                return;
            }

            StopScreencastCommandResult result = null;

            try
            {
                result = await _page.BidiMainFrame.BrowsingContext.StopScreencastAsync(_screencastId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to stop screencast.");
            }

            if (!string.IsNullOrEmpty(result?.Error))
            {
                Logger.LogError("Failed to stop screencast: {Error}", result.Error);
            }

            var filePath = result?.Path ?? _path;
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
#if NETSTANDARD2_0
                    var buffer = File.ReadAllBytes(filePath);
#else
                    var buffer = await File.ReadAllBytesAsync(filePath).ConfigureAwait(false);
#endif
                    EnqueueChunk(buffer);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to read screencast file.");
                }
            }
        }
        finally
        {
            await CloseDestinationsAsync().ConfigureAwait(false);
        }
    }

    internal override async Task StartAsync()
    {
        var frameRate = Options.FrameRate ?? Options.Fps;
        StartScreencastVideoParameters video = null;

        if (Options.MaxWidth != null || Options.MaxHeight != null || frameRate != null)
        {
            video = new StartScreencastVideoParameters
            {
                Width = Options.MaxWidth,
                Height = Options.MaxHeight,
                FrameRate = frameRate,
            };
        }

        var result = await _page.BidiMainFrame.BrowsingContext.StartScreencastAsync(new StartScreencastCommandParameters(_page.BidiMainFrame.BrowsingContext.Id)
        {
            Audio = Options.Audio,
            Video = video,
        }).ConfigureAwait(false);

        _screencastId = result.Screencast;
        _path = result.Path;
    }

    private void OnBrowsingContextClosed(object sender, Core.ClosedEventArgs e)
    {
        _ = StopAsync();
    }
}

#endif
