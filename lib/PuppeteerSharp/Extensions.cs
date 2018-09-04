using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="JSHandle"/> and <see cref="ElementHandle"/> Extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Runs <paramref name="pageFunction"/> within the frame and passes it the outcome of <paramref name="elementHandleTask"/> as the first argument
        /// </summary>
        /// <typeparam name="T">The type of the response</typeparam>
        /// <param name="elementHandleTask">A task that returns an <see cref="ElementHandle"/> that will be used as the first argument in <paramref name="pageFunction"/></param>
        /// <param name="pageFunction">Function to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to <c>pageFunction</c></param>
        /// <returns>Task which resolves to the return value of <c>pageFunction</c></returns>
        /// <exception cref="SelectorException">If <paramref name="elementHandleTask"/> resolves to <c>null</c></exception>
        public static async Task<T> EvaluateFunctionAsync<T>(this Task<ElementHandle> elementHandleTask, string pageFunction, params object[] args)
        {
            var elementHandle = await elementHandleTask.ConfigureAwait(false);
            if (elementHandle == null)
            {
                throw new SelectorException("Error: failed to find element matching selector");
            }

            var newArgs = new object[args.Length + 1];
            newArgs[0] = elementHandle;
            args.CopyTo(newArgs, 1);
            var result = await elementHandle.ExecutionContext.EvaluateFunctionAsync<T>(pageFunction, newArgs).ConfigureAwait(false);
            await elementHandle.DisposeAsync().ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Runs <paramref name="pageFunction"/> within the frame and passes it the outcome of <paramref name="arrayHandleTask"/> as the first argument. Use only after <see cref="Page.QuerySelectorAllHandleAsync(string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayHandleTask">A task that returns an <see cref="JSHandle"/> that represents an array of <see cref="ElementHandle"/> that will be used as the first argument in <paramref name="pageFunction"/></param>
        /// <param name="pageFunction">Function to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to <c>pageFunction</c></param>
        /// <returns>Task which resolves to the return value of <c>pageFunction</c></returns>
        public static async Task<T> EvaluateFunctionAsync<T>(this Task<JSHandle> arrayHandleTask, string pageFunction, params object[] args)
        {
            var arrayHandle = await arrayHandleTask.ConfigureAwait(false);
            var response = await arrayHandle.JsonValueAsync<object[]>().ConfigureAwait(false);

            var newArgs = new object[args.Length + 1];
            newArgs[0] = arrayHandle;
            args.CopyTo(newArgs, 1);
            var result = await arrayHandle.ExecutionContext.EvaluateFunctionAsync<T>(pageFunction, newArgs).ConfigureAwait(false);
            await arrayHandle.DisposeAsync().ConfigureAwait(false);
            return result;
        }
    }
}