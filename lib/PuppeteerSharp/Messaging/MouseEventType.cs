using System.Runtime.Serialization;

namespace PuppeteerSharp.Messaging
{
    internal enum MouseEventType
    {
        [EnumMember(Value = "mouseMoved")]
        MouseMoved,
        [EnumMember(Value = "mousePressed")]
        MousePressed,
        [EnumMember(Value = "mouseReleased")]
        MouseReleased
    }
}