using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// The Worker class represents a WebWorker (<see href="https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API"/>).
    /// The events <see cref="Page.WorkerCreated"/> and <see cref="Page.WorkerDestroyed"/> are emitted on the page object to signal the worker lifecycle.
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
    public class Worker
    {
        private readonly ILogger _logger;
        private readonly CDPSession _client;
        private readonly Func<ConsoleType, JSHandle[], StackTrace, Task> _consoleAPICalled;
        private readonly Action<EvaluateExceptionResponseDetails> _exceptionThrown;
        private readonly TaskCompletionSource<ExecutionContext> _executionContextCallback;
        private ExecutionContext _executionContext;
        private Func<ExecutionContext, RemoteObject, JSHandle> _jsHandleFactory;

        internal Worker(
            CDPSession client,
            string url,
            Func<ConsoleType, JSHandle[], StackTrace, Task> consoleAPICalled,
            Action<EvaluateExceptionResponseDetails> exceptionThrown)
        {
            _logger = client.Connection.LoggerFactory.CreateLogger<Worker>();
            _client = client;
            Url = url;
            _consoleAPICalled = consoleAPICalled;
            _exceptionThrown = exceptionThrown;
            _client.MessageReceived += OnMessageReceived;

            _executionContextCallback = new TaskCompletionSource<ExecutionContext>(TaskCreationOptions.RunContinuationsAsynchronously);

            _ = _client.SendAsync("Runtime.enable").ContinueWith(
                task =>
                {
                    if (task.IsFaulted)
                    {
                        _logger.LogError(task.Exception.Message);
                    }
                },
                TaskScheduler.Default);

            _ = _client.SendAsync("Log.enable").ContinueWith(
                task =>
                {
                    if (task.IsFaulted)
                    {
                        _logger.LogError(task.Exception.Message);
                    }
                },
                TaskScheduler.Default);
        }

        /// <summary>
        /// Gets the Worker URL.
        /// </summary>
        /// <value>Worker URL.</value>
        public string Url { get; }

        internal Task<ExecutionContext> ExecutionContextTask => _executionContextCallback.Task;

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <seealso cref="ExecutionContext.EvaluateExpressionAsync(string)"/>
        /// <returns>Task which resolves to script return value</returns>
        public async Task<T> EvaluateExpressionAsync<T>(string script)
            => await (await ExecutionContextTask.ConfigureAwait(false)).EvaluateExpressionAsync<T>(script).ConfigureAwait(false);

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="JSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        public async Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
            => await (await ExecutionContextTask.ConfigureAwait(false)).EvaluateFunctionAsync(script, args).ConfigureAwait(false);

        /// <summary>
        /// Executes a function in the context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="JSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        public async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
            => await (await ExecutionContextTask.ConfigureAwait(false)).EvaluateFunctionAsync<T>(script, args).ConfigureAwait(false);

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        /// <seealso cref="ExecutionContext.EvaluateExpressionHandleAsync(string)"/>
        public async Task<JSHandle> EvaluateExpressionHandleAsync(string script)
            => await (await ExecutionContextTask.ConfigureAwait(false)).EvaluateExpressionHandleAsync(script).ConfigureAwait(false);

        internal async void OnMessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Runtime.executionContextCreated":
                        OnExecutionContextCreated(e.MessageData.ToObject<RuntimeExecutionContextCreatedResponse>(true));
                        break;
                    case "Runtime.consoleAPICalled":
                        await OnConsoleAPICalled(e).ConfigureAwait(false);
                        break;
                    case "Runtime.exceptionThrown":
                        OnExceptionThrown(e.MessageData.ToObject<RuntimeExceptionThrownResponse>(true));
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = $"Worker failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                _client.Close(message);
            }
        }

        private void OnExceptionThrown(RuntimeExceptionThrownResponse e) => _exceptionThrown(e.ExceptionDetails);

        private async Task OnConsoleAPICalled(MessageEventArgs e)
        {
            var consoleData = e.MessageData.ToObject<PageConsoleResponse>(true);
            await _consoleAPICalled(
                consoleData.Type,
                consoleData.Args.Select(i => _jsHandleFactory(_executionContext, i)).ToArray(),
                consoleData.StackTrace)
                    .ConfigureAwait(false);
        }

        private void OnExecutionContextCreated(RuntimeExecutionContextCreatedResponse e)
        {
            if (_jsHandleFactory == null)
            {
                _jsHandleFactory = (ctx, remoteObject) => new JSHandle(ctx, _client, remoteObject);
                _executionContext = new ExecutionContext(
                    _client,
                    e.Context,
                    null);
                _executionContextCallback.TrySetResult(_executionContext);
            }
        }
    }
}
