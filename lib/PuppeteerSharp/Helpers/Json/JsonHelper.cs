using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace PuppeteerSharp.Helpers.Json
{
    internal static class JsonHelper
    {
        public static readonly Lazy<JsonSerializerOptions> DefaultJsonSerializerSettings = new(() =>
        {
#if NET8_0_OR_GREATER
            IJsonTypeInfoResolver context;

            if (JsonSerializer.IsReflectionEnabledByDefault)
            {
                context = new DefaultJsonTypeInfoResolver();
            }
            else if (Puppeteer.ExtraJsonSerializerContext != null)
            {
                context = JsonTypeInfoResolver.Combine(
                    SystemTextJsonSerializationContext.Default,
                    Puppeteer.ExtraJsonSerializerContext);
            }
            else
            {
                context = SystemTextJsonSerializationContext.Default;
            }
#endif

            return new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
#if NET8_0_OR_GREATER
                TypeInfoResolver = context,
#endif
                Converters =
                {
                    new HttpMethodConverter(), new JSHandleConverter(),
                },
            };
        });

        /// <summary>
        /// Convert a <see cref="JsonElement"/> to an object.
        /// </summary>
        /// <typeparam name="T">Type to convert the <see cref="JsonElement"/> to.</typeparam>
        /// <param name="element">Element to convert.</param>
        /// <param name="options">Serialization options.</param>
        /// <returns>Converted value.</returns>
        public static T ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
            => element.Deserialize<T>(options ?? DefaultJsonSerializerSettings.Value);

        /// <summary>
        /// Convert a <see cref="JsonElement"/> to an object.
        /// </summary>
        /// <param name="element">Element to convert.</param>
        /// <param name="type">Type to convert the <see cref="JsonElement"/> to.</param>
        /// <param name="options">Serialization options.</param>
        /// <returns>Converted value.</returns>
        public static object ToObject(this JsonElement element, Type type, JsonSerializerOptions options = null)
            => element.Deserialize(type, options ?? DefaultJsonSerializerSettings.Value);

        /// <summary>
        /// Serialize an object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="value">Object to serialize.</param>
        /// <param name="options">Serialization options.</param>
        /// <returns>Serialized object.</returns>
        public static string ToJson<T>(this T value, JsonSerializerOptions options = null)
            => JsonSerializer.Serialize(value, options ?? DefaultJsonSerializerSettings.Value);

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

            return document.RootElement.ToObject<T>(options ?? DefaultJsonSerializerSettings.Value);
        }
    }
}
