namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLSpanElement interface represents a span element and derives from the <see cref="HtmlElement"/> interface,
    /// but without implementing any additional properties or methods.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLSpanElement" />
    public partial class HtmlSpanElement : HtmlElement
    {
        internal HtmlSpanElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }
}
