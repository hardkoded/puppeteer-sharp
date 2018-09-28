using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageConsoleResponse
    {
        [JsonProperty(Constants.TYPE)]
        internal ConsoleType Type { get; set; }

        [JsonProperty(Constants.ARGS)]
        internal dynamic[] Args { get; set; }

        [JsonProperty(Constants.EXECUTION_CONTEXT_ID)]
        internal int ExecutionContextId { get; set; }
    }
}
