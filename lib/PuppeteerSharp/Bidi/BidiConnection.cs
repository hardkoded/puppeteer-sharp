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
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Transport;
using WebDriverBiDi.Protocol;
using Connection = PuppeteerSharp.Cdp.Connection;

namespace PuppeteerSharp.Bidi;

// TODO: Evaluate if we can have a ConnectionBase class  where CdpConnection and BidiConnection can share some code.
internal class BidiConnection : IDisposable
{
    internal const int DefaultCommandTimeout = 180_000;
    private readonly ILogger _logger;
    private readonly TaskQueue _callbackQueue = new();

    private readonly ConcurrentDictionary<int, MessageTask> _callbacks = new();
    private readonly AsyncDictionaryHelper<string, CdpCDPSession> _sessions = new("Session {0} not found");

    private int _lastId;

    public BidiConnection(string url, IConnectionTransport transport, int delay, int protocolTimeout, ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory ?? new LoggerFactory();
        Url = url;
        Delay = delay;
        Transport = transport;

        _logger = LoggerFactory.CreateLogger<Connection>();
        ProtocolTimeout = protocolTimeout;
        MessageQueue = new AsyncMessageQueue(true, _logger);

        Transport.MessageReceived += Transport_MessageReceived;
        Transport.Closed += Transport_Closed;
    }

    /// <summary>
    /// Occurs when the connection is closed.
    /// </summary>
    public event EventHandler Disconnected;

    /// <summary>
    /// Gets the logger factory.
    /// </summary>
    /// <value>The logger factory.</value>
    public ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// Gets the WebSocket URL.
    /// </summary>
    /// <value>The URL.</value>
    public string Url { get; }

    /// <summary>
    /// Gets the sleep time when a message is received.
    /// </summary>
    /// <value>The delay.</value>
    public int Delay { get; }

    /// <summary>
    /// Gets the Connection transport.
    /// </summary>
    /// <value>Connection transport.</value>
    public IConnectionTransport Transport { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Connection"/> is closed.
    /// </summary>
    /// <value><c>true</c> if is closed; otherwise, <c>false</c>.</value>
    public bool IsClosed { get; internal set; }

    /// <summary>
    /// Connection close reason.
    /// </summary>
    public string CloseReason { get; private set; }

    internal int ProtocolTimeout { get; }

    internal AsyncMessageQueue MessageQueue { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<JObject> SendAsync(string method, object args = null, bool waitForCallback = true, CommandOptions options = null)
    {
        if (IsClosed)
        {
            throw new TargetClosedException($"Protocol error({method}): Target closed.", CloseReason);
        }

        var id = GetMessageID();
        var message = GetMessage(id, method, args);

        MessageTask callback = null;
        if (waitForCallback)
        {
            callback = new MessageTask
            {
                TaskWrapper = new TaskCompletionSource<JObject>(TaskCreationOptions.RunContinuationsAsynchronously),
                Method = method,
                Message = message,
            };
            _callbacks[id] = callback;
        }

        await RawSendAsync(message, options).ConfigureAwait(false);
        return waitForCallback ? await callback.TaskWrapper.Task.WithTimeout(ProtocolTimeout).ConfigureAwait(false) : null;
    }

    public async Task<T> SendAsync<T>(string method, object args = null, CommandOptions options = null)
    {
        var response = await SendAsync(method, args, true, options).ConfigureAwait(false);
        return response.ToObject<T>(true);
    }

    internal Task RawSendAsync(string message, CommandOptions options = null)
    {
        _logger.LogTrace("Send ► {Message}", message);
        return Transport.SendAsync(message);
    }

    internal string GetMessage(int id, string method, object args, string sessionId = null)
        => JsonConvert.SerializeObject(
            new BidiCommand { Id = id, Method = method, Params = args },
            JsonHelper.DefaultJsonSerializerSettings);

    internal int GetMessageID() => Interlocked.Increment(ref _lastId);

    internal void Close(string closeReason)
    {
        if (IsClosed)
        {
            return;
        }

        IsClosed = true;
        CloseReason = closeReason;

        Transport.StopReading();
        Disconnected?.Invoke(this, EventArgs.Empty);

        foreach (var session in _sessions.Values.ToArray())
        {
            session.Close(closeReason);
        }

        _sessions.Clear();

        foreach (var response in _callbacks.Values.ToArray())
        {
            response.TaskWrapper.TrySetException(new TargetClosedException(
                $"Protocol error({response.Method}): Target closed.",
                closeReason));
        }

        _callbacks.Clear();
        MessageQueue.Dispose();
    }

    private void Transport_Closed(object sender, TransportClosedEventArgs e) => Close(e.CloseReason);

    /// <summary>
    /// Releases all resource used by the <see cref="BidiConnection"/> object.
    /// It will raise the <see cref="Disconnected"/> event and dispose <see cref="Transport"/>.
    /// </summary>
    /// <remarks>Call <see cref="Dispose()"/> when you are finished using the <see cref="Connection"/>. The
    /// <see cref="Dispose()"/> method leaves the <see cref="Connection"/> in an unusable state.
    /// After calling <see cref="Dispose()"/>, you must release all references to the
    /// <see cref="Connection"/> so the garbage collector can reclaim the memory that the
    /// <see cref="Connection"/> was occupying.</remarks>
    /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
    private void Dispose(bool disposing)
    {
        Close("Connection disposed");
        Transport.MessageReceived -= Transport_MessageReceived;
        Transport.Closed -= Transport_Closed;
        Transport.Dispose();
        _callbackQueue.Dispose();
    }

    private async void Transport_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        try
        {
            await _callbackQueue.Enqueue(() => ProcessMessage(e)).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            // We could just catch ObjectDisposedException but as this is an event listener
            // we don't want to crash the whole process.
            _logger.LogError(exception, $"Failed to process message {e.Message}");
        }
    }

    private async Task ProcessMessage(MessageReceivedEventArgs e)
    {
        try
        {
            var response = e.Message;
            CommandResponseMessage obj;

            if (response.Length > 0 && Delay > 0)
            {
                await Task.Delay(Delay).ConfigureAwait(false);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<CommandResponseMessage>(response, JsonHelper.DefaultJsonSerializerSettings);
            }
            catch (JsonException exc)
            {
                _logger.LogError(exc, "Failed to deserialize response");
                return;
            }

            _logger.LogTrace("◀ Receive {Message}", response);
            ProcessIncomingMessage(obj);
        }
        catch (Exception ex)
        {
            var message = $"Connection failed to process {e.Message}. {ex.Message}. {ex.StackTrace}";
            _logger.LogError(ex, message);
            Close(message);
        }
    }


    private void ProcessIncomingMessage(CommandResponseMessage obj)
    {
        if(obj.Type == )
        var method = obj.Method;
        var param = obj.Params?.ToObject<ConnectionResponseParams>();

        if (method == "Target.attachedToTarget")
        {
            var sessionId = param.SessionId;
            var session = new CdpCDPSession(this, param.TargetInfo.Type, sessionId, obj.SessionId);
            _sessions.AddItem(sessionId, session);

            SessionAttached?.Invoke(this, new SessionEventArgs(session));

            if (obj.SessionId != null && _sessions.TryGetValue(obj.SessionId, out var parentSession))
            {
                parentSession.OnSessionAttached(session);
            }
        }
        else if (method == "Target.detachedFromTarget")
        {
            var sessionId = param.SessionId;
            if (_sessions.TryRemove(sessionId, out var session) && !session.IsClosed)
            {
                session.Close("Target.detachedFromTarget");
                SessionDetached?.Invoke(this, new SessionEventArgs(session));

                if (_sessions.TryGetValue(sessionId, out var parentSession))
                {
                    parentSession.OnSessionDetached(session);
                }
            }
        }

        if (!string.IsNullOrEmpty(obj.SessionId))
        {
            var session = GetSession(obj.SessionId);
            session?.OnMessage(obj);
        }
        else if (obj.Id.HasValue)
        {
            // If we get the object we are waiting for we return it.
            // If not we add this to the list, sooner or later someone will come for it
            if (_callbacks.TryRemove(obj.Id.Value, out var callback))
            {
                MessageQueue.Enqueue(callback, obj);
            }
        }
        else
        {
            MessageReceived?.Invoke(this, new MessageEventArgs
            {
                MessageID = method,
                MessageData = obj.Params,
            });
        }
    }
}
