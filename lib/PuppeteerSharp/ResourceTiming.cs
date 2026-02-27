// * MIT License
//  *
//  * Copyright (c) Dario Kondratiuk
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

namespace PuppeteerSharp;

/// <summary>
/// Resource timing information as returned by the DevTools Network domain.
/// </summary>
public class ResourceTiming
{
    /// <summary>
    /// Timing's requestTime is a baseline in seconds, while the other numbers are ticks in milliseconds relatively to this requestTime.
    /// </summary>
    public decimal RequestTime { get; set; }

    /// <summary>
    /// Started resolving proxy.
    /// </summary>
    public decimal ProxyStart { get; set; }

    /// <summary>
    /// Finished resolving proxy.
    /// </summary>
    public decimal ProxyEnd { get; set; }

    /// <summary>
    /// Started DNS address resolve.
    /// </summary>
    public decimal DnsStart { get; set; }

    /// <summary>
    /// Finished DNS address resolve.
    /// </summary>
    public decimal DnsEnd { get; set; }

    /// <summary>
    /// Started connecting to the remote host.
    /// </summary>
    public decimal ConnectStart { get; set; }

    /// <summary>
    /// Connected to the remote host.
    /// </summary>
    public decimal ConnectEnd { get; set; }

    /// <summary>
    /// Started SSL handshake.
    /// </summary>
    public decimal SslStart { get; set; }

    /// <summary>
    /// Finished SSL handshake.
    /// </summary>
    public decimal SslEnd { get; set; }

    /// <summary>
    /// Started running ServiceWorker.
    /// </summary>
    public decimal WorkerStart { get; set; }

    /// <summary>
    /// Finished Starting ServiceWorker.
    /// </summary>
    public decimal WorkerReady { get; set; }

    /// <summary>
    /// Started fetch event.
    /// </summary>
    public decimal WorkerFetchStart { get; set; }

    /// <summary>
    /// Settled fetch event respondWith promise.
    /// </summary>
    public decimal WorkerRespondWithSettled { get; set; }

    /// <summary>
    /// Started sending request.
    /// </summary>
    public decimal SendStart { get; set; }

    /// <summary>
    /// Finished sending request.
    /// </summary>
    public decimal SendEnd { get; set; }

    /// <summary>
    /// Time the server started pushing request.
    /// </summary>
    public decimal PushStart { get; set; }

    /// <summary>
    /// Time the server finished pushing request.
    /// </summary>
    public decimal PushEnd { get; set; }

    /// <summary>
    /// Started receiving response headers.
    /// </summary>
    public decimal ReceiveHeadersStart { get; set; }

    /// <summary>
    /// Finished receiving response headers.
    /// </summary>
    public decimal ReceiveHeadersEnd { get; set; }
}
