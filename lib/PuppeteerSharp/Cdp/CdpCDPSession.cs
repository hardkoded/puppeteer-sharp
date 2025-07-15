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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp;

// This is a pretty terrible name, but it matches upstream

/// <inheritdoc />
public class CdpCDPSession : CDPSession
{
    private readonly ConcurrentDictionary<int, MessageTask> _callbacks = new();
    private readonly string _parentSessionId;
    private readonly TargetType _targetType;
    private int _lastId;

    internal CdpCDPSession(Connection connection, TargetType targetType, string sessionId, string parentSessionId)
    {
        Connection = connection;
        _targetType = targetType;
        Id = sessionId;
        _parentSessionId = parentSessionId;
    }

    internal override CDPSession ParentSession
        => string.IsNullOrEmpty(_parentSessionId) ? this : Connection.GetSession(_parentSessionId) ?? this;

    internal bool IsClosed { get; private set; }

    /// <inheritdoc />
    public override Task DetachAsync()
    {
        if (Connection == null)
        {
            throw new PuppeteerException($"Session already detached.Most likely the {_targetType} has been closed.");
        }

        return Connection.SendAsync("Target.detachFromTarget", new TargetDetachFromTargetRequest { SessionId = Id });
    }

    /// <inheritdoc />
    public override async Task<JsonElement?> SendAsync(
        string method,
        object args = null,
        bool waitForCallback = true,
        CommandOptions options = null)
    {
        if (Connection == null)
        {
            throw new TargetClosedException(
                $"Protocol error ({method}): Session closed. " +
                $"Most likely the {_targetType} has been closed." +
                $"Close reason: {CloseReason}",
                CloseReason);
        }

        var id = GetMessageId();
        var message = Connection.GetMessage(id, method, args, Id);

        MessageTask callback = null;
        if (waitForCallback)
        {
            callback = new MessageTask
            {
                TaskWrapper =
                    new TaskCompletionSource<JsonElement?>(TaskCreationOptions.RunContinuationsAsynchronously),
                Method = method,
                Message = message,
            };
            _callbacks[id] = callback;
        }

        try
        {
            await Connection.RawSendAsync(message, options).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (waitForCallback && _callbacks.TryRemove(id, out _))
            {
                callback.TaskWrapper.TrySetException(new MessageException(ex.Message, ex));
            }
        }

        return waitForCallback
            ? await callback.TaskWrapper.Task.WithTimeout(options?.Timeout ?? Connection.ProtocolTimeout)
                .ConfigureAwait(false)
            : null;
    }

    internal bool HasPendingCallbacks() => !_callbacks.IsEmpty;

    internal int GetMessageId() => Interlocked.Increment(ref _lastId);

    internal IEnumerable<MessageTask> GetPendingMessages() => _callbacks.Values;

    internal void OnMessage(ConnectionResponse obj)
    {
        var id = obj.Id;

        if (id.HasValue && _callbacks.TryRemove(id.Value, out var callback))
        {
            Connection.MessageQueue.Enqueue(callback, obj);
        }
        else if (obj.Method is not null && obj.Params is not null)
        {
            // If the message was set to not wait for callback, the message won't be in the callbacks dictionary
            // And it might have no method set.
            var method = obj.Method;
            OnMessageReceived(new MessageEventArgs { MessageID = method, MessageData = (JsonElement)obj.Params });
        }
    }

    internal override void Close(string closeReason)
    {
        if (IsClosed)
        {
            return;
        }

        CloseReason = closeReason;
        IsClosed = true;

        foreach (var callback in _callbacks.Values.ToArray())
        {
            callback.TaskWrapper.TrySetException(new TargetClosedException(
                $"Protocol error({callback.Method}): Target closed.",
                closeReason));
        }

        _callbacks.Clear();
        OnDisconnected();
        Connection = null;
    }
}
