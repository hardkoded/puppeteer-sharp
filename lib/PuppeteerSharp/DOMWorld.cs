using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Input;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class DOMWorld
    {
        private readonly FrameManager _frameManager;
        private readonly TimeoutSettings _timeoutSettings;
        private readonly CDPSession _client;
        private bool _detached;
        private TaskCompletionSource<ExecutionContext> _contextResolveTaskWrapper;
        private TaskCompletionSource<ElementHandle> _documentCompletionSource;

        public DOMWorld(CDPSession client, FrameManager frameManager, Frame frame, TimeoutSettings timeoutSettings)
        {
            Logger = client.Connection.LoggerFactory.CreateLogger<DOMWorld>();
            _client = client;
            _frameManager = frameManager;
            Frame = frame;
            _timeoutSettings = timeoutSettings;

            SetContext(null);

            WaitTasks = new ConcurrentSet<WaitTask>();
            _detached = false;
            _client.MessageReceived += Client_MessageReceived;
        }

        private CustomQueriesManager CustomQueriesManager => _frameManager.Page.Browser.CustomQueriesManager;

        internal ICollection<WaitTask> WaitTasks { get; set; }

        internal Frame Frame { get; }

        internal bool HasContext => _contextResolveTaskWrapper?.Task.IsCompleted == true;

        internal ILogger Logger { get; }

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

        private Task OnBindingCalled(BindingCalledResponse bindingCalledResponse)
        {
            // TODO
            return Task.CompletedTask;
        }

        internal void SetContext(ExecutionContext context)
        {
            if (context != null)
            {
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

        internal async Task<JSHandle> EvaluateExpressionHandleAsync(string script)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionHandleAsync(script).ConfigureAwait(false);
        }

        internal async Task<JSHandle> EvaluateFunctionHandleAsync(string script, params object[] args)
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

        internal async Task<ElementHandle> QuerySelectorAsync(string selector)
        {
            var document = await GetDocument().ConfigureAwait(false);
            return await document.QuerySelectorAsync(selector).ConfigureAwait(false);
        }

        internal async Task<JSHandle> QuerySelectorAllHandleAsync(string selector)
        {
            var document = await GetDocument().ConfigureAwait(false);
            return await document.QuerySelectorAllHandleAsync(selector).ConfigureAwait(false);
        }

        internal async Task<ElementHandle[]> QuerySelectorAllAsync(string selector)
        {
            var document = await GetDocument().ConfigureAwait(false);
            return await document.QuerySelectorAllAsync(selector).ConfigureAwait(false);
        }

        internal async Task<ElementHandle[]> XPathAsync(string expression)
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

        internal async Task<ElementHandle> AddScriptTagAsync(AddTagOptions options)
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

            async Task<ElementHandle> AddScriptTagPrivate(string script, string urlOrContent, string type)
            {
                var context = await GetExecutionContextAsync().ConfigureAwait(false);
                return (string.IsNullOrEmpty(type)
                        ? await context.EvaluateFunctionHandleAsync(script, urlOrContent).ConfigureAwait(false)
                        : await context.EvaluateFunctionHandleAsync(script, urlOrContent, type).ConfigureAwait(false)) as ElementHandle;
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

        internal async Task<ElementHandle> WaitForSelectorInPageAsync(string queryOne, string selector, WaitForSelectorOptions options)
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

        internal async Task<ElementHandle> AddStyleTagAsync(AddTagOptions options)
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
                    return (await context.EvaluateFunctionHandleAsync(addStyleUrl, url).ConfigureAwait(false)) as ElementHandle;
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
                return (await context.EvaluateFunctionHandleAsync(addStyleContent, contents).ConfigureAwait(false)) as ElementHandle;
            }

            if (!string.IsNullOrEmpty(options.Content))
            {
                var context = await GetExecutionContextAsync().ConfigureAwait(false);
                return (await context.EvaluateFunctionHandleAsync(addStyleContent, options.Content).ConfigureAwait(false)) as ElementHandle;
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
            if ((await QuerySelectorAsync(selector).ConfigureAwait(false)) is not ElementHandle handle)
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

        internal Task<ElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
        {
            var (updatedSelector, queryHandler) = CustomQueriesManager.GetQueryHandlerAndSelector(selector);
            return queryHandler.WaitFor(this, updatedSelector, options);
        }

        internal Task<ElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
            => WaitForSelectorOrXPathAsync(xpath, true, options);

        internal async Task<JSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args)
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
                 args);

            return await waitTask
                .Task
                .ConfigureAwait(false);
        }

        internal async Task<JSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options)
        {
            using var waitTask = new WaitTask(
                this,
                script,
                true,
                "function",
                options.Polling,
                options.PollingInterval,
                options.Timeout ?? _timeoutSettings.Timeout,
                null);

            return await waitTask
                .Task
                .ConfigureAwait(false);
        }

        internal Task<string> GetTitleAsync() => EvaluateExpressionAsync<string>("document.title");

        private async Task<ElementHandle> GetDocument()
        {
            if (_documentCompletionSource == null)
            {
                _documentCompletionSource = new TaskCompletionSource<ElementHandle>(TaskCreationOptions.RunContinuationsAsynchronously);
                var context = await GetExecutionContextAsync().ConfigureAwait(false);
                var document = await context.EvaluateExpressionHandleAsync("document").ConfigureAwait(false);
                _documentCompletionSource.TrySetResult(document as ElementHandle);
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

        private async Task<ElementHandle> WaitForSelectorOrXPathAsync(string selectorOrXPath, bool isXPath, WaitForSelectorOptions options = null)
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
                null,
                timeout,
                options.Root,
                new object[] { selectorOrXPath, isXPath, options.Visible, options.Hidden });

            var handle = await waitTask.Task.ConfigureAwait(false);

            if (handle is not ElementHandle elementHandle)
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
