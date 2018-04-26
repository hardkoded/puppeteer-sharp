using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum ResourceType
    {
        Document,
        [EnumMember(Value = "stylesheet")]
        StyleSheet,
        Image,
        Media,
        Font,
        Script,
        [EnumMember(Value = "texttrack")]
        TextTrack,
        Xhr,
        Fetch,
        [EnumMember(Value = "eventsource")]
        EventSource,
        [EnumMember(Value = "websocket")]
        WebSocket,
        Manifest,
        Other
    }
}
