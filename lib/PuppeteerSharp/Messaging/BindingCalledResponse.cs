using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    internal class BindingCalledResponse
    {
        private string _payloadJson;

        [JsonProperty(Constants.EXECUTION_CONTEXT_ID)]
        public int ExecutionContextId { get; set; }
        public BindingPayload Payload { get; set; }
        [JsonProperty(Constants.PAYLOAD)]
        public string PayloadJson
        {
            get => _payloadJson;
            set
            {
                _payloadJson = value;
                var json = JsonConvert.DeserializeObject(_payloadJson) as JObject;
                Payload = json.ToObject<BindingPayload>();
                Payload.JsonObject = json;
            }
        }

        public class BindingPayload
        {
            [JsonProperty(Constants.NAME)]
            public string Name { get; set; }
            [JsonProperty(Constants.ARGS)]
            public object[] Args { get; set; }
            [JsonProperty(Constants.SEQ)]
            public int Seq { get; set; }

            public JObject JsonObject { get; set; }
        }
    }
}