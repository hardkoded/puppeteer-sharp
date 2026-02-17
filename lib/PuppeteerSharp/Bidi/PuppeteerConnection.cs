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
    private TaskCompletionSource<bool> _startCompletionSource;

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
    public override async Task StartAsync(string url)
    {
        // The transport is already connected (it was created by the TransportFactory)
        // We just need to mark ourselves as active and set the connected URL
        ConnectedUrl = url;
        _startCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task StopAsync()
    {
        _transport.MessageReceived -= OnTransportMessageReceived;
        _transport.Closed -= OnTransportClosed;
        _transport.StopReading();
        _transport.Dispose();
        ConnectedUrl = string.Empty;
        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task SendDataAsync(byte[] data)
    {
        await _transport.SendAsync(data).ConfigureAwait(false);
    }

    private async void OnTransportMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        try
        {
            await OnDataReceived.NotifyObserversAsync(new ConnectionDataReceivedEventArgs(e.Message)).ConfigureAwait(false);
        }
        catch
        {
            // Exceptions during message processing are logged by the BiDi driver
            // We swallow them here to prevent crashing the transport receive loop
        }
    }

    private void OnTransportClosed(object sender, TransportClosedEventArgs e)
    {
        _startCompletionSource?.TrySetResult(false);
    }
}

#endif
