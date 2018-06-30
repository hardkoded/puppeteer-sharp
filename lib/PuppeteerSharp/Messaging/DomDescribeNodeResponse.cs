using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class DomDescribeNodeResponse
    {
        [JsonProperty("node")]
        public DomNode Node { get; set; }
    }

    internal class DomNode
    {
        [JsonProperty("frameId")]
        public string FrameId { get; set; }
    }
}