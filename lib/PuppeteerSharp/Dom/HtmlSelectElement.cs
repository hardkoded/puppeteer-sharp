using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLSelectElement interface represents a select HTML Element.
    /// These elements also share all of the properties and methods of other HTML elements via the <see cref="HtmlElement"/> class.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLSelectElement" />
    public partial class HtmlSelectElement : HtmlElement
    {
        internal HtmlSelectElement(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Sets the index of the first selected option element. The value -1 indicates no element is selected.
        /// </summary>
        /// <param name="index">width in CSS pixels</param>
        /// <returns>A Task that when awaited sets the href property</returns>
        public Task SetSelectedIndexAsync(int index)
        {
            return SetPropertyValueAsync("selectedIndex", index);
        }

        /// <summary>
        /// Creates a new Option element and then adds it to the collection of option elements for this select element.
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="text">text</param>
        /// <param name="before">If this parameter is null (or the index does not exist), the new element is appended to the end of the collection.</param>
        /// <returns>A Task that when awaited adds the element at the specified index</returns>
        public async Task<HtmlOptionElement> AddAsync(string value, string text, int? before = null)
        {
            var element = await EvaluateFunctionHandleInternalAsync<HtmlOptionElement>(
                @"(e, val, text) => {
                    let option = document.createElement('option');
                    option.value = val;
                    option.text = text;
                    return option;
                }",
                value,
                text).ConfigureAwait(false);

            await EvaluateFunctionInternalAsync("(e, item, before) => e.add(item, before)", element, before).ConfigureAwait(false);

            return element;
        }

        /// <summary>
        /// Adds an element to the collection of option elements for this select element.
        /// </summary>
        /// <param name="element">element</param>
        /// <param name="before">If this parameter is null (or the index does not exist), the new element is appended to the end of the collection.</param>
        /// <returns>A Task that when awaited adds the element at the specified index</returns>
        public Task AddAsync(HtmlOptionElement element, int? before = null)
        {
            return EvaluateFunctionInternalAsync("(e, item, before) => e.add(item, before)", element, before);
        }

        /// <summary>
        /// Gets an item from the options collection for this select element.
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>A Task that when awaited gets the element at the specified index</returns>
        public Task<HtmlOptionElement> ItemAsync(int index)
        {
            return EvaluateFunctionHandleInternalAsync<HtmlOptionElement>("(e, index) => e.item(index)", index);
        }

        /// <summary>
        /// Gets the item in the options collection with the specified name. The name string can match either the id or the name attribute of an option node.
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>A Task that when awaited gets the element at the specified index</returns>
        public Task<HtmlOptionElement> NamedItemAsync(string name)
        {
            return EvaluateFunctionHandleInternalAsync<HtmlOptionElement>("(e, name) => e.namedItem(name)", name);
        }
    }
}
