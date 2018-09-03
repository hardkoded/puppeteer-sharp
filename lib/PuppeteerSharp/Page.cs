using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;
using PuppeteerSharp.Media;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.PageCoverage;

namespace PuppeteerSharp
{
    /// <summary>
    /// Provides methods to interact with a single tab in Chromium. One <see cref="Browser"/> instance might have multiple <see cref="Page"/> instances.
    /// </summary>
    /// <example>
    /// This example creates a page, navigates it to a URL, and then saves a screenshot:
    /// <code>
    /// var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
    /// var page = await browser.NewPageAsync();
    /// await page.GoToAsync("https://example.com");
    /// await page.ScreenshotAsync("screenshot.png");
    /// await browser.CloseAsync();
    /// </code>
    /// </example>
    [DebuggerDisplay("Page {Url}")]
    public class Page : IDisposable
    {
        private readonly bool _ignoreHTTPSErrors;
        private readonly NetworkManager _networkManager;
        private readonly FrameManager _frameManager;
        private readonly TaskQueue _screenshotTaskQueue;
        private readonly EmulationManager _emulationManager;
        private readonly Dictionary<string, Delegate> _pageBindings;
        private readonly Dictionary<string, Worker> _workers;
        private readonly ILogger _logger;

        private static readonly Dictionary<string, decimal> _unitToPixels = new Dictionary<string, decimal> {
            {"px", 1},
            {"in", 96},
            {"cm", 37.8m},
            {"mm", 3.78m}
        };

        private Page(CDPSession client, Target target, FrameTree frameTree, bool ignoreHTTPSErrors, TaskQueue screenshotTaskQueue)
        {
            Client = client;
            Target = target;
            Keyboard = new Keyboard(client);
            Mouse = new Mouse(client, Keyboard);
            Touchscreen = new Touchscreen(client, Keyboard);
            Tracing = new Tracing(client);
            Coverage = new Coverage(client);

            _frameManager = new FrameManager(client, frameTree, this);
            _networkManager = new NetworkManager(client, _frameManager);
            _emulationManager = new EmulationManager(client);
            _pageBindings = new Dictionary<string, Delegate>();
            _workers = new Dictionary<string, Worker>();
            _logger = Client.Connection.LoggerFactory.CreateLogger<Page>();

            _ignoreHTTPSErrors = ignoreHTTPSErrors;

            _screenshotTaskQueue = screenshotTaskQueue;

            _frameManager.FrameAttached += (sender, e) => FrameAttached?.Invoke(this, e);
            _frameManager.FrameDetached += (sender, e) => FrameDetached?.Invoke(this, e);
            _frameManager.FrameNavigated += (sender, e) => FrameNavigated?.Invoke(this, e);

            _networkManager.Request += (sender, e) => Request?.Invoke(this, e);
            _networkManager.RequestFailed += (sender, e) => RequestFailed?.Invoke(this, e);
            _networkManager.Response += (sender, e) => Response?.Invoke(this, e);
            _networkManager.RequestFinished += (sender, e) => RequestFinished?.Invoke(this, e);

            target.CloseTask.ContinueWith((arg) =>
            {
                Close?.Invoke(this, EventArgs.Empty);
                IsClosed = true;
            });

            Client.MessageReceived += Client_MessageReceived;
        }

        internal CDPSession Client { get; }

        #region Public Properties

        /// <summary>
        /// Raised when the JavaScript <c>load</c> <see href="https://developer.mozilla.org/en-US/docs/Web/Events/load"/> event is dispatched.
        /// </summary>
        public event EventHandler Load;

        /// <summary>
        /// Raised when the page crashes
        /// </summary>
        public event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// Raised when the JavaScript code makes a call to <c>console.timeStamp</c>. For the list of metrics see <see cref="Page.MetricsAsync"/>.
        /// </summary>
        public event EventHandler<MetricEventArgs> Metrics;

        /// <summary>
        /// Raised when a JavaScript dialog appears, such as <c>alert</c>, <c>prompt</c>, <c>confirm</c> or <c>beforeunload</c>. Puppeteer can respond to the dialog via <see cref="Dialog"/>'s <see cref="Dialog.Accept(string)"/> or <see cref="Dialog.Dismiss"/> methods.
        /// </summary>
        public event EventHandler<DialogEventArgs> Dialog;

        /// <summary>
        /// Raised when the JavaScript <c>DOMContentLoaded</c> <see href="https://developer.mozilla.org/en-US/docs/Web/Events/DOMContentLoaded"/> event is dispatched.
        /// </summary>
        public event EventHandler DOMContentLoaded;

        /// <summary>
        /// Raised when JavaScript within the page calls one of console API methods, e.g. <c>console.log</c> or <c>console.dir</c>. Also emitted if the page throws an error or a warning.
        /// The arguments passed into <c>console.log</c> appear as arguments on the event handler.
        /// </summary>
        /// <example>
        /// An example of handling <see cref="Console"/> event:
        /// <code>
        /// <![CDATA[
        /// page.Console += (sender, e) => 
        /// {
        ///     for (var i = 0; i < e.Message.Args.Count; ++i)
        ///     {
        ///         System.Console.WriteLine($"{i}: {e.Message.Args[i]}");
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public event EventHandler<ConsoleEventArgs> Console;

        /// <summary>
        /// Raised when a frame is attached.
        /// </summary>
        public event EventHandler<FrameEventArgs> FrameAttached;

        /// <summary>
        /// Raised when a frame is detached.
        /// </summary>
        public event EventHandler<FrameEventArgs> FrameDetached;

        /// <summary>
        /// Raised when a frame is navigated to a new url.
        /// </summary>
        public event EventHandler<FrameEventArgs> FrameNavigated;

        /// <summary>
        /// Raised when a <see cref="Response"/> is received.
        /// </summary>
        public event EventHandler<ResponseCreatedEventArgs> Response;

        /// <summary>
        /// Raised when a page issues a request. The <see cref="Request"/> object is read-only.
        /// In order to intercept and mutate requests, see <see cref="SetRequestInterceptionAsync(bool)"/>
        /// </summary>
        public event EventHandler<RequestEventArgs> Request;

        /// <summary>
        /// Raised when a request finishes successfully.
        /// </summary>
        public event EventHandler<RequestEventArgs> RequestFinished;

        /// <summary>
        /// Raised when a request fails, for example by timing out.
        /// </summary>
        public event EventHandler<RequestEventArgs> RequestFailed;

        /// <summary>
        /// Raised when an uncaught exception happens within the page.
        /// </summary>
        public event EventHandler<PageErrorEventArgs> PageError;

        /// <summary>
        /// Emitted when a dedicated WebWorker (<see href="https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API"/>) is spawned by the page.
        /// </summary>
        public event EventHandler<WorkerEventArgs> WorkerCreated;

        /// <summary>
        /// Emitted when a dedicated WebWorker (<see href="https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API"/>) is terminated.
        /// </summary>
        public event EventHandler<WorkerEventArgs> WorkerDestroyed;

        /// <summary>
        /// Raised when the page closes.
        /// </summary>
        public event EventHandler Close;

        /// <summary>
        /// This setting will change the default maximum navigation time of 30 seconds for the following methods:
        /// - <see cref="GoToAsync(string, NavigationOptions)"/>
        /// - <see cref="GoBackAsync(NavigationOptions)"/>
        /// - <see cref="GoForwardAsync(NavigationOptions)"/>
        /// - <see cref="ReloadAsync(NavigationOptions)"/>
        /// - <see cref="WaitForNavigationAsync(NavigationOptions)"/>
        /// </summary>
        public int DefaultNavigationTimeout { get; set; } = 30000;

        /// <summary>
        /// This setting will change the default maximum navigation time of 30 seconds for the following methods:
        /// - <see cref="WaitForOptions"/>
        /// </summary>
        public int DefaultWaitForTimeout { get; set; } = 30000;

        /// <summary>
        /// Gets page's main frame
        /// </summary>
        /// <remarks>
        /// Page is guaranteed to have a main frame which persists during navigations.
        /// </remarks>
        public Frame MainFrame => _frameManager.MainFrame;

        /// <summary>
        /// Gets all frames attached to the page.
        /// </summary>
        /// <value>An array of all frames attached to the page.</value>
        public Frame[] Frames => _frameManager.Frames.Values.ToArray();

        /// <summary>
        /// Gets all workers in the page.
        /// </summary>
        public Worker[] Workers => _workers.Values.ToArray();

        /// <summary>
        /// Shortcut for <c>page.MainFrame.Url</c>
        /// </summary>
        public string Url => MainFrame.Url;

        /// <summary>
        /// Gets that target this page was created from.
        /// </summary>
        public Target Target { get; }

        /// <summary>
        /// Gets this page's keyboard
        /// </summary>
        public Keyboard Keyboard { get; }

        /// <summary>
        /// Gets this page's touchscreen
        /// </summary>
        public Touchscreen Touchscreen { get; }

        /// <summary>
        /// Gets this page's coverage
        /// </summary>
        public Coverage Coverage { get; }

        /// <summary>
        /// Gets this page's tracing
        /// </summary>
        public Tracing Tracing { get; }

        /// <summary>
        /// Gets this page's mouse
        /// </summary>
        public Mouse Mouse { get; }

        /// <summary>
        /// Gets this page's viewport
        /// </summary>
        public ViewPortOptions Viewport { get; private set; }

        /// <summary>
        /// List of supported metrics provided by the <see cref="Metrics"/> event.
        /// </summary>
        public static readonly IEnumerable<string> SupportedMetrics = new List<string>
        {
            "Timestamp",
            "Documents",
            "Frames",
            "JSEventListeners",
            "Nodes",
            "LayoutCount",
            "RecalcStyleCount",
            "LayoutDuration",
            "RecalcStyleDuration",
            "ScriptDuration",
            "TaskDuration",
            "JSHeapUsedSize",
            "JSHeapTotalSize"
        };

        /// <summary>
        /// Get the browser the page belongs to.
        /// </summary>
        public Browser Browser => Target.Browser;

        /// <summary>
        /// Get an indication that the page has been closed.
        /// </summary>
        public bool IsClosed { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns metrics
        /// </summary>
        /// <returns>Task which resolves into a list of metrics</returns>
        /// <remarks>
        /// All timestamps are in monotonic time: monotonically increasing time in seconds since an arbitrary point in the past.
        /// </remarks>
        public async Task<Dictionary<string, decimal>> MetricsAsync()
        {
            var response = await Client.SendAsync<PerformanceGetMetricsResponse>("Performance.getMetrics").ConfigureAwait(false);
            return BuildMetricsObject(response.Metrics);
        }

        /// <summary>
        /// Fetches an element with <paramref name="selector"/>, scrolls it into view if needed, and then uses <see cref="Touchscreen"/> to tap in the center of the element.
        /// </summary>
        /// <param name="selector">A selector to search for element to tap. If there are multiple elements satisfying the selector, the first will be clicked.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/></exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully tapped</returns>
        public async Task TapAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }
            await handle.TapAsync().ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// The method runs <c>document.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">A selector to query page for</param>
        /// <returns>Task which resolves to <see cref="ElementHandle"/> pointing to the frame element</returns>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.QuerySelectorAsync(selector)</c>
        /// </remarks>
        /// <seealso cref="Frame.QuerySelectorAsync(string)"/>
        public Task<ElementHandle> QuerySelectorAsync(string selector)
            => MainFrame.QuerySelectorAsync(selector);

        /// <summary>
        /// Runs <c>document.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="selector">A selector to query page for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        /// <seealso cref="Frame.QuerySelectorAllAsync(string)"/>
        public Task<ElementHandle[]> QuerySelectorAllAsync(string selector)
            => MainFrame.QuerySelectorAllAsync(selector);

        /// <summary>
        /// A utility function to be used with <see cref="Extensions.EvaluateFunctionAsync{T}(Task{JSHandle}, string, object[])"/>
        /// </summary>
        /// <param name="selector">A selector to query page for</param>
        /// <returns>Task which resolves to a <see cref="JSHandle"/> of <c>document.querySelectorAll</c> result</returns>
        public Task<JSHandle> QuerySelectorAllHandleAsync(string selector)
            => EvaluateFunctionHandleAsync("selector => Array.from(document.querySelectorAll(selector))", selector);

        /// <summary>
        /// Evaluates the XPath expression
        /// </summary>
        /// <param name="expression">Expression to evaluate <see href="https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate"/></param>
        /// <returns>Task which resolves to an array of <see cref="ElementHandle"/></returns>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.XPathAsync(expression)</c>
        /// </remarks>
        /// <seealso cref="Frame.XPathAsync(string)"/>
        public Task<ElementHandle[]> XPathAsync(string expression) => MainFrame.XPathAsync(expression);

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        public async Task<JSHandle> EvaluateExpressionHandleAsync(string script)
        {
            var context = await MainFrame.GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionHandleAsync(script).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="pageFunction">Script to be evaluated in browser context</param>
        /// <param name="args">Function arguments</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="JSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        public async Task<JSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
        {
            var context = await MainFrame.GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateFunctionHandleAsync(pageFunction, args).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds a function which would be invoked in one of the following scenarios:
        /// - whenever the page is navigated
        /// - whenever the child frame is attached or navigated. In this case, the function is invoked in the context of the newly attached frame
        /// </summary>
        /// <param name="pageFunction">Function to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to <c>pageFunction</c></param>
        /// <remarks>
        /// The function is invoked after the document was created but before any of its scripts were run. This is useful to amend JavaScript environment, e.g. to seed <c>Math.random</c>.
        /// </remarks>
        /// <example>
        /// An example of overriding the navigator.languages property before the page loads:
        /// <code>
        /// var overrideNavigatorLanguages = @"Object.defineProperty(navigator, 'languages', {
        ///   get: function() {
        ///     return ['en-US', 'en', 'bn'];
        ///   };
        /// });";
        /// await page.EvaluateOnNewDocumentAsync(overrideNavigatorLanguages);
        /// </code>
        /// </example>
        /// <returns>Task</returns>
        public Task EvaluateOnNewDocumentAsync(string pageFunction, params object[] args)
        {
            var source = EvaluationString(pageFunction, args);
            return Client.SendAsync("Page.addScriptToEvaluateOnNewDocument", new { source });
        }

        /// <summary>
        /// The method iterates JavaScript heap and finds all the objects with the given prototype.
        /// Shortcut for <c>page.MainFrame.GetExecutionContextAsync().QueryObjectsAsync(prototypeHandle)</c>.
        /// </summary>
        /// <returns>A task which resolves to a handle to an array of objects with this prototype.</returns>
        /// <param name="prototypeHandle">A handle to the object prototype.</param>
        public async Task<JSHandle> QueryObjectsAsync(JSHandle prototypeHandle)
        {
            var context = await MainFrame.GetExecutionContextAsync().ConfigureAwait(false);
            return await context.QueryObjectsAsync(prototypeHandle).ConfigureAwait(false);
        }

        /// <summary>
        /// Activating request interception enables <see cref="Request.AbortAsync(RequestAbortErrorCode)">request.AbortAsync</see>, 
        /// <see cref="Request.ContinueAsync(Payload)">request.ContinueAsync</see> and <see cref="Request.RespondAsync(ResponseData)">request.RespondAsync</see> methods.
        /// </summary>
        /// <returns>The request interception task.</returns>
        /// <param name="value">Whether to enable request interception..</param>
        public Task SetRequestInterceptionAsync(bool value)
            => _networkManager.SetRequestInterceptionAsync(value);

        /// <summary>
        /// Set offline mode for the page.
        /// </summary>
        /// <returns>Result task</returns>
        /// <param name="value">When <c>true</c> enables offline mode for the page.</param>
        public Task SetOfflineModeAsync(bool value) => _networkManager.SetOfflineModeAsync(value);

        /// <summary>
        /// Returns the page's cookies
        /// </summary>
        /// <param name="urls">Url's to return cookies for</param>
        /// <returns>Array of cookies</returns>
        /// <remarks>
        /// If no URLs are specified, this method returns cookies for the current page URL.
        /// If URLs are specified, only cookies for those URLs are returned.
        /// </remarks>
        public async Task<CookieParam[]> GetCookiesAsync(params string[] urls)
        {
            var response = await Client.SendAsync("Network.getCookies", new Dictionary<string, object>
            {
                { "urls", urls.Length > 0 ? urls : new string[] { Url } }
            }).ConfigureAwait(false);
            return response.cookies.ToObject<CookieParam[]>();
        }

        /// <summary>
        /// Clears all of the current cookies and then sets the cookies for the page
        /// </summary>
        /// <param name="cookies">Cookies to set</param>
        /// <returns>Task</returns>
        public async Task SetCookieAsync(params CookieParam[] cookies)
        {
            foreach (var cookie in cookies)
            {
                if (string.IsNullOrEmpty(cookie.Url) && Url.StartsWith("http", StringComparison.Ordinal))
                {
                    cookie.Url = Url;
                }
                if (cookie.Url == "about:blank")
                {
                    throw new PuppeteerException($"Blank page can not have cookie \"{cookie.Name}\"");
                }
            }

            await DeleteCookieAsync(cookies).ConfigureAwait(false);

            if (cookies.Length > 0)
            {
                await Client.SendAsync("Network.setCookies", new Dictionary<string, object>
                {
                    { "cookies", cookies}
                }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes cookies from the page
        /// </summary>
        /// <param name="cookies">Cookies to delete</param>
        /// <returns>Task</returns>
        public async Task DeleteCookieAsync(params CookieParam[] cookies)
        {
            var pageURL = Url;
            foreach (var cookie in cookies)
            {
                if (string.IsNullOrEmpty(cookie.Url) && pageURL.StartsWith("http", StringComparison.Ordinal))
                {
                    cookie.Url = pageURL;
                }
                await Client.SendAsync("Network.deleteCookies", cookie).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Adds a <c><![CDATA[<script>]]></c> tag into the page with the desired url or content
        /// </summary>
        /// <param name="options">add script tag options</param>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.AddScriptTagAsync(options)</c>
        /// </remarks>
        /// <returns>Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame</returns>
        /// <seealso cref="Frame.AddScriptTag(AddTagOptions)"/>
        public Task<ElementHandle> AddScriptTagAsync(AddTagOptions options) => MainFrame.AddScriptTag(options);

        /// <summary>
        /// Adds a <c><![CDATA[<script>]]></c> tag into the page with the desired url or content
        /// </summary>
        /// <param name="url">script url</param>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.AddScriptTagAsync(new AddTagOptions { Url = url })</c>
        /// </remarks>
        /// <returns>Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame</returns>
        public Task<ElementHandle> AddScriptTagAsync(string url) => AddScriptTagAsync(new AddTagOptions { Url = url });

        /// <summary>
        /// Adds a <c><![CDATA[<link rel="stylesheet">]]></c> tag into the page with the desired url or a <c><![CDATA[<link rel="stylesheet">]]></c> tag with the content
        /// </summary>
        /// <param name="options">add style tag options</param>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.AddStyleTagAsync(options)</c>
        /// </remarks>
        /// <returns>Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame</returns>
        /// <seealso cref="Frame.AddStyleTag(AddTagOptions)"/>
        public Task<ElementHandle> AddStyleTagAsync(AddTagOptions options) => MainFrame.AddStyleTag(options);

        /// <summary>
        /// Adds a <c><![CDATA[<link rel="stylesheet">]]></c> tag into the page with the desired url or a <c><![CDATA[<link rel="stylesheet">]]></c> tag with the content
        /// </summary>
        /// <param name="url">stylesheel url</param>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.AddStyleTagAsync(new AddTagOptions { Url = url })</c>
        /// </remarks>
        /// <returns>Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame</returns>
        public Task<ElementHandle> AddStyleTagAsync(string url) => AddStyleTagAsync(new AddTagOptions { Url = url });

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves when <paramref name="puppeteerFunction"/> completes.
        /// </summary>
        /// <param name="name">Name of the function on the window object</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync(string, Action)"/> survive navigations
        /// </remarks>
        /// <returns>Task</returns>
        public Task ExposeFunctionAsync(string name, Action puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves to the return value of <paramref name="puppeteerFunction"/>.
        /// </summary>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/></typeparam>
        /// <param name="name">Name of the function on the window object</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync{TResult}(string, Func{TResult})"/> survive navigations
        /// </remarks>
        /// <returns>Task</returns>
        public Task ExposeFunctionAsync<TResult>(string name, Func<TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves to the return value of <paramref name="puppeteerFunction"/>.
        /// </summary>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T">The parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <param name="name">Name of the function on the window object</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync{T, TResult}(string, Func{T, TResult})"/> survive navigations
        /// </remarks>
        /// <returns>Task</returns>
        public Task ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves to the return value of <paramref name="puppeteerFunction"/>.
        /// </summary>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T1">The first parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T2">The second parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <param name="name">Name of the function on the window object</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync{T1, T2, TResult}(string, Func{T1, T2, TResult})"/> survive navigations
        /// </remarks>
        /// <returns>Task</returns>
        public Task ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves to the return value of <paramref name="puppeteerFunction"/>.
        /// </summary>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T1">The first parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T2">The second parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T3">The third parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <param name="name">Name of the function on the window object</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync{T1, T2, T3, TResult}(string, Func{T1, T2, T3, TResult})"/> survive navigations
        /// </remarks>
        /// <returns>Task</returns>
        public Task ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves to the return value of <paramref name="puppeteerFunction"/>.
        /// </summary>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T1">The first parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T2">The second parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T3">The third parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T4">The fourth parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <param name="name">Name of the function on the window object</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync{T1, T2, T3, T4, TResult}(string, Func{T1, T2, T3, T4, TResult})"/> survive navigations
        /// </remarks>
        /// <returns>Task</returns>
        public Task ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <summary>
        /// Gets the full HTML contents of the page, including the doctype.
        /// </summary>
        /// <returns>Task which resolves to the HTML content.</returns>
        /// <seealso cref="Frame.GetContentAsync"/>
        public Task<string> GetContentAsync() => _frameManager.MainFrame.GetContentAsync();

        /// <summary>
        /// Sets the HTML markup to the page
        /// </summary>
        /// <param name="html">HTML markup to assign to the page.</param>
        /// <returns>Task.</returns>
        /// <seealso cref="Frame.SetContentAsync(string)"/>
        public Task SetContentAsync(string html) => _frameManager.MainFrame.SetContentAsync(html);

        /// <summary>
        /// Navigates to an url
        /// </summary>        
        /// <param name="url">URL to navigate page to. The url should include scheme, e.g. https://.</param>
        /// <param name="options">Navigation parameters.</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.</returns>
        /// <seealso cref="GoToAsync(string, int?, WaitUntilNavigation[])"/>
        public async Task<Response> GoToAsync(string url, NavigationOptions options)
        {
            var referrer = _networkManager.ExtraHTTPHeaders?.GetValueOrDefault("referer");
            var requests = new Dictionary<string, Request>();

            void createRequestEventListener(object sender, RequestEventArgs e)
            {
                if (!requests.ContainsKey(e.Request.Url))
                {
                    requests.Add(e.Request.Url, e.Request);
                }
            }

            _networkManager.Request += createRequestEventListener;

            var mainFrame = _frameManager.MainFrame;
            var timeout = options?.Timeout ?? DefaultNavigationTimeout;

            var watcher = new NavigatorWatcher(_frameManager, mainFrame, timeout, options);
            var navigateTask = Navigate(Client, url, referrer);

            await Task.WhenAny(
                watcher.NavigationTask,
                navigateTask).ConfigureAwait(false);

            AggregateException exception = null;

            if (navigateTask.IsFaulted)
            {
                exception = navigateTask.Exception;
            }
            else if (watcher.NavigationTask.IsCompleted &&
                watcher.NavigationTask.Result.IsFaulted)
            {
                exception = watcher.NavigationTask.Result?.Exception;
            }

            if (exception == null)
            {
                await Task.WhenAll(
                    watcher.NavigationTask,
                    navigateTask).ConfigureAwait(false);
                exception = navigateTask.Exception ?? watcher.NavigationTask.Result.Exception;
            }

            watcher.Cancel();
            _networkManager.Request -= createRequestEventListener;

            if (exception != null)
            {
                throw new NavigationException(exception.InnerException.Message, exception.InnerException);
            }

            requests.TryGetValue(MainFrame.NavigationURL, out var request);

            return request?.Response;
        }

        /// <summary>
        /// Navigates to an url
        /// </summary>
        /// <param name="url">URL to navigate page to. The url should include scheme, e.g. https://.</param>
        /// <param name="timeout">Maximum navigation time in milliseconds, defaults to 30 seconds, pass <c>0</c> to disable timeout. </param>
        /// <param name="waitUntil">When to consider navigation succeeded, defaults to <see cref="WaitUntilNavigation.Load"/>. Given an array of <see cref="WaitUntilNavigation"/>, navigation is considered to be successful after all events have been fired</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect</returns>
        /// <seealso cref="GoToAsync(string, NavigationOptions)"/>
        public Task<Response> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null)
            => GoToAsync(url, new NavigationOptions { Timeout = timeout, WaitUntil = waitUntil });

        /// <summary>
        /// generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <param name="file">The file path to save the PDF to. paths are resolved using <see cref="Path.GetFullPath(string)"/></param>
        /// <returns></returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public Task PdfAsync(string file) => PdfAsync(file, new PdfOptions());

        /// <summary>
        ///  generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <param name="file">The file path to save the PDF to. paths are resolved using <see cref="Path.GetFullPath(string)"/></param>
        /// <param name="options">pdf options</param>
        /// <returns></returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public async Task PdfAsync(string file, PdfOptions options)
        {
            var data = await PdfDataAsync(options).ConfigureAwait(false);

            using (var fs = AsyncFileHelper.CreateStream(file, FileMode.Create))
            {
                await fs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public Task<Stream> PdfStreamAsync() => PdfStreamAsync(new PdfOptions());

        /// <summary>
        /// Generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <param name="options">pdf options</param>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public async Task<Stream> PdfStreamAsync(PdfOptions options)
            => new MemoryStream(await PdfDataAsync(options).ConfigureAwait(false));

        /// <summary>
        /// Generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public Task<byte[]> PdfDataAsync() => PdfDataAsync(new PdfOptions());

        /// <summary>
        /// Generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <param name="options">pdf options</param>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public async Task<byte[]> PdfDataAsync(PdfOptions options)
        {
            var paperWidth = PaperFormat.Letter.Width;
            var paperHeight = PaperFormat.Letter.Height;

            if (options.Format != null)
            {
                paperWidth = options.Format.Width;
                paperHeight = options.Format.Height;
            }
            else
            {
                if (options.Width != null)
                {
                    paperWidth = ConvertPrintParameterToInches(options.Width);
                }
                if (options.Height != null)
                {
                    paperHeight = ConvertPrintParameterToInches(options.Height);
                }
            }

            var marginTop = ConvertPrintParameterToInches(options.MarginOptions.Top);
            var marginLeft = ConvertPrintParameterToInches(options.MarginOptions.Left);
            var marginBottom = ConvertPrintParameterToInches(options.MarginOptions.Bottom);
            var marginRight = ConvertPrintParameterToInches(options.MarginOptions.Right);

            JObject result = await Client.SendAsync("Page.printToPDF", new
            {
                landscape = options.Landscape,
                displayHeaderFooter = options.DisplayHeaderFooter,
                headerTemplate = options.HeaderTemplate,
                footerTemplate = options.FooterTemplate,
                printBackground = options.PrintBackground,
                scale = options.Scale,
                paperWidth,
                paperHeight,
                marginTop,
                marginBottom,
                marginLeft,
                marginRight,
                pageRanges = options.PageRanges
            }).ConfigureAwait(false);

            var buffer = Convert.FromBase64String(result.GetValue("data").Value<string>());
            return buffer;
        }

        /// <summary>
        /// Enables/Disables Javascript on the page
        /// </summary>
        /// <returns>Task.</returns>
        /// <param name="enabled">Whether or not to enable JavaScript on the page.</param>
        public Task SetJavaScriptEnabledAsync(bool enabled)
            => Client.SendAsync("Emulation.setScriptExecutionDisabled", new { value = !enabled });

        /// <summary>
        /// Toggles bypassing page's Content-Security-Policy.
        /// </summary>
        /// <param name="enabled">sets bypassing of page's Content-Security-Policy.</param>
        /// <returns></returns>
        /// <remarks>
        /// CSP bypassing happens at the moment of CSP initialization rather then evaluation.
        /// Usually this means that <see cref="SetBypassCSPAsync(bool)"/> should be called before navigating to the domain.
        /// </remarks>
        public Task SetBypassCSPAsync(bool enabled) => Client.SendAsync("Page.setBypassCSP", new { enabled });

        /// <summary>
        /// Emulates a media such as screen or print.
        /// </summary>
        /// <returns>Task.</returns>
        /// <param name="media">Media to set.</param>
        public Task EmulateMediaAsync(MediaType media)
            => Client.SendAsync("Emulation.setEmulatedMedia", new { media });

        /// <summary>
        /// Sets the viewport.
        /// In the case of multiple pages in a single browser, each page can have its own viewport size.
        /// NOTE in certain cases, setting viewport will reload the page in order to set the isMobile or hasTouch properties.
        /// </summary>
        /// <returns>The viewport task.</returns>
        /// <param name="viewport">Viewport options.</param>
        public async Task SetViewportAsync(ViewPortOptions viewport)
        {
            var needsReload = await _emulationManager.EmulateViewport(viewport).ConfigureAwait(false);
            Viewport = viewport;

            if (needsReload)
            {
                await ReloadAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Emulates given device metrics and user agent. 
        /// </summary>
        /// <remarks>
        /// This method is a shortcut for calling two methods:
        /// page.SetViewportAsync(userAgent)
        /// page.SetUserAgentAsync(viewport)
        /// </remarks>
        /// <returns>Task.</returns>
        /// <param name="options">Emulation options.</param>
        public Task EmulateAsync(DeviceDescriptor options) => Task.WhenAll(
            SetViewportAsync(options.ViewPort),
            SetUserAgentAsync(options.UserAgent)
        );

        /// <summary>
        /// Takes a screenshot of the page
        /// </summary>
        /// <returns>The screenshot task.</returns>
        /// <param name="file">The file path to save the image to. The screenshot type will be inferred from file extension. 
        /// If path is a relative path, then it is resolved relative to current working directory. If no path is provided, 
        /// the image won't be saved to the disk.</param>
        public Task ScreenshotAsync(string file) => ScreenshotAsync(file, new ScreenshotOptions());

        /// <summary>
        /// Takes a screenshot of the page
        /// </summary>
        /// <returns>The screenshot task.</returns>
        /// <param name="file">The file path to save the image to. The screenshot type will be inferred from file extension. 
        /// If path is a relative path, then it is resolved relative to current working directory. If no path is provided, 
        /// the image won't be saved to the disk.</param>
        /// <param name="options">Screenshot options.</param>
        public async Task ScreenshotAsync(string file, ScreenshotOptions options)
        {
            if (!options.Type.HasValue)
            {
                options.Type = ScreenshotOptions.GetScreenshotTypeFromFile(file);
            }
            var data = await ScreenshotDataAsync(options).ConfigureAwait(false);

            using (var fs = AsyncFileHelper.CreateStream(file, FileMode.Create))
            {
                await fs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Takes a screenshot of the page
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the image data.</returns>
        public Task<Stream> ScreenshotStreamAsync() => ScreenshotStreamAsync(new ScreenshotOptions());

        /// <summary>
        /// Takes a screenshot of the page
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the image data.</returns>
        /// <param name="options">Screenshot options.</param>
        public async Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options)
            => new MemoryStream(await ScreenshotDataAsync(options).ConfigureAwait(false));

        /// <summary>
        /// Takes a screenshot of the page
        /// </summary>
        /// <returns>Task which resolves to a <see cref="string"/> containing the image data as base64.</returns>
        public Task<string> ScreenshotBase64Async() => ScreenshotBase64Async(new ScreenshotOptions());

        /// <summary>
        /// Takes a screenshot of the page
        /// </summary>
        /// <returns>Task which resolves to a <see cref="string"/> containing the image data as base64.</returns>
        /// <param name="options">Screenshot options.</param>
        public Task<string> ScreenshotBase64Async(ScreenshotOptions options)
        {
            var screenshotType = options.Type;

            if (!screenshotType.HasValue)
            {
                screenshotType = ScreenshotType.Png;
            }

            if (options.Quality.HasValue)
            {
                if (screenshotType != ScreenshotType.Jpeg)
                {
                    throw new ArgumentException($"options.Quality is unsupported for the {screenshotType} screenshots");
                }

                if (options.Quality < 0 || options.Quality > 100)
                {
                    throw new ArgumentException($"Expected options.quality to be between 0 and 100 (inclusive), got {options.Quality}");
                }
            }

            if (options.Clip != null && options.FullPage)
            {
                throw new ArgumentException("options.clip and options.fullPage are exclusive");
            }

            return _screenshotTaskQueue.Enqueue(() => PerformScreenshot(screenshotType.Value, options));
        }

        /// <summary>
        /// Takes a screenshot of the page
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the image data.</returns>
        public Task<byte[]> ScreenshotDataAsync() => ScreenshotDataAsync(new ScreenshotOptions());

        /// <summary>
        /// Takes a screenshot of the page
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the image data.</returns>
        /// <param name="options">Screenshot options.</param>
        public async Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options)
            => Convert.FromBase64String(await ScreenshotBase64Async(options).ConfigureAwait(false));

        /// <summary>
        /// Returns page's title
        /// </summary>
        /// <returns>page's title</returns>
        /// <see cref="Frame.GetTitleAsync"/>
        public Task<string> GetTitleAsync() => MainFrame.GetTitleAsync();

        /// <summary>
        /// Closes the page.
        /// </summary>
        /// <returns>Task.</returns>
        public Task CloseAsync(PageCloseOptions options = null)
        {
            if (!(Client?.Connection?.IsClosed ?? true))
            {
                var runBeforeUnload = options?.RunBeforeUnload ?? false;

                if (runBeforeUnload)
                {
                    return Client.SendAsync("Page.close");
                }
                else
                {
                    return Client.Connection.SendAsync("Target.closeTarget", new
                    {
                        targetId = Target.TargetId
                    }).ContinueWith((task) => Target.CloseTask);
                }
            }

            _logger.LogWarning("Protocol error: Connection closed. Most likely the page has been closed.");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Toggles ignoring cache for each request based on the enabled state. By default, caching is enabled.
        /// </summary>
        /// <param name="enabled">sets the <c>enabled</c> state of the cache</param>
        /// <returns>Task</returns>
        public Task SetCacheEnabledAsync(bool enabled = true)
            => Client.SendAsync("Network.setCacheDisabled", new { cacheDisabled = !enabled });

        /// <summary>
        /// Fetches an element with <paramref name="selector"/>, scrolls it into view if needed, and then uses <see cref="Page.Mouse"/> to click in the center of the element.
        /// </summary>
        /// <param name="selector">A selector to search for element to click. If there are multiple elements satisfying the selector, the first will be clicked.</param>
        /// <param name="options">click options</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/></exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully clicked</returns>
        public async Task ClickAsync(string selector, ClickOptions options = null)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }
            await handle.ClickAsync(options).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches an element with <paramref name="selector"/>, scrolls it into view if needed, and then uses <see cref="Page.Mouse"/> to hover over the center of the element.
        /// </summary>
        /// <param name="selector">A selector to search for element to hover. If there are multiple elements satisfying the selector, the first will be hovered.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/></exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully hovered</returns>
        public async Task HoverAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }
            await handle.HoverAsync().ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches an element with <paramref name="selector"/> and focuses it
        /// </summary>
        /// <param name="selector">A selector to search for element to focus. If there are multiple elements satisfying the selector, the first will be focused.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/></exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully focused</returns>
        public async Task FocusAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }
            await handle.FocusAsync().ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <seealso cref="EvaluateFunctionAsync(string, object[])"/>
        /// <returns>Task which resolves to script return value</returns>
        public Task<dynamic> EvaluateExpressionAsync(string script)
            => _frameManager.MainFrame.EvaluateExpressionAsync(script);

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
        public Task<T> EvaluateExpressionAsync<T>(string script)
            => _frameManager.MainFrame.EvaluateExpressionAsync<T>(script);

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
        public Task<dynamic> EvaluateFunctionAsync(string script, params object[] args)
            => _frameManager.MainFrame.EvaluateFunctionAsync(script, args);

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
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
            => _frameManager.MainFrame.EvaluateFunctionAsync<T>(script, args);

        /// <summary>
        /// Sets the user agent to be used in this page
        /// </summary>
        /// <param name="userAgent">Specific user agent to use in this page</param>
        /// <returns>Task</returns>
        public Task SetUserAgentAsync(string userAgent)
            => _networkManager.SetUserAgentAsync(userAgent);

        /// <summary>
        /// Sets extra HTTP headers that will be sent with every request the page initiates
        /// </summary>
        /// <param name="headers">Additional http headers to be sent with every request</param>
        /// <returns>Task</returns>
        public Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
            => _networkManager.SetExtraHTTPHeadersAsync(headers);

        /// <summary>
        /// Provide credentials for http authentication <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication"/>
        /// </summary>
        /// <param name="credentials">The credentials</param>
        /// <returns></returns>
        /// <remarks>
        /// To disable authentication, pass <c>null</c>
        /// </remarks>
        public Task AuthenticateAsync(Credentials credentials) => _networkManager.AuthenticateAsync(credentials);

        /// <summary>
        /// Reloads the page
        /// </summary>
        /// <param name="options">Navigation options</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect</returns>
        /// <seealso cref="ReloadAsync(int?, WaitUntilNavigation[])"/>
        public async Task<Response> ReloadAsync(NavigationOptions options)
        {
            var navigationTask = WaitForNavigationAsync(options);

            await Task.WhenAll(
              navigationTask,
              Client.SendAsync("Page.reload")
            ).ConfigureAwait(false);

            return navigationTask.Result;
        }

        /// <summary>
        /// Reloads the page
        /// </summary>
        /// <param name="timeout">Maximum navigation time in milliseconds, defaults to 30 seconds, pass <c>0</c> to disable timeout. </param>
        /// <param name="waitUntil">When to consider navigation succeeded, defaults to <see cref="WaitUntilNavigation.Load"/>. Given an array of <see cref="WaitUntilNavigation"/>, navigation is considered to be successful after all events have been fired</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect</returns>
        /// <seealso cref="ReloadAsync(NavigationOptions)"/>
        public Task<Response> ReloadAsync(int? timeout = null, WaitUntilNavigation[] waitUntil = null)
            => ReloadAsync(new NavigationOptions { Timeout = timeout, WaitUntil = waitUntil });

        /// <summary>
        /// Triggers a change and input event once all the provided options have been selected. 
        /// If there's no <![CDATA[<select>]]> element matching selector, the method throws an error.
        /// </summary>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/></exception>
        /// <param name="selector">A selector to query page for</param>
        /// <param name="values">Values of options to select. If the <![CDATA[<select>]]> has the multiple attribute, 
        /// all values are considered, otherwise only the first one is taken into account.</param>
        /// <returns>Returns an array of option values that have been successfully selected.</returns>
        /// <seealso cref="Frame.SelectAsync(string, string[])"/>
        public Task<string[]> SelectAsync(string selector, params string[] values)
            => MainFrame.SelectAsync(selector, values);

        /// <summary>
        /// Sends a <c>keydown</c>, <c>keypress</c>/<c>input</c>, and <c>keyup</c> event for each character in the text.
        /// </summary>
        /// <param name="selector">A selector of an element to type into. If there are multiple elements satisfying the selector, the first will be used.</param>
        /// <param name="text">A text to type into a focused element</param>
        /// <param name="options"></param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/></exception>
        /// <remarks>
        /// To press a special key, like <c>Control</c> or <c>ArrowDown</c> use <see cref="Keyboard.PressAsync(string, PressOptions)"/>
        /// </remarks>
        /// <example>
        /// <code>
        /// page.TypeAsync("#mytextarea", "Hello"); // Types instantly
        /// page.TypeAsync("#mytextarea", "World", new TypeOptions { Delay = 100 }); // Types slower, like a user
        /// </code>
        /// </example>
        /// <returns>Task</returns>
        public async Task TypeAsync(string selector, string text, TypeOptions options = null)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }
            await handle.TypeAsync(text, options).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Waits for a timeout
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns>A task that resolves when after the timeout</returns>
        /// <seealso cref="Frame.WaitForTimeoutAsync(int)"/>
        public Task WaitForTimeoutAsync(int milliseconds)
            => MainFrame.WaitForTimeoutAsync(milliseconds);

        /// <summary>
        /// Waits for a function to be evaluated to a truthy value
        /// </summary>
        /// <param name="script">Function to be evaluated in browser context</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <param name="args">Arguments to pass to <c>script</c></param>
        /// <returns>A task that resolves when the <c>script</c> returns a truthy value</returns>
        /// <seealso cref="Frame.WaitForFunctionAsync(string, WaitForFunctionOptions, object[])"/>
        public Task<JSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args)
            => MainFrame.WaitForFunctionAsync(script, options ?? new WaitForFunctionOptions(), args);

        /// <summary>
        /// Waits for a function to be evaluated to a truthy value
        /// </summary>
        /// <param name="script">Function to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to <c>script</c></param>
        /// <returns>A task that resolves when the <c>script</c> returns a truthy value</returns>
        public Task<JSHandle> WaitForFunctionAsync(string script, params object[] args) => WaitForFunctionAsync(script, null, args);

        /// <summary>
        /// Waits for an expression to be evaluated to a truthy value
        /// </summary>
        /// <param name="script">Expression to be evaluated in browser context</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <returns>A task that resolves when the <c>script</c> returns a truthy value</returns>
        /// <seealso cref="Frame.WaitForExpressionAsync(string, WaitForFunctionOptions)"/>
        public Task<JSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options = null)
            => MainFrame.WaitForExpressionAsync(script, options ?? new WaitForFunctionOptions());

        /// <summary>
        /// Waits for a selector to be added to the DOM
        /// </summary>
        /// <param name="selector">A selector of an element to wait for</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <returns>A task that resolves when element specified by selector string is added to DOM</returns>
        /// <seealso cref="WaitForXPathAsync(string, WaitForSelectorOptions)"/>
        /// <seealso cref="Frame.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
        public Task<ElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
            => MainFrame.WaitForSelectorAsync(selector, options ?? new WaitForSelectorOptions());

        /// <summary>
        /// Waits for a xpath selector to be added to the DOM
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
        /// page
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
        /// <seealso cref="Frame.WaitForXPathAsync(string, WaitForSelectorOptions)"/>
        public Task<ElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
            => MainFrame.WaitForXPathAsync(xpath, options ?? new WaitForSelectorOptions());

        /// <summary>
        /// This resolves when the page navigates to a new URL or reloads.
        /// It is useful for when you run code which will indirectly cause the page to navigate.
        /// </summary>
        /// <param name="options">navigation options</param>
        /// <returns>Task which resolves to the main resource response. 
        /// In case of multiple redirects, the navigation will resolve with the response of the last redirect.
        /// In case of navigation to a different anchor or navigation due to History API usage, the navigation will resolve with `null`.
        /// </returns>
        /// <remarks>
        /// Usage of the <c>History API</c> <see href="https://developer.mozilla.org/en-US/docs/Web/API/History_API"/> to change the URL is considered a navigation
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var navigationTask = page.WaitForNavigationAsync();
        /// await page.ClickAsync("a.my-link");
        /// await navigationTask;
        /// ]]>
        /// </code>
        /// </example>
        public async Task<Response> WaitForNavigationAsync(NavigationOptions options = null)
        {
            var mainFrame = _frameManager.MainFrame;
            var timeout = options?.Timeout ?? DefaultNavigationTimeout;
            var watcher = new NavigatorWatcher(_frameManager, mainFrame, timeout, options);
            var responses = new Dictionary<string, Response>();

            void createResponseEventListener(object sender, ResponseCreatedEventArgs e) => responses[e.Response.Url] = e.Response;

            _networkManager.Response += createResponseEventListener;

            await watcher.NavigationTask.ConfigureAwait(false);

            _networkManager.Response -= createResponseEventListener;

            var exception = watcher.NavigationTask.Exception;
            if (exception != null)
            {
                throw new NavigationException(exception.Message, exception);
            }

            return responses.GetValueOrDefault(_frameManager.MainFrame.Url);
        }

        /// <summary>
        /// Waits for a request.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var firstRequest = await page.WaitForRequestAsync("http://example.com/resource");
        /// return firstRequest.Url;
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>A task which resolves when a matching request was made.</returns>
        /// <param name="url">URL to wait for.</param>
        /// <param name="options">Options.</param>
        public Task<Request> WaitForRequestAsync(string url, WaitForOptions options = null)
            => WaitForRequestAsync(request => request.Url == url, options);

        /// <summary>
        /// Waits for a request.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var request = await page.WaitForRequestAsync(request => request.Url === "http://example.com" && request.Method === HttpMethod.Get;
        /// return request.Url;
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>A task which resolves when a matching request was made.</returns>
        /// <param name="predicate">Function which looks for a matching request.</param>
        /// <param name="options">Options.</param>
        public async Task<Request> WaitForRequestAsync(Func<Request, bool> predicate, WaitForOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultWaitForTimeout;
            var requestTcs = new TaskCompletionSource<Request>();

            void requestEventListener(object sender, RequestEventArgs e)
            {
                if (predicate(e.Request))
                {
                    requestTcs.TrySetResult(e.Request);
                    _networkManager.Request -= requestEventListener;
                }
            }

            _networkManager.Request += requestEventListener;

            await Task.WhenAny(new[]
            {
                TaskHelper.CreateTimeoutTask(timeout),
                requestTcs.Task
            }).ConfigureAwait(false);

            return requestTcs.Task.Result;
        }

        /// <summary>
        /// Waits for a response.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var firstResponse = await page.WaitForResponseAsync("http://example.com/resource");
        /// return firstResponse.Url;
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>A task which resolves when a matching response is received.</returns>
        /// <param name="url">URL to wait for.</param>
        /// <param name="options">Options.</param>
        public Task<Response> WaitForResponseAsync(string url, WaitForOptions options = null)
            => WaitForResponseAsync(response => response.Url == url, options);

        /// <summary>
        /// Waits for a response.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var response = await page.WaitForResponseAsync(response => response.Url === "http://example.com" && response.Status === HttpStatus.Ok;
        /// return response.Url;
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>A task which resolves when a matching response is received.</returns>
        /// <param name="predicate">Function which looks for a matching response.</param>
        /// <param name="options">Options.</param>
        public async Task<Response> WaitForResponseAsync(Func<Response, bool> predicate, WaitForOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultWaitForTimeout;
            var responseTcs = new TaskCompletionSource<Response>();

            void responseEventListener(object sender, ResponseCreatedEventArgs e)
            {
                if (predicate(e.Response))
                {
                    responseTcs.TrySetResult(e.Response);
                    _networkManager.Response -= responseEventListener;
                }
            }

            _networkManager.Response += responseEventListener;

            await Task.WhenAny(new[]
            {
                TaskHelper.CreateTimeoutTask(timeout),
                responseTcs.Task
            }).ConfigureAwait(false);

            return responseTcs.Task.Result;
        }

        /// <summary>
        /// Navigate to the previous page in history.
        /// </summary>
        /// <returns>Task which which resolves to the main resource response. In case of multiple redirects, 
        /// the navigation will resolve with the response of the last redirect. If can not go back, resolves to null.</returns>
        /// <param name="options">Navigation parameters.</param>
        public Task<Response> GoBackAsync(NavigationOptions options = null) => GoAsync(-1, options);

        /// <summary>
        /// Navigate to the next page in history.
        /// </summary>
        /// <returns>Task which which resolves to the main resource response. In case of multiple redirects, 
        /// the navigation will resolve with the response of the last redirect. If can not go forward, resolves to null.</returns>
        /// <param name="options">Navigation parameters.</param>
        public Task<Response> GoForwardAsync(NavigationOptions options = null) => GoAsync(1, options);

        #endregion

        #region Private Method

        internal static async Task<Page> CreateAsync(
            CDPSession client,
            Target target,
            bool ignoreHTTPSErrors,
            bool setDefaultViewPort,
            TaskQueue screenshotTaskQueue)
        {
            await client.SendAsync("Page.enable", null).ConfigureAwait(false);
            dynamic result = await client.SendAsync("Page.getFrameTree").ConfigureAwait(false);
            var page = new Page(client, target, new FrameTree(result.frameTree), ignoreHTTPSErrors, screenshotTaskQueue);

            await Task.WhenAll(
                client.SendAsync("Target.setAutoAttach", new { autoAttach = true, waitForDebuggerOnStart = false }),
                client.SendAsync("Page.setLifecycleEventsEnabled", new { enabled = true }),
                client.SendAsync("Network.enable", null),
                client.SendAsync("Runtime.enable", null),
                client.SendAsync("Security.enable", null),
                client.SendAsync("Performance.enable", null),
                client.SendAsync("Log.enable", null)
            ).ConfigureAwait(false);

            if (ignoreHTTPSErrors)
            {
                await client.SendAsync("Security.setOverrideCertificateErrors", new Dictionary<string, object>
                {
                    {"override", true}
                }).ConfigureAwait(false);
            }

            if (setDefaultViewPort)
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 800,
                    Height = 600
                }).ConfigureAwait(false);
            }
            return page;
        }

        private async Task<Response> GoAsync(int delta, NavigationOptions options)
        {
            var history = await Client.SendAsync<PageGetNavigationHistoryResponse>("Page.getNavigationHistory").ConfigureAwait(false);

            if (history.Entries.Count <= history.CurrentIndex + delta)
            {
                return null;
            }
            var entry = history.Entries[history.CurrentIndex + delta];
            var waitTask = WaitForNavigationAsync(options);

            await Task.WhenAll(
                waitTask,
                Client.SendAsync("Page.navigateToHistoryEntry", new
                {
                    entryId = entry.Id
                })
            ).ConfigureAwait(false);

            return waitTask.Result;
        }

        private Dictionary<string, decimal> BuildMetricsObject(List<Metric> metrics)
        {
            var result = new Dictionary<string, decimal>();

            foreach (var item in metrics)
            {
                if (SupportedMetrics.Contains(item.Name))
                {
                    result.Add(item.Name, item.Value);
                }
            }

            return result;
        }

        private async Task<string> PerformScreenshot(ScreenshotType type, ScreenshotOptions options)
        {
            await Client.SendAsync("Target.activateTarget", new
            {
                targetId = Target.TargetId
            }).ConfigureAwait(false);

            var clip = options.Clip?.Clone();
            if (clip != null)
            {
                clip.Scale = 1;
            }

            if (options != null && options.FullPage)
            {
                dynamic metrics = await Client.SendAsync("Page.getLayoutMetrics").ConfigureAwait(false);
                var width = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(metrics.contentSize.width.Value)));
                var height = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(metrics.contentSize.height.Value)));

                // Overwrite clip for full page at all times.
                clip = new Clip
                {
                    X = 0,
                    Y = 0,
                    Width = width,
                    Height = height,
                    Scale = 1
                };

                var mobile = Viewport.IsMobile;
                var deviceScaleFactor = Viewport.DeviceScaleFactor;
                var landscape = Viewport.IsLandscape;
                var screenOrientation = landscape ?
                    new ScreenOrientation
                    {
                        Angle = 90,
                        Type = ScreenOrientationType.LandscapePrimary
                    } :
                    new ScreenOrientation
                    {
                        Angle = 0,
                        Type = ScreenOrientationType.PortraitPrimary
                    };

                await Client.SendAsync("Emulation.setDeviceMetricsOverride", new
                {
                    mobile,
                    width,
                    height,
                    deviceScaleFactor,
                    screenOrientation
                }).ConfigureAwait(false);
            }

            if (options != null && options.OmitBackground)
            {
                await Client.SendAsync("Emulation.setDefaultBackgroundColorOverride", new
                {
                    color = new
                    {
                        r = 0,
                        g = 0,
                        b = 0,
                        a = 0
                    }
                }).ConfigureAwait(false);
            }

            dynamic screenMessage = new ExpandoObject();

            screenMessage.format = type.ToString().ToLower();

            if (options.Quality.HasValue)
            {
                screenMessage.quality = options.Quality.Value;
            }

            if (clip != null)
            {
                screenMessage.clip = clip;
            }

            JObject result = await Client.SendAsync("Page.captureScreenshot", screenMessage).ConfigureAwait(false);

            if (options != null && options.OmitBackground)
            {
                await Client.SendAsync("Emulation.setDefaultBackgroundColorOverride").ConfigureAwait(false);
            }

            if (options != null && options.FullPage)
            {
                await SetViewportAsync(Viewport).ConfigureAwait(false);
            }

            return result.GetValue("data").Value<string>();
        }

        private decimal ConvertPrintParameterToInches(object parameter)
        {
            if (parameter == null)
            {
                return 0;
            }

            var pixels = 0m;

            if (parameter is decimal || parameter is int)
            {
                pixels = Convert.ToDecimal(parameter);
            }
            else
            {
                var text = parameter.ToString();
                var unit = text.Substring(text.Length - 2).ToLower();
                var valueText = "";

                if (_unitToPixels.ContainsKey(unit))
                {
                    valueText = text.Substring(0, text.Length - 2);
                }
                else
                {
                    // In case of unknown unit try to parse the whole parameter as number of pixels.
                    // This is consistent with phantom's paperSize behavior.
                    unit = "px";
                    valueText = text;
                }

                if (decimal.TryParse(valueText, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out var number))
                {
                    pixels = number * _unitToPixels[unit];
                }
                else
                {
                    throw new ArgumentException($"Failed to parse parameter value: '{text}'", nameof(parameter));
                }
            }

            return pixels / 96;
        }

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Page.domContentEventFired":
                    DOMContentLoaded?.Invoke(this, EventArgs.Empty);
                    break;
                case "Page.loadEventFired":
                    Load?.Invoke(this, EventArgs.Empty);
                    break;
                case "Runtime.consoleAPICalled":
                    await OnConsoleAPI(e.MessageData.ToObject<PageConsoleResponse>()).ConfigureAwait(false);
                    break;
                case "Page.javascriptDialogOpening":
                    OnDialog(e.MessageData.ToObject<PageJavascriptDialogOpeningResponse>());
                    break;
                case "Runtime.exceptionThrown":
                    HandleException(e.MessageData.SelectToken("exceptionDetails").ToObject<EvaluateExceptionDetails>());
                    break;
                case "Security.certificateError":
                    await OnCertificateError(e.MessageData.ToObject<CertificateErrorResponse>()).ConfigureAwait(false);
                    break;
                case "Inspector.targetCrashed":
                    OnTargetCrashed();
                    break;
                case "Performance.metrics":
                    EmitMetrics(e.MessageData.ToObject<PerformanceMetricsResponse>());
                    break;
                case "Target.attachedToTarget":
                    await OnAttachedToTarget(e);
                    break;
                case "Target.detachedFromTarget":
                    OnDetachedFromTarget(e);
                    break;
                case "Log.entryAdded":
                    OnLogEntryAdded(e.MessageData.ToObject<LogEntryAddedResponse>());
                    break;
            }
        }

        private void OnDetachedFromTarget(MessageEventArgs e)
        {
            var sessionId = e.MessageData.SelectToken("sessionId").Value<string>();
            if (_workers.TryGetValue(sessionId, out var worker))
            {
                WorkerDestroyed?.Invoke(this, new WorkerEventArgs(worker));
                _workers.Remove(sessionId);
            }
        }

        private async Task OnAttachedToTarget(MessageEventArgs e)
        {
            var targetInfo = e.MessageData.SelectToken("targetInfo").ToObject<TargetInfo>();
            var sessionId = e.MessageData.SelectToken("sessionId").ToObject<string>();
            if (targetInfo.Type != TargetType.Worker)
            {
                try
                {
                    await Client.SendAsync("Target.detachFromTarget", new { sessionId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
                return;
            }
            var session = Client.CreateSession(TargetType.Worker, sessionId);
            var worker = new Worker(session, targetInfo.Url, AddConsoleMessage, HandleException);
            _workers[sessionId] = worker;
            WorkerCreated?.Invoke(this, new WorkerEventArgs(worker));
        }

        private void OnLogEntryAdded(LogEntryAddedResponse e)
        {
            if (e.Entry.Args != null)
            {
                foreach (var arg in e.Entry?.Args)
                {
                    RemoteObjectHelper.ReleaseObject(Client, arg, _logger);
                }
            }
            if (e.Entry.Source != TargetType.Worker)
            {
                Console?.Invoke(this, new ConsoleEventArgs(new ConsoleMessage(e.Entry.Level, e.Entry.Text)));
            }
        }

        private void OnTargetCrashed()
        {
            if (Error == null)
            {
                throw new TargetCrashedException();
            }

            Error.Invoke(this, new ErrorEventArgs("Page crashed!"));
        }

        private void EmitMetrics(PerformanceMetricsResponse metrics)
            => Metrics?.Invoke(this, new MetricEventArgs(metrics.Title, BuildMetricsObject(metrics.Metrics)));

        private async Task OnCertificateError(CertificateErrorResponse e)
        {
            if (_ignoreHTTPSErrors)
            {
                try
                {
                    await Client.SendAsync("Security.handleCertificateError", new Dictionary<string, object>
                    {
                        {"eventId", e.EventId },
                        {"action", "continue"}
                    }).ConfigureAwait(false);
                }
                catch (PuppeteerException ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }

        private void HandleException(EvaluateExceptionDetails exceptionDetails)
            => PageError?.Invoke(this, new PageErrorEventArgs(GetExceptionMessage(exceptionDetails)));

        private string GetExceptionMessage(EvaluateExceptionDetails exceptionDetails)
        {
            if (exceptionDetails.Exception != null)
            {
                return exceptionDetails.Exception.Description;
            }
            var message = exceptionDetails.Text;
            if (exceptionDetails.StackTrace != null)
            {
                foreach (var callframe in exceptionDetails.StackTrace.CallFrames)
                {
                    var location = $"{callframe.Url}:{callframe.LineNumber}:{callframe.ColumnNumber}";
                    var functionName = callframe.FunctionName ?? "<anonymous>";
                    message += $"\n at {functionName} ({location})";
                }
            }
            return message;
        }

        private void OnDialog(PageJavascriptDialogOpeningResponse message)
        {
            var dialog = new Dialog(Client, message.Type, message.Message, message.DefaultPrompt);
            Dialog?.Invoke(this, new DialogEventArgs(dialog));
        }

        private async Task OnConsoleAPI(PageConsoleResponse message)
        {
            if (message.Type == ConsoleType.Debug && message.Args.Length > 0 && message.Args[0].value == "driver:page-binding")
            {
                const string deliverResult = @"function deliverResult(name, seq, result) {
                  window[name]['callbacks'].get(seq)(result);
                  window[name]['callbacks'].delete(seq);
                }";
                JObject arg1Value = JObject.Parse(message.Args[1].value.ToString());
                var name = arg1Value.Value<string>("name");
                var seq = arg1Value.Value<int>("seq");

                var binding = _pageBindings[name];
                var methodParams = binding.Method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();

                var args = arg1Value.GetValue("args").Select((token, i) => token.ToObject(methodParams[i])).ToArray();

                var result = binding.DynamicInvoke(args);
                if (result is Task taskResult)
                {
                    await taskResult.ConfigureAwait(false);

                    if (taskResult.GetType().IsGenericType)
                    {
                        // the task is already awaited and therefore the call to property Result will not deadlock
                        result = ((dynamic)taskResult).Result;
                    }
                }

                var expression = EvaluationString(deliverResult, name, seq, result);
                _ = Client.SendAsync("Runtime.evaluate", new { expression, contextId = message.ExecutionContextId })
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            _logger.LogError(task.Exception.ToString());
                        }
                    });
                return;
            }

            var values = message.Args.Select<dynamic, JSHandle>(i =>
                _frameManager.CreateJSHandle(message.ExecutionContextId, i)).ToArray();

            await AddConsoleMessage(message.Type, values);
        }

        private async Task AddConsoleMessage(ConsoleType type, JSHandle[] values)
        {
            if (Console?.GetInvocationList().Length == 0)
            {
                foreach (var arg in values)
                {
                    await RemoteObjectHelper.ReleaseObject(Client, arg, _logger).ConfigureAwait(false);
                }

                return;
            }

            var tokens = values.Select(i => i.RemoteObject.objectId != null
                ? i.ToString()
                : RemoteObjectHelper.ValueFromRemoteObject<string>(i.RemoteObject));

            var consoleMessage = new ConsoleMessage(type, string.Join(" ", tokens), values);
            Console?.Invoke(this, new ConsoleEventArgs(consoleMessage));
        }

        private async Task ExposeFunctionAsync(string name, Delegate puppeteerFunction)
        {
            if (_pageBindings.ContainsKey(name))
            {
                throw new PuppeteerException($"Failed to add page binding with name {name}: window['{name}'] already exists!");
            }
            _pageBindings.Add(name, puppeteerFunction);

            const string addPageBinding = @"function addPageBinding(bindingName) {
                window[bindingName] = async(...args) => {
                    const me = window[bindingName];
                    let callbacks = me['callbacks'];
                    if (!callbacks)
                    {
                        callbacks = new Map();
                        me['callbacks'] = callbacks;
                    }
                    const seq = (me['lastSeq'] || 0) + 1;
                    me['lastSeq'] = seq;
                    const promise = new Promise(fulfill => callbacks.set(seq, fulfill));
                    // eslint-disable-next-line no-console
                    console.debug('driver:page-binding', JSON.stringify({ name: bindingName, seq, args}));
                    return promise;
                };
            }";
            var expression = EvaluationString(addPageBinding, name);
            await Client.SendAsync("Page.addScriptToEvaluateOnNewDocument", new { source = expression }).ConfigureAwait(false);

            await Task.WhenAll(Frames.Select(frame => frame.EvaluateExpressionAsync(expression)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _logger.LogError(task.Exception.ToString());
                    }
                }))).ConfigureAwait(false);
        }

        private async Task Navigate(CDPSession client, string url, string referrer)
        {
            dynamic response = await client.SendAsync("Page.navigate", new
            {
                url,
                referrer = referrer ?? string.Empty
            }).ConfigureAwait(false);

            if (response.errorText != null)
            {
                throw new NavigationException(response.errorText.ToString(), url);
            }
        }

        private static string EvaluationString(string fun, params object[] args)
        {
            return $"({fun})({string.Join(",", args.Select(SerializeArgument))})";

            string SerializeArgument(object arg)
            {
                return arg == null ? "undefined" : JsonConvert.SerializeObject(arg);
            }
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Releases all resource used by the <see cref="Page"/> object by calling the <see cref="CloseAsync"/> method.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Page"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Page"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="Page"/> so
        /// the garbage collector can reclaim the memory that the <see cref="Page"/> was occupying.</remarks>
        public void Dispose() => CloseAsync();
        #endregion
    }
}