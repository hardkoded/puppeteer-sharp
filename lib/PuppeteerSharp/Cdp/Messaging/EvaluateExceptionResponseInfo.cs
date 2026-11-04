using System.Text.Json;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class EvaluateExceptionResponseInfo
    {
        public string Description { get; set; }

        public RemoteObjectType Type { get; set; }

        public RemoteObjectSubtype Subtype { get; set; }

        public string ObjectId { get; set; }

        [JsonConverter(typeof(AnyTypeToStringConverter))]
        public string Value { get; set; }
    }
}
