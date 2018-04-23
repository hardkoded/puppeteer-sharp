using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Input;

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

        public async Task<dynamic> EvaluateExpressionAsync(string script)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateExpressionAsync(script);
        }

        public async Task<T> EvaluateExpressionAsync<T>(string script)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateExpressionAsync<T>(script);
        }

        public async Task<dynamic> EvaluateFunctionAsync(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateFunctionAsync(script, args);
        }

        public async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateFunctionAsync<T>(script, args);
        }

        /// <summary>
        /// Gets the execution context associated with the frame.
        /// </summary>
        /// <returns>Execution context associated with the frame.</returns>
        public Task<ExecutionContext> GetExecutionContextAsync() => ContextResolveTaskWrapper.Task;

        internal async Task<ElementHandle> GetElementAsync(string selector)
        {
            var document = await GetDocument();
            var value = await document.GetElementAsync(selector);
            return value;
        }

        internal Task<object> Eval(string selector, Func<object> pageFunction, object[] args)
        {
            throw new NotImplementedException();
        }

        internal Task<object> Eval(string selector, string pageFunction, object[] args)
        {
            throw new NotImplementedException();
        }

        internal Task<object> EvalMany(string selector, Func<object> pageFunction, object[] args)
        {
            throw new NotImplementedException();
        }

        internal Task<object> EvalMany(string selector, string pageFunction, object[] args)
        {
            throw new NotImplementedException();
        }

        internal Task<IEnumerable<ElementHandle>> GetElementsAsync(string selector)
        {
            throw new NotImplementedException();
        }

        internal Task<ElementHandle> AddStyleTag(dynamic options)
        {
            throw new NotImplementedException();
        }

        internal Task<ElementHandle> AddScriptTag(dynamic options)
        {
            throw new NotImplementedException();
        }

        internal async Task<string> GetContentAsync()
        {
            return await EvaluateFunctionAsync<string>(@"() => {
                let retVal = '';
                if (document.doctype)
                    retVal = new XMLSerializer().serializeToString(document.doctype);
                if (document.documentElement)
                    retVal += document.documentElement.outerHTML;
                return retVal;
            }");
        }

        internal async Task SetContentAsync(string html)
        {
            await EvaluateFunctionAsync(@"html => {
                document.open();
                document.write(html);
                document.close();
            }", html);
        }

        internal async Task<string> GetTitleAsync() => await EvaluateExpressionAsync<string>("document.title");

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