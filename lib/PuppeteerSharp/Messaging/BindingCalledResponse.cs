using System.Text.Json;
using System.Text.Json.Nodes;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Messaging
{
    internal class BindingCalledResponse
    {
        private string _payloadJson;

        public int ExecutionContextId { get; set; }

        public BindingCalledResponsePayload BindingPayload { get; set; }

        public string Payload
        {
            get => _payloadJson;
            set
            {
                _payloadJson = value;
                var json = JsonSerializer.Deserialize<JsonObject>(_payloadJson, JsonHelper.DefaultJsonSerializerSettings);
                BindingPayload = json.Deserialize<BindingCalledResponsePayload>();
                BindingPayload.JsonObject = json;
            }
        }

        internal class BindingCalledResponsePayload
        {
            public string Type { get; set; }

            public string Name { get; set; }

            public JsonElement[] Args { get; set; }

            public int Seq { get; set; }

            public JsonObject JsonObject { get; set; }
        }
    }
}
