#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json
{
    internal sealed class CookiePartitionKeyConverter : JsonConverter<CookiePartitionKey>
    {
        /// <inheritdoc cref="JsonConverter"/>
        public override bool CanConvert(Type objectType) => typeof(CookiePartitionKey).IsAssignableFrom(objectType);

        /// <inheritdoc cref="JsonConverter"/>
        public override CookiePartitionKey? Read(
            ref Utf8JsonReader reader,
            Type objectType,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            // Handle string format (legacy)
            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                return value == null ? null : new CookiePartitionKey { SourceOrigin = value };
            }

            // Handle object format (from CDP)
            JsonNode? node = JsonNode.Parse(ref reader);
            if (node == null)
            {
                return null;
            }

            var sourceOrigin = node["topLevelSite"]?.GetValue<string>() ?? node["sourceOrigin"]?.GetValue<string>();
            if (sourceOrigin == null)
            {
                return null;
            }

            var result = new CookiePartitionKey { SourceOrigin = sourceOrigin };
            if (node["hasCrossSiteAncestor"] != null)
            {
                result.HasCrossSiteAncestor = node["hasCrossSiteAncestor"]?.GetValue<bool>();
            }

            return result;
        }

        /// <inheritdoc cref="JsonConverter"/>
        public override void Write(
            Utf8JsonWriter writer,
            CookiePartitionKey? value,
            JsonSerializerOptions options)
        {
            if (value != null)
            {
                writer.WriteStartObject();
                writer.WriteString("topLevelSite", value.SourceOrigin);
                writer.WriteBoolean("hasCrossSiteAncestor", value.HasCrossSiteAncestor ?? false);
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
