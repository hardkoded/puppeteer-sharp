using System.Text.Json.Serialization;

namespace PuppeteerSharp.Messaging
{
    internal class RuntimeCallFunctionOnRequestArgument
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object Value { get; set; }
    }
}
