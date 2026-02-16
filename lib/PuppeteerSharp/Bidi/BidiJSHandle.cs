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
using System.Globalization;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi;

internal class BidiJSHandle(RemoteValue value, BidiRealm realm) : JSHandle
{
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

    public override async Task<T> JsonValueAsync<T>()
    {
        // If it's a primitive value or doesn't have a handle, deserialize directly
        if (IsPrimitiveValue || string.IsNullOrEmpty(Id))
        {
            return DeserializeValue<T>(RemoteValue.Value);
        }

        // Otherwise, use evaluation for objects with handles
        return await EvaluateFunctionAsync<T>("value => value").ConfigureAwait(false);
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

    /// <inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        if (Disposed)
        {
            return;
        }

        Disposed = true;
        await realm.DestroyHandlesAsync(this).ConfigureAwait(false);
    }

    private T DeserializeValue<T>(object value)
    {
        if (value is null)
        {
            return default;
        }

        if (typeof(T) == typeof(object))
        {
            return (T)value;
        }

        if (typeof(T) == typeof(string))
        {
            return (T)value;
        }

        if (typeof(T) == typeof(int))
        {
            return (T)(object)Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }

        if (typeof(T) == typeof(double))
        {
            return (T)(object)Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        if (typeof(T) == typeof(bool))
        {
            return (T)(object)Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        }

        if (typeof(T) == typeof(decimal))
        {
            return (T)(object)Convert.ToDecimal(value, CultureInfo.InvariantCulture);
        }

        if (typeof(T) == typeof(DateTime) && RemoteValue.Type == "date")
        {
            return (T)(object)DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture);
        }

        return (T)value;
    }
}

#endif
