using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// A LazyArg is an evaluation argument that will be resolved when the CDP call is built.
    /// </summary>
    /// <param name="context">Execution context.</param>
    /// <returns>Resolved argument.</returns>
    public delegate Task<object> LazyArg(ExecutionContext context);

    internal class IsolatedWorld : Realm, IDisposable, IAsyncDisposable
    {
        private readonly ILogger _logger;
        private readonly List<string> _contextBindings = new();
        private readonly TaskQueue _bindingQueue = new();
        private bool _detached;
        private TaskCompletionSource<ExecutionContext> _contextResolveTaskWrapper = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private ExecutionContext _context;

        public IsolatedWorld(
            Frame frame,
            WebWorker worker,
            TimeoutSettings timeoutSettings,
            bool isMainWorld) : base(timeoutSettings)
        {
            Frame = frame;
            Worker = worker;
            IsMainWorld = isMainWorld;
            _logger = Client.Connection.LoggerFactory.CreateLogger<IsolatedWorld>();

            _detached = false;
            FrameUpdated();
        }

        /// <summary>
        /// This property is not upstream. It's helpful for debugging.
        /// </summary>
        internal bool IsMainWorld { get; }

        internal Frame Frame { get; }

        internal CDPSession Client => Frame?.Client ?? Worker?.Client;

        internal bool HasContext => _contextResolveTaskWrapper?.Task.IsCompleted == true;

        internal ConcurrentDictionary<string, Binding> Bindings { get; } = new();

        internal override IEnvironment Environment => (IEnvironment)Frame ?? Worker;

        private WebWorker Worker { get; }

        public void Dispose()
        {
            _bindingQueue.Dispose();
            _context?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _bindingQueue.DisposeAsync().ConfigureAwait(false);
            if (_context != null)
            {
                await _context.DisposeAsync().ConfigureAwait(false);
            }
        }

        internal void FrameUpdated() => Client.MessageReceived += Client_MessageReceived;

        internal async Task AddBindingToContextAsync(ExecutionContext context, string name)
        {
            // Previous operation added the binding so we are done.
            if (_contextBindings.Contains(name))
            {
                return;
            }

            await _bindingQueue.Enqueue(async () =>
            {
                var expression = BindingUtils.PageBindingInitString("internal", name);
                try
                {
                    // TODO: In theory, it would be enough to call this just once
                    await context.Client.SendAsync(
                        "Runtime.addBinding",
                        new RuntimeAddBindingRequest
                        {
                            Name = name,
                            ExecutionContextName = !string.IsNullOrEmpty(context.ContextName) ? context.ContextName : null,
                            ExecutionContextId = string.IsNullOrEmpty(context.ContextName) ? context.ContextId : null,
                        }).ConfigureAwait(false);

                    await context.EvaluateExpressionAsync(expression).ConfigureAwait(false);
                    _contextBindings.Add(name);
                }
                catch (Exception ex)
                {
                    var ctxDestroyed = ex.Message.Contains("Execution context was destroyed");
                    var ctxNotFound = ex.Message.Contains("Cannot find context with specified id");
                    if (ctxDestroyed || ctxNotFound)
                    {
                        return;
                    }

                    _logger.LogError(ex.ToString());
                }
            }).ConfigureAwait(false);
        }

        internal override async Task<IElementHandle> AdoptBackendNodeAsync(object backendNodeId)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            var obj = await Client.SendAsync<DomResolveNodeResponse>("DOM.resolveNode", new DomResolveNodeRequest
            {
                BackendNodeId = backendNodeId,
                ExecutionContextId = context.ContextId,
            }).ConfigureAwait(false);

            return context.CreateJSHandle(obj.Object) as IElementHandle;
        }

        internal override async Task<IJSHandle> TransferHandleAsync(IJSHandle handle)
        {
            if ((handle as JSHandle)?.Realm == this)
            {
                return handle;
            }

            if (handle.RemoteObject.ObjectId == null)
            {
                return handle;
            }

            var info = await Client.SendAsync<DomDescribeNodeResponse>(
                "DOM.describeNode",
                new DomDescribeNodeRequest
                {
                    ObjectId = handle.RemoteObject.ObjectId,
                }).ConfigureAwait(false);

            var newHandle = await AdoptBackendNodeAsync(info.Node.BackendNodeId).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
            return newHandle;
        }

        internal override async Task<IJSHandle> AdoptHandleAsync(IJSHandle handle)
        {
            if ((handle as JSHandle)?.Realm == this)
            {
                return handle;
            }

            var nodeInfo = await Client.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
            {
                ObjectId = ((JSHandle)handle).RemoteObject.ObjectId,
            }).ConfigureAwait(false);
            return await AdoptBackendNodeAsync(nodeInfo.Node.BackendNodeId).ConfigureAwait(false);
        }

        internal void Detach()
        {
            _detached = true;
            Client.MessageReceived -= Client_MessageReceived;
            TaskManager.TerminateAll(new PuppeteerException("waitForFunction failed: frame got detached."));
        }

        internal Task<ExecutionContext> GetExecutionContextAsync()
        {
            if (_detached)
            {
                throw new PuppeteerException($"Execution Context is not available in detached frame \"{Frame.Url}\" (are you trying to evaluate?)");
            }

            return _contextResolveTaskWrapper.Task;
        }

        internal override async Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionHandleAsync(script).ConfigureAwait(false);
        }

        internal override async Task<IJSHandle> EvaluateFunctionHandleAsync(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateFunctionHandleAsync(script, args).ConfigureAwait(false);
        }

        internal override async Task<T> EvaluateExpressionAsync<T>(string script)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionAsync<T>(script).ConfigureAwait(false);
        }

        internal override async Task<JToken> EvaluateExpressionAsync(string script)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionAsync(script).ConfigureAwait(false);
        }

        internal override async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateFunctionAsync<T>(script, args).ConfigureAwait(false);
        }

        internal override async Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateFunctionAsync(script, args).ConfigureAwait(false);
        }

        internal void ClearContext()
        {
            _contextResolveTaskWrapper.TrySetException(new PuppeteerException("Execution Context was destroyed"));
            _contextResolveTaskWrapper = new TaskCompletionSource<ExecutionContext>(TaskCreationOptions.RunContinuationsAsynchronously);
            _context?.Dispose();
            _context = null;
            Frame?.ClearDocumentHandle();
        }

        internal void SetNewContext(
            CDPSession client,
            ContextPayload contextPayload,
            IsolatedWorld world)
        {
            _context = new ExecutionContext(
                client,
                contextPayload,
                world);

            SetContext(_context);
        }

        internal void SetContext(ExecutionContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _contextBindings.Clear();
            _contextResolveTaskWrapper.TrySetResult(context);
            TaskManager.RerunAll();
        }

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Runtime.bindingCalled":
                        await OnBindingCalledAsync(e.MessageData.ToObject<BindingCalledResponse>(true)).ConfigureAwait(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = $"IsolatedWorld failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                Client.Close(message);
            }
        }

        private async Task OnBindingCalledAsync(BindingCalledResponse e)
        {
            var payload = e.BindingPayload;

            if (payload.Type != "internal")
            {
                return;
            }

            if (!_contextBindings.Contains(payload.Name))
            {
                return;
            }

            try
            {
                var context = await GetExecutionContextAsync().ConfigureAwait(false);

                if (e.ExecutionContextId != context.ContextId)
                {
                    return;
                }

                if (Bindings.TryGetValue(payload.Name, out var binding))
                {
                    await binding.RunAsync(context, payload.Seq, payload.Args.Cast<object>().ToArray(), payload.IsTrivial).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Protocol error"))
                {
                    return;
                }

                _logger.LogError(ex.ToString());
            }
        }
    }
}
