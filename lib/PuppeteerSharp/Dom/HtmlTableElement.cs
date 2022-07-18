using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLTableElement interface provides special properties and methods (beyond the regular HTMLElement object interface it also has available to it by inheritance)
    /// for manipulating the layout and presentation of tables in an HTML document.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableElement" />
    public partial class HtmlTableElement : HtmlElement
    {
        internal HtmlTableElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Is a HTMLTableSectionElement representing the first thead that is a child of the element, or null if none is found.
        /// When set, if the object doesn't represent a thead, a DOMException with the HierarchyRequestError name is thrown.
        /// If a correct object is given, it is inserted in the tree immediately before the first element that is neither a caption,
        /// nor a colgroup, or as the last child if there is no such element, and the first thead that is a child of this element is removed from the tree, if any.
        /// </summary>
        /// <returns>Task</returns>
        public Task<HtmlTableSectionElement> GetTHeadAsync()
        {
            return GetHeadAsync();
        }

        /// <summary>
        /// Is a HTMLTableSectionElement representing the first thead that is a child of the element, or null if none is found.
        /// When set, if the object doesn't represent a thead, a DOMException with the HierarchyRequestError name is thrown.
        /// If a correct object is given, it is inserted in the tree immediately before the first element that is neither a caption,
        /// nor a colgroup, or as the last child if there is no such element, and the first thead that is a child of this element is removed from the tree, if any.
        /// </summary>
        /// <returns>Task</returns>
        public Task<HtmlTableSectionElement> GetTFootAsync()
        {
            return GetFootAsync();
        }

        /// <summary>
        /// Returns a live HTMLCollection containing all the tbody of the element. The HTMLCollection is live and is automatically updated when the HTMLTableElement changes.
        /// </summary>
        /// <returns>HTMLCollection</returns>
        public Task<HtmlCollection<HtmlTableSectionElement>> GetTBodiesAsync()
        {
            return EvaluateFunctionHandleInternalAsync<HtmlCollection<HtmlTableSectionElement>>("e => e.tBodies");
        }

        /// <summary>
        /// Returns the first <see cref="HtmlTableSectionElement"/> in the tBodies HTMLCollection.
        /// To get all the bodies use <see cref="GetTBodiesAsync"/> instead.
        /// </summary>
        /// <returns>HtmlTableSectionElement</returns>
        public Task<HtmlTableSectionElement> GetBodyAsync()
        {
            return EvaluateFunctionHandleInternalAsync<HtmlTableSectionElement>("e => e.tBodies[0]");
        }

        /// <summary>
        /// Returns an HTMLTableRowElement representing a new row of the table. The new cell is appended to the collection.
        /// </summary>
        /// <returns>A Task that when awaited inserts the cell at the specified index</returns>
        public Task<HtmlTableRowElement> InsertRowAsync()
        {
            return InsertRowAsync(-1);
        }
    }
}
