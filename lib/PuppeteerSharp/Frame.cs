using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

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
    /// var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
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
        private TaskCompletionSource<ElementHandle> _documentCompletionSource;
        private TaskCompletionSource<ExecutionContext> _contextResolveTaskWrapper;

        internal List<WaitTask> WaitTasks { get; }
        internal string Id { get; set; }
        internal string LoaderId { get; set; }
        internal List<string> LifecycleEvents { get; }
        internal string NavigationURL { get; private set; }

        internal Frame(CDPSession client, Frame parentFrame, string frameId)
        {
            _client = client;
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
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionAsync(script).ConfigureAwait(false);
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
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionAsync<T>(script).ConfigureAwait(false);
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
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateFunctionAsync(script, args).ConfigureAwait(false);
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
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateFunctionAsync<T>(script, args).ConfigureAwait(false);
        }

        /// <summary>
        /// Passes an expression to the <see cref="ExecutionContext.EvaluateExpressionHandleAsync(string)"/>, returns a <see cref="Task"/>, then <see cref="ExecutionContext.EvaluateExpressionHandleAsync(string)"/> would wait for the <see cref="Task"/> to resolve and return its value.
        /// </summary>
        /// <example>
        /// <code>
        /// var frame = page.MainFrame;
        /// const handle = Page.MainFrame.EvaluateExpressionHandleAsync("1 + 2");
        /// </code>
        /// </example>
        /// <returns>Resolves to the return value of <paramref name="script"/></returns>
        /// <param name="script">Expression to be evaluated in the <seealso cref="ExecutionContext"/></param>
        public async Task<JSHandle> EvaluateExpressionHandleAsync(string script)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionHandleAsync(script).ConfigureAwait(false);
        }

        /// <summary>
        /// Passes a function to the <see cref="ExecutionContext.EvaluateFunctionAsync(string, object[])"/>, returns a <see cref="Task"/>, then <see cref="ExecutionContext.EvaluateFunctionHandleAsync(string, object[])"/> would wait for the <see cref="Task"/> to resolve and return its value.
        /// </summary>
        /// <example>
        /// <code>
        /// var frame = page.MainFrame;
        /// const handle = Page.MainFrame.EvaluateFunctionHandleAsync("() => Promise.resolve(self)");
        /// return handle; // Handle for the global object.
        /// </code>
        /// <see cref="JSHandle"/> instances can be passed as arguments to the <see cref="ExecutionContext.EvaluateFunctionAsync(string, object[])"/>:
        /// 
        /// const handle = await Page.MainFrame.EvaluateExpressionHandleAsync("document.body");
        /// const resultHandle = await Page.MainFrame.EvaluateFunctionHandleAsync("body => body.innerHTML", handle);
        /// return await resultHandle.JsonValueAsync(); // prints body's innerHTML
        /// </example>
        /// <returns>Resolves to the return value of <paramref name="function"/></returns>
        /// <param name="function">Function to be evaluated in the <see cref="ExecutionContext"/></param>
        /// <param name="args">Arguments to pass to <paramref name="function"/></param>
        public async Task<JSHandle> EvaluateFunctionHandleAsync(string function, params object[] args)
        {
            var context = await GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateFunctionHandleAsync(function, args).ConfigureAwait(false);
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
        /// <seealso cref="WaitForXPathAsync(string, WaitForSelectorOptions)"/>
        /// <seealso cref="Page.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
        public Task<ElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
            => WaitForSelectorOrXPathAsync(selector, false, options);

        /// <summary>
        /// Waits for a selector to be added to the DOM
        /// </summary>
        /// <param name="xpath">A xpath selector of an element to wait for</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <returns>A task that resolves when element specified by selector string is added to DOM</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
        /// var page = await browser.NewPageAsync();
        /// string currentURL = null;
        /// page.MainFrame
        ///     .WaitForXPathAsync("//img")
        ///     .ContinueWith(_ => Console.WriteLine("First URL with image: " + currentURL));
        /// foreach (var current in new[] { "https://example.com", "https://google.com", "https://bbc.com" })
        /// {
        ///     currentURL = current;
        ///     await page.GoToAsync(currentURL);
        /// }
        /// await browser.CloseAsync();
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
        /// <seealso cref="Page.WaitForXPathAsync(string, WaitForSelectorOptions)"/>
        public Task<ElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
            => WaitForSelectorOrXPathAsync(xpath, true, options);

        /// <summary>
        /// Waits for a timeout
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns>A task that resolves when after the timeout</returns>
        /// <seealso cref="Page.WaitForTimeoutAsync(int)"/>
        public Task WaitForTimeoutAsync(int milliseconds) => Task.Delay(milliseconds);

        /// <summary>
        /// Waits for a function to be evaluated to a truthy value
        /// </summary>
        /// <param name="script">Function to be evaluated in browser context</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <param name="args">Arguments to pass to <c>script</c></param>
        /// <returns>A task that resolves when the <c>script</c> returns a truthy value</returns>
        /// <seealso cref="Page.WaitForFunctionAsync(string, WaitForFunctionOptions, object[])"/>
        public Task<JSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args)
            => new WaitTask(this, script, false, "function", options.Polling, options.PollingInterval, options.Timeout, args).Task;

        /// <summary>
        /// Waits for an expression to be evaluated to a truthy value
        /// </summary>
        /// <param name="script">Expression to be evaluated in browser context</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <returns>A task that resolves when the <c>script</c> returns a truthy value</returns>
        /// <seealso cref="Page.WaitForExpressionAsync(string, WaitForFunctionOptions)"/>
        public Task<JSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options)
            => new WaitTask(this, script, true, "function", options.Polling, options.PollingInterval, options.Timeout).Task;

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
                for (const option of options) {
                    option.selected = values.includes(option.value);
                    if (option.selected && !element.multiple)
                      break;
                }
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
            var document = await GetDocument().ConfigureAwait(false);
            var value = await document.QuerySelectorAsync(selector).ConfigureAwait(false);
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
            var document = await GetDocument().ConfigureAwait(false);
            var value = await document.QuerySelectorAllAsync(selector).ConfigureAwait(false);
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
            var document = await GetDocument().ConfigureAwait(false);
            var value = await document.XPathAsync(expression).ConfigureAwait(false);
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
                string contents = await AsyncFileHelper.ReadAllText(options.Path).ConfigureAwait(false);
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

        /// <summary>
        /// Adds a <c><![CDATA[<script>]]></c> tag into the page with the desired url or content
        /// </summary>
        /// <param name="options">add script tag options</param>
        /// <returns>Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame</returns>
        /// <seealso cref="Page.AddScriptTagAsync(AddTagOptions)"/>
        /// <seealso cref="Page.AddScriptTagAsync(string)"/>
        public async Task<ElementHandle> AddScriptTag(AddTagOptions options)
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
                string contents = await AsyncFileHelper.ReadAllText(options.Path).ConfigureAwait(false);
                contents += "//# sourceURL=" + options.Path.Replace("\n", string.Empty);
                return await AddScriptTagPrivate(addScriptContent, contents, options.Type).ConfigureAwait(false);
            }

            if (!string.IsNullOrEmpty(options.Content))
            {
                return await AddScriptTagPrivate(addScriptContent, options.Content, options.Type).ConfigureAwait(false);
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

        internal async Task<ElementHandle> WaitForSelectorOrXPathAsync(string selectorOrXPath, bool isXPath, WaitForSelectorOptions options = null)
        {
            options = options ?? new WaitForSelectorOptions();
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
            var handle = await new WaitTask(
                this,
                predicate,
                false,
                $"{(isXPath ? "XPath" : "selector")} '{selectorOrXPath}'",
                options.Polling,
                options.PollingInterval,
                options.Timeout,
                new object[]
                {
                    selectorOrXPath,
                    isXPath,
                    options.Visible,
                    options.Hidden
            }).Task.ConfigureAwait(false);
            return handle as ElementHandle;
        }

        internal void OnLoadingStopped()
        {
            LifecycleEvents.Add("DOMContentLoaded");
            LifecycleEvents.Add("load");
        }

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
            NavigationURL = framePayload.Url;
            Url = framePayload.Url;
        }

        internal void NavigatedWithinDocument(string url) => Url = url;

        internal void SetDefaultContext(ExecutionContext context)
        {
            if (context != null)
            {
                _contextResolveTaskWrapper.SetResult(context);

                foreach (var waitTask in WaitTasks)
                {
                    _ = waitTask.Rerun();
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
                WaitTasks[0].Termiante(new Exception("waitForFunction failed: frame got detached."));
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
                var context = await GetExecutionContextAsync().ConfigureAwait(false);
                var document = await context.EvaluateExpressionHandleAsync("document").ConfigureAwait(false);
                _documentCompletionSource.SetResult(document as ElementHandle);
            }
            return await _documentCompletionSource.Task.ConfigureAwait(false);
        }

        #endregion
    }
}