using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace PuppeteerSharp.Helpers.Json
{
    internal class HttpMethodConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(string);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => new HttpMethod((string)reader.Value);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var httpMethod = (HttpMethod)value;
            serializer.Serialize(writer, httpMethod.Method);
        }
    }
}
