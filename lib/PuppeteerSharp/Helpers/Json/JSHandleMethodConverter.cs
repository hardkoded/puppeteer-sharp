using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Helpers.Json
{
    /// <summary>
    /// JSHandleMethodConverter will throw an exception if a JSHandle object is trying to be serialized
    /// </summary>
    internal class JSHandleMethodConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => null;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new EvaluationFailedException("Unable to make function call. Are you passing a nested JSHandle?");
    }
}
