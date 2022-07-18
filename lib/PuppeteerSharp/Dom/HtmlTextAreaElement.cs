using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLTextAreaElement interface provides special properties and methods for manipulating the layout and presentation of textarea elements.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLTextAreaElement" />
    public partial class HtmlTextAreaElement : HtmlElement
    {
        internal HtmlTextAreaElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Sets a boolean value reflecting the autofocus HTML attribute, which indicates whether the control
        /// should have input focus when the page loads, unless the user overrides it, for example by typing
        /// in a different control. Only one form-associated element in a document can have this attribute specified.
        /// </summary>
        /// <param name="autoFocus">autoFocus</param>
        /// <returns>A Task that when awaited sets the autofocus property</returns>
        /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLSelectElement/autofocus"/>
        public Task SetAutoFocusAsync(bool autoFocus)
        {
            return SetPropertyValueAsync("autofocus", autoFocus);
        }

        /// <summary>
        /// Returns the element's cols attribute, indicating the visible width of the text area.
        /// </summary>
        /// <returns>int</returns>
        public Task<int> GetColsAsync()
        {
            return GetColumnsAsync();
        }

        /// <summary>
        /// Returns the direction in which selection occurred. This is "forward" if selection was performed
        /// in the start-to-end direction of the current locale, or "backward" for the opposite direction.
        /// This can also be "none" if the direction is unknown.
        /// </summary>
        /// <param name="value">string</param>
        /// <returns>A Task that when awaited sets the value property</returns>
        public Task SetSelectionDirectionAsync(string value)
        {
            return SetPropertyValueAsync("selectionDirection", value);
        }

        /// <summary>
        /// Replaces a range of text in the element with new text.
        /// </summary>
        /// <param name="replacement">replacement text</param>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <param name="selectMode">how the selection should be set after the text has been replaced.</param>
        /// <returns>A Task that when awaited selects the contents of the control</returns>
        public Task SetRangeTextAsync(string replacement, int? start = null, int? end = null, HtmlElementSelectModeType? selectMode = null)
        {
            return EvaluateFunctionInternalAsync("(e, replacement, start, end, selectMode) => e.setRangeText(replacement, start, end, selectMode)", replacement, start, end, selectMode);
        }

        /// <summary>
        /// Selects a range of text in the element (but does not focus it).
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <param name="selectionDirection ">indicating the direction in which the selection is considered to have been performed.</param>
        /// <returns>A Task that when awaited sets the start and end positions of the current text selection</returns>
        public Task SetSelectionRangeAsync(int start, int end, HtmlElementSelectionDirectionType? selectionDirection = null)
        {
            return EvaluateFunctionInternalAsync("(e, start, end, selectMode) => e.setSelectionRange(start, end, selectMode)", start, end, selectionDirection);
        }
    }
}
