using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// IJSHandle represents an in-page JavaScript object. JSHandles can be created with the <see cref="IPage.EvaluateExpressionHandleAsync(string)"/> and <see cref="IPage.EvaluateFunctionHandleAsync(string, object[])"/> methods.
    /// </summary>
    public interface IJSHandle : IAsyncDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IJSHandle"/> is disposed.
        /// </summary>
        /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
        bool Disposed { get; }

        /// <summary>
        /// Gets the execution context.
        /// </summary>
        /// <value>The execution context.</value>
        IExecutionContext ExecutionContext { get; }

        /// <summary>
        /// Gets the remote object.
        /// </summary>
        /// <value>The remote object.</value>
        RemoteObject RemoteObject { get; }

        /// <summary>
        /// Executes a function in browser context.
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to script.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IJSHandle"/> instances can be passed as arguments.
        /// </remarks>
        /// <returns>Task which resolves to script return value.</returns>
        Task<JsonElement> EvaluateFunctionAsync(string script, params object[] args);

        /// <summary>
        /// Executes a function in browser context.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to script.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IJSHandle"/> instances can be passed as arguments.
        /// </remarks>
        /// <returns>Task which resolves to script return value.</returns>
        Task<T> EvaluateFunctionAsync<T>(string script, params object[] args);

        /// <summary>
        /// Executes a script in browser context.
        /// </summary>
        /// <param name="pageFunction">Script to be evaluated in browser context.</param>
        /// <param name="args">Function arguments.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IJSHandle"/> instances can be passed as arguments.
        /// </remarks>
        /// <returns>Task which resolves to script return value.</returns>
        Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args);

        /// <summary>
        /// Returns a <see cref="Dictionary{TKey, TValue}"/> with property names as keys and <see cref="IJSHandle"/> instances for the property values.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <example>
        /// <code>
        /// var handle = await page.EvaluateExpressionHandle("({window, document})");
        /// var properties = await handle.GetPropertiesAsync();
        /// var windowHandle = properties["window"];
        /// var documentHandle = properties["document"];
        /// await handle.DisposeAsync();
        /// </code>
        /// </example>
        Task<Dictionary<string, IJSHandle>> GetPropertiesAsync();

        /// <summary>
        /// Fetches a single property from the referenced object.
        /// </summary>
        /// <param name="propertyName">property to get.</param>
        /// <returns>Task of <see cref="IJSHandle"/>.</returns>
        Task<IJSHandle> GetPropertyAsync(string propertyName);

        /// <summary>
        /// Returns a JSON representation of the object.
        /// </summary>
        /// <returns>Task.</returns>
        /// <remarks>
        /// The method will return an empty JSON if the referenced object is not stringifiable. It will throw an error if the object has circular references.
        /// </remarks>
        Task<object> JsonValueAsync();

        /// <summary>
        /// Returns a JSON representation of the object.
        /// </summary>
        /// <typeparam name="T">A strongly typed object to parse to.</typeparam>
        /// <returns>Task.</returns>
        /// <remarks>
        /// The method will return an empty JSON if the referenced object is not stringifiable. It will throw an error if the object has circular references.
        /// </remarks>
        Task<T> JsonValueAsync<T>();
    }
}
