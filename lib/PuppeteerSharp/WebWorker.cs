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
        /// Fired when an uncaught exception happens within the worker. Contains a <see cref="RemoteException"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="RemoteException.Handle"/> of the exception is disposed right after the event has been
        /// dispatched. Call <see cref="IJSHandle.Move"/> on it synchronously inside the handler to keep it alive.
        /// </remarks>
        public event EventHandler<RemoteExceptionEventArgs> ExceptionThrown;

        /// <summary>
        /// Emitted when the worker issues a request. Contains a <see cref="IRequest"/>.
        /// </summary>
        /// <remarks>
        /// Only supported with CDP. With WebDriver BiDi these events are never emitted.
        /// <para>
        /// For service workers and shared workers, network events are only delivered once a <see cref="WebWorker"/>
        /// instance exists, i.e. after <see cref="ITarget.WorkerAsync"/> has been called. Requests issued before that,
        /// such as requests made while a service worker starts up, are not reported.
        /// </para>
        /// <para>
        /// Listeners are bound to this worker instance: if the worker is terminated and restarted (e.g. an idle
        /// extension service worker), a new <see cref="WebWorker"/> is created and listeners must be re-attached.
        /// </para>
        /// <para>
        /// Requests issued by service workers and shared workers cannot be intercepted:
        /// <see cref="IRequest.ContinueAsync"/>, <see cref="IRequest.AbortAsync"/> and <see cref="IRequest.RespondAsync"/> throw.
        /// </para>
        /// This API is experimental.
        /// </remarks>
        public event EventHandler<RequestEventArgs> Request;

        /// <summary>
        /// Emitted when a request issued by the worker is served from cache. Contains a <see cref="IRequest"/>.
        /// </summary>
        /// <remarks>
        /// The same delivery rules as for <see cref="Request"/> apply. This API is experimental.
        /// </remarks>
        public event EventHandler<RequestEventArgs> RequestServedFromCache;

        /// <summary>
        /// Emitted when a response is received for a request issued by the worker. Contains a <see cref="IResponse"/>.
        /// </summary>
        /// <remarks>
        /// The same delivery rules as for <see cref="Request"/> apply. This API is experimental.
        /// </remarks>
        public event EventHandler<ResponseCreatedEventArgs> Response;

        /// <summary>
        /// Emitted when a request issued by the worker fails. Contains a <see cref="IRequest"/>.
        /// </summary>
        /// <remarks>
        /// The same delivery rules as for <see cref="Request"/> apply. This API is experimental.
        /// </remarks>
        public event EventHandler<RequestEventArgs> RequestFailed;

        /// <summary>
        /// Emitted when a request issued by the worker finishes successfully. Contains a <see cref="IRequest"/>.
        /// </summary>
        /// <remarks>
        /// The same delivery rules as for <see cref="Request"/> apply. This API is experimental.
        /// </remarks>
        public event EventHandler<RequestEventArgs> RequestFinished;

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

        internal bool HasConsoleListeners => Console != null;

        internal bool HasExceptionThrownListeners => ExceptionThrown != null;

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
        public virtual async Task<T> EvaluateExpressionAsync<T>(string script)
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
        public virtual async Task EvaluateFunctionAsync(string script, params object[] args)
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
        public virtual async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
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
        public virtual async Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
            => await World.EvaluateExpressionHandleAsync(script).ConfigureAwait(false);

        /// <summary>
        /// Waits for the provided function, <paramref name="script"/>, to return a truthy value when
        /// evaluated in the worker's context.
        /// </summary>
        /// <param name="script">Function to be evaluated in the worker context until it returns a truthy value.</param>
        /// <param name="options">Options for configuring waiting behavior.</param>
        /// <param name="args">Arguments to pass to <paramref name="script"/>.</param>
        /// <returns>A <see cref="Task"/> that resolves to a <see cref="IJSHandle"/> of the truthy value returned by the function.</returns>
        public virtual Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args)
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

        internal void OnExceptionThrown(RemoteExceptionEventArgs e) => ExceptionThrown?.Invoke(this, e);

        internal void OnRequest(RequestEventArgs e) => Request?.Invoke(this, e);

        internal void OnRequestServedFromCache(RequestEventArgs e) => RequestServedFromCache?.Invoke(this, e);

        internal void OnResponse(ResponseCreatedEventArgs e) => Response?.Invoke(this, e);

        internal void OnRequestFailed(RequestEventArgs e) => RequestFailed?.Invoke(this, e);

        internal void OnRequestFinished(RequestEventArgs e) => RequestFinished?.Invoke(this, e);

        internal void OnConsole(ConsoleEventArgs e) => Console?.Invoke(this, e);
    }
}
