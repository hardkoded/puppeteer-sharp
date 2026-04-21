using System.Text.Json;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents an execution context (realm) within a frame or worker.
    /// </summary>
    public abstract class Realm(TimeoutSettings timeoutSettings)
    {
        /// <summary>
        /// Gets the origin that created this Realm.
        /// For example, a Chrome extension content script would have an origin like
        /// <c>chrome-extension://&lt;extension-id&gt;</c>.
        /// </summary>
        /// <remarks>This API is experimental.</remarks>
        public abstract string Origin { get; }

        internal TaskManager TaskManager { get; } = new();

        internal TimeoutSettings TimeoutSettings { get; } = timeoutSettings;

        internal abstract IEnvironment Environment { get; }

        /// <summary>
        /// Returns the extension that created this realm, if the realm was created from an Extension.
        /// An example of this is an extension content script running on a page.
        /// </summary>
        /// <returns>A task that resolves to the <see cref="Extension"/> that created this realm, or <c>null</c>.</returns>
        /// <remarks>This API is experimental.</remarks>
        public abstract Task<Extension> ExtensionAsync();

        internal abstract Task<IJSHandle> AdoptHandleAsync(IJSHandle handle);

        internal abstract Task<IElementHandle> AdoptBackendNodeAsync(object backendNodeId);

        internal abstract Task<IJSHandle> TransferHandleAsync(IJSHandle handle);

        internal abstract Task<IJSHandle> EvaluateExpressionHandleAsync(string script);

        internal abstract Task<IJSHandle> EvaluateFunctionHandleAsync(string script, params object[] args);

        internal abstract Task<T> EvaluateExpressionAsync<T>(string script);

        internal abstract Task EvaluateExpressionAsync(string script);

        internal abstract Task<T> EvaluateFunctionAsync<T>(string script, params object[] args);

        internal abstract Task EvaluateFunctionAsync(string script, params object[] args);

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
                options.CancellationToken,
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
                options.CancellationToken,
                null); // args

            return await waitTask
                .Task
                .ConfigureAwait(false);
        }
    }
}
