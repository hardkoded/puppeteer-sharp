using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLImageElement interface represents an HTML img element,
    /// providing the properties and methods used to manipulate image elements.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLImageElement" />
    public partial class HtmlImageElement : HtmlElement
    {
        internal HtmlImageElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Gets the (alternate) text to display when the image specified by the img element is not loaded.
        /// </summary>
        /// <param name="alt">alt text</param>
        /// <returns>A Task that when awaited sets the alt property</returns>
        /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLImageElement/alt"/>
        public Task SetDisabledAsync(string alt)
        {
            return SetPropertyValueAsync("alt", alt);
        }

        /// <summary>
        /// Returns a boolean value that is true if the browser has finished fetching the image, whether successful or not.
        /// That means this value is also true if the image has no src value indicating an image to load.
        /// </summary>
        /// <returns>bool</returns>
        public Task<bool> GetCompleteAsync()
        {
            return EvaluateFunctionInternalAsync<bool>("(element) => { return element.complete; }");
        }

        /// <summary>
        /// Sets a string that corresponds to the CORS setting for this link element.
        /// </summary>
        /// <param name="crossOrigin">Is a DOMString reflecting the CORS setting</param>
        /// <returns>A Task that when awaited sets the crossOrigin property</returns>
        /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Attributes/crossorigin" />
        public Task SetCrossOriginnAsync(string crossOrigin)
        {
            return SetPropertyValueAsync("crossOrigin", crossOrigin);
        }

        /// <summary>
        /// Gets the horizontal offset of the left border edge of the image's
        /// CSS layout box relative to the origin of the html element's containing block.
        /// </summary>
        /// <returns>int</returns>
        public Task<int> GetXAsync()
        {
            return EvaluateFunctionInternalAsync<int>("(element) => { return element.x; }");
        }

        /// <summary>
        /// Gets the vertical offset of the top border edge of the image's
        /// CSS layout box relative to the origin of the html element's containing block.
        /// </summary>
        /// <returns>int</returns>
        public Task<int> GetYAsync()
        {
            return EvaluateFunctionInternalAsync<int>("(element) => { return element.y; }");
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

        /// <summary>
        /// Returns a Task that resolves when the image is decoded and it's safe to append the image to the DOM.
        /// This prevents rendering of the next frame from having to pause to decode the image, as would happen if an undecoded image were added to the DOM.
        /// </summary>
        /// <returns>Returns a Task that resolves when the image is decoded and it's safe to append the image to the DOM.</returns>
        /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLImageElement/decode"/>
        public Task DecodeAsync()
        {
            return EvaluateFunctionInternalAsync("(e) => e.decode()");
        }
    }
}
