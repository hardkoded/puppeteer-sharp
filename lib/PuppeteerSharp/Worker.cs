using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private ExecutionContext _executionContext;
        private readonly Func<ConsoleType, JSHandle[], Task> _consoleAPICalled;
        private readonly Action<EvaluateExceptionDetails> _exceptionThrown;
        private readonly TaskCompletionSource<ExecutionContext> _executionContextCallback;
        private Func<ExecutionContext, dynamic, JSHandle> _jsHandleFactory;

        internal Worker(
            CDPSession client,
            string url,
            Func<ConsoleType, JSHandle[], Task> consoleAPICalled,
            Action<EvaluateExceptionDetails> exceptionThrown)
        {
            _logger = client.Connection.LoggerFactory.CreateLogger<Worker>();
            _client = client;
            Url = url;
            _consoleAPICalled = consoleAPICalled;
            _exceptionThrown = exceptionThrown;
            _client.MessageReceived += OnMessageReceived;

            _executionContextCallback = new TaskCompletionSource<ExecutionContext>();
            _ = _client.SendAsync("Runtime.enable").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    _logger.LogError(task.Exception.Message);
                }
            });
            _ = _client.SendAsync("Log.enable").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    _logger.LogError(task.Exception.Message);
                }
            });
        }

        /// <summary>
        /// Gets the Worker URL.
        /// </summary>
        /// <value>Worker URL.</value>
        public string Url { get; }

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

        internal Task<ExecutionContext> ExecutionContextTask => _executionContextCallback.Task;

        internal async void OnMessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Runtime.executionContextCreated":
                    OnExecutionContextCreated(e);
                    break;
                case "Runtime.consoleAPICalled":
                    await OnConsoleAPICalled(e);
                    break;
                case "Runtime.exceptionThrown":
                    OnExceptionThrown(e);
                    break;
            }
        }

        private void OnExceptionThrown(MessageEventArgs e)
            => _exceptionThrown(e.MessageData.SelectToken("exceptionDetails").ToObject<EvaluateExceptionDetails>());

        private async Task OnConsoleAPICalled(MessageEventArgs e)
        {
            var consoleData = e.MessageData.ToObject<PageConsoleResponse>();
            await _consoleAPICalled(
                consoleData.Type,
                consoleData.Args.Select<dynamic, JSHandle>(i => _jsHandleFactory(_executionContext, i)).ToArray());
        }

        private void OnExecutionContextCreated(MessageEventArgs e)
        {
            if (_jsHandleFactory == null)
            {
                _jsHandleFactory = (ctx, remoteObject) => new JSHandle(ctx, _client, remoteObject);
                _executionContext = new ExecutionContext(
                    _client,
                    e.MessageData.SelectToken("context").ToObject<ContextPayload>(),
                    _jsHandleFactory,
                    null);
                _executionContextCallback.TrySetResult(_executionContext);
            }
        }
    }
}