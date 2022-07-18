using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLAnchorElement interface represents hyperlink elements and provides special properties and methods
    /// (beyond those of the regular HTMLElement object interface that they inherit from) for manipulating the
    /// layout and presentation of such elements. This interface corresponds to a element; not to be confused with link, which is represented by HTMLLinkElement)
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLAnchorElement" />
    public partial class HtmlAnchorElement : HtmlElement
    {
        internal HtmlAnchorElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Sets string representing the MIME type of the linked resource.
        /// </summary>
        /// <param name="type">string</param>
        /// <returns>A Task that when awaited sets the type property</returns>
        public Task SetTypeAsync(string type)
        {
            return SetPropertyValueAsync("type", type);
        }

        /// <summary>
        /// Sets the text content of the element.
        /// </summary>
        /// <param name="text">string</param>
        /// <returns>A Task that when awaited sets the text property</returns>
        public Task SetTextAsync(string text)
        {
            return SetPropertyValueAsync("text", text);
        }
    }
}
