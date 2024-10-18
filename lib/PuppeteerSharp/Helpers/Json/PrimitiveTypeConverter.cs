#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json
{
    /// <summary>
    /// Support types (<see cref="decimal"/>, <see cref="int"/> and <see cref="string"/>)
    /// used by <see cref="PdfOptions"/> and <see cref="Media.MarginOptions"/> for serialization / deserialization.
    /// For usecases like <see href="https://github.com/hardkoded/puppeteer-sharp/issues/1001"/>.
    /// </summary>
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
