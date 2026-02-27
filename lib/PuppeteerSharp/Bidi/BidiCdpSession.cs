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

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

#if !CDP_ONLY

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiCdpSession : CDPSession
{
    // I don't like the idea of having this static field. But this is the original implementation.
    private static readonly ConcurrentDictionary<string, BidiCdpSession> _sessions = new();

    internal BidiCdpSession(BidiFrame bidiFrame, ILoggerFactory loggerFactory)
    {
        Frame = bidiFrame;
        LoggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public override ILoggerFactory LoggerFactory { get; }

    /// <inheritdoc />
    public override bool Detached { get; }

    internal static IEnumerable<BidiCdpSession> Sessions => _sessions.Values;

    internal override CDPSession ParentSession => null;

    internal BidiFrame Frame { get; set; }

    /// <inheritdoc />
    public override Task<JsonElement?> SendAsync(string method, object args = null, bool waitForCallback = true, CommandOptions options = null)
        => throw new PuppeteerException("CDP support is required for this feature. The current browser does not support CDP.");

    /// <inheritdoc />
    public override Task DetachAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override void Close(string reason = null)
    {
        CloseReason = reason;
        OnClose();
    }

    internal void OnClose()
    {
        throw new NotImplementedException();
    }
}

#endif
