using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Provides methods to interact with a single page frame in Chromium. One <see cref="Page"/> instance might have multiple <see cref="Frame"/> instances.
    /// At every point of time, page exposes its current frame tree via the <see cref="Page.MainFrame"/> and <see cref="ChildFrames"/> properties.
    /// 
    /// <see cref="Frame"/> object's lifecycle is controlled by three events, dispatched on the page object
    /// - <see cref="Page.FrameAttached"/> - fires when the frame gets attached to the page. A Frame can be attached to the page only once
    /// - <see cref="Page.FrameNavigated"/> - fired when the frame commits navigation to a different URL
    /// - <see cref="Page.FrameDetached"/> - fired when the frame gets detached from the page.  A Frame can be detached from the page only once
    /// </summary>
    /// <example>
    /// An example of dumping frame tree
    /// <code>
    /// <![CDATA[
    /// var browser = await Puppeteer.LaunchAsync(new LaunchOptions(), Downloader.DefaultRevision);
    /// var page = await browser.NewPageAsync();
    /// await page.GoToAsync("https://www.google.com/chrome/browser/canary.html");
    /// dumpFrameTree(page.MainFrame, string.Empty);
    /// await browser.CloseAsync();
    /// 
    /// void dumpFrameTree(Frame frame, string indent)
    /// {
    ///     Console.WriteLine(indent + frame.Url);
    ///     foreach (var child in frame.ChildFrames)
    ///     {
    ///         dumpFrameTree(child, indent + "  ");
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class Frame
    {
        private readonly CDPSession _client;
        private readonly Page _page;
        private TaskCompletionSource<ElementHandle> _documentCompletionSource;
        private TaskCompletionSource<ExecutionContext> _contextResolveTaskWrapper;

        internal List<WaitTask> WaitTasks { get; }
        internal string Id { get; set; }
        internal string LoaderId { get; set; }
        internal List<string> LifecycleEvents { get; }

        internal Frame(CDPSession client, Page page, Frame parentFrame, string frameId)
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
        /// <summary>
        /// Gets the child frames of the this frame
        /// </summary>
        public List<Frame> ChildFrames { get; } = new List<Frame>();

        /// <summary>
        /// Gets the frame's name attribute as specified in the tag
        /// If the name is empty, returns the id attribute instead
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the frame's url
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets a value indicating if the frame is detached or not
        /// </summary>
        public bool Detached { get; set; }

        /// <summary>
        /// Gets the parent frame, if any. Detached frames and main frames return <c>null</c>
        /// </summary>
        public Frame ParentFrame { get; private set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        /// <seealso cref="EvaluateFunctionAsync(string, object[])"/>
        /// <seealso cref="Page.EvaluateExpressionAsync(string)"/>
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
        /// <returns>Task which resolves to script return value</returns>
        /// <seealso cref="EvaluateFunctionAsync{T}(string, object[])"/>
        /// <seealso cref="Page.EvaluateExpressionAsync{T}(string)"/>
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
        /// <returns>Task which resolves to script return value</returns>
        /// <seealso cref="EvaluateExpressionAsync(string)"/>
        /// <seealso cref="Page.EvaluateFunctionAsync(string, object[])"/>
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
        /// <returns>Task which resolves to script return value</returns>
        /// <seealso cref="EvaluateExpressionAsync{T}(string)"/>
        /// <seealso cref="Page.EvaluateFunctionAsync{T}(string, object[])"/>
        public async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateFunctionAsync<T>(script, args);
        }

        /// <summary>
        /// Gets the <see cref="ExecutionContext"/> associated with the frame.
        /// </summary>
        /// <returns><see cref="ExecutionContext"/> associated with the frame.</returns>
        public Task<ExecutionContext> GetExecutionContextAsync() => _contextResolveTaskWrapper.Task;

        /// <summary>
        /// Waits for a selector to be added to the DOM
        /// </summary>
        /// <param name="selector">A selector of an element to wait for</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <returns>A task that resolves when element specified by selector string is added to DOM</returns>
        /// <seealso cref="Page.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
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
            return handle as ElementHandle;
        }

        /// <summary>
        /// Waits for a timeout
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns>A task that resolves when after the timeout</returns>
        /// <seealso cref="Page.WaitForTimeoutAsync(int)"/>
        public Task WaitForTimeoutAsync(int milliseconds) => Task.Delay(milliseconds);

        /// <summary>
        /// Waits for a script to be evaluated to a truthy value
        /// </summary>
        /// <param name="script">Function to be evaluated in browser context</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <param name="args">Arguments to pass to <c>script</c></param>
        /// <returns>A task that resolves when the <c>script</c> returns a truthy value</returns>
        /// <seealso cref="Page.WaitForFunctionAsync(string, WaitForFunctionOptions, object[])"/>
        public Task<JSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args)
            => new WaitTask(this, script, options.Polling, options.PollingInterval, options.Timeout, args).Task;

        /// <summary>
        /// Triggers a change and input event once all the provided options have been selected. 
        /// If there's no <![CDATA[<select>]]> element matching selector, the method throws an error.
        /// </summary>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/></exception>        
        /// <param name="selector">A selector to query page for</param>
        /// <param name="values">Values of options to select. If the <![CDATA[<select>]]> has the multiple attribute, 
        /// all values are considered, otherwise only the first one is taken into account.</param>
        /// <returns>Returns an array of option values that have been successfully selected.</returns>
        /// <seealso cref="Page.SelectAsync(string, string[])"/>
        public Task<string[]> SelectAsync(string selector, params string[] values)
            => QuerySelectorAsync(selector).EvaluateFunctionAsync<string[]>(@"(element, values) => {
                if (element.nodeName.toLowerCase() !== 'select')
                    throw new Error('Element is not a <select> element.');

                const options = Array.from(element.options);
                element.value = undefined;
                for (const option of options)
                    option.selected = values.includes(option.value);
                element.dispatchEvent(new Event('input', { 'bubbles': true }));
                element.dispatchEvent(new Event('change', { 'bubbles': true }));
                return options.filter(option => option.selected).map(option => option.value);
            }", new[] { values });

        /// <summary>
        /// Queries frame for the selector. If there's no such element within the frame, the method will resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">Selector to query frame for</param>
        /// <returns>Task which resolves to <see cref="ElementHandle"/> pointing to the frame element</returns>
        /// <seealso cref="Page.QuerySelectorAsync(string)"/>
        public async Task<ElementHandle> QuerySelectorAsync(string selector)
        {
            var document = await GetDocument();
            var value = await document.QuerySelectorAsync(selector);
            return value;
        }

        /// <summary>
        /// Queries frame for the selector. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="selector">A selector to query frame for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        /// <seealso cref="Page.QuerySelectorAllAsync(string)"/>
        public async Task<ElementHandle[]> QuerySelectorAllAsync(string selector)
        {
            var document = await GetDocument();
            var value = await document.QuerySelectorAllAsync(selector);
            return value;
        }

        /// <summary>
        /// Evaluates the XPath expression
        /// </summary>
        /// <param name="expression">Expression to evaluate <see href="https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate"/></param>
        /// <returns>Task which resolves to an array of <see cref="ElementHandle"/></returns>
        /// <seealso cref="Page.XPathAsync(string)"/>
        public async Task<ElementHandle[]> XPathAsync(string expression)
        {
            var document = await GetDocument();
            var value = await document.XPathAsync(expression);
            return value;
        }

        /// <summary>
        /// Adds a <c><![CDATA[<link rel="stylesheet">]]></c> tag into the page with the desired url or a <c><![CDATA[<link rel="stylesheet">]]></c> tag with the content
        /// </summary>
        /// <param name="options">add style tag options</param>
        /// <returns>Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame</returns>
        /// <seealso cref="Page.AddStyleTagAsync(AddTagOptions)"/>
        /// <seealso cref="Page.AddStyleTagAsync(string)"/>
        public async Task<ElementHandle> AddStyleTag(AddTagOptions options)
        {
            const string addStyleUrl = @"async function addStyleUrl(url) {
              const link = document.createElement('link');
              link.rel = 'stylesheet';
              link.href = url;
              document.head.appendChild(link);
              await new Promise((res, rej) => {
                link.onload = res;
                link.onerror = rej;
              });
              return link;
            }";
            const string addStyleContent = @"function addStyleContent(content) {
              const style = document.createElement('style');
              style.type = 'text/css';
              style.appendChild(document.createTextNode(content));
              document.head.appendChild(style);
              return style;
            }";

            if (!string.IsNullOrEmpty(options.Url))
            {
                var url = options.Url;
                try
                {
                    var context = await GetExecutionContextAsync();
                    return (await context.EvaluateFunctionHandleAsync(addStyleUrl, url)) as ElementHandle;
                }
                catch (PuppeteerException)
                {
                    throw new PuppeteerException($"Loading style from {url} failed");
                }
            }

            if (!string.IsNullOrEmpty(options.Path))
            {
                var contents = File.ReadAllText(options.Path, Encoding.UTF8);
                contents += "//# sourceURL=" + options.Path.Replace("\n", string.Empty);
                var context = await GetExecutionContextAsync();
                return (await context.EvaluateFunctionHandleAsync(addStyleContent, contents)) as ElementHandle;
            }

            if (!string.IsNullOrEmpty(options.Content))
            {
                var context = await GetExecutionContextAsync();
                return (await context.EvaluateFunctionHandleAsync(addStyleContent, options.Content)) as ElementHandle;
            }

            throw new ArgumentException("Provide options with a `Url`, `Path` or `Content` property");
        }

        /// <summary>
        /// Adds a <c><![CDATA[<script>]]></c> tag into the page with the desired url or content
        /// </summary>
        /// <param name="options">add script tag options</param>
        /// <returns>Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame</returns>
        /// <seealso cref="Page.AddScriptTagAsync(AddTagOptions)"/>
        /// <seealso cref="Page.AddScriptTagAsync(string)"/>
        public async Task<ElementHandle> AddScriptTag(AddTagOptions options)
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
                    return (await context.EvaluateFunctionHandleAsync(addScriptUrl, url)) as ElementHandle;
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
                return (await context.EvaluateFunctionHandleAsync(addScriptContent, contents)) as ElementHandle;
            }

            if (!string.IsNullOrEmpty(options.Content))
            {
                var context = await GetExecutionContextAsync();
                return (await context.EvaluateFunctionHandleAsync(addScriptContent, options.Content)) as ElementHandle;
            }

            throw new ArgumentException("Provide options with a `Url`, `Path` or `Content` property");
        }

        /// <summary>
        /// Gets the full HTML contents of the page, including the doctype.
        /// </summary>
        /// <returns>Task which resolves to the HTML content.</returns>
        /// <seealso cref="Page.GetContentAsync"/>
        public Task<string> GetContentAsync()
            => EvaluateFunctionAsync<string>(@"() => {
                let retVal = '';
                if (document.doctype)
                    retVal = new XMLSerializer().serializeToString(document.doctype);
                if (document.documentElement)
                    retVal += document.documentElement.outerHTML;
                return retVal;
            }");

        /// <summary>
        /// Sets the HTML markup to the page
        /// </summary>
        /// <param name="html">HTML markup to assign to the page.</param>
        /// <returns>Task.</returns>
        /// <seealso cref="Page.SetContentAsync(string)"/>
        public Task SetContentAsync(string html)
            => EvaluateFunctionAsync(@"html => {
                document.open();
                document.write(html);
                document.close();
            }", html);

        /// <summary>
        /// Returns page's title
        /// </summary>
        /// <returns>page's title</returns>
        /// <seealso cref="Page.GetTitleAsync"/>
        public Task<string> GetTitleAsync() => EvaluateExpressionAsync<string>("document.title");

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
                _contextResolveTaskWrapper.SetResult(context);

                foreach (var waitTask in WaitTasks)
                {
                    waitTask.Rerun();
                }
            }
            else
            {
                _documentCompletionSource = null;
                _contextResolveTaskWrapper = new TaskCompletionSource<ExecutionContext>();
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

        #endregion

        #region Private Methods

        private async Task<ElementHandle> GetDocument()
        {
            if (_documentCompletionSource == null)
            {
                _documentCompletionSource = new TaskCompletionSource<ElementHandle>();
                var context = await GetExecutionContextAsync();
                var document = await context.EvaluateExpressionHandleAsync("document");
                _documentCompletionSource.SetResult(document as ElementHandle);
            }
            return await _documentCompletionSource.Task;
        }

        #endregion
    }
}