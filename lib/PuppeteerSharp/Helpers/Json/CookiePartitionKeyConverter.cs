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
            JsonNode? node = JsonNode.Parse(ref reader);

            return node?["topLevelSite"]?.GetValue<string>() ?? null;
        }

        /// <inheritdoc cref="JsonConverter"/>
        public override void Write(
            Utf8JsonWriter writer,
            string value,
            JsonSerializerOptions options)
        {
            if (value != null && writer != null)
            {
                writer.WriteStartObject();
                writer.WriteString("topLevelSite", value);
                writer.WriteBoolean("hasCrossSiteAncestor", false);
                writer.WriteEndObject();
            }
        }
    }
}
