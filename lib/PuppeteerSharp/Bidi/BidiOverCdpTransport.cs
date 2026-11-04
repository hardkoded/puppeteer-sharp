// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

#if !CDP_ONLY

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Transport;
using CdpConnection = PuppeteerSharp.Cdp.Connection;

namespace PuppeteerSharp.Bidi;

/// <summary>
/// A transport that bridges BiDi protocol over a CDP connection by injecting
/// the chromium-bidi mapper into a hidden Chrome tab.
/// </summary>
/// <remarks>
/// This allows Chrome to be used with WebDriver BiDi protocol. The mapper
/// JavaScript runs inside a dedicated Chrome tab and translates BiDi commands
/// to CDP commands. Communication happens via CDP Runtime.evaluate (to send
/// BiDi commands) and Runtime.bindingCalled (to receive BiDi responses).
/// </remarks>
internal sealed class BidiOverCdpTransport : IConnectionTransport
{
    private static readonly Lazy<string> MapperTabSource = new(LoadMapperTabSource);

    private readonly CdpConnection _cdpConnection;
    private readonly CDPSession _mapperSession;
    private readonly ILogger _logger;
    private bool _isClosed;

    private BidiOverCdpTransport(CdpConnection cdpConnection, CDPSession mapperSession, ILogger logger)
    {
        _cdpConnection = cdpConnection;
        _mapperSession = mapperSession;
        _logger = logger;

        // Listen for BiDi responses from the mapper via Runtime.bindingCalled
        _mapperSession.MessageReceived += OnMapperSessionMessage;
    }

    /// <inheritdoc/>
    public event EventHandler<TransportClosedEventArgs> Closed;

    /// <inheritdoc/>
    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    /// <inheritdoc/>
    public bool IsClosed => _isClosed;

    internal CdpConnection CdpConnection => _cdpConnection;

    /// <inheritdoc/>
    public async Task SendAsync(byte[] message)
    {
        if (_isClosed)
        {
            return;
        }

        // Send BiDi command to the mapper via Runtime.evaluate
        var messageString = Encoding.UTF8.GetString(message);
        try
        {
            await _mapperSession.SendAsync(
                "Runtime.evaluate",
                new { expression = $"onBidiMessage({JsonSerializer.Serialize(messageString)})" }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send BiDi message via CDP");
        }
    }

    /// <inheritdoc/>
    public void StopReading()
    {
        Close();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Close();
        _cdpConnection.Dispose();
    }

    /// <summary>
    /// Creates a new BidiOverCdpTransport by connecting to Chrome via CDP,
    /// creating a mapper tab, and injecting the chromium-bidi mapper JavaScript.
    /// </summary>
    /// <param name="cdpEndpoint">The CDP WebSocket endpoint URL.</param>
    /// <param name="options">Connection options.</param>
    /// <param name="loggerFactory">Logger factory.</param>
    /// <returns>A configured BidiOverCdpTransport ready for BiDi communication.</returns>
    internal static async Task<BidiOverCdpTransport> CreateAsync(
        string cdpEndpoint,
        IConnectionOptions options,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger<BidiOverCdpTransport>();

        // 1. Establish CDP connection to Chrome
        var cdpConnection = await CdpConnection.Create(cdpEndpoint, options, loggerFactory).ConfigureAwait(false);

        try
        {
            // 2. Create a hidden mapper tab
            var createTargetResponse = await cdpConnection.SendAsync<JsonElement>(
                "Target.createTarget",
                new { url = "about:blank", background = true }).ConfigureAwait(false);

            var mapperTargetId = createTargetResponse.GetProperty("targetId").GetString();

            // 3. Attach to the mapper tab
            var attachResponse = await cdpConnection.SendAsync<JsonElement>(
                "Target.attachToTarget",
                new { targetId = mapperTargetId, flatten = true }).ConfigureAwait(false);

            var mapperSessionId = attachResponse.GetProperty("sessionId").GetString();

            // Get the CDPSession for the mapper tab
            var mapperSession = await cdpConnection.GetSessionAsync(mapperSessionId).ConfigureAwait(false);

            // 4. Enable Runtime domain on the mapper session
            await mapperSession.SendAsync("Runtime.enable").ConfigureAwait(false);

            // 5. Expose DevTools protocol to the mapper tab
            await cdpConnection.SendAsync(
                "Target.exposeDevToolsProtocol",
                new { bindingName = "cdp", targetId = mapperTargetId, inheritPermissions = true }).ConfigureAwait(false);

            // 6. Add binding for BiDi responses
            await mapperSession.SendAsync(
                "Runtime.addBinding",
                new { name = "sendBidiResponse" }).ConfigureAwait(false);

            // 7. Add binding for debug messages (optional)
            await mapperSession.SendAsync(
                "Runtime.addBinding",
                new { name = "sendDebugMessage" }).ConfigureAwait(false);

            // 8. Inject the mapper JavaScript
            var mapperSource = MapperTabSource.Value;
            await mapperSession.SendAsync(
                "Runtime.evaluate",
                new { expression = mapperSource }).ConfigureAwait(false);

            // 9. Start the mapper instance
            await mapperSession.SendAsync(
                "Runtime.evaluate",
                new { expression = $"window.runMapperInstance('{mapperTargetId}')", awaitPromise = true }).ConfigureAwait(false);

            logger.LogDebug("BiDi-over-CDP mapper initialized for target {TargetId}", mapperTargetId);

            return new BidiOverCdpTransport(cdpConnection, mapperSession, logger);
        }
        catch
        {
            cdpConnection.Dispose();
            throw;
        }
    }

    private static string LoadMapperTabSource()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("PuppeteerSharp.Bidi.mapperTab.js");
        if (stream == null)
        {
            throw new PuppeteerException("Could not load embedded mapperTab.js resource. BiDi-over-CDP requires the chromium-bidi mapper.");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private void OnMapperSessionMessage(object sender, MessageEventArgs e)
    {
        if (e.MessageID != "Runtime.bindingCalled")
        {
            return;
        }

        try
        {
            var name = e.MessageData.GetProperty("name").GetString();

            if (name == "sendBidiResponse")
            {
                var payload = e.MessageData.GetProperty("payload").GetString();
                var bytes = Encoding.UTF8.GetBytes(payload);
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(bytes));
            }
            else if (name == "sendDebugMessage")
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    var payload = e.MessageData.GetProperty("payload").GetString();
                    _logger.LogTrace("BiDi mapper debug: {Message}", payload);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing mapper binding call");
        }
    }

    private void Close()
    {
        if (_isClosed)
        {
            return;
        }

        _isClosed = true;
        _mapperSession.MessageReceived -= OnMapperSessionMessage;
        Closed?.Invoke(this, new TransportClosedEventArgs("BidiOverCdp transport closed"));
    }
}

#endif
