using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLParagraphElement interface provides special properties
    /// (beyond those of the regular HTMLElement object interface it inherits) for manipulating p elements.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLParagraphElement" />
    public partial class HtmlParagraphElement : HtmlElement
    {
        internal HtmlParagraphElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Gets the align property
        /// </summary>
        /// <returns><see cref="HtmlElementAlignType"/></returns>
        public Task<HtmlElementAlignType> GetAlignAsync()
        {
            return EvaluateFunctionInternalAsync<HtmlElementAlignType>("(element) => { return element.align; }");
        }

        /// <summary>
        /// Sets the align property
        /// </summary>
        /// <param name="align">align</param>
        /// <returns>A Task that when awaited sets the align property</returns>
        public Task SetAlignAsync(HtmlElementAlignType align)
        {
            return SetPropertyValueAsync("align", align);
        }
    }
}
