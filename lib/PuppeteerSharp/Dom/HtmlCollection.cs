using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The HTMLCollection interface represents a generic collection (array-like object similar to arguments)
    /// of elements (in document order) and offers methods and properties for selecting from the list.
    /// </summary>
    /// <typeparam name="T">Type derived from <see cref="Element"/></typeparam>
    public class HtmlCollection<T> : DomHandle, IAsyncEnumerable<T>
        where T : Element
    {
        internal HtmlCollection(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        /// <summary>
        /// Exposes an enumerator that provides asynchronous iteration over values of a specified type.
        /// </summary>
        /// <param name="token">cancellation token</param>
        /// <returns>IAsyncEnumerator</returns>
        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token)
        {
            var arr = await GetArray<T>().ConfigureAwait(false);

            foreach (var element in arr)
            {
                yield return element;
            }
        }

        /// <summary>
        /// Returns the number of items in the collection.
        /// </summary>
        /// <returns>length</returns>
        public Task<int> GetLengthAsync()
        {
            return EvaluateFunctionInternalAsync<int>("(element) => { return element.length; }");
        }

        /// <summary>
        /// Returns the specific node at the given zero-based index into the list. Returns null if the index is out of range.
        /// https://developer.mozilla.org/en-US/docs/Web/API/HTMLCollection/item
        /// </summary>
        /// <param name="index">The position of the Node to be returned. Elements appear in an HTMLCollection in the same order in which they appear in the document's source.</param>
        /// <returns>
        /// A Task that evaluates to the Node at the specified index,
        /// or null if index is less than zero or greater than or equal to the length property.
        /// </returns>
        public Task<T> ItemAsync(int index)
        {
            return EvaluateFunctionHandleInternalAsync<T>("(element, index) => element.item(index)", index);
        }

        /// <summary>
        /// The namedItem() method of the HTMLCollection interface returns the first Element
        /// in the collection whose id or name attribute match the specified name, or null if no element matches.
        /// </summary>
        /// <param name="key">is a string representing the value of the id or name attribute of the element we are looking for.</param>
        /// <returns>A Task that when awaited is the first Element in the HTMLCollection matching the key, or null, if there are none.</returns>
        public Task<T> NamedItemAsync(string key)
        {
            return EvaluateFunctionHandleInternalAsync<T>("(e, name) => e.namedItem(name)", key);
        }

        /// <summary>
        /// To Array
        /// </summary>
        /// <returns>Task</returns>
        public async Task<T[]> ToArrayAsync()
        {
            return (await GetArray<T>().ConfigureAwait(false)).ToArray();
        }
    }
}
