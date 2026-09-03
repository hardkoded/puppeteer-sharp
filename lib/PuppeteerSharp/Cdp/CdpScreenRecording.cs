using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp
{
    internal sealed class CdpScreenRecording : ScreenRecording
    {
        private readonly CdpPage _page;
        private string _streamHandle;

        internal CdpScreenRecording(CdpPage page, RecordOptions options, ILogger logger)
            : base(page, options, logger)
        {
            _page = page;
            _page.Client.Disconnected += OnClientDisconnected;
        }

        /// <inheritdoc/>
        public override async Task StopAsync()
        {
            if (Stopped)
            {
                return;
            }

            Stopped = true;
            var client = _page.Client;
            client.Disconnected -= OnClientDisconnected;

            try
            {
                try
                {
                    await client.SendAsync("Page.stopScreenRecording").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to stop screen recording.");
                }

                if (string.IsNullOrEmpty(_streamHandle))
                {
                    throw new PuppeteerException("Screen recording stream handle is missing.");
                }

                await ProtocolStreamReader.ReadProtocolStreamBytesAsync(
                    client,
                    _streamHandle,
                    EnqueueChunk).ConfigureAwait(false);
            }
            finally
            {
                await CloseDestinationsAsync().ConfigureAwait(false);
            }
        }

        internal override async Task StartAsync()
        {
            var frameRate = Options.FrameRate ?? Options.Fps;
            var response = await _page.Client.SendAsync<PageStartScreenRecordingResponse>(
                "Page.startScreenRecording",
                new PageStartScreenRecordingRequest
                {
                    Audio = Options.Audio,
                    MaxWidth = Options.MaxWidth,
                    MaxHeight = Options.MaxHeight,
                    FrameRate = frameRate,
                }).ConfigureAwait(false);

            _streamHandle = response.Stream;
        }

        private void OnClientDisconnected(object sender, EventArgs e)
        {
            _ = StopAsync();
        }
    }
}
