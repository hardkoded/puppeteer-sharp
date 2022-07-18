namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLOptionElement interface represents option elements and inherits all properties and methods of the <see cref="HtmlElement"/> class.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLOptionElement" />
    public partial class HtmlOptionElement : HtmlElement
    {
        internal HtmlOptionElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }
}
