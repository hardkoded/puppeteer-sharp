using System;
using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLTableRowElement interface provides special properties and methods (beyond the HTMLElement interface it also has available to it by inheritance)
    /// for manipulating the layout and presentation of rows in an HTML table.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableRowElement" />
    public partial class HtmlTableRowElement : HtmlElement
    {
        internal HtmlTableRowElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Returns a long value which gives the logical position of the row within the entire table. If the row is not part of a table, returns -1.
        /// </summary>
        /// <returns>Index</returns>
        public Task<int> GetRowIndexAsync()
        {
            return GetIndexAsync();
        }

        /// <summary>
        /// Returns a value which gives the logical position of the row within the table section it belongs to. If the row is not part of a section, returns -1.
        /// </summary>
        /// <returns>Index</returns>
        public Task<int> GetSectionRowIndexAsync()
        {
            return GetIndexInSectionAsync();
        }

        /// <summary>
        /// Removes the cell. Equivilent to calling cellIndex and deleteCell
        /// </summary>
        /// <param name="cell">Cell to delete</param>
        /// <returns>A Task that when awaited deletes the cell</returns>
        public async Task DeleteCellAsync(HtmlTableCellElement cell)
        {
            if (cell == null)
            {
                throw new ArgumentNullException(nameof(cell));
            }

            var index = await cell.GetCellIndexAsync().ConfigureAwait(false);

            await DeleteCellAsync(index).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns an HTMLTableCellElement representing a new cell of the row.
        /// The cell is inserted in the collection of cells immediately before the given index position in the row.
        /// If index is -1, the new cell is appended to the collection.
        /// If index is less than -1 or greater than the number of cells in the collection, a DOMException with the value IndexSizeError is raised.
        /// </summary>
        /// <param name="index">
        /// The cell is inserted in the collection of cells immediately before the given index position in the row.
        /// If index is -1, the new cell is appended to the collection.
        /// If index is less than -1 or greater than the number of cells in the collection, a DOMException with the value IndexSizeError is raised.
        /// </param>
        /// <returns>A Task that when awaited inserts the cell at the specified index</returns>
        public Task<HtmlTableCellElement> InsertCellAsync(int index = -1)
        {
            return EvaluateFunctionHandleInternalAsync<HtmlTableCellElement>("(e, index) => e.insertCell(index)", index);
        }

        /// <summary>
        /// Returns an HTMLTableCellElement representing a new cell of the row.
        /// The cell is inserted in the collection of cells immediately before the given index position in the row.
        /// If index is -1, the new cell is appended to the collection.
        /// If index is less than -1 or greater than the number of cells in the collection, a DOMException with the value IndexSizeError is raised.
        /// </summary>
        /// <param name="index">
        /// The cell is inserted in the collection of cells immediately before the given index position in the row.
        /// If index is -1, the new cell is appended to the collection.
        /// If index is less than -1 or greater than the number of cells in the collection, a DOMException with the value IndexSizeError is raised.
        /// </param>
        /// <param name="text">Text that will be set as the innerText of the newly created element</param>
        /// <returns>A Task that when awaited inserts the cell at the specified index</returns>
        public async Task<HtmlTableCellElement> InsertCellAsync(int index, string text)
        {
            var cell = await InsertCellAsync(index).ConfigureAwait(false);

            await cell.SetInnerTextAsync(text).ConfigureAwait(false);

            return cell;
        }
    }
}
