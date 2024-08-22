using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json
{
    /// <summary>
    /// JSHandleMethodConverter will throw an exception if a JSHandle object is trying to be serialized.
    /// </summary>
    internal sealed class JSHandleConverter : JsonConverter<IJSHandle>
    {
        /// <inheritdoc cref="JsonConverter"/>
        public override IJSHandle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => null;

        /// <inheritdoc cref="JsonConverter"/>
        public override void Write(Utf8JsonWriter writer, IJSHandle value, JsonSerializerOptions options)
            => throw new PuppeteerException("Unable to make function call. Are you passing a nested JSHandle?");
    }
}
