using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLTableCellElement interface provides special properties and methods (beyond the regular HTMLElement interface it also has available to it by inheritance)
    /// for manipulating the layout and presentation of table cells, either header or data cells, in an HTML document.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableCellElement" />
    public partial class HtmlTableCellElement : HtmlElement
    {
        internal HtmlTableCellElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Sets a string which can be used on th elements (not on td), specifying an alternative label for the header cell.
        /// This alternate label can be used in other contexts, such as when describing the headers that apply to a data cell.
        /// This is used to offer a shorter term for use by screen readers in particular, and is a valuable accessibility tool.
        /// Usually the value of abbr is an abbreviation or acronym, but can be any text that's appropriate contextually.
        /// </summary>
        /// <param name="abbr">abbr</param>
        /// <returns>A Task that when awaited sets the abbr property</returns>
        public Task SetAbbrAsync(string abbr)
        {
            return SetPropertyValueAsync("abbr", abbr);
        }

        /// <summary>
        /// Gets a string which can be used on th elements (not on td), specifying an alternative label for the header cell.
        /// This alternate label can be used in other contexts, such as when describing the headers that apply to a data cell.
        /// This is used to offer a shorter term for use by screen readers in particular, and is a valuable accessibility tool.
        /// Usually the value of abbr is an abbreviation or acronym, but can be any text that's appropriate contextually.
        /// </summary>
        /// <returns>Task</returns>
        public Task<string> GetAbbrAsync()
        {
            return EvaluateFunctionInternalAsync<string>("(element) => { return element.abbr; }");
        }

        /// <summary>
        /// A integer representing the cell's position in the cells collection of the tr the cell is contained within. If the cell doesn't belong to a tr, it returns -1.
        /// </summary>
        /// <returns>Index</returns>
        public Task<int> GetCellIndexAsync()
        {
            return GetIndexAsync();
        }

        /// <summary>
        /// Sets a string indicating the scope of a th cell. Header cells can be configured, using the scope property, the apply to a specified row or column,
        /// or to the not-yet-scoped cells within the current row group (that is, the same ancestor thead, tbody, or tfoot element).
        /// If no value is specified for scope, the header is not associated directly with cells in this way.
        /// </summary>
        /// <param name="scope">scope</param>
        /// <returns>A Task that when awaited sets the scope property</returns>
        public Task SetScopeAsync(string scope)
        {
            return SetPropertyValueAsync("scope", scope);
        }

        /// <summary>
        /// Gets a string indicating the scope of a th cell. Header cells can be configured, using the scope property, the apply to a specified row or column,
        /// or to the not-yet-scoped cells within the current row group (that is, the same ancestor thead, tbody, or tfoot element).
        /// If no value is specified for scope, the header is not associated directly with cells in this way.
        /// </summary>
        /// <returns>Task</returns>
        public Task<string> GetScopeAsync()
        {
            return EvaluateFunctionInternalAsync<string>("(element) => { return element.scope; }");
        }
    }
}
