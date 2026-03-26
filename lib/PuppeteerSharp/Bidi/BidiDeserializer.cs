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
using System.Collections.Generic;
using System.Globalization;
using WebDriverBiDi.Script;

#if !CDP_ONLY

namespace PuppeteerSharp.Bidi;

internal static class BidiDeserializer
{
    public static object Deserialize(RemoteValue value)
    {
        if (value == null)
        {
            return null;
        }

        return value switch
        {
            UndefinedRemoteValue or NullRemoteValue => null,
            StringRemoteValue s => s.Value,
            BooleanRemoteValue b => (object)b.Value,
            LongRemoteValue l => (object)l.Value,
            DoubleRemoteValue d => (object)d.Value,
            ValueHoldingRemoteValue v when value.Type == RemoteValueType.BigInt =>
                v.ValueObject != null ? Convert.ToInt64(v.ValueObject, CultureInfo.InvariantCulture) : null,
            _ => null,
        };
    }

    public static object ToPrettyPrint(this RemoteValue value)
    {
        if (value is null)
        {
            return null;
        }

        switch (value.Type)
        {
            case RemoteValueType.Array:
            case RemoteValueType.Set:
                if (value is CollectionRemoteValue collection)
                {
                    var list = new List<object>();

                    foreach (var item in collection.Value)
                    {
                        list.Add(item.ToPrettyPrint());
                    }

                    return list.ToArray();
                }

                return Array.Empty<object>();
            case RemoteValueType.Object:
            case RemoteValueType.Map:
                // TODO
                return value is ValueHoldingRemoteValue vhMap ? vhMap.ValueObject : null;
            case RemoteValueType.RegExp:
            case RemoteValueType.Date:
                // TODO
                return value is ValueHoldingRemoteValue vhDate ? vhDate.ValueObject : null;
            case RemoteValueType.Promise:
                return "{}";
            case RemoteValueType.Undefined:
                return "undefined";
            case RemoteValueType.Null:
                return "null";
            case RemoteValueType.Number:
                return value is ValueHoldingRemoteValue vhNum ? vhNum.ValueObject : null;
            case RemoteValueType.BigInt:
                return value is ValueHoldingRemoteValue vhBigInt
                    ? Convert.ToInt64(vhBigInt.ValueObject, CultureInfo.InvariantCulture)
                    : null;
            case RemoteValueType.Boolean:
                return value is BooleanRemoteValue b
                    ? (object)b.Value
                    : null;
            default:
                return value is ValueHoldingRemoteValue vh ? vh.ValueObject : null;
        }
    }
}

#endif
