using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Input;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// A LazyArg is an evaluation argument that will be resolved when the CDP call is built.
    /// </summary>
    /// <param name="context">Execution context.</param>
    /// <returns>Resolved argument.</returns>
    public delegate Task<object> LazyArg(ExecutionContext context);

    internal class IsolatedWorld
    {
        private static string _injectedSource;
        private readonly FrameManager _frameManager;
        private readonly TimeoutSettings _timeoutSettings;
        private readonly CDPSession _client;
        private readonly ILogger _logger;
        private readonly List<string> _ctxBindings = new();
        private bool _detached;
        private TaskCompletionSource<ExecutionContext> _contextResolveTaskWrapper = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private Task _settingUpBinding;
        private Task<ElementHandle> _documentTask;

        public IsolatedWorld(
            CDPSession client,
            FrameManager frameManager,
            Frame frame,
            TimeoutSettings timeoutSettings)
        {
            _logger = client.Connection.LoggerFactory.CreateLogger<IsolatedWorld>();
            _client = client;
            _frameManager = frameManager;
            Frame = frame;
            _timeoutSettings = timeoutSettings;

            _detached = false;
            _client.MessageReceived += Client_MessageReceived;
        }

        internal TaskManager TaskManager { get; set; } = new();

        internal Frame Frame { get; }

        internal bool HasContext => _contextResolveTaskWrapper?.Task.IsCompleted == true;

        internal ConcurrentDictionary<string, Delegate> BoundFunctions { get; } = new();

        internal async Task AddBindingToContextAsync(ExecutionContext context, string name)
        {
            // Previous operation added the binding so we are done.
            if (_ctxBindings.Contains(GetBindingIdentifier(name, context.ContextId)))
            {
                return;
            }

            // Wait for other operation to finish
            if (_settingUpBinding != null)
            {
                await _settingUpBinding.ConfigureAwait(false);
                await AddBindingToContextAsync(context, name).ConfigureAwait(false);
                return;
            }

            async Task BindAsync(string name)
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
                            ExecutionContextName = context.ContextName,
                        }).ConfigureAwait(false);
                    await context.EvaluateExpressionAsync(expression).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var ctxDestroyed = ex.Message.Contains("Execution context was destroyed");
                    var ctxNotFound = ex.Message.Contains("Cannot find context with specified id");
                    if (ctxDestroyed || ctxNotFound)
                    {
                        return;
                    }
                    else
                    {
                        _logger.LogError(ex.ToString());
                        return;
                    }
                }

                _ctxBindings.Add(GetBindingIdentifier(name, context.ContextId));
            }

            _settingUpBinding = BindAsync(name);
            await _settingUpBinding.ConfigureAwait(false);
            _settingUpBinding = null;
        }

        internal async Task<IElementHandle> AdoptBackendNodeAsync(object backendNodeId)
        {
            var executionContext = await GetExecutionContextAsync().ConfigureAwait(false);
            var obj = await _client.SendAsync<DomResolveNodeResponse>("DOM.resolveNode", new DomResolveNodeRequest
            {
                BackendNodeId = backendNodeId,
                ExecutionContextId = executionContext.ContextId,
            }).ConfigureAwait(false);

            return executionContext.CreateJSHandle(obj.Object) as IElementHandle;
        }

        internal async Task<IJSHandle> TransferHandleAsync(IJSHandle handle)
        {
            var result = await AdoptHandleAsync(handle).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
            return result;
        }

        internal async Task<IJSHandle> AdoptHandleAsync(IJSHandle handle)
        {
            var executionContext = await GetExecutionContextAsync().ConfigureAwait(false);

            if (executionContext == handle.ExecutionContext)
            {
                return handle;
            }

            var nodeInfo = await _client.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
            {
                ObjectId = ((ElementHandle)handle).RemoteObject.ObjectId,
            }).ConfigureAwait(false);
            return await AdoptBackendNodeAsync(nodeInfo.Node.BackendNodeId).ConfigureAwait(false);
        }

        internal void Detach()
        {
            _detached = true;
            TaskManager.TerminateAll(new Exception("waitForFunction failed: frame got detached."));
        }

        internal Task<ExecutionContext> GetExecutionContextAsync()
        {
            if (_detached)
            {
                throw new PuppeteerException($"Execution Context is not available in detached frame \"{Frame.Url}\"(are you trying to evaluate?)");
            }

            return _contextResolveTaskWrapper.Task;
        }

        internal async Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionHandleAsync(script).ConfigureAwait(false);
        }

        internal async Task<IJSHandle> EvaluateFunctionHandleAsync(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateFunctionHandleAsync(script, args).ConfigureAwait(false);
        }

        internal async Task<T> EvaluateExpressionAsync<T>(string script)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionAsync<T>(script).ConfigureAwait(false);
        }

        internal async Task<JToken> EvaluateExpressionAsync(string script)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionAsync(script).ConfigureAwait(false);
        }

        internal async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateFunctionAsync<T>(script, args).ConfigureAwait(false);
        }

        internal async Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateFunctionAsync(script, args).ConfigureAwait(false);
        }

        internal async Task<IElementHandle> QuerySelectorAsync(string selector)
        {
            var document = await GetDocumentAsync().ConfigureAwait(false);
            return await document.QuerySelectorAsync(selector).ConfigureAwait(false);
        }

        internal async Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
        {
            var document = await GetDocumentAsync().ConfigureAwait(false);
            return await document.QuerySelectorAllHandleAsync(selector).ConfigureAwait(false);
        }

        internal async Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
        {
            var document = await GetDocumentAsync().ConfigureAwait(false);
            return await document.QuerySelectorAllAsync(selector).ConfigureAwait(false);
        }

        internal async Task<IElementHandle[]> XPathAsync(string expression)
        {
            var document = await GetDocumentAsync().ConfigureAwait(false);
            return await document.XPathAsync(expression).ConfigureAwait(false);
        }

        internal Task<string> GetContentAsync() => EvaluateFunctionAsync<string>(
            @"() => {
                let retVal = '';
                if (document.doctype)
                    retVal = new XMLSerializer().serializeToString(document.doctype);
                if (document.documentElement)
                    retVal += document.documentElement.outerHTML;
                return retVal;
            }");

        internal async Task SetContentAsync(string html, NavigationOptions options = null)
        {
            var waitUntil = options?.WaitUntil ?? new[] { WaitUntilNavigation.Load };
            var timeout = options?.Timeout ?? _timeoutSettings.NavigationTimeout;

            // We rely upon the fact that document.open() will reset frame lifecycle with "init"
            // lifecycle event. @see https://crrev.com/608658
            await EvaluateFunctionAsync(
                @"html => {
                    document.open();
                    document.write(html);
                    document.close();
                }",
                html).ConfigureAwait(false);

            using (var watcher = new LifecycleWatcher(_frameManager, Frame, waitUntil, timeout))
            {
                var watcherTask = await Task.WhenAny(
                    watcher.TimeoutOrTerminationTask,
                    watcher.LifecycleTask).ConfigureAwait(false);

                await watcherTask.ConfigureAwait(false);
            }
        }

        internal async Task<IElementHandle> WaitForSelectorInPageAsync(string queryOne, IElementHandle root, string selector, WaitForSelectorOptions options, PageBinding[] bindings = null)
        {
            try
            {
                var executionContext = await GetExecutionContextAsync().ConfigureAwait(false);
                var waitForVisible = options?.Visible ?? false;
                var waitForHidden = options?.Hidden ?? false;
                var timeout = options?.Timeout ?? _timeoutSettings.Timeout;

                var predicate = @$"async (PuppeteerUtil, query, selector, root, visible) => {{
                  if (!PuppeteerUtil) {{
                    return;
                  }}
                  const node = (await PuppeteerUtil.createFunction(query)(
                    root || document,
                    selector,
                    PuppeteerUtil,
                  ));
                  return PuppeteerUtil.checkVisibility(node, visible);
                }}";

                var args = new List<object>
                {
                    await executionContext.GetPuppeteerUtilAsync().ConfigureAwait(false),
                    queryOne,
                    selector,
                    root,
                };

                // Puppeteer's injected code checks for visible to be undefined
                // As we don't support passing undefined values we need to ignore sending this value
                // if visible is false
                if (waitForVisible || waitForHidden)
                {
                    args.Add(waitForVisible);
                }

                var jsHandle = await WaitForFunctionAsync(
                    predicate,
                    new()
                    {
                        Bindings = bindings,
                        Polling = waitForVisible || waitForHidden ? WaitForFunctionPollingOption.Raf : WaitForFunctionPollingOption.Mutation,
                        Root = root,
                        Timeout = timeout,
                    },
                    args.ToArray()).ConfigureAwait(false);

                if (jsHandle is not ElementHandle elementHandle)
                {
                    await jsHandle.DisposeAsync().ConfigureAwait(false);
                    return null;
                }

                return elementHandle;
            }
            catch (Exception ex)
            {
                throw new WaitTaskTimeoutException($"Waiting for selector `{selector}` failed: {ex.Message}", ex);
            }
        }

        internal async Task ClickAsync(string selector, ClickOptions options = null)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false) ?? throw new SelectorException($"No node found for selector: {selector}", selector);
            await handle.ClickAsync(options).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        internal async Task HoverAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false)
                ?? throw new SelectorException($"No node found for selector: {selector}", selector);
            await handle.HoverAsync().ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        internal async Task FocusAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false) ?? throw new SelectorException($"No node found for selector: {selector}", selector);
            await handle.FocusAsync().ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        internal async Task<string[]> SelectAsync(string selector, params string[] values)
        {
            if ((await QuerySelectorAsync(selector).ConfigureAwait(false)) is not IElementHandle handle)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }

            var result = await handle.SelectAsync(values).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
            return result;
        }

        internal async Task TapAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false)
                ?? throw new SelectorException($"No node found for selector: {selector}", selector);
            await handle.TapAsync().ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        internal async Task TypeAsync(string selector, string text, TypeOptions options = null)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false)
                ?? throw new SelectorException($"No node found for selector: {selector}", selector);
            await handle.TypeAsync(text, options).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        internal async Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args)
        {
            using var waitTask = new WaitTask(
                 this,
                 script,
                 false,
                 options.Polling,
                 options.PollingInterval,
                 options.Timeout ?? _timeoutSettings.Timeout,
                 options.Root,
                 options.Bindings,
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
                options.Timeout ?? _timeoutSettings.Timeout,
                null, // Root
                null, // PageBinding
                null); // args

            return await waitTask
                .Task
                .ConfigureAwait(false);
        }

        internal Task<string> GetTitleAsync() => EvaluateExpressionAsync<string>("document.title");

        internal Task<ElementHandle> GetDocumentAsync()
        {
            if (_documentTask != null)
            {
                return _documentTask;
            }

            async Task<ElementHandle> EvalauteDocumentInContext()
            {
                var context = await GetExecutionContextAsync().ConfigureAwait(false);
                var document = await context.EvaluateFunctionHandleAsync("() => document").ConfigureAwait(false);

                if (document is not ElementHandle element)
                {
                    throw new PuppeteerException("Document is null");
                }

                return element;
            }

            _documentTask = EvalauteDocumentInContext();

            return _documentTask;
        }

        internal void ClearContext()
        {
            _documentTask = null;
            _contextResolveTaskWrapper = new TaskCompletionSource<ExecutionContext>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        internal void SetContext(ExecutionContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _ctxBindings.Clear();
            _contextResolveTaskWrapper.TrySetResult(context);
            TaskManager.RerunAll();
        }

        private static string GetInjectedSource()
        {
            if (string.IsNullOrEmpty(_injectedSource))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "PuppeteerSharp.Injected.injected.js";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream);
                var fileContent = reader.ReadToEnd();
                _injectedSource = fileContent;
            }

            return _injectedSource;
        }

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Runtime.bindingCalled":
                        await OnBindingCalled(e.MessageData.ToObject<BindingCalledResponse>(true)).ConfigureAwait(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = $"IsolatedWorld failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                _client.Close(message);
            }
        }

        private async Task OnBindingCalled(BindingCalledResponse e)
        {
            var payload = e.BindingPayload;
            if (!HasContext)
            {
                return;
            }

            var context = await GetExecutionContextAsync().ConfigureAwait(false);

            if (e.BindingPayload.Type != "internal" ||
                !_ctxBindings.Contains(GetBindingIdentifier(payload.Name, context.ContextId)))
            {
                return;
            }

            if (context.ContextId != e.ExecutionContextId)
            {
                return;
            }

            try
            {
                if (!BoundFunctions.TryGetValue(payload.Name, out var fn))
                {
                    throw new PuppeteerException($"Bound function {payload.Name} is not found");
                }

                var result = await BindingUtils.ExecuteBindingAsync(e, BoundFunctions).ConfigureAwait(false);

                await context.EvaluateFunctionAsync(
                    @"(name, seq, result) => {
                      globalThis[name].callbacks.get(seq).resolve(result);
                      globalThis[name].callbacks.delete(seq);
                    }",
                    payload.Name,
                    payload.Seq,
                    result).ConfigureAwait(false);
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

        private string GetBindingIdentifier(string name, int contextId) => $"{name}_{contextId}";

        private async Task<IElementHandle> WaitForSelectorAsync(string selectorOrXPath, bool isXPath, WaitForSelectorOptions options = null)
        {
            options ??= new WaitForSelectorOptions();
            var waitForVisible = options?.Visible ?? false;
            var waitForHidden = options?.Hidden ?? false;
            var timeout = options.Timeout ?? _timeoutSettings.Timeout;

            const string predicate = @"function predicate(selectorOrXPath, isXPath, waitForVisible, waitForHidden) {
                const node = isXPath
                  ? document.evaluate(selectorOrXPath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue
                  : document.querySelector(selectorOrXPath);
                if (!node)
                  return waitForHidden;
                if (!waitForVisible && !waitForHidden)
                  return node;
                const element = node.nodeType === Node.TEXT_NODE ? node.parentElement : node;

                const style = window.getComputedStyle(element);
                const isVisible = style && style.visibility !== 'hidden' && hasVisibleBoundingBox();
                const success = (waitForVisible === isVisible || waitForHidden === !isVisible);
                return success ? node : null;

                function hasVisibleBoundingBox() {
                  const rect = element.getBoundingClientRect();
                  return !!(rect.top || rect.bottom || rect.width || rect.height);
                }
              }";
            var polling = waitForVisible || waitForHidden ? WaitForFunctionPollingOption.Raf : WaitForFunctionPollingOption.Mutation;

            using var waitTask = new WaitTask(
                this,
                predicate,
                false,
                polling,
                null, // Polling interval
                timeout,
                options.Root,
                null,
                new object[] { selectorOrXPath, isXPath, options.Visible, options.Hidden });

            var handle = await waitTask.Task.ConfigureAwait(false);

            if (handle is not IElementHandle elementHandle)
            {
                if (handle != null)
                {
                    await handle.DisposeAsync().ConfigureAwait(false);
                }

                return null;
            }

            return elementHandle;
        }
    }
}
