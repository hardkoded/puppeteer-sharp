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
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json;

internal sealed class AnyTypeToStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString() ?? string.Empty;
        }

        // There could be many types by we assume that, in general, this will be used for ints.
        if (reader.TokenType == JsonTokenType.Number)
        {
            var stringValue = reader.GetInt32();
            return stringValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        if (reader.TokenType is JsonTokenType.False or JsonTokenType.True)
        {
            return reader.GetBoolean().ToString();
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            reader.Skip();
            return "(not supported)";
        }

        throw new JsonException($"Unsupported token type: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (int.TryParse(value, out var number))
        {
            writer.WriteNumberValue(number);
        }
        else
        {
            writer.WriteStringValue(value);
        }
    }
}
