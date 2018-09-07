using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    internal class BindingCalledResponse
    {
        private string _payloadJson;

        [JsonProperty("executionContextId")]
        public int ExecutionContextId { get; set; }
        public BindingPayload Payload { get; set; }
        [JsonProperty("payload")]
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
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("args")]
            public object[] Args { get; set; }
            [JsonProperty("seq")]
            public int Seq { get; set; }

            public JObject JsonObject { get; set; }
        }
    }
}