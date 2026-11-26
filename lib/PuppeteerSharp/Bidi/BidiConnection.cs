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
using WebDriverBiDi;
using WebDriverBiDi.Protocol;
using BidiTransport = WebDriverBiDi.Protocol.Transport;

namespace PuppeteerSharp.Bidi;

/// <summary>
/// A <see cref="BiDiDriver"/> which notifies when its connection is closed.
/// </summary>
internal sealed class BidiConnection : BiDiDriver
{
    private readonly ClosableTransport _transport;

    private BidiConnection(TimeSpan commandTimeout, ClosableTransport transport)
        : base(commandTimeout, transport) => _transport = transport;

    /// <summary>
    /// Occurs once, when the connection is closed by the remote end or the driver is stopped.
    /// It is raised after all the messages received before closing have been dispatched.
    /// </summary>
    public event EventHandler Closed
    {
        add => _transport.Closed += value;
        remove => _transport.Closed -= value;
    }

    /// <summary>
    /// Gets a value indicating whether the connection is closed.
    /// </summary>
    public bool IsClosed => _transport.IsClosed;

    /// <summary>
    /// Creates a new <see cref="BidiConnection"/>.
    /// </summary>
    /// <param name="commandTimeout">Default timeout for commands.</param>
    /// <param name="connection">Connection to the remote end. The returned driver takes ownership of it.</param>
    /// <returns>A driver which has not been started yet.</returns>
    public static BidiConnection Create(TimeSpan commandTimeout, Connection connection) =>
#pragma warning disable CA2000 // Ownership is transferred to the driver
        new(commandTimeout, new ClosableTransport(connection));
#pragma warning restore CA2000

    private sealed class ClosableTransport(Connection connection) : BidiTransport(connection)
    {
        private volatile bool _isClosed;

        public event EventHandler Closed;

        public bool IsClosed => _isClosed;

        // This loop ends only after the incoming message queue is completed, which happens
        // when the connection is lost or disconnected, and after the queue has been drained.
        protected override async Task ReadIncomingMessagesAsync()
        {
            try
            {
                await base.ReadIncomingMessagesAsync().ConfigureAwait(false);
            }
            finally
            {
                _isClosed = true;
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

#endif
