using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Input;

namespace PuppeteerSharp
{
    internal abstract class Realm
    {
        public Realm(TimeoutSettings timeoutSettings)
        {
            TimeoutSettings = timeoutSettings;
        }

        internal TaskManager TaskManager { get; set; } = new();

        internal TimeoutSettings TimeoutSettings { get; }

        internal abstract IEnvironment Environment { get; }

        internal abstract Task<IJSHandle> AdoptHandleAsync(IJSHandle handle);

        internal abstract Task<IElementHandle> AdoptBackendNodeAsync(object backendNodeId);

        internal abstract Task<IJSHandle> TransferHandleAsync(IJSHandle handle);

        internal abstract Task<IJSHandle> EvaluateExpressionHandleAsync(string script);

        internal abstract Task<IJSHandle> EvaluateFunctionHandleAsync(string script, params object[] args);

        internal abstract Task<T> EvaluateExpressionAsync<T>(string script);

        internal abstract Task<JToken> EvaluateExpressionAsync(string script);

        internal abstract Task<T> EvaluateFunctionAsync<T>(string script, params object[] args);

        internal abstract Task<JToken> EvaluateFunctionAsync(string script, params object[] args);

        internal async Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args)
        {
            using var waitTask = new WaitTask(
                this,
                script,
                false,
                options.Polling,
                options.PollingInterval,
                options.Timeout ?? TimeoutSettings.Timeout,
                options.Root,
                args);

            return await waitTask
                .Task
                .ConfigureAwait(false);
        }

        internal async Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options)
        {
            using var waitTask = new WaitTask(
                this,
                script,
                true,
                options.Polling,
                options.PollingInterval,
                options.Timeout ?? TimeoutSettings.Timeout,
                null, // Root
                null); // args

            return await waitTask
                .Task
                .ConfigureAwait(false);
        }
    }
}
