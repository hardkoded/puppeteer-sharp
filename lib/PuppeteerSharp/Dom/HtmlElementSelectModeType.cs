using System.ComponentModel;
using System.Runtime.Serialization;
using CefSharp.DevTools.Dom.Converters;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// HtmlElementSelectMode Type
    /// </summary>
    [TypeConverter(typeof(StringToEnumTypeConverter))]
    public enum HtmlElementSelectModeType
    {
        /// <summary>
        /// selects the newly inserted text.
        /// </summary>
        [EnumMember(Value = "select")]
        Select,
        /// <summary>
        /// moves the selection to just before the inserted text.
        /// </summary>
        [EnumMember(Value = "start")]
        Start,
        /// <summary>
        /// moves the selection to just after the inserted text.
        /// </summary>
        [EnumMember(Value = "end")]
        End,
        /// <summary>
        /// attempts to preserve the selection. This is the default.
        /// </summary>
        [EnumMember(Value = "preserve")]
        Preserve
    }
}
