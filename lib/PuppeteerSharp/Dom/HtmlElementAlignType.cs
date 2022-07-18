using System.Runtime.Serialization;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// Indicates alignment of the element's contents with respect to the surrounding context.
    /// </summary>
    public enum HtmlElementAlignType
    {
        /// <summary>
        /// Left
        /// </summary>
        [EnumMember(Value = "left")]
        Left,
        /// <summary>
        /// Right
        /// </summary>
        [EnumMember(Value = "right")]
        Right,
        /// <summary>
        /// Justify
        /// </summary>
        [EnumMember(Value = "justify")]
        Justify,
        /// <summary>
        /// Center
        /// </summary>
        [EnumMember(Value = "center")]
        Center
    }
}
