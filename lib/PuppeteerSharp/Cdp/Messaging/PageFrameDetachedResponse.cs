using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp.Messaging
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter<FrameDetachedReason>))]
    internal enum FrameDetachedReason
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Remove.
        /// </summary>
        Remove,

        /// <summary>
        /// Swap.
        /// </summary>
        Swap,
    }

    internal class PageFrameDetachedResponse : BasicFrameResponse
    {
        public FrameDetachedReason Reason { get; set; }
    }
}
