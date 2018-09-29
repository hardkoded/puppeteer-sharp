using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageConsoleResponse
    {
        [JsonProperty("type")]
        internal ConsoleType Type { get; set; }

        [JsonProperty("args")]
        internal dynamic[] Args { get; set; }

        [JsonProperty("executionContextId")]
        internal int ExecutionContextId { get; set; }
    }
}
