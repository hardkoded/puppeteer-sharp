using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json;

internal class LowSurrogateConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            return null;
        }

        var span = reader.HasValueSequence
            ? reader.ValueSequence.ToArray()
#if NET8_0_OR_GREATER
            : reader.ValueSpan;
#else
            : reader.ValueSpan.ToArray();
#endif
        var value = Encoding.UTF8.GetString(span);

        try
        {
            if (reader.ValueIsEscaped)
            {
                value = JsonUnescape(value);
            }

            return value;
        }
        catch
        {
            return value;
        }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }

    private static string JsonUnescape(string jsonString)
    {
        using var doc = JsonDocument.Parse($"\"{jsonString}\"");
        return doc.RootElement.GetString();
    }
}
