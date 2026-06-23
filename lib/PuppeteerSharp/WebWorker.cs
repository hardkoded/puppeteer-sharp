using System;
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
        /// Fired when the worker calls a console API method.
        /// </summary>
        public event EventHandler<ConsoleEventArgs> Console;

        /// <summary>
        /// Gets the Worker URL.
        /// </summary>
        /// <value>Worker URL.</value>
        public string Url { get; }

        /// <summary>
        /// The CDP session client the WebWorker belongs to.
        /// </summary>
        public abstract ICDPSession Client { get; }

        /// <inheritdoc/>
        Realm IEnvironment.MainRealm => GetMainRealm();

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
        public async Task EvaluateFunctionAsync(string script, params object[] args)
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
        /// Waits for the provided function, <paramref name="script"/>, to return a truthy value when
        /// evaluated in the worker's context.
        /// </summary>
        /// <param name="script">Function to be evaluated in the worker context until it returns a truthy value.</param>
        /// <param name="options">Options for configuring waiting behavior.</param>
        /// <param name="args">Arguments to pass to <paramref name="script"/>.</param>
        /// <returns>A <see cref="Task"/> that resolves to a <see cref="IJSHandle"/> of the truthy value returned by the function.</returns>
        public Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args)
        {
            var opts = options ?? new WaitForFunctionOptions();

            // Default to interval polling (100ms) for workers, matching upstream behavior.
            if (!opts.PollingInterval.HasValue && opts.Polling == WaitForFunctionPollingOption.Raf)
            {
                opts = new WaitForFunctionOptions
                {
                    PollingInterval = 100,
                    Timeout = opts.Timeout,
                    Root = opts.Root,
                    CancellationToken = opts.CancellationToken,
                };
            }

            return World.WaitForFunctionAsync(script, opts, args);
        }

        /// <summary>
        /// Closes the worker.
        /// </summary>
        /// <returns>A <see cref="Task"/> that completes when the worker is closed.</returns>
        public abstract Task CloseAsync();

        internal virtual Realm GetMainRealm() => World;

        internal void OnConsole(ConsoleEventArgs e) => Console?.Invoke(this, e);
    }
}
