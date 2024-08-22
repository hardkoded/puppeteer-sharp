/*
 * MIT License
 *
 * Copyright (c) Microsoft Corporation.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json;

/// <summary>
/// Converts an enum value to or from a JSON string.
/// </summary>
/// <typeparam name="TEnum">The enum type.</typeparam>
internal class JsonStringEnumMemberConverter<TEnum> : JsonConverterFactory
    where TEnum : struct, Enum
{
    private static readonly ConcurrentDictionary<Type, JsonConverter> _jsonConverterCache = new();

    public static JsonConverter CreateJsonConverter()
    {
        var nullableType = Nullable.GetUnderlyingType(typeof(TEnum));
        JsonConverter jsonConverter = nullableType == null ? new EnumMemberConverter<TEnum>() : new NullableEnumMemberConverter<TEnum>();
        _jsonConverterCache.TryAdd(typeof(TEnum), jsonConverter);
        return jsonConverter;
    }

    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsEnum || Nullable.GetUnderlyingType(typeToConvert)?.IsEnum == true;

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => _jsonConverterCache.TryGetValue(typeToConvert, out var jsonConverter) ? jsonConverter : CreateJsonConverter();

    private static T? Read<T>(ref Utf8JsonReader reader)
        where T : struct, Enum
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return EnumHelper.FromValueString<T>(reader.GetString());
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            var enumTypeCode = Type.GetTypeCode(typeof(T));
            return enumTypeCode switch
            {
                TypeCode.Int32 => (T)(object)reader.GetInt32(),
                TypeCode.Int64 => (T)(object)reader.GetInt64(),
                TypeCode.Int16 => (T)(object)reader.GetInt16(),
                TypeCode.Byte => (T)(object)reader.GetByte(),
                TypeCode.UInt32 => (T)(object)reader.GetUInt32(),
                TypeCode.UInt64 => (T)(object)reader.GetUInt64(),
                TypeCode.UInt16 => (T)(object)reader.GetUInt16(),
                TypeCode.SByte => (T)(object)reader.GetSByte(),
                _ => throw new JsonException($"Enum '{typeof(TEnum).Name}' of {enumTypeCode} type is not supported."),
            };
        }

        throw new JsonException();
    }

    private static void Write<T>(Utf8JsonWriter writer, T? value)
        where T : struct, Enum
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToValueString());
        }
        else
        {
            writer.WriteNullValue();
        }
    }

    private class EnumMemberConverter<T> : JsonConverter<T>
        where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Read<T>(ref reader) ?? default;

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            => Write<T>(writer, value);
    }

    private class NullableEnumMemberConverter<T> : JsonConverter<T?>
        where T : struct, Enum
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Read<T>(ref reader);

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
            => Write<T>(writer, value);
    }
}
