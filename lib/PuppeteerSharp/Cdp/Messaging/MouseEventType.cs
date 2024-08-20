using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp.Messaging
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter<MouseEventType>))]
    internal enum MouseEventType
    {
        /// <summary>
        /// Mouse moved.
        /// </summary>
        [EnumMember(Value = "mouseMoved")]
        MouseMoved,

        /// <summary>
        /// Mouse clicked.
        /// </summary>
        [EnumMember(Value = "mousePressed")]
        MousePressed,

        /// <summary>
        /// Mouse click released.
        /// </summary>
        [EnumMember(Value = "mouseReleased")]
        MouseReleased,

        /// <summary>
        /// Mouse wheel.
        /// </summary>
        [EnumMember(Value = "mouseWheel")]
        MouseWheel,
    }
}
