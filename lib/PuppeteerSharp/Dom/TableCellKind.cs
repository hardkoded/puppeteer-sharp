using System.Runtime.Serialization;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// Represents kinds of table cells.
    /// </summary>
    public enum TableCellKind
    {
        /// <summary>
        /// Td table cell.
        /// </summary>
        [EnumMember(Value = "td")]
        Td,
        /// <summary>
        /// Th table cell.
        /// </summary>
        [EnumMember(Value = "th")]
        Th,
    }
}
