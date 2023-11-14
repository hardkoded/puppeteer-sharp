using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="IJSHandle"/> and <see cref="IElementHandle"/> Extensions.
    /// </summary>
    public static class PuppeteerHandleExtensions
    {
        /// <summary>
        /// Runs <paramref name="pageFunction"/> within the frame and passes it the outcome of <paramref name="elementHandleTask"/> as the first argument.
        /// </summary>
        /// <param name="elementHandleTask">A task that returns an <see cref="IElementHandle"/> that will be used as the first argument in <paramref name="pageFunction"/>.</param>
        /// <param name="pageFunction">Function to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to <c>pageFunction</c>.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SelectorException">If <paramref name="elementHandleTask"/> resolves to <c>null</c>.</exception>
        public static Task EvaluateFunctionAsync(this Task<IElementHandle> elementHandleTask, string pageFunction, params object[] args)
            => elementHandleTask.EvaluateFunctionAsync<object>(pageFunction, args);

        /// <summary>
        /// Runs <paramref name="pageFunction"/> within the frame and passes it the outcome of <paramref name="elementHandleTask"/> as the first argument.
        /// </summary>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <param name="elementHandleTask">A task that returns an <see cref="IElementHandle"/> that will be used as the first argument in <paramref name="pageFunction"/>.</param>
        /// <param name="pageFunction">Function to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to <c>pageFunction</c>.</param>
        /// <returns>Task which resolves to the return value of <c>pageFunction</c>.</returns>
        /// <exception cref="SelectorException">If <paramref name="elementHandleTask"/> resolves to <c>null</c>.</exception>
        public static async Task<T> EvaluateFunctionAsync<T>(this Task<IElementHandle> elementHandleTask, string pageFunction, params object[] args)
        {
            if (elementHandleTask == null)
            {
                throw new ArgumentNullException(nameof(elementHandleTask));
            }

            var elementHandle = await elementHandleTask.ConfigureAwait(false);
            if (elementHandle == null)
            {
                throw new SelectorException("Error: failed to find element matching selector");
            }

            return await elementHandle.EvaluateFunctionAsync<T>(pageFunction, args).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs <paramref name="pageFunction"/> within the frame and passes it the outcome the <paramref name="elementHandle"/> as the first argument.
        /// </summary>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <param name="elementHandle">An <see cref="IElementHandle"/> that will be used as the first argument in <paramref name="pageFunction"/>.</param>
        /// <param name="pageFunction">Function to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to <c>pageFunction</c>.</param>
        /// <returns>Task which resolves to the return value of <c>pageFunction</c>.</returns>
        /// <exception cref="SelectorException">If <paramref name="elementHandle"/> is <c>null</c>.</exception>
        public static async Task<T> EvaluateFunctionAsync<T>(this IElementHandle elementHandle, string pageFunction, params object[] args)
        {
            if (elementHandle == null)
            {
                throw new SelectorException("Error: failed to find element matching selector");
            }

            var result = await elementHandle.EvaluateFunctionAsync<T>(pageFunction, args).ConfigureAwait(false);
            await elementHandle.DisposeAsync().ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Runs <paramref name="pageFunction"/> within the frame and passes it the outcome of <paramref name="arrayHandleTask"/> as the first argument. Use only after <see cref="IPage.QuerySelectorAllHandleAsync(string)"/>.
        /// </summary>
        /// <param name="arrayHandleTask">A task that returns an <see cref="IJSHandle"/> that represents an array of <see cref="IElementHandle"/> that will be used as the first argument in <paramref name="pageFunction"/>.</param>
        /// <param name="pageFunction">Function to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to <c>pageFunction</c>.</param>
        /// <returns>Task.</returns>
        public static Task EvaluateFunctionAsync(this Task<IJSHandle> arrayHandleTask, string pageFunction, params object[] args)
            => arrayHandleTask.EvaluateFunctionAsync<object>(pageFunction, args);

        /// <summary>
        /// Runs <paramref name="pageFunction"/> within the frame and passes it the outcome of <paramref name="arrayHandleTask"/> as the first argument. Use only after <see cref="IPage.QuerySelectorAllHandleAsync(string)"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="arrayHandleTask">A task that returns an <see cref="IJSHandle"/> that represents an array of <see cref="IElementHandle"/> that will be used as the first argument in <paramref name="pageFunction"/>.</param>
        /// <param name="pageFunction">Function to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to <c>pageFunction</c>.</param>
        /// <returns>Task which resolves to the return value of <c>pageFunction</c>.</returns>
        public static async Task<T> EvaluateFunctionAsync<T>(this Task<IJSHandle> arrayHandleTask, string pageFunction, params object[] args)
        {
            if (arrayHandleTask == null)
            {
                throw new ArgumentNullException(nameof(arrayHandleTask));
            }

            return await (await arrayHandleTask.ConfigureAwait(false)).EvaluateFunctionAsync<T>(pageFunction, args).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs <paramref name="pageFunction"/> within the frame and passes it the outcome of <paramref name="arrayHandle"/> as the first argument. Use only after <see cref="IPage.QuerySelectorAllHandleAsync(string)"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="arrayHandle">An <see cref="IJSHandle"/> that represents an array of <see cref="IElementHandle"/> that will be used as the first argument in <paramref name="pageFunction"/>.</param>
        /// <param name="pageFunction">Function to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to <c>pageFunction</c>.</param>
        /// <returns>Task which resolves to the return value of <c>pageFunction</c>.</returns>
        public static async Task<T> EvaluateFunctionAsync<T>(this IJSHandle arrayHandle, string pageFunction, params object[] args)
        {
            if (arrayHandle == null)
            {
                throw new ArgumentNullException(nameof(arrayHandle));
            }

            var result = await arrayHandle.EvaluateFunctionAsync<T>(pageFunction, args).ConfigureAwait(false);
            await arrayHandle.DisposeAsync().ConfigureAwait(false);
            return result;
        }

        internal static object FormatArgument(this IJSHandle jSHandle, ExecutionContext context)
        {
            if (jSHandle.Disposed)
            {
                throw new PuppeteerException("JSHandle is disposed!");
            }

            if ((jSHandle as JSHandle).Realm != context.World)
            {
                throw new PuppeteerException("JSHandles can be evaluated only in the context they were created!");
            }

            var unserializableValue = jSHandle.RemoteObject.UnserializableValue;

            if (unserializableValue != null)
            {
                return new { unserializableValue };
            }

            if (jSHandle.RemoteObject.ObjectId == null)
            {
                return new { jSHandle.RemoteObject.Value };
            }

            var objectId = jSHandle.RemoteObject.ObjectId;

            return new { objectId };
        }

        internal static async IAsyncEnumerable<IElementHandle> TransposeIterableHandleAsync(this IJSHandle handle)
        {
            var iterator = await handle.EvaluateFunctionHandleAsync(@"iterable => {
                return (async function* () {
                    yield* iterable;
                })();
            }").ConfigureAwait(false);

            await foreach (var item in iterator.TransposeIteratorHandleAsync())
            {
                yield return item;
            }
        }

        internal static async IAsyncEnumerable<IElementHandle> TransposeIteratorHandleAsync(this IJSHandle iterator)
        {
            try
            {
                IEnumerable<IElementHandle> result;
                do
                {
                    result = await iterator.FastTransposeIteratorHandleAsync().ConfigureAwait(false);
                    foreach (var item in result)
                    {
                        yield return item;
                    }
                }
                while (result.Any());
            }
            finally
            {
                await iterator.DisposeAsync().ConfigureAwait(false);
            }
        }

        internal static async Task<IEnumerable<IElementHandle>> FastTransposeIteratorHandleAsync(this IJSHandle handle)
        {
            var array = await handle.EvaluateFunctionHandleAsync(
                @"async (iterator, size) =>
                {
                    const results = [];
                    while (results.length < size)
                    {
                        const result = await iterator.next();
                        if (result.done)
                        {
                            break;
                        }
                        results.push(result.value);
                    }
                    return results;
                }",
                20).ConfigureAwait(false);

            var properties = await array.GetPropertiesAsync().ConfigureAwait(false);

            await array.DisposeAsync().ConfigureAwait(false);
            return properties.Values.Where(handle => handle is IElementHandle).Cast<IElementHandle>();
        }
    }
}
