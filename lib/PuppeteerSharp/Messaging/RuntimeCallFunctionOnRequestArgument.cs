using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class RuntimeCallFunctionOnRequestArgument
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object Value { get; set; }
    }
}
