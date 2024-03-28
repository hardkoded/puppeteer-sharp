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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests;

public class MockCDPSession : ICDPSession
{
    public event EventHandler<MessageEventArgs> MessageReceived;

    public Task<JObject> SendAsync(string method, object args = null, bool waitForCallback = true, CommandOptions options = null)
        => SendAsync<JObject>(method, args, options);

    public Task<T> SendAsync<T>(string method, object args = null, CommandOptions options = null)
    {
        Console.WriteLine($"Mock Session send: {method} {args?.ToString() ?? string.Empty}");
        return Task.FromResult<T>(default);
    }

#pragma warning disable CS0067
    public event EventHandler Disconnected;
    public event EventHandler<SessionEventArgs> SessionAttached;
    public event EventHandler<SessionEventArgs> SessionDetached;
#pragma warning disable CS0067

    public string CloseReason { get; }
    public bool IsClosed { get; }
    public ILoggerFactory LoggerFactory { get; }
    public string Id { get; } = "1";
    public TargetType TargetType { get; }
    public Task DetachAsync() => Task.CompletedTask;

    internal void OnMessage(ConnectionResponse obj)
        => MessageReceived?.Invoke(this, new MessageEventArgs
        {
            MessageID = obj.Method,
            MessageData = obj.Params,
        });
}
