#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json
{
    // Support types used in PdfOptions / MarginOptions: string, decimal, int
    internal sealed class PrimitiveTypeConverter : JsonConverter<object>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out var i))
                {
                    return i;
                }
                else if (reader.TryGetDecimal(out var dec))
                {
                    return dec;
                }
            }

            return JsonSerializer.Deserialize(ref reader, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
            }
            else if (value is string str)
            {
                writer.WriteStringValue(str);
            }
            else if (value is decimal dec)
            {
                writer.WriteNumberValue(dec);
            }
            else if (value is int i)
            {
                writer.WriteNumberValue(i);
            }
            else
            {
                JsonSerializer.Serialize(writer, value, options);
            }
        }
    }
}
