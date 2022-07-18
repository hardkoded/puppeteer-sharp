using System.ComponentModel;
using System.Runtime.Serialization;
using CefSharp.DevTools.Dom.Converters;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// HtmlButtonElement Type
    /// </summary>
    [TypeConverter(typeof(StringToEnumTypeConverter))]
    public enum HtmlButtonElementType
    {
        /// <summary>
        /// The button submits the form. This is the default value if the attribute is not specified, or if it is dynamically changed to an empty or invalid value.
        /// </summary>
        [EnumMember(Value = "submit")]
        Submit,
        /// <summary>
        /// The button resets the form.
        /// </summary>
        [EnumMember(Value = "reset")]
        Reset,
        /// <summary>
        /// The button does nothing.
        /// </summary>
        [EnumMember(Value = "button")]
        Button,
        /// <summary>
        /// The button displays a menu.
        /// </summary>
        [EnumMember(Value = "menu")]
        Menu
    }
}
