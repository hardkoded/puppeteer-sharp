using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    /// <summary>
    /// The class represents a context for JavaScript execution. Examples of JavaScript contexts are:
    /// Each <see cref="Frame"/> has a separate <see cref="IExecutionContext"/>
    /// All kind of web workers have their own contexts
    /// </summary>
    public interface IExecutionContext
    {
        /// <summary>
        /// Frame associated with this execution context.
        /// </summary>
        /// <remarks>
        /// NOTE Not every execution context is associated with a frame. For example, workers and extensions have execution contexts that are not associated with frames.
        /// </remarks>
        IFrame Frame { get; }

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <seealso cref="EvaluateFunctionAsync{T}(string, object[])"/>
        /// <returns>Task which resolves to script return value</returns>
        Task<JToken> EvaluateExpressionAsync(string script);

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <seealso cref="EvaluateFunctionAsync{T}(string, object[])"/>
        /// <seealso cref="EvaluateExpressionHandleAsync(string)"/>
        /// <returns>Task which resolves to script return value</returns>
        Task<T> EvaluateExpressionAsync<T>(string script);

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        Task<IJSHandle> EvaluateExpressionHandleAsync(string script);

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IJSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <seealso cref="EvaluateExpressionAsync{T}(string)"/>
        /// <returns>Task which resolves to script return value</returns>
        Task<JToken> EvaluateFunctionAsync(string script, params object[] args);

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IJSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <seealso cref="EvaluateExpressionAsync{T}(string)"/>
        /// <returns>Task which resolves to script return value</returns>
        Task<T> EvaluateFunctionAsync<T>(string script, params object[] args);

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="pageFunction">Script to be evaluated in browser context</param>
        /// <param name="args">Function arguments</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IJSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args);

        /// <summary>
        /// The method iterates JavaScript heap and finds all the objects with the given prototype.
        /// </summary>
        /// <returns>A task which resolves to a handle to an array of objects with this prototype.</returns>
        /// <param name="prototypeHandle">A handle to the object prototype.</param>
        Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle);
    }
}
