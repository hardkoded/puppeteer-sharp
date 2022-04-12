using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CefSharp.Puppeteer.Messaging
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
