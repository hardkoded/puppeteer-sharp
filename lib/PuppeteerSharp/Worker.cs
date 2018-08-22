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
        private readonly Func<string, JSHandle[], object> _consoleAPICalled;
        private readonly TaskCompletionSource<ExecutionContext> _executionContextCallback;

        private Func<dynamic, JSHandle> _jsHandleFactory;

        public Worker(CDPSession client, string url, Func<string, JSHandle[], object> consoleAPICalled)
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

        public Task<ExecutionContext> ExecutionContextTask => _executionContextCallback.Task;

        public async Task<T> EvaluateExpressionAsync<T>(string script)
            => await (await ExecutionContextTask.ConfigureAwait(false)).EvaluateExpressionAsync<T>(script).ConfigureAwait(false);

        internal void OnMessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Runtime.executionContextCreated":
                    var executionContext = new ExecutionContext(_client, e.MessageData.SelectToken("context").ToObject<ContextPayload>(), _jsHandleFactory, null);
                    _jsHandleFactory = remoteObject => new JSHandle(executionContext, _client, remoteObject);
                    _executionContextCallback.TrySetResult(executionContext);
                    break;
                case "Runtime.consoleAPICalled":
                    var type = e.MessageData.SelectToken("type").ToObject<string>();
                    var args = e.MessageData.SelectToken("args").ToObject<dynamic[]>();
                    _consoleAPICalled(type, args.Select(arg => (JSHandle)_jsHandleFactory(arg)).ToArray());
                    break;
            }
        }
    }
}