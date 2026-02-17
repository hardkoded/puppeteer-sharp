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

        return value.Type switch
        {
            "undefined" => null,
            "null" => null,
            "string" => value.Value,
            "number" => DeserializeNumber(value.Value),
            "boolean" => value.Value,
            "bigint" => value.Value != null ? Convert.ToInt64(value.Value, CultureInfo.InvariantCulture) : null,
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
            case "array":
            case "set":
                if (value.Value is IEnumerable<RemoteValue> enumerable)
                {
                    var list = new List<object>();

                    foreach (var item in enumerable)
                    {
                        list.Add(item.ToPrettyPrint());
                    }

                    return list.ToArray();
                }

                return Array.Empty<object>();
            case "object":
            case "map":
                // TODO
                return value.Value;
            case "regexp":
            case "date":
                // TODO
                return value.Value;
            case "promise":
                return "{}";
            case "undefined":
                return "undefined";
            case "null":
                return "null";
            case "number":
                return DeserializeNumber(value.Value);
            case "bigint":
                return Convert.ToInt64(value.Value, CultureInfo.InvariantCulture);
            case "boolean":
                return Convert.ToBoolean(value.Value, CultureInfo.InvariantCulture);
            default:
                return value.Value;
        }
    }

    private static object DeserializeNumber(object value)
    {
        switch (value)
        {
            case "-0":
                return -0;
            case "NaN":
                return double.NaN;
            case "Infinity":
                return double.PositiveInfinity;
            case "-Infinity":
                return double.NegativeInfinity;
            default:
                return value;
        }
    }
}

#endif
