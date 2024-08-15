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
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc/>
public class CdpJSHandle : JSHandle
{
    internal CdpJSHandle(IsolatedWorld world, RemoteObject remoteObject) : base(world, remoteObject)
    {
        Logger = Client.Connection.LoggerFactory.CreateLogger(GetType());
    }

    /// <summary>
    /// Logger.
    /// </summary>
    private ILogger Logger { get; }

    private CDPSession Client => Realm.Environment.Client;

    /// <inheritdoc/>
    public override async Task<T> JsonValueAsync<T>()
    {
        var objectId = RemoteObject.ObjectId;

        if (objectId == null)
        {
            return (T)RemoteObjectHelper.ValueFromRemoteObject<T>(RemoteObject);
        }

        var value = await EvaluateFunctionAsync<T>("object => object").ConfigureAwait(false);

        return value ?? throw new PuppeteerException("Could not serialize referenced object");
    }

    /// <inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        if (Disposed)
        {
            return;
        }

        Disposed = true;

        if (DisposeAction != null)
        {
            await DisposeAction().ConfigureAwait(false);
        }

        await RemoteObjectHelper.ReleaseObjectAsync(Client, RemoteObject, Logger).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (RemoteObject.ObjectId == null)
        {
            return "JSHandle:" + RemoteObjectHelper.ValueFromRemoteObject<object>(RemoteObject, true);
        }

        var type = RemoteObject.Subtype != RemoteObjectSubtype.Other
            ? RemoteObject.Subtype.ToString()
            : RemoteObject.Type.ToString();
        return "JSHandle@" + type.ToLower(System.Globalization.CultureInfo.CurrentCulture);
    }
}
