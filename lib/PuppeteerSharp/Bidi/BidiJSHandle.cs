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

using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi;

internal class BidiJSHandle(RemoteValue value, BidiRealm realm) : JSHandle
{
    private bool _disposed;

    public RemoteValue RemoteValue { get; } = value;

    public bool IsPrimitiveValue
    {
        get
        {
            return RemoteValue.Type switch
            {
                "string" or "number" or "bigint" or "boolean" or "undefined" or "null" => true,
                _ => false,
            };
        }
    }

    internal string Id => RemoteValue.Handle;

    internal override Realm Realm { get; } = realm;

    public static BidiJSHandle From(RemoteValue value, BidiRealm realm)
    {
        return new BidiJSHandle(value, realm);
    }

    public override Task<T> JsonValueAsync<T>() => throw new System.NotImplementedException();

    public override async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        await realm.DestroyHandlesAsync(this).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (IsPrimitiveValue)
        {
            return "JSHandle:" + RemoteValue.ToPrettyPrint();
        }

        return "JSHandle@" + RemoteValue.Type;
    }
}
