using Newtonsoft.Json;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Messaging
{
    [JsonConverter(typeof(FlexibleStringEnumConverter), Unknown)]
    internal enum FrameDetachedReason
    {
        Unknown = -1,
        Remove,
        Swap,
    }

    internal class PageFrameDetachedResponse : BasicFrameResponse
    {
        public FrameDetachedReason Reason { get; set; }
    }
}
