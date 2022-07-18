using System.Runtime.Serialization;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// Enumeration with possible values for the adjacent position insertion.
    /// </summary>
    public enum AdjacentPosition : byte
    {
        /// <summary>
        /// Before the element itself.
        /// </summary>
        [EnumMember(Value = "beforebegin")]
        BeforeBegin,
        /// <summary>
        /// Just inside the element, before its first child.
        /// </summary>
        [EnumMember(Value = "afterbegin")]
        AfterBegin,
        /// <summary>
        /// Just inside the element, after its last child.
        /// </summary>
        [EnumMember(Value = "beforeend")]
        BeforeEnd,
        /// <summary>
        /// After the element itself.
        /// </summary>
        [EnumMember(Value = "afterend")]
        AfterEnd
    }
}
