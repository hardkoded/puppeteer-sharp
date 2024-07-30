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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json;

/// <summary>
/// Converts an enum value to or from a JSON string.
/// </summary>
internal sealed class JsonStringEnumMemberConverter : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert != null &&
           (typeToConvert.IsEnum || Nullable.GetUnderlyingType(typeToConvert)?.IsEnum == true) &&
           Attribute.IsDefined(typeToConvert, typeof(JsonStringIgnoreAttribute));

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var nullableType = Nullable.GetUnderlyingType(typeToConvert);

        return (JsonConverter)Activator.CreateInstance(
            nullableType == null ?
                typeof(EnumMemberConverter<>).MakeGenericType(typeToConvert) :
                typeof(NullableEnumMemberConverter<>).MakeGenericType(nullableType));
    }

    private static TEnum? Read<TEnum>(ref Utf8JsonReader reader)
        where TEnum : struct, Enum
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return EnumHelper.FromValueString<TEnum>(reader.GetString());
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            var enumTypeCode = Type.GetTypeCode(typeof(TEnum));
            return enumTypeCode switch
            {
                TypeCode.Int32 => (TEnum)(object)reader.GetInt32(),
                TypeCode.Int64 => (TEnum)(object)reader.GetInt64(),
                TypeCode.Int16 => (TEnum)(object)reader.GetInt16(),
                TypeCode.Byte => (TEnum)(object)reader.GetByte(),
                TypeCode.UInt32 => (TEnum)(object)reader.GetUInt32(),
                TypeCode.UInt64 => (TEnum)(object)reader.GetUInt64(),
                TypeCode.UInt16 => (TEnum)(object)reader.GetUInt16(),
                TypeCode.SByte => (TEnum)(object)reader.GetSByte(),
                _ => throw new JsonException($"Enum '{typeof(TEnum).Name}' of {enumTypeCode} type is not supported."),
            };
        }

        throw new JsonException();
    }

    private static void Write<TEnum>(Utf8JsonWriter writer, TEnum? value)
        where TEnum : struct, Enum
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

    private class EnumMemberConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : struct, Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Read<TEnum>(ref reader) ?? default;

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
            => Write<TEnum>(writer, value);
    }

    private class NullableEnumMemberConverter<TEnum> : JsonConverter<TEnum?>
        where TEnum : struct, Enum
    {
        public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Read<TEnum>(ref reader);

        public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
            => Write<TEnum>(writer, value);
    }
}
