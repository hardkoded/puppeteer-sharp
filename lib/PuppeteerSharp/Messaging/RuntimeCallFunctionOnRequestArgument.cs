using Newtonsoft.Json;

namespace CefSharp.Puppeteer.Messaging
{
    internal class RuntimeCallFunctionOnRequestArgument
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object Value { get; set; }
    }
}
