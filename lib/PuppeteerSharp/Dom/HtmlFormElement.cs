namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLFormElement interface represents a form element in the DOM.
    /// It allows access to—and, in some cases, modification of—aspects
    /// of the form, as well as access to its component elements.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement" />
    public partial class HtmlFormElement : HtmlElement
    {
        internal HtmlFormElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }
}
