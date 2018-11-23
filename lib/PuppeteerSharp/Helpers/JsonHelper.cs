using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PuppeteerSharp.Helpers
{
    internal static class JsonHelper
    {
        public static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }
}
