using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    public class PageConsoleResponse
    {
        [JsonProperty("type")]
        public ConsoleType Type { get; set; }
        
        [JsonProperty("args")]
        public dynamic[] Args { get; set; }
        
        [JsonProperty("executionContextId")]
        public int ExecutionContextId { get; set; }
    }
}
