using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1601 // Partial elements should be documented
#nullable enable
    public static partial class HtmlObjectFactory
    {
        internal static T? CreateObject<T>(string className, JSHandle jsHandle)
            where T : DomHandle
        {
            var type = typeof(T);

            switch (className)
            {
                case "CSSStyleDeclaration":
                {
                    return (T)(object)new CssStyleDeclaration(className, jsHandle);
                }
                case "HTMLCollection":
                {
                    const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                    return Activator.CreateInstance(type, flags, null, new object[] { className, jsHandle }, CultureInfo.InvariantCulture) as T;
                }
                case "FileList":
                {
                    return (T)(object)new FileList(className, jsHandle);
                }
                case "File":
                {
                    return (T)(object)new File(className, jsHandle);
                }
            }

            var handle = CreateObjectInternal(className, jsHandle);
            if (handle != null)
            {
                return (T)handle;
            }

            return default;
        }
    }

    public partial class DocumentType
    {
        internal DocumentType(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class Range
    {
        internal Range(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class StyleSheetList
    {
        internal StyleSheetList(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class StringList
    {
        internal StringList(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class ValidityState
    {
        internal ValidityState(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class MediaList
    {
        internal MediaList(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class StyleSheet
    {
        internal StyleSheet(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    /// <summary>
    /// Node partial
    /// </summary>
    public partial class Node
    {
        internal Node(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        ///  <summary>
        ///  Gets an Element that is the parent of this node. If the node has no
        ///  parent, or if that parent is not an Element, this property returns
        ///  null.
        ///  </summary>
        ///  <typeparam name="T">Type</typeparam>
        ///  <returns>Parent Element or null</returns>
        public virtual Task<T?> GetParentElementAsync<T>()
            where T : Element
        {
            return EvaluateFunctionHandleInternalAsync<T?>("(element) => { return element.parentElement; }");
        }
    }

    public partial class UrlUtilities
    {
        internal UrlUtilities(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class NavigatorId
    {
        internal NavigatorId(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class Navigator
    {
        internal Navigator(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class Location
    {
        internal Location(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class History
    {
        internal History(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class Window
    {
        internal Window(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlBaseElement
    {
        internal HtmlBaseElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlBodyElement
    {
        internal HtmlBodyElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlBreakRowElement
    {
        internal HtmlBreakRowElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlDetailsElement
    {
        internal HtmlDetailsElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlDialogElement
    {
        internal HtmlDialogElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlEmbedElement
    {
        internal HtmlEmbedElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlFieldSetElement
    {
        internal HtmlFieldSetElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlHeadElement
    {
        internal HtmlHeadElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlHeadingElement
    {
        internal HtmlHeadingElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlHrElement
    {
        internal HtmlHrElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlInlineFrameElement
    {
        internal HtmlInlineFrameElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlKeygenElement
    {
        internal HtmlKeygenElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlLabelElement
    {
        internal HtmlLabelElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlListItemElement
    {
        internal HtmlListItemElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlAreaElement
    {
        internal HtmlAreaElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlTitleElement
    {
        internal HtmlTitleElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlUnorderedListElement
    {
        internal HtmlUnorderedListElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlUnknownElement
    {
        internal HtmlUnknownElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlTimeElement
    {
        internal HtmlTimeElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlTemplateElement
    {
        internal HtmlTemplateElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlCommandElement
    {
        internal HtmlCommandElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlDataElement
    {
        internal HtmlDataElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlDocument
    {
        internal HtmlDocument(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// The method runs <c>element.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="HtmlElement"/> or derived type</typeparam>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to a <see cref="HtmlElement"/> or derived type pointing to the frame element</returns>
        public async Task<T> QuerySelectorAsync<T>(string selector)
            where T : Element
        {
            var handle = await EvaluateFunctionHandleInternalAsync<T>(
                "(element, selector) => element.querySelector(selector)",
                selector).ConfigureAwait(false);

            return handle;
        }

        /// <summary>
        /// The method runs <c>element.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to <see cref="HtmlElement"/> pointing to the frame element</returns>
        public Task<Element> QuerySelectorAsync(string selector)
        {
            return QuerySelectorAsync<Element>(selector);
        }

        /// <summary>
        /// Runs <c>element.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type derived from <see cref="HtmlElement"/></typeparam>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        public async Task<T[]> QuerySelectorAllAsync<T>(string selector)
            where T : Element
        {
            var arrayHandle = await EvaluateFunctionHandleInternalAsync<DomHandle>(
                "(element, selector) => element.querySelectorAll(selector)",
                selector).ConfigureAwait(false);

            var properties = await arrayHandle.GetArray<T>().ConfigureAwait(false);
            await arrayHandle.DisposeAsync().ConfigureAwait(false);

            return properties.ToArray();
        }

        /// <summary>
        /// Runs <c>element.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        public Task<Element[]> QuerySelectorAllAsync(string selector)
        {
            return QuerySelectorAllAsync<Element>(selector);
        }

        /// <summary>
        /// Evaluates the XPath expression relative to the elementHandle. If there's no such element, the method will resolve to <c>null</c>.
        /// </summary>
        /// <param name="expression">Expression to evaluate <see href="https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate"/></param>
        /// <returns>Task which resolves to an array of <see cref="HtmlElement"/></returns>
        public async Task<Element[]> XPathAsync(string expression)
        {
            var arrayHandle = await EvaluateFunctionHandleInternalAsync<DomHandle>(
                @"(element, expression) => {
                    const document = element.ownerDocument || element;
                    const iterator = document.evaluate(expression, element, null, XPathResult.ORDERED_NODE_ITERATOR_TYPE);
                    const array = [];
                    let item;
                    while ((item = iterator.iterateNext()))
                        array.push(item);
                    return array;
                }",
                this,
                expression).ConfigureAwait(false);
            var properties = await arrayHandle.GetArray<HtmlElement>().ConfigureAwait(false);
            await arrayHandle.DisposeAsync().ConfigureAwait(false);

            return properties.ToArray();
        }
    }

    public partial class HtmlLegendElement
    {
        internal HtmlLegendElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlMapElement
    {
        internal HtmlMapElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlMarqueeElement
    {
        internal HtmlMarqueeElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlHtmlElement
    {
        internal HtmlHtmlElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlMenuItemElement
    {
        internal HtmlMenuItemElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlMetaElement
    {
        internal HtmlMetaElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlMeterElement
    {
        internal HtmlMeterElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlModElement
    {
        internal HtmlModElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlObjectElement
    {
        internal HtmlObjectElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlOrderedListElement
    {
        internal HtmlOrderedListElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlOutputElement
    {
        internal HtmlOutputElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlParamElement
    {
        internal HtmlParamElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlPictureElement
    {
        internal HtmlPictureElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlPreElement
    {
        internal HtmlPreElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlProgressElement
    {
        internal HtmlProgressElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlQuoteElement
    {
        internal HtmlQuoteElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlScriptElement
    {
        internal HtmlScriptElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlSourceElement
    {
        internal HtmlSourceElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlStyleElement
    {
        internal HtmlStyleElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlTableColumnElement
    {
        internal HtmlTableColumnElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlTableDataCellElement
    {
        internal HtmlTableDataCellElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    /// <summary>
    /// Element is the most general base class from which all element objects (i.e. objects that represent elements) in a Document inherit.
    /// It only has methods and properties common to all kinds of elements. More specific classes inherit from Element.
    /// </summary>
    public partial class Element
    {
        internal Element(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Triggers a `change` and `input` event once all the provided options have been selected.
        /// If there's no `select` element matching `selector`, the method throws an exception.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// await handle.SelectAsync("blue"); // single selection
        /// await handle.SelectAsync("red", "green", "blue"); // multiple selections
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="values">Values of options to select. If the `select` has the `multiple` attribute, all values are considered, otherwise only the first one is taken into account.</param>
        /// <returns>A task that resolves to an array of option values that have been successfully selected.</returns>
        public Task<string[]> SelectAsync(params string[] values)
        {
            return EvaluateFunctionInternalAsync<string[]>(
                           @"(element, values) =>
                {
                    if (element.nodeName.toLowerCase() !== 'select')
                        throw new Error('Element is not a <select> element.');

                    const options = Array.from(element.options);
                    element.value = undefined;
                    for (const option of options) {
                        option.selected = values.includes(option.value);
                        if (option.selected && !element.multiple)
                            break;
                    }
                    element.dispatchEvent(new Event('input', { 'bubbles': true }));
                    element.dispatchEvent(new Event('change', { 'bubbles': true }));
                    return options.filter(option => option.selected).map(option => option.value);
                }",
                           new[] { values });
        }

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IDomHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task</returns>
        public Task EvaluateFunctionAsync(string script, params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            return EvaluateFunctionInternalAsync(script, args);
        }

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IDomHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            return EvaluateFunctionInternalAsync<T>(script, args);
        }

        /// <summary>
        /// Get element attribute value
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="attribute">attribute</param>
        /// <returns>Task which resolves to the attributes value.</returns>
        public async Task<T> GetAttributeAsync<T>(string attribute)
        {
            var attr = await Handle.EvaluateFunctionHandleAsync("(element, attr) => element.getAttribute(attr)", attribute).ConfigureAwait(false);

            var val = await attr.JsonValueAsync<T>().ConfigureAwait(false);

            return val;
        }

        /// <summary>
        /// Set element attribute value
        /// </summary>
        /// <param name="attribute">attribute name</param>
        /// <param name="value">attribute value</param>
        /// <returns>Task which resolves when the attribute value has been set.</returns>
        public Task SetAttributeAsync(string attribute, object value)
        {
            return EvaluateFunctionInternalAsync("(element, attr, val) => element.setAttribute(attr, val)", attribute, value);
        }

        /// <summary>
        /// The method runs <c>element.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="HtmlElement"/> or derived type</typeparam>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to a <see cref="HtmlElement"/> or derived type pointing to the frame element</returns>
        public async Task<T> QuerySelectorAsync<T>(string selector)
            where T : Element
        {
            var handle = await EvaluateFunctionHandleInternalAsync<T>(
                "(element, selector) => element.querySelector(selector)",
                selector).ConfigureAwait(false);

            return handle;
        }

        /// <summary>
        /// The method runs <c>element.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to <see cref="HtmlElement"/> pointing to the frame element</returns>
        public Task<Element> QuerySelectorAsync(string selector)
        {
            return QuerySelectorAsync<Element>(selector);
        }

        /// <summary>
        /// Runs <c>element.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type derived from <see cref="HtmlElement"/></typeparam>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        public async Task<T[]> QuerySelectorAllAsync<T>(string selector)
            where T : Element
        {
            var arrayHandle = await EvaluateFunctionHandleInternalAsync<DomHandle>(
                "(element, selector) => element.querySelectorAll(selector)",
                selector).ConfigureAwait(false);

            var properties = await arrayHandle.GetArray<T>().ConfigureAwait(false);
            await arrayHandle.DisposeAsync().ConfigureAwait(false);

            return properties.ToArray();
        }

        /// <summary>
        /// Runs <c>element.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        public Task<Element[]> QuerySelectorAllAsync(string selector)
        {
            return QuerySelectorAllAsync<Element>(selector);
        }

        ///  <summary>
        ///  Inserts nodes just before the current node.
        ///  </summary>
        ///  <param name="nodes">The nodes to insert.</param>
        ///  <returns>Task</returns>
        public virtual Task BeforeAsync(params Node[] nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (nodes.Length == 0)
            {
                throw new ArgumentException("Must specify at least one node.");
            }

            return EvaluateFunctionInternalAsync("(element, nodes) => { element.before(nodes); }", nodes);
        }

        ///  <summary>
        ///  Inserts nodes just after the current node.
        ///  </summary>
        ///  <param name="nodes">The nodes to insert.</param>
        ///  <returns>Task</returns>
        public virtual Task AfterAsync(params Node[] nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (nodes.Length == 0)
            {
                throw new ArgumentException("Must specify at least one node.");
            }

            return EvaluateFunctionInternalAsync("(element, nodes) => { element.after(nodes); }", nodes);
        }

        ///  <summary>
        ///  Replaces the current node with nodes.
        ///  </summary>
        ///  <param name="nodes">The nodes to insert.</param>
        ///  <returns>Task</returns>
        public virtual Task ReplaceAsync(params Node[] nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (nodes.Length == 0)
            {
                throw new ArgumentException("Must specify at least one node.");
            }

            return EvaluateFunctionInternalAsync("(element, nodes) => { element.replace(nodes); }", nodes);
        }

        ///  <summary>
        ///  Removes the current node.
        ///  </summary>
        ///  <returns>Task</returns>
        public virtual Task RemoveAsync()
        {
            return EvaluateFunctionInternalAsync("(element) => { element.remove(); }");
        }

        ///  <summary>
        ///  Appends nodes to current document.
        ///  </summary>
        ///  <param name="nodes">The nodes to append.</param>
        ///  <returns>Task</returns>
        public virtual Task AppendAsync(params Node[] nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (nodes.Length == 0)
            {
                throw new ArgumentException("Must append at least one node.");
            }

            return EvaluateFunctionInternalAsync("(element, nodes) => { element.append(nodes); }", nodes);
        }

        ///  <summary>
        ///  Prepends nodes to the current document.
        ///  </summary>
        ///  <param name="nodes">The nodes to prepend.</param>
        ///  <returns>Task</returns>
        public virtual Task PrependAsync(params Node[] nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (nodes.Length == 0)
            {
                throw new ArgumentException("Must prepend at least one node.");
            }

            return EvaluateFunctionInternalAsync("(element, nodes) => { element.prepend(nodes); }", nodes);
        }

        ///  <summary>
        ///  Gets the child elements.
        ///  </summary>
        ///  <typeparam name="T">Type</typeparam>
        ///  <returns>Task</returns>
        public virtual Task<HtmlCollection<T>> GetChildrenAsync<T>()
            where T : Element
        {
            return EvaluateFunctionHandleInternalAsync<HtmlCollection<T>>("(element) => { return element.children; }");
        }

        ///  <summary>
        ///  Gets the first child element of this element.
        ///  </summary>
        ///  <typeparam name="T">Type</typeparam>
        ///  <returns>Task</returns>
        public virtual Task<T?> GetFirstElementChildAsync<T>()
            where T : Element
        {
            return EvaluateFunctionHandleInternalAsync<T?>("(element) => { return element.firstElementChild; }");
        }

        ///  <summary>
        ///  Gets the last child element of this element.
        ///  </summary>
        ///  <typeparam name="T">Type</typeparam>
        ///  <returns>Task</returns>
        public virtual Task<T?> GetLastElementChildAsync<T>()
            where T : Element
        {
            return EvaluateFunctionHandleInternalAsync<T?>("(element) => { return element.lastElementChild; }");
        }

        ///  <summary>
        ///  Gets the Element immediately following this ChildNode in its
        ///  parent's children list, or null if there is no Element in the list
        ///  following this ChildNode.
        ///  </summary>
        ///  <typeparam name="T">type</typeparam>
        ///  <returns>Element</returns>
        public virtual Task<T?> GetNextElementSiblingAsync<T>()
            where T : Element
        {
            return EvaluateFunctionHandleInternalAsync<T?>("(element) => { return element.nextElementSibling; }");
        }

        ///  <summary>
        ///  Gets the Element immediately prior to this ChildNode in its
        ///  parent's children list, or null if there is no Element in the list
        ///  prior to this ChildNode.
        ///  </summary>
        ///  <typeparam name="T">type</typeparam>
        ///  <returns>Element</returns>
        public virtual Task<T?> GetPreviousElementSiblingAsync<T>()
            where T : Element
        {
            return EvaluateFunctionHandleInternalAsync<T?>("(element) => { return element.previousElementSibling; }");
        }
    }

    /// <summary>
    /// The Document interface represents any web page loaded in the browser and serves as an entry point into the web page's content, which is the DOM tree.
    /// </summary>
    public partial class Document : Node
    {
        internal Document(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class CharacterData
    {
        internal CharacterData(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class Text
    {
        internal Text(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    /// <summary>
    /// The ShadowRoot interface represents the shadow root.
    /// </summary>
    public partial class ShadowRoot : DocumentFragment
    {
        internal ShadowRoot(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    /// <summary>
    /// The DocumentFragment interface represents a minimal document object
    /// that has no parent.
    /// </summary>
    public partial class DocumentFragment : DomHandle
    {
        internal DocumentFragment(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    /// <summary>
    /// The Attr interface represents one of an element's attributes as an object. In most situations,
    /// you will directly retrieve the attribute value as a string (e.g., Element.getAttribute()),
    /// but certain functions (e.g., Element.getAttributeNode()) or means of iterating return Attr instances.
    /// </summary>
    public partial class Attr
    {
        internal Attr(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class NamedNodeMap : IAsyncEnumerable<Attr>
    {
        internal NamedNodeMap(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Exposes an enumerator that provides asynchronous iteration over values of a specified type.
        /// </summary>
        /// <param name="token">cancellation token</param>
        /// <returns>IAsyncEnumerator</returns>
        public async IAsyncEnumerator<Attr> GetAsyncEnumerator(CancellationToken token)
        {
            var arr = await GetArray<Attr>().ConfigureAwait(false);

            foreach (var element in arr)
            {
                yield return element;
            }
        }

        /// <summary>
        /// To Array
        /// </summary>
        /// <returns>Task</returns>
        public async Task<Attr[]> ToArrayAsync()
        {
            return (await GetArray<Attr>().ConfigureAwait(false)).ToArray();
        }
    }

    public partial class TokenList : IAsyncEnumerable<string>
    {
        internal TokenList(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Exposes an enumerator that provides asynchronous iteration over values of a specified type.
        /// </summary>
        /// <param name="token">cancellation token</param>
        /// <returns>IAsyncEnumerator</returns>
        public async IAsyncEnumerator<string> GetAsyncEnumerator(CancellationToken token)
        {
            var arr = await GetStringArray().ConfigureAwait(false);

            foreach (var element in arr)
            {
                yield return element;
            }
        }

        /// <summary>
        /// To Array
        /// </summary>
        /// <returns>Task</returns>
        public async Task<string[]> ToArrayAsync()
        {
            return (await GetStringArray().ConfigureAwait(false)).ToArray();
        }
    }

    public partial class StringMap : IAsyncEnumerable<KeyValuePair<string, string>>
    {
        internal StringMap(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Exposes an enumerator that provides asynchronous iteration over values of a specified type.
        /// </summary>
        /// <param name="token">cancellation token</param>
        /// <returns>IAsyncEnumerator</returns>
        public async IAsyncEnumerator<KeyValuePair<string, string>> GetAsyncEnumerator(CancellationToken token)
        {
            var arr = await GetStringMapArray().ConfigureAwait(false);

            foreach (var element in arr)
            {
                yield return element;
            }
        }

        /// <summary>
        /// To Array
        /// </summary>
        /// <returns>Task</returns>
        public async Task<KeyValuePair<string, string>[]> ToArrayAsync()
        {
            return (await GetStringMapArray().ConfigureAwait(false)).ToArray();
        }
    }

    public partial class SettableTokenList
    {
        internal SettableTokenList(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlOptionsCollection
    {
        internal HtmlOptionsCollection(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlFormControlsCollection
    {
        internal HtmlFormControlsCollection(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlOptionsGroupElement
    {
        internal HtmlOptionsGroupElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlDataListElement
    {
        internal HtmlDataListElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlTableCaptionElement
    {
        internal HtmlTableCaptionElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }

    public partial class HtmlMenuElement
    {
        internal HtmlMenuElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }
    }
#pragma warning restore SA1601 // Partial elements should be documented
#pragma warning restore SA1402 // File may only contain a single type
}
