using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The CSSStyleDeclaration interface represents an object that is a CSS declaration block, and exposes style information and various style-related methods.
    /// </summary>
    public class CssStyleDeclaration : DomHandle
    {
        internal CssStyleDeclaration(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Returns a CSS property name by it's index.
        /// https://developer.mozilla.org/en-US/docs/Web/API/CSSStyleDeclaration/setProperty
        /// </summary>
        /// <param name="propertyName">CSS property name (hyphen case) to be modified.</param>
        /// <param name="value">optional value</param>
        /// <param name="important">optional important</param>
        /// <returns>Task</returns>
        public Task SetPropertyAsync(string propertyName, object value = null, bool important = false)
        {
            const string Important = "important";

            return EvaluateFunctionInternalAsync("(e, name, val, priority) => { e.setProperty(name, val, priority); }", propertyName, value ?? string.Empty, important ? Important : string.Empty);
        }

        /// <summary>
        /// returns the value of a specified CSS property.
        /// </summary>
        /// <typeparam name="T">Property Value Type e.g. string, int</typeparam>
        /// <param name="propertyName">property to get</param>
        /// <returns>Task of <typeparamref name="T"/></returns>
        /// <exception cref="PuppeteerException">Thrown if no matching property is found</exception>
        public Task<string> GetPropertyValueAsync<T>(string propertyName)
        {
            return EvaluateFunctionInternalAsync<string>("(e, name) => { return e.getPropertyValue(name); }", propertyName);
        }

        /// <summary>
        /// Returns a CSS property name by it's index.
        /// https://developer.mozilla.org/en-US/docs/Web/API/CSSStyleDeclaration/item
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>A Task that evaluates to property name.</returns>
        public Task<string> ItemAsync(int index)
        {
            return EvaluateFunctionInternalAsync<string>("(element, index) => { return element.item(index); }", index);
        }

        /// <summary>
        /// Removes a property from a CSS style declaration object.
        /// https://developer.mozilla.org/en-US/docs/Web/API/CSSStyleDeclaration/removeProperty
        /// </summary>
        /// <param name="propertyName">CSS property name (hyphen case) to be removed.</param>
        /// <returns>A Task that evaluates to the value of the CSS property before it was removed.</returns>
        public Task<string> RemovePropertyAsync(string propertyName)
        {
            return EvaluateFunctionInternalAsync<string>("(element, property) => { return element.removeProperty(property); }", propertyName);
        }

        /// <summary>
        /// Returns all explicitly set priorities on the CSS property.
        /// https://developer.mozilla.org/en-US/docs/Web/API/CSSStyleDeclaration/getPropertyPriority
        /// </summary>
        /// <param name="propertyName">CSS property name (hyphen case) to be checked.</param>
        /// <returns>A Task that evaluates the priority (e.g. "important") if one exists returns true. If none exists, returns false.</returns>
        public Task<bool> GetPropertyPriorityAsync(string propertyName)
        {
            return EvaluateFunctionInternalAsync<bool>("(element, property) => { return element.getPropertyPriority(property) === 'important'; }", propertyName);
        }
    }
}
