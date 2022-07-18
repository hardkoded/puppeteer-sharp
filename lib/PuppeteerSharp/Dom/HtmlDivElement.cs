using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLDivElement interface provides special properties
    /// (beyond the regular HTMLElement interface it also has available to it by inheritance) for manipulating div elements.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLDivElement" />
    public partial class HtmlDivElement : HtmlElement
    {
        internal HtmlDivElement(string className, JSHandle jsHandle) : base(className, jsHandle)
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
