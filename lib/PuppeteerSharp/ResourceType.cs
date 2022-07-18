using System.Runtime.Serialization;
using CefSharp.DevTools.Dom.Helpers.Json;
using Newtonsoft.Json;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// Resource type.
    /// </summary>
    /// <seealso cref="Request.ResourceType"/>
    [JsonConverter(typeof(FlexibleStringEnumConverter), Unknown)]
    public enum ResourceType
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = -1,
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
        /// Ping.
        /// </summary>
        Ping,
        /// <summary>
        /// Image.
        /// </summary>
        Img,
        /// <summary>
        /// Other.
        /// </summary>
        Other
    }
}
