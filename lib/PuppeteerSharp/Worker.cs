using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
        private readonly Action<ConsoleType, dynamic[]> _consoleAPICalled;
        private readonly TaskCompletionSource<ExecutionContext> _executionContextCallback;

        private Func<dynamic, JSHandle> _jsHandleFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PuppeteerSharp.Worker"/> class.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="url">URL.</param>
        /// <param name="consoleAPICalled">Console APIC alled.</param>
        public Worker(CDPSession client, string url, Action<ConsoleType, dynamic[]> consoleAPICalled)
        {
            _logger = client.Connection.LoggerFactory.CreateLogger<Worker>();
            _client = client;
            Url = url;
            _consoleAPICalled = consoleAPICalled;
            _client.MessageReceived += OnMessageReceived;

            _executionContextCallback = new TaskCompletionSource<ExecutionContext>();
            _ = _client.SendAsync("Runtime.enable").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    _logger.LogError(task.Exception.Message);
                }
            }).ConfigureAwait(false);
        }

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

        internal void OnMessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Runtime.executionContextCreated":
                    if (_jsHandleFactory != null)
                    {
                        return;
                    }
                    var executionContext = new ExecutionContext(_client, e.MessageData.SelectToken("context").ToObject<ContextPayload>(), _jsHandleFactory, null);
                    _jsHandleFactory = remoteObject => new JSHandle(executionContext, _client, remoteObject);
                    _executionContextCallback.TrySetResult(executionContext);
                    break;
                case "Runtime.consoleAPICalled":
                    var type = e.MessageData.SelectToken("type").ToObject<ConsoleType>();
                    var args = e.MessageData.SelectToken("args").ToObject<dynamic[]>();
                    _consoleAPICalled(type, args);
                    break;
            }
        }
    }
}