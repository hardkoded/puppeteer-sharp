using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using CefSharp.Dom.Helpers;
using CefSharp.Dom.Helpers.Json;
using CefSharp.Dom.Input;
using CefSharp.Dom.Media;
using CefSharp.Dom.Messaging;
using CefSharp.Dom.Mobile;
using CefSharp.Dom.PageCoverage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CefSharp.Dom
{
    /// <summary>
    /// Provides methods to interact with a ChromiumWebBrowser instance
    /// </summary>
    /// <example>
    /// This example creates a devToolsContext, navigates it to a URL, and then saves a screenshot:
    /// <code>
    /// var devToolsContext = await chromiumWebBrowser.GetDevToolsContextAsync();
    /// await devToolsContext.GoToAsync("https://example.com");
    /// await devToolsContext.ScreenshotAsync("screenshot.png");
    /// </code>
    /// </example>
    [DebuggerDisplay("DevToolsContext {Url}")]
    public class DevToolsContext : IDisposable, IAsyncDisposable, IDevToolsContext
    {
        private readonly EmulationManager _emulationManager;
        private readonly Dictionary<string, Delegate> _pageBindings;
        private readonly ILogger _logger;
        private readonly TimeoutSettings _timeoutSettings;
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<FileChooser>> _fileChooserInterceptors;
        private PageGetLayoutMetricsResponse _burstModeMetrics;
        private bool _screenshotBurstModeOn;
        private ScreenshotOptions _screenshotBurstModeOptions;
        private bool _frameTreeLoaded;

        private static readonly Dictionary<string, decimal> _unitToPixels = new Dictionary<string, decimal> {
            { "px", 1 },
            { "in", 96 },
            { "cm", 37.8m },
            { "mm", 3.78m }
        };

        internal DevToolsContext(
            DevToolsConnection client)
        {
            Connection = client;
            Keyboard = new Keyboard(client);
            Mouse = new Mouse(client, Keyboard);
            Touchscreen = new Touchscreen(client, Keyboard);
            Tracing = new Tracing(client);
            Coverage = new Coverage(client);

            _fileChooserInterceptors = new ConcurrentDictionary<Guid, TaskCompletionSource<FileChooser>>();
            _timeoutSettings = new TimeoutSettings();
            _emulationManager = new EmulationManager(client);
            _pageBindings = new Dictionary<string, Delegate>();
            _logger = Connection.LoggerFactory.CreateLogger<DevToolsContext>();
            Accessibility = new PageAccessibility.Accessibility(client);
        }

        /// <inheritdoc/>
        public bool IsDisposed { get; private set; }

        /// <inheritdoc/>
        public DevToolsConnection Connection { get; }

        /// <inheritdoc/>
        public event EventHandler Load;

        /// <inheritdoc/>
        public event EventHandler<ErrorEventArgs> Error;

        /// <inheritdoc/>
        public event EventHandler<MetricEventArgs> Metrics;

        /// <inheritdoc/>
        public event EventHandler DOMContentLoaded;

        /// <inheritdoc/>
        public event EventHandler<LifecycleEventArgs> LifecycleEvent;

        /// <summary>
        /// Raised when JavaScript within the page calls one of console API methods, e.g. <c>console.log</c> or <c>console.dir</c>. Also emitted if the page throws an error or a warning.
        /// The arguments passed into <c>console.log</c> appear as arguments on the event handler.
        /// </summary>
        /// <example>
        /// An example of handling <see cref="Console"/> event:
        /// <code>
        /// <![CDATA[
        /// devToolsContext.Console += (sender, e) =>
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
        /// <example>
        /// An example of handling <see cref="Response"/> event:
        /// <code>
        /// <![CDATA[
        /// var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        /// devToolsContext.Response += async(sender, e) =>
        /// {
        ///     if (e.Response.Url.Contains("script.js"))
        ///     {
        ///         tcs.TrySetResult(await e.Response.TextAsync());
        ///     }
        /// };
        ///
        /// await Task.WhenAll(
        ///     devToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html"),
        ///     tcs.Task);
        /// Console.WriteLine(await tcs.Task);
        /// ]]>
        /// </code>
        /// </example>
        public event EventHandler<ResponseCreatedEventArgs> Response;

        /// <summary>
        /// Raised when a browser issues a request. The <see cref="Request"/> object is read-only.
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
        /// Raised when a request ended up loading from cache.
        /// </summary>
        public event EventHandler<RequestEventArgs> RequestServedFromCache;

        /// <summary>
        /// Raised when an uncaught exception happens within the page.
        /// </summary>
        public event EventHandler<PageErrorEventArgs> PageError;

        /// <summary>
        /// Raised when the browser opens a new tab or window.
        /// </summary>
        public event EventHandler<PopupEventArgs> Popup;

        /// <summary>
        /// This setting will change the default maximum time for the following methods:
        /// - <see cref="GoToAsync(string, NavigationOptions)"/>
        /// - <see cref="GoBackAsync(NavigationOptions)"/>
        /// - <see cref="GoForwardAsync(NavigationOptions)"/>
        /// - <see cref="ReloadAsync(NavigationOptions)"/>
        /// - <see cref="SetContentAsync(string, NavigationOptions)"/>
        /// - <see cref="WaitForNavigationAsync(NavigationOptions)"/>
        /// **NOTE** <see cref="DefaultNavigationTimeout"/> takes priority over <seealso cref="DefaultTimeout"/>
        /// </summary>
        public int DefaultNavigationTimeout
        {
            get => _timeoutSettings.NavigationTimeout;
            set => _timeoutSettings.NavigationTimeout = value;
        }

        /// <summary>
        /// This setting will change the default maximum times for the following methods:
        /// - <see cref="GoBackAsync(NavigationOptions)"/>
        /// - <see cref="GoForwardAsync(NavigationOptions)"/>
        /// - <see cref="GoToAsync(string, NavigationOptions)"/>
        /// - <see cref="ReloadAsync(NavigationOptions)"/>
        /// - <see cref="SetContentAsync(string, NavigationOptions)"/>
        /// - <see cref="WaitForFunctionAsync(string, object[])"/>
        /// - <see cref="WaitForNavigationAsync(NavigationOptions)"/>
        /// - <see cref="WaitForRequestAsync(string, WaitForOptions)"/>
        /// - <see cref="WaitForResponseAsync(string, WaitForOptions)"/>
        /// - <see cref="WaitForXPathAsync(string, WaitForSelectorOptions)"/>
        /// - <see cref="WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
        /// - <see cref="WaitForExpressionAsync(string, WaitForFunctionOptions)"/>
        /// </summary>
        public int DefaultTimeout
        {
            get => _timeoutSettings.Timeout;
            set => _timeoutSettings.Timeout = value;
        }

        /// <summary>
        /// Gets browser's main frame
        /// </summary>
        /// <remarks>
        /// Browser is guaranteed to have a main frame which persists during navigations.
        /// </remarks>
        public Frame MainFrame => FrameManager.MainFrame;

        /// <summary>
        /// Gets all frames attached to the <see cref="DevToolsContext"/>.
        /// </summary>
        /// <value>An array of all frames attached to the browser.</value>
        public Frame[] Frames => FrameManager.GetFrames();

        /// <summary>
        /// Shortcut for <c>devToolsContext.MainFrame.Url</c>
        /// </summary>
        public string Url => MainFrame == null ? string.Empty : MainFrame.Url;

        /// <summary>
        /// Gets this devToolsContext keyboard
        /// </summary>
        public Keyboard Keyboard { get; }

        /// <summary>
        /// Gets this devToolsContext touchscreen
        /// </summary>
        public Touchscreen Touchscreen { get; }

        /// <summary>
        /// Gets this devToolsContext coverage
        /// </summary>
        public Coverage Coverage { get; }

        /// <summary>
        /// Gets this devToolsContext tracing
        /// </summary>
        public Tracing Tracing { get; }

        /// <summary>
        /// Gets this devToolsContext mouse
        /// </summary>
        public Mouse Mouse { get; }

        /// <summary>
        /// Gets this devToolsContext viewport
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
        /// Get an indication that the page has been closed.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the accessibility.
        /// </summary>
        public PageAccessibility.Accessibility Accessibility { get; }

        /// <summary>
        /// `true` if drag events are being intercepted, `false` otherwise.
        /// </summary>
        public bool IsDragInterceptionEnabled { get; private set; }

        /// <summary>
        /// Is Javascript Enabled
        /// </summary>
        public bool JavascriptEnabled { get; set; } = true;

        internal bool HasPopupEventListeners => Popup?.GetInvocationList().Any() == true;

        internal FrameManager FrameManager { get; private set; }

        /// <summary>
        /// Whether to enable drag interception.
        /// </summary>
        /// <remarks>
        /// Activating drag interception enables the `Input.drag`,
        /// methods This provides the capability to capture drag events emitted
        /// on the page, which can then be used to simulate drag-and-drop.
        /// </remarks>
        /// <param name="enabled">Interception enabled</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser</returns>
        public Task SetDragInterceptionAsync(bool enabled)
        {
            IsDragInterceptionEnabled = enabled;
            return Connection.SendAsync("Input.setInterceptDrags", new InputSetInterceptDragsRequest { Enabled = enabled });
        }

        /// <summary>
        /// Returns metrics
        /// </summary>
        /// <returns>Task which resolves into a list of metrics</returns>
        /// <remarks>
        /// All timestamps are in monotonic time: monotonically increasing time in seconds since an arbitrary point in the past.
        /// </remarks>
        public async Task<Dictionary<string, decimal>> MetricsAsync()
        {
            var response = await Connection.SendAsync<PerformanceGetMetricsResponse>("Performance.getMetrics").ConfigureAwait(false);
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
        /// The method runs <c>document.querySelector</c> within the <see cref="MainFrame"/>. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">A selector to query page for</param>
        /// <returns>Task which resolves to <see cref="ElementHandle"/> pointing to the frame element</returns>
        /// <remarks>
        /// Shortcut for <c>devToolsContext.MainFrame.QuerySelectorAsync(selector)</c>
        /// </remarks>
        /// <seealso cref="Frame.QuerySelectorAsync(string)"/>
        public Task<ElementHandle> QuerySelectorAsync(string selector)
            => MainFrame.QuerySelectorAsync(selector);

        /// <summary>
        /// Runs <c>document.querySelectorAll</c> within the <see cref="MainFrame"/>. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="selector">A selector to query page for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        /// <seealso cref="Frame.QuerySelectorAllAsync(string)"/>
        public Task<ElementHandle[]> QuerySelectorAllAsync(string selector)
            => MainFrame.QuerySelectorAllAsync(selector);

        /// <summary>
        /// A utility function to be used with <see cref="PuppeteerHandleExtensions.EvaluateFunctionAsync{T}(Task{JSHandle}, string, object[])"/>
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
        /// Shortcut for <c>devToolsContext.MainFrame.XPathAsync(expression)</c>
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
        /// await devToolsContext.EvaluateFunctionOnNewDocumentAsync("() => window.__example = true");
        /// </code>
        /// </example>
        /// <returns>Task</returns>
        public Task EvaluateFunctionOnNewDocumentAsync(string pageFunction, params object[] args)
        {
            var source = EvaluationString(pageFunction, args);
            return Connection.SendAsync("Page.addScriptToEvaluateOnNewDocument", new PageAddScriptToEvaluateOnNewDocumentRequest
            {
                Source = source
            });
        }

        /// <summary>
        /// Adds a function which would be invoked in one of the following scenarios:
        /// - whenever the page is navigated
        /// - whenever the child frame is attached or navigated. In this case, the function is invoked in the context of the newly attached frame
        /// </summary>
        /// <param name="expression">Javascript expression to be evaluated in browser context</param>
        /// <remarks>
        /// The function is invoked after the document was created but before any of its scripts were run. This is useful to amend JavaScript environment, e.g. to seed <c>Math.random</c>.
        /// </remarks>
        /// <example>
        /// An example of overriding the navigator.languages property before the page loads:
        /// <code>
        /// await devToolsContext.EvaluateExpressionOnNewDocumentAsync("window.__example = true;");
        /// </code>
        /// </example>
        /// <returns>Task</returns>
        public Task EvaluateExpressionOnNewDocumentAsync(string expression)
            => Connection.SendAsync("Page.addScriptToEvaluateOnNewDocument", new PageAddScriptToEvaluateOnNewDocumentRequest
            {
                Source = expression
            });

        /// <summary>
        /// The method iterates JavaScript heap and finds all the objects with the given prototype.
        /// Shortcut for <c>devToolsContext.MainFrame.GetExecutionContextAsync().QueryObjectsAsync(prototypeHandle)</c>.
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
            => FrameManager.NetworkManager.SetRequestInterceptionAsync(value);

        /// <summary>
        /// Set offline mode for the browser.
        /// </summary>
        /// <returns>Result task</returns>
        /// <param name="value">When <c>true</c> enables offline mode for the browser.</param>
        public Task SetOfflineModeAsync(bool value) => FrameManager.NetworkManager.SetOfflineModeAsync(value);

        /// <summary>
        /// Emulates network conditions
        /// </summary>
        /// <param name="networkConditions">Passing <c>null</c> disables network condition emulation.</param>
        /// <returns>Result task</returns>
        /// <remarks>
        /// **NOTE** This does not affect WebSockets and WebRTC PeerConnections (see https://crbug.com/563644)
        /// </remarks>
        public Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions) => FrameManager.NetworkManager.EmulateNetworkConditionsAsync(networkConditions);

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
            if (urls == null)
            {
                throw new ArgumentNullException(nameof(urls));
            }

            var response = await Connection.SendAsync<NetworkGetCookiesResponse>("Network.getCookies", new NetworkGetCookiesRequest
            {
                Urls = urls.Length > 0 ? urls : new string[] { Url }
            }).ConfigureAwait(false);

            return response.Cookies;
        }

        /// <summary>
        /// Clears all of the current cookies and then sets the cookies for the page
        /// </summary>
        /// <param name="cookies">Cookies to set</param>
        /// <returns>Task</returns>
        public async Task SetCookieAsync(params CookieParam[] cookies)
        {
            if (cookies == null)
            {
                throw new ArgumentNullException(nameof(cookies));
            }

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
                await Connection.SendAsync("Network.setCookies", new NetworkSetCookiesRequest
                {
                    Cookies = cookies
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
            if (cookies == null)
            {
                throw new ArgumentNullException(nameof(cookies));
            }

            var pageURL = Url;
            foreach (var cookie in cookies)
            {
                if (string.IsNullOrEmpty(cookie.Url) && pageURL.StartsWith("http", StringComparison.Ordinal))
                {
                    cookie.Url = pageURL;
                }
                await Connection.SendAsync("Network.deleteCookies", cookie).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Adds a <c><![CDATA[<script>]]></c> tag into the page with the desired url or content
        /// </summary>
        /// <param name="options">add script tag options</param>
        /// <remarks>
        /// Shortcut for <c>devToolsContext.MainFrame.AddScriptTagAsync(options)</c>
        /// </remarks>
        /// <returns>Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame</returns>
        /// <seealso cref="Frame.AddScriptTagAsync(AddTagOptions)"/>
        public Task<ElementHandle> AddScriptTagAsync(AddTagOptions options) => MainFrame.AddScriptTagAsync(options);

        /// <summary>
        /// Adds a <c><![CDATA[<script>]]></c> tag into the page with the desired url or content
        /// </summary>
        /// <param name="url">script url</param>
        /// <remarks>
        /// Shortcut for <c>devToolsContext.MainFrame.AddScriptTagAsync(new AddTagOptions { Url = url })</c>
        /// </remarks>
        /// <returns>Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame</returns>
        public Task<ElementHandle> AddScriptTagAsync(string url) => AddScriptTagAsync(new AddTagOptions { Url = url });

        /// <summary>
        /// Adds a <c><![CDATA[<link rel="stylesheet">]]></c> tag into the page with the desired url or a <c><![CDATA[<link rel="stylesheet">]]></c> tag with the content
        /// </summary>
        /// <param name="options">add style tag options</param>
        /// <remarks>
        /// Shortcut for <c>devToolsContext.MainFrame.AddStyleTagAsync(options)</c>
        /// </remarks>
        /// <returns>Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame</returns>
        public Task<ElementHandle> AddStyleTagAsync(AddTagOptions options) => MainFrame.AddStyleTagAsync(options);

        /// <summary>
        /// Adds a <c><![CDATA[<link rel="stylesheet">]]></c> tag into the page with the desired url or a <c><![CDATA[<link rel="stylesheet">]]></c> tag with the content
        /// </summary>
        /// <param name="url">stylesheel url</param>
        /// <remarks>
        /// Shortcut for <c>devToolsContext.MainFrame.AddStyleTagAsync(new AddTagOptions { Url = url })</c>
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
        /// <typeparam name="T">The parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/></typeparam>
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
        /// <typeparam name="T1">The first parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T2">The second parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/></typeparam>
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
        /// <typeparam name="T1">The first parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T2">The second parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T3">The third parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/></typeparam>
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
        /// <typeparam name="T1">The first parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T2">The second parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T3">The third parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="T4">The fourth parameter of <paramref name="puppeteerFunction"/></typeparam>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/></typeparam>
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
        public Task<string> GetContentAsync() => FrameManager.MainFrame.GetContentAsync();

        /// <summary>
        /// Sets the HTML markup to the page
        /// </summary>
        /// <param name="html">HTML markup to assign to the browser.</param>
        /// <param name="options">The navigations options</param>
        /// <returns>Task.</returns>
        /// <seealso cref="Frame.SetContentAsync(string, NavigationOptions)"/>
        public Task SetContentAsync(string html, NavigationOptions options = null) => FrameManager.MainFrame.SetContentAsync(html, options);

        /// <summary>
        /// Navigates to an url
        /// </summary>
        /// <remarks>
        /// <see cref="GoToAsync(string, int?, WaitUntilNavigation[])"/> will throw an error if:
        /// - there's an SSL error (e.g. in case of self-signed certificates).
        /// - target URL is invalid.
        /// - the `timeout` is exceeded during navigation.
        /// - the remote server does not respond or is unreachable.
        /// - the main resource failed to load.
        ///
        /// <see cref="GoToAsync(string, int?, WaitUntilNavigation[])"/> will not throw an error when any valid HTTP status code is returned by the remote server,
        /// including 404 "Not Found" and 500 "Internal Server Error".  The status code for such responses can be retrieved by calling <see cref="Response.Status"/>
        ///
        /// > **NOTE** <see cref="GoToAsync(string, int?, WaitUntilNavigation[])"/> either throws an error or returns a main resource response.
        /// The only exceptions are navigation to `about:blank` or navigation to the same URL with a different hash, which would succeed and return `null`.
        ///
        /// > **NOTE** Headless mode doesn't support navigation to a PDF document. See the <see fref="https://bugs.chromium.org/p/chromium/issues/detail?id=761295">upstream issue</see>.
        ///
        /// Shortcut for <seealso cref="Frame.GoToAsync(string, int?, WaitUntilNavigation[])"/>
        /// </remarks>
        /// <param name="url">URL to navigate page to. The url should include scheme, e.g. https://.</param>
        /// <param name="options">Navigation parameters.</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.</returns>
        /// <seealso cref="GoToAsync(string, int?, WaitUntilNavigation[])"/>
        public Task<Response> GoToAsync(string url, NavigationOptions options) => FrameManager.MainFrame.GoToAsync(url, options);

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
        /// Navigates to an url
        /// </summary>
        /// <param name="url">URL to navigate page to. The url should include scheme, e.g. https://.</param>
        /// <param name="waitUntil">When to consider navigation succeeded.</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect</returns>
        /// <seealso cref="GoToAsync(string, NavigationOptions)"/>
        public Task<Response> GoToAsync(string url, WaitUntilNavigation waitUntil)
            => GoToAsync(url, new NavigationOptions { WaitUntil = new[] { waitUntil } });

        /// <summary>
        /// generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <param name="file">The file path to save the PDF to. paths are resolved using <see cref="Path.GetFullPath(string)"/></param>
        /// <returns></returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public Task PdfAsync(string file) => PdfAsync(file, new PdfOptions());

        /// <summary>
        ///  generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <param name="file">The file path to save the PDF to. paths are resolved using <see cref="Path.GetFullPath(string)"/></param>
        /// <param name="options">pdf options</param>
        /// <returns></returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public async Task PdfAsync(string file, PdfOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            await PdfInternalAsync(file, options).ConfigureAwait(false);
        }

        /// <summary>
        /// generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public Task<Stream> PdfStreamAsync() => PdfStreamAsync(new PdfOptions());

        /// <summary>
        /// Generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <param name="options">pdf options</param>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public async Task<Stream> PdfStreamAsync(PdfOptions options)
            => new MemoryStream(await PdfDataAsync(options).ConfigureAwait(false));

        /// <summary>
        /// Generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public Task<byte[]> PdfDataAsync() => PdfDataAsync(new PdfOptions());

        /// <summary>
        /// Generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>
        /// </summary>
        /// <param name="options">pdf options</param>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless
        /// </remarks>
        public Task<byte[]> PdfDataAsync(PdfOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return PdfInternalAsync(null, options);
        }

        internal async Task<byte[]> PdfInternalAsync(string file, PdfOptions options)
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

            if (options.OmitBackground)
            {
                await SetTransparentBackgroundColorAsync().ConfigureAwait(false);
            }

            var result = await Connection.SendAsync<PagePrintToPDFResponse>("Page.printToPDF", new PagePrintToPDFRequest
            {
                TransferMode = "ReturnAsStream",
                Landscape = options.Landscape,
                DisplayHeaderFooter = options.DisplayHeaderFooter,
                HeaderTemplate = options.HeaderTemplate,
                FooterTemplate = options.FooterTemplate,
                PrintBackground = options.PrintBackground,
                Scale = options.Scale,
                PaperWidth = paperWidth,
                PaperHeight = paperHeight,
                MarginTop = marginTop,
                MarginBottom = marginBottom,
                MarginLeft = marginLeft,
                MarginRight = marginRight,
                PageRanges = options.PageRanges,
                PreferCSSPageSize = options.PreferCSSPageSize
            }).ConfigureAwait(false);

            if (options.OmitBackground)
            {
                await ResetDefaultBackgroundColorAsync().ConfigureAwait(false);
            }

            return await ProtocolStreamReader.ReadProtocolStreamByteAsync(Connection, result.Stream, file).ConfigureAwait(false);
        }

        /// <summary>
        /// Requests that page scale factor is reset to initial values.
        /// </summary>
        /// <returns>Task.</returns>
        public Task ResetPageScaleFactorAsync()
        {
            return Connection.SendAsync("Emulation.resetPageScaleFactor");
        }

        /// <summary>
        /// Sets a specified page scale factor.
        /// </summary>
        /// <param name="pageScaleFactor">Page scale factor.</param>
        /// <returns>Task.</returns>
        public Task SetPageScaleFactorAsync(double pageScaleFactor)
        {
            return Connection.SendAsync("Emulation.setPageScaleFactor", new EmulationSetPageScaleFactor { PageScaleFactor = pageScaleFactor });
        }

        /// <summary>
        /// Enables/Disables Javascript on the page
        /// </summary>
        /// <returns>Task.</returns>
        /// <param name="enabled">Whether or not to enable JavaScript on the browser.</param>
        public Task SetJavaScriptEnabledAsync(bool enabled)
        {
            if (enabled == JavascriptEnabled)
            {
                return Task.CompletedTask;
            }
            JavascriptEnabled = enabled;
            return Connection.SendAsync("Emulation.setScriptExecutionDisabled", new EmulationSetScriptExecutionDisabledRequest
            {
                Value = !enabled
            });
        }

        /// <summary>
        /// Automatically render all web contents using a dark theme.
        /// </summary>
        /// <param name="enabled">Whether to enable or disable automatic dark mode. If not specified, any existing override will be cleared.</param>
        /// <returns>Task.</returns>
        public Task SetAutoDarkModeOverrideAsync(bool? enabled)
        {
            return Connection.SendAsync("Emulation.setAutoDarkModeOverride", new SetAutoDarkModeOverrideRequest
            {
                Enabled = enabled
            });
        }

        /// <summary>
        /// Toggles bypassing page's Content-Security-Policy.
        /// </summary>
        /// <param name="enabled">sets bypassing of page's Content-Security-Policy.</param>
        /// <returns></returns>
        /// <remarks>
        /// CSP bypassing happens at the moment of CSP initialization rather then evaluation.
        /// Usually this means that <see cref="SetBypassCSPAsync(bool)"/> should be called before navigating to the domain.
        /// </remarks>
        public Task SetBypassCSPAsync(bool enabled) => Connection.SendAsync("Page.setBypassCSP", new PageSetBypassCSPRequest
        {
            Enabled = enabled
        });

        /// <summary>
        /// Emulates a media such as screen or print.
        /// </summary>
        /// <param name="type">Media to set.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('screen').matches)");
        /// // → true
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('print').matches)");
        /// // → true
        /// await devToolsContext.EmulateMediaTypeAsync(MediaType.Print);
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('screen').matches)");
        /// // → false
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('print').matches)");
        /// // → true
        /// await devToolsContext.EmulateMediaTypeAsync(MediaType.None);
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('screen').matches)");
        /// // → true
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('print').matches)");
        /// // → true
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>Emulate media type task.</returns>
        public Task EmulateMediaTypeAsync(MediaType type)
            => Connection.SendAsync("Emulation.setEmulatedMedia", new EmulationSetEmulatedMediaTypeRequest { Media = type });

        /// <summary>
        /// Given an array of media feature objects, emulates CSS media features on the page.
        /// </summary>
        /// <param name="features">Features to apply</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// await devToolsContext.EmulateMediaFeaturesAsync(new MediaFeature[]{ new MediaFeature { MediaFeature =  MediaFeature.PrefersColorScheme, Value = "dark" }});
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches)");
        /// // → true
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches)");
        /// // → false
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
        /// // → false
        /// await devToolsContext.EmulateMediaFeaturesAsync(new MediaFeature[]{ new MediaFeature { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" }});
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches)");
        /// // → true
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
        /// // → false
        /// await devToolsContext.EmulateMediaFeaturesAsync(new MediaFeature[]
        /// {
        ///   new MediaFeature { MediaFeature = MediaFeature.PrefersColorScheme, Value = "dark" },
        ///   new MediaFeature { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" },
        /// });
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches)");
        /// // → true
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches)");
        /// // → false
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
        /// // → false
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches)");
        /// // → true
        /// await devToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
        /// // → false
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>Emulate features task</returns>
        public Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features)
            => Connection.SendAsync("Emulation.setEmulatedMedia", new EmulationSetEmulatedMediaFeatureRequest { Features = features });

        /// <summary>
        /// Sets the viewport.
        /// In the case of multiple pages in a single browser, each page can have its own viewport size.
        /// <see cref="SetViewportAsync(ViewPortOptions)"/> will resize the page. A lot of websites don't expect phones to change size, so you should set the viewport before navigating to the page.
        /// </summary>
        /// <example>
        ///<![CDATA[
        /// await devToolsContext.SetViewPortAsync(new ViewPortOptions
        /// {
        ///     Width = 640,
        ///     Height = 480,
        ///     DeviceScaleFactor = 1
        /// });
        /// await devToolsContext.goto('https://www.example.com');
        /// ]]>
        /// </example>
        /// <returns>The viewport task.</returns>
        /// <param name="viewport">Viewport options.</param>
        public async Task SetViewportAsync(ViewPortOptions viewport)
        {
            if (viewport == null)
            {
                throw new ArgumentNullException(nameof(viewport));
            }

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
        /// <see cref="SetViewportAsync(ViewPortOptions)"/>
        /// <see cref="SetUserAgentAsync(string)"/>
        /// To aid emulation, puppeteer provides a list of device descriptors which can be obtained via the <see cref="Emulation.Devices"/>.
        /// <see cref="EmulateAsync(DeviceDescriptor)"/> will resize the page. A lot of websites don't expect phones to change size, so you should emulate before navigating to the page.
        /// </remarks>
        /// <example>
        ///<![CDATA[
        /// var iPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];
        /// await devToolsContext.EmulateAsync(iPhone);
        /// await devToolsContext.goto('https://www.google.com');
        /// ]]>
        /// </example>
        /// <returns>Task.</returns>
        /// <param name="options">Emulation options.</param>
        public Task EmulateAsync(DeviceDescriptor options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return Task.WhenAll(
                SetViewportAsync(options.ViewPort),
                SetUserAgentAsync(options.UserAgent));
        }

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
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (!options.Type.HasValue)
            {
                options.Type = ScreenshotOptions.GetScreenshotTypeFromFile(file);

                if (options.Type == ScreenshotType.Jpeg && !options.Quality.HasValue)
                {
                    options.Quality = 90;
                }
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
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

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
            if (options?.Clip?.Width == 0)
            {
                throw new PuppeteerException("Expected options.Clip.Width not to be 0.");
            }
            if (options?.Clip?.Height == 0)
            {
                throw new PuppeteerException("Expected options.Clip.Height not to be 0.");
            }

            if (options.Clip != null && options.FullPage)
            {
                throw new ArgumentException("options.clip and options.fullPage are exclusive");
            }

            return PerformScreenshot(screenshotType.Value, options);
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
        /// Toggles ignoring cache for each request based on the enabled state. By default, caching is enabled.
        /// </summary>
        /// <param name="enabled">sets the <c>enabled</c> state of the cache</param>
        /// <returns>Task</returns>
        public Task SetCacheEnabledAsync(bool enabled = true)
            => FrameManager.NetworkManager.SetCacheEnabledAsync(enabled);

        /// <summary>
        /// Fetches an element with <paramref name="selector"/>, scrolls it into view if needed, and then uses <see cref="Mouse"/> to click in the center of the element.
        /// </summary>
        /// <param name="selector">A selector to search for element to click. If there are multiple elements satisfying the selector, the first will be clicked.</param>
        /// <param name="options">click options</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/></exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully clicked</returns>
        public Task ClickAsync(string selector, ClickOptions options = null) => FrameManager.MainFrame.ClickAsync(selector, options);

        /// <summary>
        /// Fetches an element with <paramref name="selector"/>, scrolls it into view if needed, and then uses <see cref="Mouse"/> to hover over the center of the element.
        /// </summary>
        /// <param name="selector">A selector to search for element to hover. If there are multiple elements satisfying the selector, the first will be hovered.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/></exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully hovered</returns>
        public Task HoverAsync(string selector) => FrameManager.MainFrame.HoverAsync(selector);

        /// <summary>
        /// Fetches an element with <paramref name="selector"/> and focuses it
        /// </summary>
        /// <param name="selector">A selector to search for element to focus. If there are multiple elements satisfying the selector, the first will be focused.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/></exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully focused</returns>
        public Task FocusAsync(string selector) => FrameManager.MainFrame.FocusAsync(selector);

        /// <summary>
        /// Sends a <c>keydown</c>, <c>keypress</c>/<c>input</c>, and <c>keyup</c> event for each character in the text.
        /// </summary>
        /// <param name="selector">A selector of an element to type into. If there are multiple elements satisfying the selector, the first will be used.</param>
        /// <param name="text">A text to type into a focused element</param>
        /// <param name="options">The options to apply to the type operation.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/></exception>
        /// <remarks>
        /// To press a special key, like <c>Control</c> or <c>ArrowDown</c> use <see cref="Input.Keyboard.PressAsync(string, PressOptions)"/>
        /// </remarks>
        /// <example>
        /// <code>
        /// await devToolsContext.TypeAsync("#mytextarea", "Hello"); // Types instantly
        /// await devToolsContext.TypeAsync("#mytextarea", "World", new TypeOptions { Delay = 100 }); // Types slower, like a user
        /// </code>
        /// </example>
        /// <returns>Task</returns>
        public Task TypeAsync(string selector, string text, TypeOptions options = null)
            => FrameManager.MainFrame.TypeAsync(selector, text, options);

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <example>
        /// An example of scraping information from all hyperlinks on the page.
        /// <code>
        /// var hyperlinkInfo = await devToolsContext.EvaluateExpressionAsync(@"
        ///     Array
        ///        .from(document.querySelectorAll('a'))
        ///        .map(n => ({
        ///            text: n.innerText,
        ///            href: n.getAttribute('href'),
        ///            target: n.getAttribute('target')
        ///         }))
        /// ");
        /// Console.WriteLine(hyperlinkInfo.ToString()); // Displays JSON array of hyperlinkInfo objects
        /// </code>
        /// </example>
        /// <seealso href="https://www.newtonsoft.com/json/help/html/t_newtonsoft_json_linq_jtoken.htm"/>
        /// <seealso cref="EvaluateFunctionAsync{T}(string, object[])"/>
        /// <returns>Task which resolves to script return value</returns>
        public Task<JToken> EvaluateExpressionAsync(string script)
            => FrameManager.MainFrame.EvaluateExpressionAsync<JToken>(script);

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
            => FrameManager.MainFrame.EvaluateExpressionAsync<T>(script);

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="JSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <seealso cref="EvaluateExpressionAsync{T}(string)"/>
        /// <returns>Task which resolves to script return value</returns>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
            => FrameManager.MainFrame.EvaluateFunctionAsync<JToken>(script, args);

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
            => FrameManager.MainFrame.EvaluateFunctionAsync<T>(script, args);

        /// <summary>
        /// Sets the user agent to be used in this page
        /// </summary>
        /// <param name="userAgent">Specific user agent to use in this page</param>
        /// <returns>Task</returns>
        public Task SetUserAgentAsync(string userAgent)
            => FrameManager.NetworkManager.SetUserAgentAsync(userAgent);

        /// <summary>
        /// Sets extra HTTP headers that will be sent with every request the page initiates
        /// </summary>
        /// <param name="headers">Additional http headers to be sent with every request</param>
        /// <returns>Task</returns>
        public Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            return FrameManager.NetworkManager.SetExtraHTTPHeadersAsync(headers);
        }

        /// <summary>
        /// Provide credentials for http authentication <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication"/>
        /// </summary>
        /// <param name="credentials">The credentials</param>
        /// <returns></returns>
        /// <remarks>
        /// To disable authentication, pass <c>null</c>
        /// </remarks>
        public Task AuthenticateAsync(Credentials credentials) => FrameManager.NetworkManager.AuthenticateAsync(credentials);

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
              Connection.SendAsync("Page.reload", new PageReloadRequest { FrameId = MainFrame.Id })).ConfigureAwait(false);

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
        /// Waits for a timeout
        /// </summary>
        /// <param name="milliseconds">The amount of time to wait.</param>
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
        /// <returns>A task that resolves when element specified by selector string is added to DOM.
        /// Resolves to `null` if waiting for `hidden: true` and selector is not found in DOM.</returns>
        /// <seealso cref="WaitForXPathAsync(string, WaitForSelectorOptions)"/>
        /// <seealso cref="Frame.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
        public Task<ElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
            => MainFrame.WaitForSelectorAsync(selector, options ?? new WaitForSelectorOptions());

        /// <summary>
        /// Waits for a xpath selector to be added to the DOM
        /// </summary>
        /// <param name="xpath">A xpath selector of an element to wait for</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <returns>A task which resolves when element specified by xpath string is added to DOM.
        /// Resolves to `null` if waiting for `hidden: true` and xpath is not found in DOM.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// string currentURL = null;
        /// devToolsContext
        ///     .WaitForXPathAsync("//img")
        ///     .ContinueWith(_ => Console.WriteLine("First URL with image: " + currentURL));
        /// foreach (var current in new[] { "https://example.com", "https://google.com", "https://bbc.com" })
        /// {
        ///     currentURL = current;
        ///     await devToolsContext.GoToAsync(currentURL);
        /// }
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
        /// var navigationTask = devToolsContext.WaitForNavigationAsync();
        /// await devToolsContext.ClickAsync("a.my-link");
        /// await navigationTask;
        /// ]]>
        /// </code>
        /// </example>
        public Task<Response> WaitForNavigationAsync(NavigationOptions options = null) => FrameManager.WaitForFrameNavigationAsync(FrameManager.MainFrame, options);

        /// <summary>
        /// Waits for Network Idle
        /// </summary>
        /// <param name="options">Optional waiting parameters</param>
        /// <returns>returns Task which resolves when network is idle</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// devToolsContext.EvaluateFunctionAsync("() => fetch('some-url')");
        /// await devToolsContext.WaitForNetworkIdle(); // The Task resolves after fetch above finishes
        /// ]]>
        /// </code>
        /// </example>
        public async Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultTimeout;
            var idleTime = options?.IdleTime ?? 500;

            var networkIdleTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var idleTimer = new Timer
            {
                Interval = idleTime
            };

            idleTimer.Elapsed += (sender, args) =>
            {
                networkIdleTcs.TrySetResult(true);
            };

            var networkManager = FrameManager.NetworkManager;

            void Evaluate()
            {
                idleTimer.Stop();

                if (networkManager.NumRequestsInProgress == 0)
                {
                    idleTimer.Start();
                }
            }

            void RequestEventListener(object sender, RequestEventArgs e) => Evaluate();
            void ResponseEventListener(object sender, ResponseCreatedEventArgs e) => Evaluate();

            void Cleanup()
            {
                idleTimer.Stop();
                idleTimer.Dispose();

                networkManager.Request -= RequestEventListener;
                networkManager.Response -= ResponseEventListener;
            }

            networkManager.Request += RequestEventListener;
            networkManager.Response += ResponseEventListener;

            Evaluate();

            await networkIdleTcs.Task.WithTimeout(timeout, t =>
            {
                Cleanup();

                return new TimeoutException($"Timeout of {t.TotalMilliseconds} ms exceeded");
            }).ConfigureAwait(false);

            Cleanup();
        }

        /// <summary>
        /// Waits for a request.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var firstRequest = await devToolsContext.WaitForRequestAsync("http://example.com/resource");
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
        /// var request = await devToolsContext.WaitForRequestAsync(request => request.Url === "http://example.com" && request.Method === HttpMethod.Get;
        /// return request.Url;
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>A task which resolves when a matching request was made.</returns>
        /// <param name="predicate">Function which looks for a matching request.</param>
        /// <param name="options">Options.</param>
        public async Task<Request> WaitForRequestAsync(Func<Request, bool> predicate, WaitForOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultTimeout;
            var requestTcs = new TaskCompletionSource<Request>(TaskCreationOptions.RunContinuationsAsynchronously);

            void requestEventListener(object sender, RequestEventArgs e)
            {
                if (predicate(e.Request))
                {
                    requestTcs.TrySetResult(e.Request);
                    FrameManager.NetworkManager.Request -= requestEventListener;
                }
            }

            FrameManager.NetworkManager.Request += requestEventListener;

            await requestTcs.Task.WithTimeout(timeout, t =>
            {
                FrameManager.NetworkManager.Request -= requestEventListener;
                return new TimeoutException($"Timeout of {t.TotalMilliseconds} ms exceeded");
            }).ConfigureAwait(false);

            return await requestTcs.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Waits for a response.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var firstResponse = await devToolsContext.WaitForResponseAsync("http://example.com/resource");
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
        /// var response = await devToolsContext.WaitForResponseAsync(response => response.Url === "http://example.com" && response.Status === HttpStatus.Ok;
        /// return response.Url;
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>A task which resolves when a matching response is received.</returns>
        /// <param name="predicate">Function which looks for a matching response.</param>
        /// <param name="options">Options.</param>
        public async Task<Response> WaitForResponseAsync(Func<Response, bool> predicate, WaitForOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultTimeout;
            var responseTcs = new TaskCompletionSource<Response>(TaskCreationOptions.RunContinuationsAsynchronously);

            void responseEventListener(object sender, ResponseCreatedEventArgs e)
            {
                if (predicate(e.Response))
                {
                    responseTcs.TrySetResult(e.Response);
                    FrameManager.NetworkManager.Response -= responseEventListener;
                }
            }

            FrameManager.NetworkManager.Response += responseEventListener;

            await responseTcs.Task.WithTimeout(timeout).ConfigureAwait(false);

            return await responseTcs.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Waits for a page to open a file picker
        /// </summary>
        /// <remarks>
        /// In non-headless Chromium, this method results in the native file picker dialog **not showing up** for the user.
        /// </remarks>
        /// <example>
        /// This method is typically coupled with an action that triggers file choosing.
        /// The following example clicks a button that issues a file chooser, and then
        /// responds with `/tmp/myfile.pdf` as if a user has selected this file.
        /// <code>
        /// <![CDATA[
        /// var waitTask = devToolsContext.WaitForFileChooserAsync();
        /// await Task.WhenAll(
        ///     waitTask,
        ///     page.ClickAsync("#upload-file-button")); // some button that triggers file selection
        ///
        /// await waitTask.Result.AcceptAsync('/tmp/myfile.pdf');
        /// ]]>
        /// </code>
        ///
        /// This must be called *before* the file chooser is launched. It will not return a currently active file chooser.
        /// </example>
        /// <param name="options">Optional waiting parameters.</param>
        /// <returns>A task that resolves after a page requests a file picker.</returns>
        public async Task<FileChooser> WaitForFileChooserAsync(WaitForFileChooserOptions options = null)
        {
            if (!_fileChooserInterceptors.Any())
            {
                await Connection.SendAsync("Page.setInterceptFileChooserDialog", new PageSetInterceptFileChooserDialog
                {
                    Enabled = true
                }).ConfigureAwait(false);
            }

            var timeout = options?.Timeout ?? _timeoutSettings.Timeout;
            var tcs = new TaskCompletionSource<FileChooser>(TaskCreationOptions.RunContinuationsAsynchronously);
            var guid = Guid.NewGuid();
            _fileChooserInterceptors.TryAdd(guid, tcs);

            try
            {
                return await tcs.Task.WithTimeout(timeout).ConfigureAwait(false);
            }
            catch (Exception)
            {
                _fileChooserInterceptors.TryRemove(guid, out _);
                throw;
            }
        }

        /// <summary>
        /// Navigate to the previous page in history.
        /// </summary>
        /// <returns>Task that resolves to the main resource response. In case of multiple redirects,
        /// the navigation will resolve with the response of the last redirect. If can not go back, resolves to null.</returns>
        /// <param name="options">Navigation parameters.</param>
        public Task<Response> GoBackAsync(NavigationOptions options = null) => GoAsync(-1, options);

        /// <summary>
        /// Navigate to the next page in history.
        /// </summary>
        /// <returns>Task that resolves to the main resource response. In case of multiple redirects,
        /// the navigation will resolve with the response of the last redirect. If can not go forward, resolves to null.</returns>
        /// <param name="options">Navigation parameters.</param>
        public Task<Response> GoForwardAsync(NavigationOptions options = null) => GoAsync(1, options);

        /// <summary>
        /// Resets the background color and Viewport after taking Screenshots using BurstMode.
        /// </summary>
        /// <returns>The burst mode off.</returns>
        public Task SetBurstModeOffAsync()
        {
            _screenshotBurstModeOn = false;
            if (_screenshotBurstModeOptions != null)
            {
                ResetBackgroundColorAndViewportAsync(_screenshotBurstModeOptions);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Brings page to front (activates tab).
        /// </summary>
        /// <returns>A task that resolves when the message has been sent to Chromium.</returns>
        public Task BringToFrontAsync() => Connection.SendAsync("Page.bringToFront");

        /// <summary>
        /// Enable/disable whether all certificate errors should be ignored.
        /// </summary>
        /// <param name="ignore">if true all certificate errors will be ignored</param>
        /// <returns>A task that resolves when the command has executed.</returns>
        public Task IgnoreCertificateErrorsAsync(bool ignore = true)
        {
            return Connection.SendAsync("Security.setIgnoreCertificateErrors", new SecuritySetIgnoreCertificateErrorsRequest
            {
                Ignore = ignore
            });
        }

        /// <summary>
        /// Simulates the given vision deficiency on the page.
        /// </summary>
        /// <example>
        /// await devToolsContext.EmulateVisionDeficiencyAsync(VisionDeficiency.Achromatopsia);
        /// await devToolsContext.ScreenshotAsync("Achromatopsia.png");
        /// </example>
        /// <param name="type">The type of deficiency to simulate, or <see cref="VisionDeficiency.None"/> to reset.</param>
        /// <returns>A task that resolves when the message has been sent to the browser.</returns>
        public Task EmulateVisionDeficiencyAsync(VisionDeficiency type)
            => Connection.SendAsync("Emulation.setEmulatedVisionDeficiency", new EmulationSetEmulatedVisionDeficiencyRequest
            {
                Type = type,
            });

        /// <summary>
        /// Changes the timezone of the browser.
        /// </summary>
        /// <param name="timezoneId">Timezone to set. See <seealso href="https://cs.chromium.org/chromium/src/third_party/icu/source/data/misc/metaZones.txt?rcl=faee8bc70570192d82d2978a71e2a615788597d1" >ICU’s `metaZones.txt`</seealso>
        /// for a list of supported timezone IDs. Passing `null` disables timezone emulation.</param>
        /// <returns>The viewport task.</returns>
        public async Task EmulateTimezoneAsync(string timezoneId)
        {
            try
            {
                await Connection.SendAsync("Emulation.setTimezoneOverride", new EmulateTimezoneRequest
                {
                    TimezoneId = timezoneId ?? string.Empty
                }).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.Message.Contains("Invalid timezone"))
            {
                throw new PuppeteerException($"Invalid timezone ID: {timezoneId}");
            }
        }

        /// <summary>
        /// Enables CPU throttling to emulate slow CPUs.
        /// </summary>
        /// <param name="factor">Throttling rate as a slowdown factor (1 is no throttle, 2 is 2x slowdown, etc).</param>
        /// <returns>A task that resolves when the message has been sent to the browser.</returns>
        public Task EmulateCPUThrottlingAsync(decimal? factor = null)
        {
            if (factor != null && factor < 1)
            {
                throw new ArgumentException("Throttling rate should be greater or equal to 1", nameof(factor));
            }

            return Connection.SendAsync("Emulation.setCPUThrottlingRate", new EmulationSetCPUThrottlingRateRequest
            {
                Rate = factor ?? 1
            });
        }

        internal void OnPopup(DevToolsContext popupPage) => Popup?.Invoke(this, new PopupEventArgs { PopupPage = popupPage });

        /// <summary>
        /// Create a new <see cref="DevToolsContext"/> instance. It's reccommended that you only
        /// create a single <see cref="DevToolsContext"/> per ChromiumWebBrowser instance. Store and reuse a single reference.
        /// If you need to create multiple, make sure to Dispose of the previous instance before creating a new instance.
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="ignoreHTTPSerrors">ignore certificate errors</param>
        /// <returns>Task that can be awaited to obtain the DevToolsContext</returns>
        public static Task<DevToolsContext> CreateDevToolsContextAsync(DevToolsConnection connection, bool ignoreHTTPSerrors = false)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            return CreateAsync(connection, ignoreHTTPSerrors);
        }

        /// <summary>
        /// Create a new <see cref="DevToolsContext"/> instance. It's reccommended that you only
        /// create a single <see cref="DevToolsContext"/> per ChromiumWebBrowser instance. Store and reuse a single reference.
        /// If you need to create multiple, make sure to Dispose of the previous instance before creating a new instance.
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="ignoreHTTPSerrors">ignore certificate errors</param>
        /// <returns>Task that can be awaited to obtain the DevToolsContext</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Task<DevToolsContext> GetDevToolsContextAsync(DevToolsConnection connection, bool ignoreHTTPSerrors = false)
        {
            return CreateDevToolsContextAsync(connection, ignoreHTTPSerrors);
        }

        internal static async Task<DevToolsContext> CreateAsync(
            DevToolsConnection client,
            bool ignoreHTTPSErrors)
        {
            var devToolsContext = new DevToolsContext(client);

            await devToolsContext.InitializeAsync().ConfigureAwait(false);

            if (ignoreHTTPSErrors)
            {
                await devToolsContext.IgnoreCertificateErrorsAsync().ConfigureAwait(false);
            }

            return devToolsContext;
        }

        /// <summary>
        /// Create a new <see cref="IDevToolsContext"/> instance that's used
        /// when running CefSharp out of process. Doesn't send and domain
        /// enable commands or get the frame tree
        /// </summary>
        /// <param name="connection">connection</param>
        /// <returns>DevToolsContext</returns>
        public static IDevToolsContext CreateForOutOfProcess(DevToolsConnection connection)
        {
            var devToolsContext = new DevToolsContext(connection);

            devToolsContext.WireUpEvents();

            return devToolsContext;
        }

        /// <summary>
        /// Passes an expression to the <see cref="DevToolsContext.EvaluateExpressionHandleAsync(string)"/>, returns a <see cref="Task"/>, then <see cref="DevToolsContext.EvaluateExpressionHandleAsync(string)"/> would wait for the <see cref="Task"/> to resolve and return its value.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var handle = await devToolsContext.EvaluateExpressionHandleAsync<HtmlElement>("button");
        /// ]]>
        /// </code>
        /// </example>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="script">Expression to be evaluated in the <seealso cref="ExecutionContext"/></param>
        /// <returns>Resolves to the return value of <paramref name="script"/></returns>
        public async Task<T> EvaluateExpressionHandleAsync<T>(string script)
            where T : DomHandle
        {
            var handle = await EvaluateExpressionHandleAsync(script).ConfigureAwait(false);

            return handle.ToDomHandle<T>();
        }

        /// <summary>
        /// Creates the HTML element specified
        /// </summary>
        /// <typeparam name="T">HtmlElementType</typeparam>
        /// <param name="tagName">
        /// A string that specifies the type of element to be created.
        /// The nodeName of the created element is initialized with the
        /// value of tagName. Don't use qualified names (like "html:a")
        /// with this method.
        /// </param>
        /// <returns>Created element</returns>
        public async Task<T> CreateHtmlElementAsync<T>(string tagName)
            where T : HtmlElement
        {
            var handle = await EvaluateFunctionHandleAsync(
                @"(tagName) => {
                    return document.createElement(tagName);
                }",
                tagName).ConfigureAwait(false);

            return handle.ToDomHandle<T>();
        }

        /// <summary>
        /// Creates the HTML element specified
        /// </summary>
        /// <typeparam name="T">HtmlElementType</typeparam>
        /// <param name="tagName">
        /// A string that specifies the type of element to be created.
        /// The nodeName of the created element is initialized with the
        /// value of tagName. Don't use qualified names (like "html:a")
        /// with this method.
        /// </param>
        /// <param name="id">element id</param>
        /// <returns>Created element</returns>
        public async Task<T> CreateHtmlElementAsync<T>(string tagName, string id)
            where T : HtmlElement
        {
            var handle = await EvaluateFunctionHandleAsync(
                @"(tagName, id) => {
                    let e = document.createElement(tagName);
                    e.id = id;
                    return e;
                }",
                tagName,
                id).ConfigureAwait(false);

            return handle.ToDomHandle<T>();
        }

        /// <summary>
        /// The method runs <c>document.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="HtmlElement"/> or derived type</typeparam>
        /// <param name="querySelector">A selector to query page for</param>
        /// <returns>Task which resolves to <see cref="HtmlElement"/> pointing to the frame element</returns>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.QuerySelectorAsync(selector)</c>
        /// </remarks>
        /// <seealso cref="Frame.QuerySelectorAsync(string)"/>
        public Task<T> QuerySelectorAsync<T>(string querySelector)
            where T : Element
        {
            return MainFrame.QuerySelectorAsync<T>(querySelector);
        }

        /// <summary>
        /// Runs <c>document.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type derived from <see cref="Element"/></typeparam>
        /// <param name="querySelector">A selector to query page for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        /// <seealso cref="Frame.QuerySelectorAllAsync(string)"/>
        public Task<T[]> QuerySelectorAllAsync<T>(string querySelector)
            where T : Element
        {
            return MainFrame.QuerySelectorAllAsync<T>(querySelector);
        }

        internal void WireUpEvents()
        {
            FrameManager = new FrameManager(Connection, this, _timeoutSettings);

            Connection.MessageReceived += OnConnectionMessageReceived;
            Connection.Disconnected += OnConnectionDisconnected;
            FrameManager.FrameAttached += (_, e) => FrameAttached?.Invoke(this, e);
            FrameManager.FrameDetached += (_, e) => FrameDetached?.Invoke(this, e);
            FrameManager.FrameNavigated += (_, e) => FrameNavigated?.Invoke(this, e);
            FrameManager.LifecycleEvent += (_, e) => LifecycleEvent?.Invoke(this, e);

            FrameManager.NetworkManager.Request += (_, e) => Request?.Invoke(this, e);
            FrameManager.NetworkManager.RequestFailed += (_, e) => RequestFailed?.Invoke(this, e);
            FrameManager.NetworkManager.Response += (_, e) => Response?.Invoke(this, e);
            FrameManager.NetworkManager.RequestFinished += (_, e) => RequestFinished?.Invoke(this, e);
            FrameManager.NetworkManager.RequestServedFromCache += (_, e) => RequestServedFromCache?.Invoke(this, e);
        }

        private void OnConnectionDisconnected(object sender, EventArgs e)
        {
            // DevTools was disconnected, we'll mark ourselves as Disposed
            // and unhook the event handlers
            IsDisposed = true;

            Connection.MessageReceived -= OnConnectionMessageReceived;
            Connection.Disconnected -= OnConnectionDisconnected;
        }

        /// <summary>
        /// Loads the frame tree. Multiple calls will be ignored.
        /// Generally speaking this method is not expected to be called directly by the user
        /// </summary>
        /// <returns>A Task that when awaited loads the frame tree</returns>
        public async Task InvokeGetFrameTreeAsync()
        {
            if (_frameTreeLoaded)
            {
                return;
            }

            var frameTreeResponse = await Connection.SendAsync<PageGetFrameTreeResponse>("Page.getFrameTree").ConfigureAwait(false);

            await FrameManager.HandleFrameTreeAsync(new FrameTree(frameTreeResponse.FrameTree)).ConfigureAwait(false);

            await FrameManager.EnsureIsolatedWorldAsync(FrameManager.UtilityWorldName).ConfigureAwait(false);

            _frameTreeLoaded = true;
        }

        internal async Task InitializeAsync()
        {
            WireUpEvents();

            await Task.WhenAll(
               Connection.SendAsync("Page.enable"),
               Connection.SendAsync("Page.setLifecycleEventsEnabled", new PageSetLifecycleEventsEnabledRequest { Enabled = true }),
               Connection.SendAsync("Runtime.enable"),
               Connection.SendAsync("Network.enable"),
               Connection.SendAsync("Performance.enable"),
               Connection.SendAsync("Log.enable")).ConfigureAwait(false);

            await InvokeGetFrameTreeAsync().ConfigureAwait(false);
        }

        private async Task<Response> GoAsync(int delta, NavigationOptions options)
        {
            var history = await Connection.SendAsync<PageGetNavigationHistoryResponse>("Page.getNavigationHistory").ConfigureAwait(false);

            if (history.Entries.Count <= history.CurrentIndex + delta || history.CurrentIndex + delta < 0)
            {
                return null;
            }
            var entry = history.Entries[history.CurrentIndex + delta];
            var waitTask = WaitForNavigationAsync(options);

            await Task.WhenAll(
                waitTask,
                Connection.SendAsync(
                    "Page.navigateToHistoryEntry",
                    new PageNavigateToHistoryEntryRequest { EntryId = entry.Id, })).ConfigureAwait(false);

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
            var clip = options.Clip != null ? ProcessClip(options.Clip) : null;

            if (!_screenshotBurstModeOn)
            {
                if (options != null && options.FullPage)
                {
                    var metrics = _screenshotBurstModeOn
                        ? _burstModeMetrics :
                        await Connection.SendAsync<PageGetLayoutMetricsResponse>("Page.getLayoutMetrics").ConfigureAwait(false);

                    if (options.BurstMode)
                    {
                        _burstModeMetrics = metrics;
                    }

                    var contentSize = metrics.ContentSize;

                    var width = Convert.ToInt32(Math.Ceiling(contentSize.Width));
                    var height = Convert.ToInt32(Math.Ceiling(contentSize.Height));

                    // Overwrite clip for full page at all times.
                    clip = new Clip
                    {
                        X = 0,
                        Y = 0,
                        Width = width,
                        Height = height,
                        Scale = 1
                    };

                    var isMobile = Viewport?.IsMobile ?? false;
                    var deviceScaleFactor = Viewport?.DeviceScaleFactor ?? 1;
                    var isLandscape = Viewport?.IsLandscape ?? false;
                    var screenOrientation = isLandscape ?
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

                    await Connection.SendAsync("Emulation.setDeviceMetricsOverride", new EmulationSetDeviceMetricsOverrideRequest
                    {
                        Mobile = isMobile,
                        Width = width,
                        Height = height,
                        DeviceScaleFactor = deviceScaleFactor,
                        ScreenOrientation = screenOrientation
                    }).ConfigureAwait(false);
                }

                if (options?.OmitBackground == true && type == ScreenshotType.Png)
                {
                    await SetTransparentBackgroundColorAsync().ConfigureAwait(false);
                }
            }

            var screenMessage = new PageCaptureScreenshotRequest
            {
                Format = type.ToString().ToLower(CultureInfo.CurrentCulture)
            };

            if (options.Quality.HasValue)
            {
                screenMessage.Quality = options.Quality.Value;
            }

            if (clip != null)
            {
                screenMessage.Clip = clip;
            }

            var result = await Connection.SendAsync<PageCaptureScreenshotResponse>("Page.captureScreenshot", screenMessage).ConfigureAwait(false);

            if (options.BurstMode)
            {
                _screenshotBurstModeOptions = options;
                _screenshotBurstModeOn = true;
            }
            else
            {
                await ResetBackgroundColorAndViewportAsync(options).ConfigureAwait(false);
            }
            return result.Data;
        }

        private Clip ProcessClip(Clip clip)
        {
            var x = Math.Round(clip.X);
            var y = Math.Round(clip.Y);

            return new Clip
            {
                X = x,
                Y = y,
                Width = Math.Round(clip.Width + clip.X - x, MidpointRounding.AwayFromZero),
                Height = Math.Round(clip.Height + clip.Y - y, MidpointRounding.AwayFromZero),
                Scale = clip.Scale
            };
        }

        private Task ResetBackgroundColorAndViewportAsync(ScreenshotOptions options)
        {
            var omitBackgroundTask = options?.OmitBackground == true && options.Type == ScreenshotType.Png ?
                ResetDefaultBackgroundColorAsync() : Task.CompletedTask;
            var setViewPortTask = (options?.FullPage == true && Viewport != null) ?
                SetViewportAsync(Viewport) : Task.CompletedTask;
            return Task.WhenAll(omitBackgroundTask, setViewPortTask);
        }

        private Task ResetDefaultBackgroundColorAsync()
            => Connection.SendAsync("Emulation.setDefaultBackgroundColorOverride");

        private Task SetTransparentBackgroundColorAsync()
            => Connection.SendAsync("Emulation.setDefaultBackgroundColorOverride", new EmulationSetDefaultBackgroundColorOverrideRequest
            {
                Color = new EmulationSetDefaultBackgroundColorOverrideColor
                {
                    R = 0,
                    G = 0,
                    B = 0,
                    A = 0
                }
            });

        private decimal ConvertPrintParameterToInches(object parameter)
        {
            if (parameter == null)
            {
                return 0;
            }

            decimal pixels;
            if (parameter is decimal || parameter is int)
            {
                pixels = Convert.ToDecimal(parameter, CultureInfo.CurrentCulture);
            }
            else
            {
                var text = parameter.ToString();
                var unit = text.Substring(text.Length - 2).ToLower(CultureInfo.CurrentCulture);
                string valueText;
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

        private async void OnConnectionMessageReceived(object sender, MessageEventArgs e)
        {
            try
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
                        await OnConsoleAPIAsync(e.MessageData.ToObject<PageConsoleResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Runtime.exceptionThrown":
                        HandleException(e.MessageData.ToObject<RuntimeExceptionThrownResponse>(true).ExceptionDetails);
                        break;
                    case "Inspector.targetCrashed":
                        OnTargetCrashed();
                        break;
                    case "Performance.metrics":
                        EmitMetrics(e.MessageData.ToObject<PerformanceMetricsResponse>(true));
                        break;
                    case "Log.entryAdded":
                        await OnLogEntryAddedAsync(e.MessageData.ToObject<LogEntryAddedResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Runtime.bindingCalled":
                        await OnBindingCalled(e.MessageData.ToObject<BindingCalledResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Page.fileChooserOpened":
                        await OnFileChooserAsync(e.MessageData.ToObject<PageFileChooserOpenedResponse>(true)).ConfigureAwait(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = $"Page failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                Connection.Close(message);
            }
        }

        private async Task OnFileChooserAsync(PageFileChooserOpenedResponse e)
        {
            if (_fileChooserInterceptors.Count == 0)
            {
                try
                {
                    await Connection.SendAsync("Page.handleFileChooser", new PageHandleFileChooserRequest
                    {
                        Action = FileChooserAction.Fallback
                    }).ConfigureAwait(false);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.ToString());
                }
            }

            var frame = await FrameManager.GetFrameAsync(e.FrameId).ConfigureAwait(false);
            var context = await frame.GetExecutionContextAsync().ConfigureAwait(false);
            var element = await context.AdoptBackendNodeAsync(e.BackendNodeId).ConfigureAwait(false);
            var fileChooser = new FileChooser(element, e);
            while (_fileChooserInterceptors.Count > 0)
            {
                var key = _fileChooserInterceptors.FirstOrDefault().Key;

                if (_fileChooserInterceptors.TryRemove(key, out var tcs))
                {
                    tcs.TrySetResult(fileChooser);
                }
            }
        }

        private async Task OnBindingCalled(BindingCalledResponse e)
        {
            string expression;
            try
            {
                var result = await ExecuteBinding(e).ConfigureAwait(false);

                expression = EvaluationString(
                    @"function deliverResult(name, seq, result) {
                        window[name]['callbacks'].get(seq).resolve(result);
                        window[name]['callbacks'].delete(seq);
                    }",
                    e.BindingPayload.Name,
                    e.BindingPayload.Seq,
                    result);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    ex = ex.InnerException;
                }

                expression = EvaluationString(
                    @"function deliverError(name, seq, message, stack) {
                        const error = new Error(message);
                        error.stack = stack;
                        window[name]['callbacks'].get(seq).reject(error);
                        window[name]['callbacks'].delete(seq);
                    }",
                    e.BindingPayload.Name,
                    e.BindingPayload.Seq,
                    ex.Message,
                    ex.StackTrace);
            }

            Connection.Send("Runtime.evaluate", new
            {
                expression,
                contextId = e.ExecutionContextId
            });
        }

        private async Task<object> ExecuteBinding(BindingCalledResponse e)
        {
            const string taskResultPropertyName = "Result";
            object result;
            var binding = _pageBindings[e.BindingPayload.Name];
            var methodParams = binding.Method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();

            var args = e.BindingPayload.Args.Select((token, i) => token.ToObject(methodParams[i])).ToArray();

            result = binding.DynamicInvoke(args);
            if (result is Task taskResult)
            {
                await taskResult.ConfigureAwait(false);

                if (taskResult.GetType().IsGenericType)
                {
                    // the task is already awaited and therefore the call to property Result will not deadlock
                    result = taskResult.GetType().GetProperty(taskResultPropertyName).GetValue(taskResult);
                }
            }

            return result;
        }

        private async Task OnLogEntryAddedAsync(LogEntryAddedResponse e)
        {
            if (e.Entry.Args != null)
            {
                foreach (var arg in e.Entry?.Args)
                {
                    await RemoteObjectHelper.ReleaseObjectAsync(Connection, arg, _logger).ConfigureAwait(false);
                }
            }
            if (e.Entry.Source != TargetType.Worker)
            {
                Console?.Invoke(this, new ConsoleEventArgs(new ConsoleMessage(
                    e.Entry.Level,
                    e.Entry.Text,
                    null,
                    new ConsoleMessageLocation
                    {
                        URL = e.Entry.URL,
                        LineNumber = e.Entry.LineNumber
                    })));
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

        private void HandleException(EvaluateExceptionResponseDetails exceptionDetails)
            => PageError?.Invoke(this, new PageErrorEventArgs(GetExceptionMessage(exceptionDetails)));

        private string GetExceptionMessage(EvaluateExceptionResponseDetails exceptionDetails)
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

        private Task OnConsoleAPIAsync(PageConsoleResponse message)
        {
            if (message.ExecutionContextId == 0)
            {
                return Task.CompletedTask;
            }
            var ctx = FrameManager.ExecutionContextById(message.ExecutionContextId);

            if (ctx == null)
            {
                return Task.CompletedTask;
            }

            var values = message.Args.Select(ctx.CreateJSHandle).ToArray();

            return AddConsoleMessageAsync(message.Type, values, message.StackTrace);
        }

        private async Task AddConsoleMessageAsync(ConsoleType type, JSHandle[] values, Messaging.StackTrace stackTrace)
        {
            if (Console?.GetInvocationList().Length == 0)
            {
                await Task.WhenAll(values.Select(v => RemoteObjectHelper.ReleaseObjectAsync(Connection, v.RemoteObject, _logger))).ConfigureAwait(false);
                return;
            }

            var tokens = values.Select(i => i.RemoteObject.ObjectId != null
                ? i.ToString()
                : RemoteObjectHelper.ValueFromRemoteObject<string>(i.RemoteObject));

            var location = new ConsoleMessageLocation();
            if (stackTrace?.CallFrames?.Length > 0)
            {
                var callFrame = stackTrace.CallFrames[0];
                location.URL = callFrame.URL;
                location.LineNumber = callFrame.LineNumber;
                location.ColumnNumber = callFrame.ColumnNumber;
            }

            var consoleMessage = new ConsoleMessage(type, string.Join(" ", tokens), values, location);
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
              const binding = window[bindingName];
              window[bindingName] = (...args) => {
                const me = window[bindingName];
                let callbacks = me['callbacks'];
                if (!callbacks) {
                  callbacks = new Map();
                  me['callbacks'] = callbacks;
                }
                const seq = (me['lastSeq'] || 0) + 1;
                me['lastSeq'] = seq;
                const promise = new Promise((resolve, reject) => callbacks.set(seq, {resolve, reject}));
                binding(JSON.stringify({name: bindingName, seq, args}));
                return promise;
              };
            }";
            var expression = EvaluationString(addPageBinding, name);
            await Connection.SendAsync("Runtime.addBinding", new RuntimeAddBindingRequest { Name = name }).ConfigureAwait(false);
            await Connection.SendAsync("Page.addScriptToEvaluateOnNewDocument", new PageAddScriptToEvaluateOnNewDocumentRequest
            {
                Source = expression
            }).ConfigureAwait(false);

            await Task.WhenAll(Frames.Select(
                frame => frame
                    .EvaluateExpressionAsync(expression)
                    .ContinueWith(
                        task =>
                        {
                            if (task.IsFaulted)
                            {
                                _logger.LogError(task.Exception.ToString());
                            }
                        },
                        TaskScheduler.Default)))
                .ConfigureAwait(false);
        }

        private static string EvaluationString(string fun, params object[] args)
        {
            return $"({fun})({string.Join(",", args.Select(SerializeArgument))})";

            string SerializeArgument(object arg)
            {
                return arg == null
                    ? "undefined"
                    : JsonConvert.SerializeObject(arg, JsonHelper.DefaultJsonSerializerSettings);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="DevToolsContext"/> object
        /// </summary>
        /// <remarks>Call <see cref="Dispose()"/> when you are finished using the <see cref="DevToolsContext"/>. The
        /// <see cref="Dispose()"/> method leaves the <see cref="DevToolsContext"/> in an unusable state. After
        /// calling <see cref="Dispose()"/>, you must release all references to the <see cref="DevToolsContext"/> so
        /// the garbage collector can reclaim the memory that the <see cref="DevToolsContext"/> was occupying.</remarks>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            _ = DisposeAsync();
        }

        /// <summary>
        /// Releases all resource used by the <see cref="DevToolsContext"/> object
        /// </summary>
        /// <remarks>Call <see cref="DisposeAsync"/> when you are finished using the <see cref="DevToolsContext"/>. The
        /// <see cref="DisposeAsync"/> method leaves the <see cref="DevToolsContext"/> in an unusable state. After
        /// calling <see cref="DisposeAsync"/>, you must release all references to the <see cref="DevToolsContext"/> so
        /// the garbage collector can reclaim the memory that the <see cref="DevToolsContext"/> was occupying.</remarks>
        /// <returns>ValueTask</returns>
        public async ValueTask DisposeAsync()
        {
            if (IsDisposed)
            {
                return;
            }

            GC.SuppressFinalize(this);

            IsDisposed = true;

            Connection.MessageReceived -= OnConnectionMessageReceived;
            Connection.Disconnected -= OnConnectionDisconnected;

            await Task.WhenAll(
               Connection.SendAsync("Log.disable"),
               Connection.SendAsync("Performance.disable"),
               Connection.SendAsync("Network.disable"),
               Connection.SendAsync("Runtime.disable"),
               Connection.SendAsync("Page.setLifecycleEventsEnabled", new PageSetLifecycleEventsEnabledRequest { Enabled = false }),
               Connection.SendAsync("Page.disable")).ConfigureAwait(false);

            Connection.Dispose();
        }
    }
}
