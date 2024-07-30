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
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json;

/// <summary>
/// <see cref="HttpMethodConverter"/> will convert a <see cref="HttpMethod"/> to and from a string.
/// </summary>
internal sealed class HttpMethodConverter : JsonConverter<HttpMethod>
{
    /// <inheritdoc cref="JsonConverter"/>
    public override bool CanConvert(Type objectType) => typeof(HttpMethod).IsAssignableFrom(objectType);

    /// <inheritdoc cref="JsonConverter"/>
    public override HttpMethod Read(
        ref Utf8JsonReader reader,
        Type objectType,
        JsonSerializerOptions options)
    {
        var readerString = reader.GetString() ?? string.Empty;
        return new HttpMethod(readerString);
    }

    /// <inheritdoc cref="JsonConverter"/>
    public override void Write(
        Utf8JsonWriter writer,
        HttpMethod value,
        JsonSerializerOptions options)
    {
        if (value != null && writer != null)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
