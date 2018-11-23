using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PuppeteerSharp.Helpers;

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
                var json = JsonConvert.DeserializeObject(_payloadJson, JsonHelper.DefaultJsonSerializerSettings) as JObject;
                BindingPayload = json.ToObject<BindingCalledResponsePayload>(true);
                BindingPayload.JsonObject = json;
            }
        }

        public class BindingCalledResponsePayload
        {
            public string Name { get; set; }
            public object[] Args { get; set; }
            public int Seq { get; set; }

            public JObject JsonObject { get; set; }
        }
    }
}