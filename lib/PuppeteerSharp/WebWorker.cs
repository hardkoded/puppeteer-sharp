using System.Text.Json;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// The Worker class represents a WebWorker (<see href="https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API"/>).
    /// The events <see cref="IPage.WorkerCreated"/> and <see cref="IPage.WorkerDestroyed"/> are emitted on the page object to signal the worker lifecycle.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// page.WorkerCreated += (sender, e) => Console.WriteLine('Worker created: ' + e.Worker.Url);
    /// page.WorkerDestroyed += (sender, e) => Console.WriteLine('Worker destroyed: ' + e.Worker.Url);
    /// for (var worker of page.Workers)
    /// {
    ///     Console.WriteLine('  ' + worker.Url);
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public abstract class WebWorker : IEnvironment
    {
        internal WebWorker(string url)
        {
            Url = url;
        }

        /// <summary>
        /// Gets the Worker URL.
        /// </summary>
        /// <value>Worker URL.</value>
        public string Url { get; }

        /// <summary>
        /// The CDP session client the WebWorker belongs to.
        /// </summary>
        public abstract CDPSession Client { get; }

        /// <inheritdoc/>
        CDPSession IEnvironment.Client => Client;

        /// <inheritdoc/>
        Realm IEnvironment.MainRealm => World;

        internal abstract IsolatedWorld World { get; }

        /// <summary>
        /// Executes a script in browser context.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <seealso cref="ExecutionContext.EvaluateExpressionAsync(string)"/>
        /// <returns>Task which resolves to script return value.</returns>
        public async Task<T> EvaluateExpressionAsync<T>(string script)
            => await World.EvaluateExpressionAsync<T>(script).ConfigureAwait(false);

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
        public async Task<JsonElement?> EvaluateFunctionAsync(string script, params object[] args)
            => await World.EvaluateFunctionAsync(script, args).ConfigureAwait(false);

        /// <summary>
        /// Executes a function in the context.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to script.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IJSHandle"/> instances can be passed as arguments.
        /// </remarks>
        /// <returns>Task which resolves to script return value.</returns>
        public async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
            => await World.EvaluateFunctionAsync<T>(script, args).ConfigureAwait(false);

        /// <summary>
        /// Executes a script in browser context.
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <returns>Task which resolves to script return value.</returns>
        /// <seealso cref="ExecutionContext.EvaluateExpressionHandleAsync(string)"/>
        public async Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
            => await World.EvaluateExpressionHandleAsync(script).ConfigureAwait(false);

        /// <summary>
        /// Closes the worker.
        /// </summary>
        /// <returns>A <see cref="Task"/> that completes when the worker is closed.</returns>
        public abstract Task CloseAsync();
    }
}
