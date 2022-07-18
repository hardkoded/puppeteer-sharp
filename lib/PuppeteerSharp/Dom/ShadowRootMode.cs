using System.Runtime.Serialization;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// List of possible shadow root mode states.
    /// </summary>
    public enum ShadowRootMode : byte
    {
        /// <summary>
        /// Specifies open encapsulation mode.
        /// </summary>
        [EnumMember(Value = "open")]
        Open = 0,
        /// <summary>
        /// Specifies closed encapsulation mode.
        /// </summary>
        [EnumMember(Value = "closed")]
        Closed = 1
    }
}
