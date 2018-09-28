using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class DomDescribeNodeResponse
    {
        [JsonProperty(Constants.NODE)]
        public DomNode Node { get; set; }

        internal class DomNode
        {
            [JsonProperty(Constants.FRAME_ID)]
            public string FrameId { get; set; }
        }
    }
}