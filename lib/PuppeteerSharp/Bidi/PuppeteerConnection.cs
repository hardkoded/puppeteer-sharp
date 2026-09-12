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
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Transport;
using WebDriverBiDi.Protocol;
using BidiConnection = WebDriverBiDi.Protocol.Connection;

namespace PuppeteerSharp.Bidi;

/// <summary>
/// A connection implementation that bridges PuppeteerSharp's IConnectionTransport to WebDriverBiDi's Connection.
/// This allows using custom transport factories with BiDi protocol.
/// </summary>
internal class PuppeteerConnection : BidiConnection
{
    private readonly IConnectionTransport _transport;
    private bool _isActive;

    /// <summary>
    /// Initializes a new instance of the <see cref="PuppeteerConnection"/> class.
    /// </summary>
    /// <param name="transport">The PuppeteerSharp transport to use.</param>
    public PuppeteerConnection(IConnectionTransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        _transport.MessageReceived += OnTransportMessageReceived;
        _transport.Closed += OnTransportClosed;
    }

    /// <inheritdoc/>
    public override bool IsActive => _isActive;

    /// <inheritdoc/>
    public override ConnectionKind ConnectionKind => ConnectionKind.WebSocket;

    /// <inheritdoc/>
    protected override Task StartConnectionAsync(CancellationToken cancellationToken = default)
    {
        _isActive = true;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task StopConnectionAsync(CancellationToken cancellationToken = default)
    {
        _transport.MessageReceived -= OnTransportMessageReceived;
        _transport.Closed -= OnTransportClosed;
        _transport.StopReading();
        _transport.Dispose();
        _isActive = false;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task SendConnectionDataAsync(ReadOnlyMemory<byte> messageBuffer, CancellationToken cancellationToken = default)
    {
        // IConnectionTransport.SendAsync takes a byte[], so the buffer has to be copied. The base
        // SendDataAsync has already checked that the connection is active, raised the trace log
        // message, and taken the send semaphore that makes this call atomic.
        return _transport.SendAsync(messageBuffer.ToArray());
    }

    /// <inheritdoc/>
    protected override Task ReceiveDataAsync()
    {
        // Data reception is handled by the transport's MessageReceived event
        // rather than a polling loop, so this is a no-op.
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override ValueTask DisposeAsyncCore()
    {
        _transport.MessageReceived -= OnTransportMessageReceived;
        _transport.Closed -= OnTransportClosed;
        _transport.StopReading();
        _transport.Dispose();
        _isActive = false;
        return default;
    }

    private async void OnTransportMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        try
        {
            using var messageBuffer = new MessageBuffer();
            messageBuffer.Append(e.Message);
            await this.NotifyDataReceivedObserverAsync(messageBuffer).ConfigureAwait(false);
        }
        catch
        {
            // Exceptions during message processing are logged by the BiDi driver
            // We swallow them here to prevent crashing the transport receive loop
        }
    }

    private void OnTransportClosed(object sender, TransportClosedEventArgs e)
    {
        _isActive = false;
    }
}

#endif
