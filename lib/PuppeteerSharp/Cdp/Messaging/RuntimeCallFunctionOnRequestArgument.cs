using System.Text.Json.Serialization;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class RuntimeCallFunctionOnRequestArgument
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public object Value { get; set; }
    }
}
