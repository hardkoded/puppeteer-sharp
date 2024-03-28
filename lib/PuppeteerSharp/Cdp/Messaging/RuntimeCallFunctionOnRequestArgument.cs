using Newtonsoft.Json;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class RuntimeCallFunctionOnRequestArgument
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object Value { get; set; }
    }
}
