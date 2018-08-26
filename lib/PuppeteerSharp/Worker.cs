using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PuppeteerSharp
{
    public class Worker
    {
        private readonly ILogger _logger;
        private readonly CDPSession _client;
        private readonly Action<ConsoleType, dynamic[]> _consoleAPICalled;
        private readonly TaskCompletionSource<ExecutionContext> _executionContextCallback;

        private Func<dynamic, JSHandle> _jsHandleFactory;

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

        public async Task<T> EvaluateExpressionAsync<T>(string script)
            => await (await ExecutionContextTask.ConfigureAwait(false)).EvaluateExpressionAsync<T>(script).ConfigureAwait(false);

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