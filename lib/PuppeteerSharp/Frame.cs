using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace PuppeteerSharp
{
    public class Frame
    {
        private Session _client;
        private Page _page;
        private string _url = string.Empty;
        private TaskCompletionSource<ElementHandle> _documentCompletionSource;

        internal List<WaitTask> WaitTasks { get; }

        public Frame(Session client, Page page, Frame parentFrame, string frameId)
        {
            _client = client;
            _page = page;
            ParentFrame = parentFrame;
            Id = frameId;

            if (parentFrame != null)
            {
                ParentFrame.ChildFrames.Add(this);
            }

            SetDefaultContext(null);

            WaitTasks = new List<WaitTask>();
            LifecycleEvents = new List<string>();
        }

        #region Properties
        public List<Frame> ChildFrames { get; set; } = new List<Frame>();
        public string Name { get; set; }

        public string Url { get; set; }
        public string ParentId { get; internal set; }
        public string Id { get; internal set; }
        public string LoaderId { get; set; }
        public TaskCompletionSource<ExecutionContext> ContextResolveTaskWrapper { get; internal set; }
        public List<string> LifecycleEvents { get; internal set; }
        public bool Detached { get; set; }
        public Frame ParentFrame { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <seealso cref="EvaluateFunctionAsync(string, object[])"/>
        /// <returns>Task which resolves to script return value</returns>
        public async Task<dynamic> EvaluateExpressionAsync(string script)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateExpressionAsync(script);
        }

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <seealso cref="EvaluateFunctionAsync{T}(string, object[])"/>
        /// <returns>Task which resolves to script return value</returns>
        public async Task<T> EvaluateExpressionAsync<T>(string script)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateExpressionAsync<T>(script);
        }

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="JSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <seealso cref="EvaluateExpressionAsync(string)"/>
        /// <returns>Task which resolves to script return value</returns>
        public async Task<dynamic> EvaluateFunctionAsync(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateFunctionAsync(script, args);
        }

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="JSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <seealso cref="EvaluateExpressionAsync{T}(string)"/>
        /// <returns>Task which resolves to script return value</returns>
        public async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateFunctionAsync<T>(script, args);
        }

        /// <summary>
        /// Gets the <see cref="ExecutionContext"/> associated with the frame.
        /// </summary>
        /// <returns><see cref="ExecutionContext"/> associated with the frame.</returns>
        public Task<ExecutionContext> GetExecutionContextAsync() => ContextResolveTaskWrapper.Task;

        /// <summary>
        /// Queries frame for the selector. If there's no such element within the frame, the method will resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">Selector to query page for</param>
        /// <returns>Task which resolves to <see cref="ElementHandle"/> pointing to the frame element</returns>
        internal async Task<ElementHandle> QuerySelectorAsync(string selector)
        {
            var document = await GetDocument();
            var value = await document.QuerySelectorAsync(selector);
            return value;
        }

        internal Task<object> EvalMany(string selector, Func<object> pageFunction, object[] args)
        {
            throw new NotImplementedException();
        }

        internal Task<object> EvalMany(string selector, string pageFunction, object[] args)
        {
            throw new NotImplementedException();
        }

        internal async Task<ElementHandle[]> QuerySelectorAllAsync(string selector)
        {
            var document = await GetDocument();
            var value = await document.QuerySelectorAllAsync(selector);
            return value;
        }

        internal Task<ElementHandle> AddStyleTag(dynamic options)
        {
            throw new NotImplementedException();
        }

        internal async Task<ElementHandle> AddScriptTag(AddScriptTagOptions options)
        {
            const string addScriptUrl = @"async function addScriptUrl(url) {
              const script = document.createElement('script');
              script.src = url;
              document.head.appendChild(script);
              await new Promise((res, rej) => {
                script.onload = res;
                script.onerror = rej;
              });
              return script;
            }";
            const string addScriptContent = @"function addScriptContent(content) {
              const script = document.createElement('script');
              script.type = 'text/javascript';
              script.text = content;
              document.head.appendChild(script);
              return script;
            }";

            if (!string.IsNullOrEmpty(options.Url))
            {
                var url = options.Url;
                try
                {
                    var context = await GetExecutionContextAsync();
                    return (await context.EvaluateFunctionHandleAsync(addScriptUrl, url)).AsElement();
                }
                catch (PuppeteerException)
                {
                    throw new PuppeteerException($"Loading script from {url} failed");
                }
            }

            if (!string.IsNullOrEmpty(options.Path))
            {
                var contents = File.ReadAllText(options.Path, Encoding.UTF8);
                contents += "//# sourceURL=" + options.Path.Replace("\n", string.Empty);
                var context = await GetExecutionContextAsync();
                return (await context.EvaluateFunctionHandleAsync(addScriptContent, contents)).AsElement();
            }

            if (!string.IsNullOrEmpty(options.Content))
            {
                var context = await GetExecutionContextAsync();
                return (await context.EvaluateFunctionHandleAsync(addScriptContent, options.Content)).AsElement();
            }

            throw new ArgumentException("Provide options with a `Url`, `Path` or `Content` property");
        }

        internal Task<string> GetContentAsync()
            => EvaluateFunctionAsync<string>(@"() => {
                let retVal = '';
                if (document.doctype)
                    retVal = new XMLSerializer().serializeToString(document.doctype);
                if (document.documentElement)
                    retVal += document.documentElement.outerHTML;
                return retVal;
            }");

        internal Task SetContentAsync(string html)
            => EvaluateFunctionAsync(@"html => {
                document.open();
                document.write(html);
                document.close();
            }", html);


        internal Task<string> GetTitleAsync() => EvaluateExpressionAsync<string>("document.title");

        internal void OnLifecycleEvent(string loaderId, string name)
        {
            if (name == "init")
            {
                LoaderId = loaderId;
                LifecycleEvents.Clear();
            }
            LifecycleEvents.Add(name);
        }

        internal void Navigated(FramePayload framePayload)
        {
            Name = framePayload.Name ?? string.Empty;
            Url = framePayload.Url;
        }

        internal void SetDefaultContext(ExecutionContext context)
        {
            if (context != null)
            {
                ContextResolveTaskWrapper.SetResult(context);

                foreach (var waitTask in WaitTasks)
                {
                    waitTask.Rerun();
                }
            }
            else
            {
                _documentCompletionSource = null;
                ContextResolveTaskWrapper = new TaskCompletionSource<ExecutionContext>();
            }
        }

        internal void Detach()
        {
            while (WaitTasks.Count > 0)
            {
                WaitTasks[0].Termiante(new Exception("waitForSelector failed: frame got detached."));
            }
            Detached = true;
            if (ParentFrame != null)
            {
                ParentFrame.ChildFrames.Remove(this);
            }
            ParentFrame = null;
        }

        internal Task WaitForTimeoutAsync(int milliseconds) => Task.Delay(milliseconds);

        internal Task<JSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args)
            => new WaitTask(this, script, options.Polling, options.PollingInterval, options.Timeout, args).Task;

        /// <summary>
        /// Waits for a selector to be added to the DOM
        /// </summary>
        /// <param name="selector">A selector of an element to wait for</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <returns>A task that resolves when element specified by selector string is added to DOM</returns>
        public async Task<ElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
        {
            options = options ?? new WaitForSelectorOptions();
            const string predicate = @"
              function predicate(selector, waitForVisible, waitForHidden) {
              const node = document.querySelector(selector);
              if (!node)
                return waitForHidden;
              if (!waitForVisible && !waitForHidden)
                return node;
              const style = window.getComputedStyle(node);
              const isVisible = style && style.visibility !== 'hidden' && hasVisibleBoundingBox();
              const success = (waitForVisible === isVisible || waitForHidden === !isVisible);
              return success ? node : null;

              function hasVisibleBoundingBox() {
                const rect = node.getBoundingClientRect();
                return !!(rect.top || rect.bottom || rect.width || rect.height);
              }
            }";
            var polling = options.Visible || options.Hidden ? WaitForFunctionPollingOption.Raf : WaitForFunctionPollingOption.Mutation;
            var handle = await WaitForFunctionAsync(predicate, new WaitForFunctionOptions
            {
                Timeout = options.Timeout,
                Polling = polling
            }, selector, options.Visible, options.Hidden);
            return handle.AsElement();
        }

        internal async Task<string[]> SelectAsync(string selector, object[] args)
        {
            return await QuerySelectorAsync(selector).EvaluateFunctionAsync<string[]>(@"(element, values) => {
                if (element.nodeName.toLowerCase() !== 'select')
                    throw new Error('Element is not a <select> element.');

                const options = Array.from(element.options);
                element.value = undefined;
                for (const option of options)
                    option.selected = values.includes(option.value);
                element.dispatchEvent(new Event('input', { 'bubbles': true }));
                element.dispatchEvent(new Event('change', { 'bubbles': true }));
                return options.filter(option => option.selected).map(option => option.value);
            }", args.Select(i => i.ToString()));
        }

        #endregion

        #region Private Methods

        private async Task<ElementHandle> GetDocument()
        {
            if (_documentCompletionSource == null)
            {
                _documentCompletionSource = new TaskCompletionSource<ElementHandle>();
                var context = await GetExecutionContextAsync();
                var document = await context.EvaluateExpressionHandleAsync("document");
                _documentCompletionSource.SetResult(document.AsElement());
            }
            return await _documentCompletionSource.Task;
        }

        #endregion
    }
}