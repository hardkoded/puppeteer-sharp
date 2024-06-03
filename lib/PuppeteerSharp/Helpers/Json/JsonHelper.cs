using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json
{
    internal static class JsonHelper
    {
        public static readonly JsonSerializerOptions DefaultJsonSerializerSettings = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JSHandleConverter(),
                new JsonStringEnumMemberConverter(),
            },
        };
    }
}
