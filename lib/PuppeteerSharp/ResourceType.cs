using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    /// <summary>
    /// Resource type.
    /// </summary>
    /// <seealso cref="Request.ResourceType"/>
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum ResourceType
    {
        /// <summary>
        /// Document.
        /// </summary>
        Document,
        /// <summary>
        /// Stylesheet.
        /// </summary>
        [EnumMember(Value = "stylesheet")]
        StyleSheet,
        /// <summary>
        /// Image.
        /// </summary>
        Image,
        /// <summary>
        /// Media.
        /// </summary>
        Media,
        /// <summary>
        /// Font.
        /// </summary>
        Font,
        /// <summary>
        /// Script.
        /// </summary>
        Script,
        /// <summary>
        /// Texttrack.
        /// </summary>
        [EnumMember(Value = "texttrack")]
        TextTrack,
        /// <summary>
        /// XHR.
        /// </summary>
        Xhr,
        /// <summary>
        /// Fetch.
        /// </summary>
        Fetch,
        /// <summary>
        /// Event source.
        /// </summary>
        [EnumMember(Value = "eventsource")]
        EventSource,
        /// <summary>
        /// Web Socket.
        /// </summary>
        [EnumMember(Value = "websocket")]
        WebSocket,
        /// <summary>
        /// Manifest.
        /// </summary>
        Manifest,
        /// <summary>
        /// Other.
        /// </summary>
        Other
    }
}
