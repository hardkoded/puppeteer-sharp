using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace PuppeteerSharp.Helpers.Json
{
    internal static class JsonHelper
    {
        public static readonly JsonSerializerOptions DefaultJsonSerializerSettings = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
#if NET8_0_OR_GREATER
            TypeInfoResolver = JsonSerializer.IsReflectionEnabledByDefault
                ? new DefaultJsonTypeInfoResolver()
                : SystemTextJsonSerializationContext.Default,
#endif
            Converters =
            {
                new HttpMethodConverter(),
                new JSHandleConverter(),
                new JsonStringEnumMemberConverter(),
            },
        };

        /// <summary>
        /// Convert a <see cref="JsonElement"/> to an object.
        /// </summary>
        /// <typeparam name="T">Type to convert the <see cref="JsonElement"/> to.</typeparam>
        /// <param name="element">Element to convert.</param>
        /// <param name="options">Serialization options.</param>
        /// <returns>Converted value.</returns>
        public static T ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
            => element.Deserialize<T>(options ?? DefaultJsonSerializerSettings);

        /// <summary>
        /// Convert a <see cref="JsonElement"/> to an object.
        /// </summary>
        /// <param name="element">Element to convert.</param>
        /// <param name="type">Type to convert the <see cref="JsonElement"/> to.</param>
        /// <param name="options">Serialization options.</param>
        /// <returns>Converted value.</returns>
        public static object ToObject(this JsonElement element, Type type, JsonSerializerOptions options = null)
            => element.Deserialize(type, options ?? DefaultJsonSerializerSettings);

        /// <summary>
        /// Serialize an object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="value">Object to serialize.</param>
        /// <param name="options">Serialization options.</param>
        /// <returns>Serialized object.</returns>
        public static string ToJson<T>(this T value, JsonSerializerOptions options = null)
            => JsonSerializer.Serialize(value, options ?? DefaultJsonSerializerSettings);

        /// <summary>
        /// Convert a <see cref="JsonDocument"/> to an object.
        /// </summary>
        /// <typeparam name="T">Type to convert the <see cref="JsonElement"/> to.</typeparam>
        /// <param name="document">Document to convert.</param>
        /// <param name="options">Serialization options.</param>
        /// <returns>Converted value.</returns>
        public static T ToObject<T>(this JsonDocument document, JsonSerializerOptions options = null)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            return document.RootElement.ToObject<T>(options ?? DefaultJsonSerializerSettings);
        }
    }
}
