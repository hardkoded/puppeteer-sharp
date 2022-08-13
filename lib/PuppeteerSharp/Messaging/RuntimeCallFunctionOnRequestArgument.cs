using Newtonsoft.Json;

namespace CefSharp.Dom.Messaging
{
    internal class RuntimeCallFunctionOnRequestArgument
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object Value { get; set; }
    }
}
