using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PuppeteerSharp.Messaging
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum MouseEventType
    {
        [EnumMember(Value = "mouseMoved")]
        MouseMoved,
        [EnumMember(Value = "mousePressed")]
        MousePressed,
        [EnumMember(Value = "mouseReleased")]
        MouseReleased,
        [EnumMember(Value = "mouseWheel")]
        MouseWheel
    }
}
