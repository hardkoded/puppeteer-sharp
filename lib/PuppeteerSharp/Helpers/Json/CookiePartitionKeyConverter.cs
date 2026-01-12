#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json
{
    internal sealed class CookiePartitionKeyConverter : JsonConverter<string>
    {
        /// <inheritdoc cref="JsonConverter"/>
        public override bool CanConvert(Type objectType) => typeof(string).IsAssignableFrom(objectType);

        /// <inheritdoc cref="JsonConverter"/>
        public override string? Read(
            ref Utf8JsonReader reader,
            Type objectType,
            JsonSerializerOptions options)
        {
            // Handle both string format (for user serialization) and object format (from CDP)
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            JsonNode? node = JsonNode.Parse(ref reader);

            return node?["topLevelSite"]?.GetValue<string>() ?? null;
        }

        /// <inheritdoc cref="JsonConverter"/>
        public override void Write(
            Utf8JsonWriter writer,
            string value,
            JsonSerializerOptions options)
        {
            // Write as a simple string for user serialization/deserialization
            // This allows cookies to be easily saved to and loaded from files
            if (value != null)
            {
                writer.WriteStringValue(value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
