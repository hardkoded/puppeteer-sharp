using Newtonsoft.Json;

namespace CefSharp.DevTools.Dom.Messaging
{
    internal class RuntimeCallFunctionOnRequestArgument
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object Value { get; set; }
    }
}
