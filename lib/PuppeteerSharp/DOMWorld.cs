using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Input;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.PageCoverage;

namespace PuppeteerSharp
{
    internal class DOMWorld
    {
        private readonly FrameManager _frameManager;
        private readonly CustomQueriesManager _customQueriesManager;
        private readonly TimeoutSettings _timeoutSettings;
        private readonly CDPSession _client;
        private readonly ILogger _logger;
        private readonly List<string> _ctxBindings = new();
        private bool _detached;
        private TaskCompletionSource<ExecutionContext> _contextResolveTaskWrapper;
        private TaskCompletionSource<IElementHandle> _documentCompletionSource;
        private Task _settingUpBinding;
        private Task<ElementHandle> _documentTask;

        public DOMWorld(CDPSession client, FrameManager frameManager, Frame frame, TimeoutSettings timeoutSettings)
        {
            Logger = client.Connection.LoggerFactory.CreateLogger<DOMWorld>();
            _client = client;
            _frameManager = frameManager;
            _customQueriesManager = ((Browser)frameManager.Page.Browser).CustomQueriesManager;
            Frame = frame;
            _timeoutSettings = timeoutSettings;

            SetContext(null);

            WaitTasks = new ConcurrentSet<WaitTask>();
            _detached = false;
            _client.MessageReceived += Client_MessageReceived;
            _logger = _client.Connection.LoggerFactory.CreateLogger<DOMWorld>();
        }

        internal ICollection<WaitTask> WaitTasks { get; set; }

        internal Frame Frame { get; }

        internal bool HasContext => _contextResolveTaskWrapper?.Task.IsCompleted == true;

        internal ILogger Logger { get; }

        internal ConcurrentDictionary<string, Delegate> BoundFunctions { get; } = new();

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
                var message = $"DOMWorld failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                Logger.LogError(ex, message);
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

            if (
                e.BindingPayload.Type != "internal" ||
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

        private string GetBindingIdentifier(string name, int contextId) => $"{name}_{contextId}";

        internal void SetContext(ExecutionContext context)
        {
            _documentTask = null;
            if (context != null)
            {
                _ctxBindings.Clear();
                _contextResolveTaskWrapper.TrySetResult(context);
                foreach (var waitTask in WaitTasks)
                {
                    _ = waitTask.Rerun();
                }
            }
            else
            {
                _documentCompletionSource = null;
                _contextResolveTaskWrapper = new TaskCompletionSource<ExecutionContext>(TaskCreationOptions.RunContinuationsAsynchronously);
            }
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

        internal async Task<IElementHandle> AdoptHandleAsync(IElementHandle handle)
        {
            var executionContext = await this.GetExecutionContextAsync().ConfigureAwait(false);

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
            while (WaitTasks.Count > 0)
            {
                WaitTasks.First().Terminate(new Exception("waitForFunction failed: frame got detached."));
            }
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
            var document = await GetDocument().ConfigureAwait(false);
            return await document.QuerySelectorAsync(selector).ConfigureAwait(false);
        }

        internal async Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
        {
            var document = await GetDocument().ConfigureAwait(false);
            return await document.QuerySelectorAllHandleAsync(selector).ConfigureAwait(false);
        }

        internal async Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
        {
            var document = await GetDocument().ConfigureAwait(false);
            return await document.QuerySelectorAllAsync(selector).ConfigureAwait(false);
        }

        internal async Task<IElementHandle[]> XPathAsync(string expression)
        {
            var document = await GetDocument().ConfigureAwait(false);
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

        internal async Task<IElementHandle> AddScriptTagAsync(AddTagOptions options)
        {
            const string addScriptUrl = @"async function addScriptUrl(url, type) {
              const script = document.createElement('script');
              script.src = url;
              if(type)
                script.type = type;
              const promise = new Promise((res, rej) => {
                script.onload = res;
                script.onerror = rej;
              });
              document.head.appendChild(script);
              await promise;
              return script;
            }";
            const string addScriptContent = @"function addScriptContent(content, type = 'text/javascript') {
              const script = document.createElement('script');
              script.type = type;
              script.text = content;
              let error = null;
              script.onerror = e => error = e;
              document.head.appendChild(script);
              if (error)
                throw error;
              return script;
            }";

            async Task<IElementHandle> AddScriptTagPrivate(string script, string urlOrContent, string type)
            {
                var context = await GetExecutionContextAsync().ConfigureAwait(false);
                return (string.IsNullOrEmpty(type)
                        ? await context.EvaluateFunctionHandleAsync(script, urlOrContent).ConfigureAwait(false)
                        : await context.EvaluateFunctionHandleAsync(script, urlOrContent, type).ConfigureAwait(false)) as IElementHandle;
            }

            if (!string.IsNullOrEmpty(options.Url))
            {
                var url = options.Url;
                try
                {
                    return await AddScriptTagPrivate(addScriptUrl, url, options.Type).ConfigureAwait(false);
                }
                catch (PuppeteerException)
                {
                    throw new PuppeteerException($"Loading script from {url} failed");
                }
            }

            if (!string.IsNullOrEmpty(options.Path))
            {
                var contents = await AsyncFileHelper.ReadAllText(options.Path).ConfigureAwait(false);
                contents += "//# sourceURL=" + options.Path.Replace("\n", string.Empty);
                return await AddScriptTagPrivate(addScriptContent, contents, options.Type).ConfigureAwait(false);
            }

            if (!string.IsNullOrEmpty(options.Content))
            {
                return await AddScriptTagPrivate(addScriptContent, options.Content, options.Type).ConfigureAwait(false);
            }

            throw new ArgumentException("Provide options with a `Url`, `Path` or `Content` property");
        }

        internal async Task<IElementHandle> WaitForSelectorInPageAsync(string queryOne, string selector, WaitForSelectorOptions options, PageBinding[] bindings = null)
        {
            var waitForVisible = options?.Visible ?? false;
            var waitForHidden = options?.Hidden ?? false;
            var timeout = options?.Timeout ?? _timeoutSettings.Timeout;

            var polling = waitForVisible || waitForHidden ? WaitForFunctionPollingOption.Raf : WaitForFunctionPollingOption.Mutation;
            var title = $"selector '{selector}'{(waitForHidden ? " to be hidden" : string.Empty)}";

            var predicate = @$"async function predicate(root, selector, waitForVisible, waitForHidden) {{
                const node = predicateQueryHandler
                  ? ((await predicateQueryHandler(root, selector)))
                  : root.querySelector(selector);
                return checkWaitForOptions(node, waitForVisible, waitForHidden);
            }}";

            using var waitTask = new WaitTask(
                this,
                MakePredicateString(predicate, queryOne),
                true,
                title,
                polling,
                null,
                timeout,
                options?.Root,
                bindings,
                new object[]
                {
                    selector,
                    waitForVisible,
                    waitForHidden,
                },
                true);

            var jsHandle = await waitTask.Task.ConfigureAwait(false);
            if (jsHandle is not ElementHandle elementHandle)
            {
                await jsHandle.DisposeAsync().ConfigureAwait(false);
                return null;
            }
            return elementHandle;
        }

        internal async Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
        {
            const string addStyleUrl = @"async function addStyleUrl(url) {
              const link = document.createElement('link');
              link.rel = 'stylesheet';
              link.href = url;
              const promise = new Promise((res, rej) => {
                link.onload = res;
                link.onerror = rej;
              });
              document.head.appendChild(link);
              await promise;
              return link;
            }";
            const string addStyleContent = @"async function addStyleContent(content) {
              const style = document.createElement('style');
              style.type = 'text/css';
              style.appendChild(document.createTextNode(content));
              const promise = new Promise((res, rej) => {
                style.onload = res;
                style.onerror = rej;
              });
              document.head.appendChild(style);
              await promise;
              return style;
            }";

            if (!string.IsNullOrEmpty(options.Url))
            {
                var url = options.Url;
                try
                {
                    var context = await GetExecutionContextAsync().ConfigureAwait(false);
                    return (await context.EvaluateFunctionHandleAsync(addStyleUrl, url).ConfigureAwait(false)) as IElementHandle;
                }
                catch (PuppeteerException)
                {
                    throw new PuppeteerException($"Loading style from {url} failed");
                }
            }

            if (!string.IsNullOrEmpty(options.Path))
            {
                var contents = await AsyncFileHelper.ReadAllText(options.Path).ConfigureAwait(false);
                contents += "//# sourceURL=" + options.Path.Replace("\n", string.Empty);
                var context = await GetExecutionContextAsync().ConfigureAwait(false);
                return (await context.EvaluateFunctionHandleAsync(addStyleContent, contents).ConfigureAwait(false)) as IElementHandle;
            }

            if (!string.IsNullOrEmpty(options.Content))
            {
                var context = await GetExecutionContextAsync().ConfigureAwait(false);
                return (await context.EvaluateFunctionHandleAsync(addStyleContent, options.Content).ConfigureAwait(false)) as IElementHandle;
            }

            throw new ArgumentException("Provide options with a `Url`, `Path` or `Content` property");
        }

        internal async Task ClickAsync(string selector, ClickOptions options = null)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }
            await handle.ClickAsync(options).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        internal async Task HoverAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }
            await handle.HoverAsync().ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        internal async Task FocusAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }
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
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }
            await handle.TapAsync().ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        internal async Task TypeAsync(string selector, string text, TypeOptions options = null)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }
            await handle.TypeAsync(text, options).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        internal async Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
        {
            var (updatedSelector, queryHandler) = _customQueriesManager.GetQueryHandlerAndSelector(selector);
            var root = options?.Root ?? await this.GetDocumentAsync().ConfigureAwait(false);
            return await queryHandler.WaitFor(root, updatedSelector, options).ConfigureAwait(false);
        }

        internal Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
            => WaitForSelectorOrXPathAsync(xpath, true, options);

        internal async Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args)
        {
            using var waitTask = new WaitTask(
                 this,
                 script,
                 false,
                 "function",
                 options.Polling,
                 options.PollingInterval,
                 options.Timeout ?? _timeoutSettings.Timeout,
                 null,
                 null,
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
                "function",
                options.Polling,
                options.PollingInterval,
                options.Timeout ?? _timeoutSettings.Timeout,
                null, // Root
                null, // PageBinding
                null, // args
                false); // predicateAcceptsContextElement

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
                var element = document as ElementHandle;

                if (element == null)
                {
                    throw new PuppeteerException("Document is null");
                }
                return element;
            }

            _documentTask = EvalauteDocumentInContext();

            return _documentTask;
        }

        private async Task<IElementHandle> GetDocument()
        {
            if (_documentCompletionSource == null)
            {
                _documentCompletionSource = new TaskCompletionSource<IElementHandle>(TaskCreationOptions.RunContinuationsAsynchronously);
                var context = await GetExecutionContextAsync().ConfigureAwait(false);
                var document = await context.EvaluateExpressionHandleAsync("document").ConfigureAwait(false);
                _documentCompletionSource.TrySetResult(document as IElementHandle);
            }
            return await _documentCompletionSource.Task.ConfigureAwait(false);
        }

        private string MakePredicateString(string predicate, string predicateQueryHandler)
        {
            var checkWaitForOptions = @"function checkWaitForOptions(
                node,
                waitForVisible,
                waitForHidden
              ) {
                if (!node) return waitForHidden;
                if (!waitForVisible && !waitForHidden) return node;
                const element =
                  node.nodeType === Node.TEXT_NODE
                    ? node.parentElement
                    : node;

                const style = window.getComputedStyle(element);
                const isVisible =
                    style && style.visibility !== 'hidden' && hasVisibleBoundingBox();
                const success =
                    waitForVisible === isVisible || waitForHidden === !isVisible;
                return success? node : null;

                function hasVisibleBoundingBox() {
                  const rect = element.getBoundingClientRect();
                  return !!(rect.top || rect.bottom || rect.width || rect.height);
                }
            }";

            var predicateQueryHandlerDef = !string.IsNullOrEmpty(predicateQueryHandler)
              ? $@"const predicateQueryHandler = {predicateQueryHandler};" : string.Empty;

            return $@"
                (() => {{
                  {predicateQueryHandlerDef}
                  const checkWaitForOptions = {checkWaitForOptions};
                  return ({predicate})(...args)
                }})() ";
        }

        private async Task<IElementHandle> WaitForSelectorOrXPathAsync(string selectorOrXPath, bool isXPath, WaitForSelectorOptions options = null)
        {
            options ??= new WaitForSelectorOptions();
            var timeout = options.Timeout ?? _timeoutSettings.Timeout;

            const string predicate = @"
              function predicate(selectorOrXPath, isXPath, waitForVisible, waitForHidden) {
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
            var polling = options.Visible || options.Hidden ? WaitForFunctionPollingOption.Raf : WaitForFunctionPollingOption.Mutation;

            using var waitTask = new WaitTask(
                this,
                predicate,
                false,
                $"{(isXPath ? "XPath" : "selector")} '{selectorOrXPath}'{(options.Hidden ? " to be hidden" : string.Empty)}",
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
