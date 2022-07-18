namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLButtonElement interface provides properties and methods
    /// (beyond the regular HTMLElement interface it also has available to it by inheritance)
    /// for manipulating button elements.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLButtonElement" />
    public partial class HtmlButtonElement : HtmlElement
    {
        internal HtmlButtonElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }
}
