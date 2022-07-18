using System.Collections.Generic;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom.Input;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// Inherits from <see cref="Element"/>. It represents an in-page DOM element.
    /// HtmlElement can be created by <see cref="DevToolsContext.QuerySelectorAsync{T}(string)"/> or <see cref="DevToolsContext.QuerySelectorAllAsync{T}(string)"/>.
    /// </summary>
    public partial class HtmlElement
        : Element
    {
        internal HtmlElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Calls <c>focus</c> <see href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/focus"/> on the element.
        /// </summary>
        /// <param name="preventScroll">
        /// A Boolean value indicating whether or not the browser should scroll the document to bring the newly-focused element
        /// into view. A value of false for preventScroll (the default) means that the browser will scroll the element into view
        /// after focusing it. If preventScroll is set to true, no scrolling will occur.
        /// </param>
        /// <returns>Task</returns>
        public Task FocusAsync(bool preventScroll) => EvaluateFunctionInternalAsync("(e, prevent) => e.focus({preventScroll:prevent})", preventScroll);

        /// <summary>
        /// Focuses the element, and sends a <c>keydown</c>, <c>keypress</c>/<c>input</c>, and <c>keyup</c> event for each character in the text.
        /// </summary>
        /// <param name="text">A text to type into a focused element</param>
        /// <param name="options">type options</param>
        /// <remarks>
        /// To press a special key, like <c>Control</c> or <c>ArrowDown</c> use <see cref="HtmlElement.PressAsync(string, PressOptions)"/>
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// elementHandle.TypeAsync("Hello"); // Types instantly
        /// elementHandle.TypeAsync("World", new TypeOptions { Delay = 100 }); // Types slower, like a user
        /// ]]>
        /// </code>
        /// An example of typing into a text field and then submitting the form:
        /// <code>
        /// <![CDATA[
        /// var elementHandle = await Page.QuerySelectorAsync("input");
        /// await elementHandle.TypeAsync("some text");
        /// await elementHandle.PressAsync("Enter");
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>Task</returns>
        public Task TypeAsync(string text, TypeOptions options = null)
        {
            var elementHandle = Handle as ElementHandle;

            if (elementHandle == null)
            {
                throw new PuppeteerException("Unable to convert to ElementHandle");
            }

            return elementHandle.TypeAsync(text, options);
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="Touchscreen.TapAsync(decimal, decimal)"/> to tap in the center of the element.
        /// </summary>
        /// <exception cref="PuppeteerException">if the element is detached from DOM</exception>
        /// <returns>Task which resolves when the element is successfully tapped</returns>
        public Task TapAsync()
        {
            var elementHandle = Handle as ElementHandle;

            if (elementHandle == null)
            {
                throw new PuppeteerException("Unable to convert to ElementHandle");
            }

            return elementHandle.TapAsync();
        }

        /// <summary>
        /// Focuses the element, and then uses <see cref="Keyboard.DownAsync(string, DownOptions)"/> and <see cref="Keyboard.UpAsync(string)"/>.
        /// </summary>
        /// <param name="key">Name of key to press, such as <c>ArrowLeft</c>. See <see cref="KeyDefinitions"/> for a list of all key names.</param>
        /// <param name="options">press options</param>
        /// <remarks>
        /// If <c>key</c> is a single character and no modifier keys besides <c>Shift</c> are being held down, a <c>keypress</c>/<c>input</c> event will also be generated. The <see cref="DownOptions.Text"/> option can be specified to force an input event to be generated.
        /// </remarks>
        /// <returns></returns>
        public Task PressAsync(string key, PressOptions options = null)
        {
            var elementHandle = Handle as ElementHandle;

            if (elementHandle == null)
            {
                throw new PuppeteerException("Unable to convert to ElementHandle");
            }

            return elementHandle.PressAsync(key, options);
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="DevToolsContext.Mouse"/> to hover over the center of the element.
        /// </summary>
        /// <returns>Task which resolves when the element is successfully hovered</returns>
        public Task HoverAsync()
        {
            var elementHandle = Handle as ElementHandle;

            if (elementHandle == null)
            {
                throw new PuppeteerException("Unable to convert to ElementHandle");
            }
            return elementHandle.HoverAsync();
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="DevToolsContext.Mouse"/> to click in the center of the element.
        /// </summary>
        /// <param name="options">click options</param>
        /// <exception cref="PuppeteerException">if the element is detached from DOM</exception>
        /// <returns>Task which resolves when the element is successfully clicked</returns>
        public Task ClickAsync(ClickOptions options = null)
        {
            var elementHandle = Handle as ElementHandle;

            if (elementHandle == null)
            {
                throw new PuppeteerException("Unable to convert to ElementHandle");
            }

            return elementHandle.ClickAsync(options);
        }

        /// <summary>
        /// Invokes a member function (method).
        /// </summary>
        /// <param name="memberFunctionName">case sensitive member function name</param>
        /// <returns>Task which resolves when member (method).</returns>
        public Task InvokeMemberAsync(string memberFunctionName)
        {
            return EvaluateFunctionInternalAsync($"(element) => element.{memberFunctionName}()");
        }

        /// <summary>
        /// Evaluates the XPath expression relative to the elementHandle. If there's no such element, the method will resolve to <c>null</c>.
        /// </summary>
        /// <param name="expression">Expression to evaluate <see href="https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate"/></param>
        /// <returns>Task which resolves to an array of <see cref="HtmlElement"/></returns>
        public async Task<HtmlElement[]> XPathAsync(string expression)
        {
            var arrayHandle = await Handle.EvaluateFunctionHandleAsync(
                @"(element, expression) => {
                    const document = element.ownerDocument || element;
                    const iterator = document.evaluate(expression, element, null, XPathResult.ORDERED_NODE_ITERATOR_TYPE);
                    const array = [];
                    let item;
                    while ((item = iterator.iterateNext()))
                        array.push(item);
                    return array;
                }",
                expression).ConfigureAwait(false);

            if (arrayHandle == null)
            {
                return default;
            }

            var properties = await arrayHandle.GetPropertiesAsync().ConfigureAwait(false);
            await arrayHandle.DisposeAsync().ConfigureAwait(false);

            var list = new List<HtmlElement>();

            foreach (var jsHandle in properties.Values)
{
                var obj = jsHandle.ToDomHandle<HtmlElement>();

                if (obj != null)
                {
                    list.Add(obj);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// This method returns the bounding box of the element (relative to the main frame),
        /// or null if the element is not visible.
        /// </summary>
        /// <returns>The BoundingBox task.</returns>
        public Task<BoundingBox> BoundingBoxAsync()
        {
            var elementHandle = Handle as ElementHandle;

            if (elementHandle == null)
            {
                throw new PuppeteerException("Unable to convert to ElementHandle");
            }

            return elementHandle.BoundingBoxAsync();
        }

        /// <summary>
        /// returns boxes of the element, or <c>null</c> if the element is not visible. Box points are sorted clock-wise.
        /// </summary>
        /// <returns>Task BoxModel task.</returns>
        public Task<BoxModel> BoxModelAsync()
        {
            var elementHandle = Handle as ElementHandle;

            if (elementHandle == null)
            {
                throw new PuppeteerException("Unable to convert to ElementHandle");
            }

            return elementHandle.BoxModelAsync();
        }

        /// <summary>
        /// Content frame for element handles referencing iframe nodes, or null otherwise.
        /// </summary>
        /// <returns>Resolves to the content frame</returns>
        public Task<Frame> ContentFrameAsync()
        {
            var elementHandle = Handle as ElementHandle;

            if (elementHandle == null)
            {
                throw new PuppeteerException("Unable to convert to ElementHandle");
            }

            return elementHandle.ContentFrameAsync();
        }

        /// <summary>
        /// Evaluates if the element is visible in the current viewport.
        /// </summary>
        /// <returns>A task which resolves to true if the element is visible in the current viewport.</returns>
        public Task<bool> IsIntersectingViewportAsync()
        {
            var elementHandle = Handle as ElementHandle;

            if (elementHandle == null)
            {
                throw new PuppeteerException("Unable to convert to ElementHandle");
            }

            return elementHandle.IsIntersectingViewportAsync();
        }

        /// <summary>
        /// Set DOM Element Property. e.g innerText
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <param name="val">value</param>
        /// <returns>Task</returns>
        internal Task SetPropertyValueAsync(string propertyName, object val)
        {
            return EvaluateFunctionInternalAsync("(element, v) => { element." + propertyName + " = v; }", val);
        }

        /// <summary>
        /// The outerText property of the HTMLElement interface returns the same value as HTMLElement.innerText.
        /// </summary>
        /// <returns>The rendered text content of a node and its descendants.</returns>
        /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/outerText"/>
        public Task<string> GetOuterTextAsync()
        {
            return EvaluateFunctionInternalAsync<string>("(element) => { return element.outerText; }");
        }

        /// <summary>
        /// The innerText property of the HTMLElement interface represents the rendered text content of a node and its descendants.
        /// As a getter, it approximates the text the user would get if they highlighted the contents of the element with the cursor and then copied it to the clipboard.
        /// </summary>
        /// <returns>The rendered text content of a node and its descendants.</returns>
        /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/innerText"/>
        public Task<string> GetInnerTextAsync()
        {
            return EvaluateFunctionInternalAsync<string>("(element) => { return element.innerText; }");
        }

        /// <summary>
        /// Sets the innerText property of the HTMLElement.
        /// As a setter this will replace the element's children with the given value, converting any line breaks into br elements.
        /// </summary>
        /// <param name="innerText">inner Text</param>
        /// <returns>A Task that when awaited sets the innerText</returns>
        /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/innerText"/>
        public Task SetInnerTextAsync(string innerText)
        {
            return SetPropertyValueAsync("innerText", innerText);
        }

        /// <summary>
        /// Sets the outerText property of the HTMLElement.
        /// Replaces the whole current node with the given text (this differs from innerText, which replaces the content inside the current node).
        /// </summary>
        /// <param name="outerText">outer Text</param>
        /// <returns>A Task that when awaited sets the innerText</returns>
        /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/outerText"/>
        public Task SetOuterTextAsync(string outerText)
        {
            return SetPropertyValueAsync("outerText", outerText);
        }

        /// <summary>
        /// returns the inline style of the element in the form of a <see cref="CssStyleDeclaration"/>
        /// object that contains a list of all styles properties for that element with values assigned
        /// for the attributes that are defined in the element's inline style attribute.
        /// </summary>
        /// <returns>A Task when awaited returns the inline style of the element.</returns>
        public Task<CssStyleDeclaration> GetStyleAsync()
        {
            return EvaluateFunctionHandleInternalAsync<CssStyleDeclaration>("(element) => { return element.style; }");
        }

        /// <summary>
        /// Adds a node to the end of the list of children
        /// </summary>
        /// <param name="htmlElement">html element</param>
        /// <returns>Task</returns>
        public Task AppendChildAsync(HtmlElement htmlElement)
        {
            return EvaluateFunctionInternalAsync("(e, aChild) => { e.appendChild(aChild); }", htmlElement);
        }

        /// <summary>
        /// Removes a child node from the DOM and returns the removed node.
        /// </summary>
        /// <param name="aChild">A Node that is the child node to be removed from the DOM.</param>
        /// <returns>Task</returns>
        public Task RemoveChildAsync(HtmlElement aChild)
        {
            return EvaluateFunctionInternalAsync("(e, aChild) => { e.removeChild(aChild); }", aChild);
        }

        /// <summary>
        /// Inserts a node before a reference node as a child
        /// </summary>
        /// <param name="newNode">The node to be inserted.</param>
        /// <param name="referenceNode">The node before which newNode is inserted. If this is null, then newNode is inserted at the end of node's child nodes.</param>
        /// <returns>Task</returns>
        public Task InsertBeforeAsync(HtmlElement newNode, HtmlElement referenceNode)
        {
            return EvaluateFunctionHandleInternalAsync<DomHandle>("(e, newNode, referenceNode) => { e.insertBefore(newNode, referenceNode); }", newNode, referenceNode);
        }

        /// <summary>
        /// Replaces a child node within the given (parent) node.
        /// </summary>
        /// <param name="newNode">The new node to replace oldChild</param>
        /// <param name="oldChild">The child to be replaced.</param>
        /// <returns>Task</returns>
        public Task ReplaceChildAsync(HtmlElement newNode, HtmlElement oldChild)
        {
            return EvaluateFunctionHandleInternalAsync<DomHandle>("(e, newNode, oldChild) => { e.replaceChild(newNode, oldChild); }", newNode, oldChild);
        }
    }
}
