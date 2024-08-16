using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class EvaluateExceptionResponseInfo
    {
        public string Description { get; set; }

        [JsonConverter(typeof(AnyTypeToStringConverter))]
        public string Value { get; set; }
    }
}
