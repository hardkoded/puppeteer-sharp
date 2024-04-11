using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Input;
using PuppeteerSharp.Media;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.PageAccessibility;
using PuppeteerSharp.PageCoverage;

namespace PuppeteerSharp
{
    /// <summary>
    /// Provides methods to interact with a single tab in Chromium. One <see cref="IBrowser"/> instance might have multiple <see cref="IPage"/> instances.
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
    public interface IPage : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Raised when the page closes.
        /// </summary>
        event EventHandler Close;

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
        event EventHandler<ConsoleEventArgs> Console;

        /// <summary>
        /// Raised when a JavaScript dialog appears, such as <c>alert</c>, <c>prompt</c>, <c>confirm</c> or <c>beforeunload</c>. Puppeteer can respond to the dialog via <see cref="Dialog"/>'s <see cref="PuppeteerSharp.Dialog.Accept(string)"/> or <see cref="PuppeteerSharp.Dialog.Dismiss"/> methods.
        /// </summary>
        event EventHandler<DialogEventArgs> Dialog;

        /// <summary>
        /// Raised when the JavaScript <c>DOMContentLoaded</c> <see href="https://developer.mozilla.org/en-US/docs/Web/Events/DOMContentLoaded"/> event is dispatched.
        /// </summary>
        event EventHandler DOMContentLoaded;

        /// <summary>
        /// Raised when the page crashes
        /// </summary>
#pragma warning disable CA1716 // Identifiers should not match keywords
        event EventHandler<ErrorEventArgs> Error;
#pragma warning restore CA1716 // Identifiers should not match keywords

        /// <summary>
        /// Raised when a frame is attached.
        /// </summary>
        event EventHandler<FrameEventArgs> FrameAttached;

        /// <summary>
        /// Raised when a frame is detached.
        /// </summary>
        event EventHandler<FrameEventArgs> FrameDetached;

        /// <summary>
        /// Raised when a frame is navigated to a new url.
        /// </summary>
        event EventHandler<FrameNavigatedEventArgs> FrameNavigated;

        /// <summary>
        /// Raised when the JavaScript <c>load</c> <see href="https://developer.mozilla.org/en-US/docs/Web/Events/load"/> event is dispatched.
        /// </summary>
        event EventHandler Load;

        /// <summary>
        /// Raised when the JavaScript code makes a call to <c>console.timeStamp</c>. For the list of metrics see <see cref="MetricsAsync"/>.
        /// </summary>
        event EventHandler<MetricEventArgs> Metrics;

        /// <summary>
        /// Raised when an uncaught exception happens within the page.
        /// </summary>
        event EventHandler<PageErrorEventArgs> PageError;

        /// <summary>
        /// Raised when the page opens a new tab or window.
        /// </summary>
        event EventHandler<PopupEventArgs> Popup;

        /// <summary>
        /// Raised when a page issues a request. The <see cref="Request"/> object is read-only.
        /// In order to intercept and mutate requests, see <see cref="SetRequestInterceptionAsync(bool)"/>
        /// </summary>
        event EventHandler<RequestEventArgs> Request;

        /// <summary>
        /// Raised when a request fails, for example by timing out.
        /// </summary>
        event EventHandler<RequestEventArgs> RequestFailed;

        /// <summary>
        /// Raised when a request finishes successfully.
        /// </summary>
        event EventHandler<RequestEventArgs> RequestFinished;

        /// <summary>
        /// Raised when a request ended up loading from cache.
        /// </summary>
        event EventHandler<RequestEventArgs> RequestServedFromCache;

        /// <summary>
        /// Raised when a <see cref="Response"/> is received.
        /// </summary>
        /// <example>
        /// An example of handling <see cref="Response"/> event:
        /// <code>
        /// <![CDATA[
        /// var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        /// page.Response += async(sender, e) =>
        /// {
        ///     if (e.Response.Url.Contains("script.js"))
        ///     {
        ///         tcs.TrySetResult(await e.Response.TextAsync());
        ///     }
        /// };
        ///
        /// await Task.WhenAll(
        ///     page.GoToAsync(TestConstants.ServerUrl + "/grid.html"),
        ///     tcs.Task);
        /// Console.WriteLine(await tcs.Task);
        /// ]]>
        /// </code>
        /// </example>
        event EventHandler<ResponseCreatedEventArgs> Response;

        /// <summary>
        /// Emitted when a dedicated WebWorker (<see href="https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API"/>) is spawned by the page.
        /// </summary>
        event EventHandler<WorkerEventArgs> WorkerCreated;

        /// <summary>
        /// Emitted when a dedicated WebWorker (<see href="https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API"/>) is terminated.
        /// </summary>
        event EventHandler<WorkerEventArgs> WorkerDestroyed;

        /// <summary>
        /// Gets the accessibility.
        /// </summary>
        IAccessibility Accessibility { get; }

        /// <summary>
        /// Get the browser the page belongs to.
        /// </summary>
        IBrowser Browser { get; }

        /// <summary>
        /// Get the browser context that the page belongs to.
        /// </summary>
        IBrowserContext BrowserContext { get; }

        /// <summary>
        /// Chrome DevTools Protocol session.
        /// </summary>
        ICDPSession Client { get; }

        /// <summary>
        /// Gets this page's coverage.
        /// </summary>
        ICoverage Coverage { get; }

        /// <summary>
        /// This setting will change the default maximum time for the following methods:
        /// - <see cref="GoToAsync(string, NavigationOptions)"/>
        /// - <see cref="GoBackAsync(NavigationOptions)"/>
        /// - <see cref="GoForwardAsync(NavigationOptions)"/>
        /// - <see cref="ReloadAsync(NavigationOptions)"/>
        /// - <see cref="SetContentAsync(string, NavigationOptions)"/>
        /// - <see cref="WaitForNavigationAsync(NavigationOptions)"/>
        /// **NOTE** <see cref="DefaultNavigationTimeout"/> takes priority over. <seealso cref="DefaultTimeout"/>
        /// </summary>
        int DefaultNavigationTimeout { get; set; }

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
        /// - <see cref="WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
        /// - <see cref="WaitForExpressionAsync(string, WaitForFunctionOptions)"/>.
        /// </summary>
        int DefaultTimeout { get; set; }

        /// <summary>
        /// Gets all frames attached to the page.
        /// </summary>
        /// <value>An array of all frames attached to the page.</value>
        IFrame[] Frames { get; }

        /// <summary>
        /// Get an indication that the page has been closed.
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// `true` if drag events are being intercepted, `false` otherwise.
        /// </summary>
        [Obsolete("We no longer support intercepting drag payloads. Use the new drag APIs found on ElementHandle to drag (or just use the Page.Mouse)")]
        bool IsDragInterceptionEnabled { get; }

        /// <summary>
        /// Gets this page's keyboard.
        /// </summary>
        IKeyboard Keyboard { get; }

        /// <summary>
        /// Gets page's main frame.
        /// </summary>
        /// <remarks>
        /// Page is guaranteed to have a main frame which persists during navigations.
        /// </remarks>
        IFrame MainFrame { get; }

        /// <summary>
        /// Gets this page's mouse.
        /// </summary>
        IMouse Mouse { get; }

        /// <summary>
        /// Gets that target this page was created from.
        /// </summary>
        [Obsolete("All the functionality provided by the Target should be already exposed by the page itself.")]
        ITarget Target { get; }

        /// <summary>
        /// Gets this page's touchscreen.
        /// </summary>
        ITouchscreen Touchscreen { get; }

        /// <summary>
        /// Gets this page's tracing.
        /// </summary>
        ITracing Tracing { get; }

        /// <summary>
        /// Shortcut for <c>page.MainFrame.Url</c>.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Gets this page's viewport.
        /// </summary>
        ViewPortOptions Viewport { get; }

        /// <summary>
        /// Gets all workers in the page.
        /// </summary>
        WebWorker[] Workers { get; }

        /// <summary>
        /// If the page has JavaScript enabled.
        /// </summary>
        bool IsJavaScriptEnabled { get; }

        /// <summary>
        /// `true` if the service worker are being bypassed.
        /// </summary>
        bool IsServiceWorkerBypassed { get; }

        /// <summary>
        /// Adds a <c><![CDATA[<script>]]></c> tag into the page with the desired url or content.
        /// </summary>
        /// <param name="options">add script tag options.</param>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.AddScriptTagAsync(options)</c>.
        /// </remarks>
        /// <returns>Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame.</returns>
        /// <seealso cref="IFrame.AddScriptTagAsync(AddTagOptions)"/>
        Task<IElementHandle> AddScriptTagAsync(AddTagOptions options);

        /// <summary>
        /// Adds a <c><![CDATA[<script>]]></c> tag into the page with the desired url or content.
        /// </summary>
        /// <param name="url">script url.</param>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.AddScriptTagAsync(new AddTagOptions { Url = url })</c>.
        /// </remarks>
        /// <returns>Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame.</returns>
        Task<IElementHandle> AddScriptTagAsync(string url);

        /// <summary>
        /// Adds a <c><![CDATA[<link rel="stylesheet">]]></c> tag into the page with the desired url or a <c><![CDATA[<link rel="stylesheet">]]></c> tag with the content.
        /// </summary>
        /// <param name="options">add style tag options.</param>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.AddStyleTagAsync(options)</c>.
        /// </remarks>
        /// <returns>Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame.</returns>
        /// <seealso cref="IFrame.AddStyleTagAsync(AddTagOptions)"/>
        Task<IElementHandle> AddStyleTagAsync(AddTagOptions options);

        /// <summary>
        /// Adds a <c><![CDATA[<link rel="stylesheet">]]></c> tag into the page with the desired url or a <c><![CDATA[<link rel="stylesheet">]]></c> tag with the content.
        /// </summary>
        /// <param name="url">stylesheet url.</param>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.AddStyleTagAsync(new AddTagOptions { Url = url })</c>.
        /// </remarks>
        /// <returns>Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame.</returns>
        Task<IElementHandle> AddStyleTagAsync(string url);

        /// <summary>
        /// Provide credentials for http authentication <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication"/>.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>A Task which resolves after the message is sent to the browser.</returns>
        /// <remarks>
        /// To disable authentication, pass <c>null</c>.
        /// </remarks>
        Task AuthenticateAsync(Credentials credentials);

        /// <summary>
        /// Brings page to front (activates tab).
        /// </summary>
        /// <returns>A task that resolves when the message has been sent to Chromium.</returns>
        Task BringToFrontAsync();

        /// <summary>
        /// Fetches an element with <paramref name="selector"/>, scrolls it into view if needed, and then uses <see cref="Mouse"/> to click in the center of the element.
        /// </summary>
        /// <param name="selector">A selector to search for element to click. If there are multiple elements satisfying the selector, the first will be clicked.</param>
        /// <param name="options">click options.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/>.</exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully clicked.</returns>
        Task ClickAsync(string selector, ClickOptions options = null);

        /// <summary>
        /// Closes the page.
        /// </summary>
        /// <param name="options">Close options.</param>
        /// <returns>Task.</returns>
        Task CloseAsync(PageCloseOptions options = null);

        /// <summary>
        /// Deletes cookies from the page.
        /// </summary>
        /// <param name="cookies">Cookies to delete.</param>
        /// <returns>Task.</returns>
        Task DeleteCookieAsync(params CookieParam[] cookies);

        /// <summary>
        /// Emulates given device metrics and user agent.
        /// </summary>
        /// <remarks>
        /// This method is a shortcut for calling two methods:
        /// <see cref="SetViewportAsync(ViewPortOptions)"/>
        /// <see cref="SetUserAgentAsync(string, UserAgentMetadata)"/>
        /// To aid emulation, puppeteer provides a list of device descriptors which can be obtained via the <see cref="Puppeteer.Devices"/>.
        /// <see cref="EmulateAsync(DeviceDescriptor)"/> will resize the page. A lot of websites don't expect phones to change size, so you should emulate before navigating to the page.
        /// </remarks>
        /// <example>
        /// <![CDATA[
        /// var iPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];
        /// using(var page = await browser.NewPageAsync())
        /// {
        ///     await page.EmulateAsync(iPhone);
        ///     await page.goto('https://www.google.com');
        /// }
        /// ]]>
        /// </example>
        /// <returns>Task.</returns>
        /// <param name="options">Emulation options.</param>
        Task EmulateAsync(DeviceDescriptor options);

        /// <summary>
        /// Enables CPU throttling to emulate slow CPUs.
        /// </summary>
        /// <param name="factor">Throttling rate as a slowdown factor (1 is no throttle, 2 is 2x slowdown, etc).</param>
        /// <returns>A task that resolves when the message has been sent to the browser.</returns>
        Task EmulateCPUThrottlingAsync(decimal? factor = null);

        /// <summary>
        /// Emulates the idle state.
        /// If no arguments set, clears idle state emulation.
        /// </summary>
        /// <example>
        /// <code>
        /// // set idle emulation
        /// await page.EmulateIdleStateAsync(new EmulateIdleOverrides() {IsUserActive = true, IsScreenUnlocked = false});
        /// // do some checks here
        /// ...
        /// // clear idle emulation
        /// await page.EmulateIdleStateAsync();
        /// </code>
        /// </example>
        /// <param name="idleOverrides">Overrides.</param>
        /// <returns>A task that resolves when the message has been sent to the browser.</returns>
        Task EmulateIdleStateAsync(EmulateIdleOverrides idleOverrides = null);

        /// <summary>
        /// Given an array of media feature objects, emulates CSS media features on the page.
        /// </summary>
        /// <param name="features">Features to apply.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// await page.EmulateMediaFeaturesAsync(new MediaFeature[]{ new MediaFeature { MediaFeature =  MediaFeature.PrefersColorScheme, Value = "dark" }});
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches)");
        /// // → true
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches)");
        /// // → false
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
        /// // → false
        /// await page.EmulateMediaFeaturesAsync(new MediaFeature[]{ new MediaFeature { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" }});
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches)");
        /// // → true
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
        /// // → false
        /// await page.EmulateMediaFeaturesAsync(new MediaFeature[]
        /// {
        ///   new MediaFeature { MediaFeature = MediaFeature.PrefersColorScheme, Value = "dark" },
        ///   new MediaFeature { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" },
        /// });
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches)");
        /// // → true
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches)");
        /// // → false
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
        /// // → false
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches)");
        /// // → true
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
        /// // → false
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>Emulate features task.</returns>
        Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features);

        /// <summary>
        /// Emulates a media such as screen or print.
        /// </summary>
        /// <param name="type">Media to set.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('screen').matches)");
        /// // → true
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('print').matches)");
        /// // → true
        /// await page.EmulateMediaTypeAsync(MediaType.Print);
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('screen').matches)");
        /// // → false
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('print').matches)");
        /// // → true
        /// await page.EmulateMediaTypeAsync(MediaType.None);
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('screen').matches)");
        /// // → true
        /// await page.EvaluateFunctionAsync<bool>("() => matchMedia('print').matches)");
        /// // → true
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>Emulate media type task.</returns>
        Task EmulateMediaTypeAsync(MediaType type);

        /// <summary>
        /// Emulates network conditions.
        /// </summary>
        /// <param name="networkConditions">Passing <c>null</c> disables network condition emulation.</param>
        /// <returns>Result task.</returns>
        /// <remarks>
        /// **NOTE** This does not affect WebSockets and WebRTC PeerConnections (see https://crbug.com/563644).
        /// </remarks>
        Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions);

        /// <summary>
        /// Changes the timezone of the page.
        /// </summary>
        /// <param name="timezoneId">Timezone to set. See <seealso href="https://cs.chromium.org/chromium/src/third_party/icu/source/data/misc/metaZones.txt?rcl=faee8bc70570192d82d2978a71e2a615788597d1" >ICU’s `metaZones.txt`</seealso>
        /// for a list of supported timezone IDs. Passing `null` disables timezone emulation.</param>
        /// <returns>The viewport task.</returns>
        Task EmulateTimezoneAsync(string timezoneId);

        /// <summary>
        /// Simulates the given vision deficiency on the page.
        /// </summary>
        /// <example>
        /// await Page.EmulateVisionDeficiencyAsync(VisionDeficiency.Achromatopsia);
        /// await Page.ScreenshotAsync("Achromatopsia.png").
        /// </example>
        /// <param name="type">The type of deficiency to simulate, or <see cref="VisionDeficiency.None"/> to reset.</param>
        /// <returns>A task that resolves when the message has been sent to the browser.</returns>
        Task EmulateVisionDeficiencyAsync(VisionDeficiency type);

        /// <summary>
        /// Executes a script in browser context.
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <example>
        /// An example of scraping information from all hyperlinks on the page.
        /// <code>
        /// var hyperlinkInfo = await page.EvaluateExpressionAsync(@"
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
        /// <returns>Task which resolves to script return value.</returns>
        Task<JToken> EvaluateExpressionAsync(string script);

        /// <summary>
        /// Executes a script in browser context.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <seealso cref="EvaluateFunctionAsync{T}(string, object[])"/>
        /// <returns>Task which resolves to script return value.</returns>
        Task<T> EvaluateExpressionAsync<T>(string script);

        /// <summary>
        /// Executes a script in browser context.
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <returns>Task which resolves to script return value.</returns>
        Task<IJSHandle> EvaluateExpressionHandleAsync(string script);

        /// <summary>
        /// Adds a function which would be invoked in one of the following scenarios:
        /// - whenever the page is navigated
        /// - whenever the child frame is attached or navigated. In this case, the function is invoked in the context of the newly attached frame.
        /// </summary>
        /// <param name="expression">Javascript expression to be evaluated in browser context.</param>
        /// <remarks>
        /// The function is invoked after the document was created but before any of its scripts were run. This is useful to amend JavaScript environment, e.g. to seed <c>Math.random</c>.
        /// </remarks>
        /// <example>
        /// An example of overriding the navigator.languages property before the page loads:
        /// <code>
        /// await page.EvaluateExpressionOnNewDocumentAsync("window.__example = true;");
        /// </code>
        /// </example>
        /// <returns>Task.</returns>
        Task<NewDocumentScriptEvaluation> EvaluateExpressionOnNewDocumentAsync(string expression);

        /// <summary>
        /// Executes a function in browser context.
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to script.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IJSHandle"/> instances can be passed as arguments.
        /// </remarks>
        /// <seealso cref="EvaluateExpressionAsync{T}(string)"/>
        /// <returns>Task which resolves to script return value.</returns>
        Task<JToken> EvaluateFunctionAsync(string script, params object[] args);

        /// <summary>
        /// Executes a function in browser context.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to script.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IJSHandle"/> instances can be passed as arguments.
        /// </remarks>
        /// <seealso cref="EvaluateExpressionAsync{T}(string)"/>
        /// <returns>Task which resolves to script return value.</returns>
        Task<T> EvaluateFunctionAsync<T>(string script, params object[] args);

        /// <summary>
        /// Executes a script in browser context.
        /// </summary>
        /// <param name="pageFunction">Script to be evaluated in browser context.</param>
        /// <param name="args">Function arguments.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IJSHandle"/> instances can be passed as arguments.
        /// </remarks>
        /// <returns>Task which resolves to script return value.</returns>
        Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args);

        /// <summary>
        /// Adds a function which would be invoked in one of the following scenarios:
        /// - whenever the page is navigated
        /// - whenever the child frame is attached or navigated. In this case, the function is invoked in the context of the newly attached frame.
        /// </summary>
        /// <param name="pageFunction">Function to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to <c>pageFunction</c>.</param>
        /// <remarks>
        /// The function is invoked after the document was created but before any of its scripts were run. This is useful to amend JavaScript environment, e.g. to seed <c>Math.random</c>.
        /// </remarks>
        /// <example>
        /// An example of overriding the navigator.languages property before the page loads:
        /// <code>
        /// await page.EvaluateFunctionOnNewDocumentAsync("() => window.__example = true");
        /// </code>
        /// </example>
        /// <returns>Task.</returns>
        Task<NewDocumentScriptEvaluation> EvaluateFunctionOnNewDocumentAsync(string pageFunction, params object[] args);

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves when <paramref name="puppeteerFunction"/> completes.
        /// </summary>
        /// <param name="name">Name of the function on the window object.</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync(string, Action)"/> survive navigations.
        /// </remarks>
        /// <returns>Task.</returns>
        Task ExposeFunctionAsync(string name, Action puppeteerFunction);

        /// <summary>
        /// Removes a previously added function via <see cref="IPage.ExposeFunctionAsync(string, Action)"/>.
        /// </summary>
        /// <param name="name">Name of the function to remove.</param>
        /// <returns>Task.</returns>
        Task RemoveExposedFunctionAsync(string name);

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves to the return value of <paramref name="puppeteerFunction"/>.
        /// </summary>
        /// <typeparam name="T">The parameter of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <param name="name">Name of the function on the window object.</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync{T, TResult}(string, Func{T, TResult})"/> survive navigations.
        /// </remarks>
        /// <returns>Task.</returns>
        Task ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> puppeteerFunction);

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves to the return value of <paramref name="puppeteerFunction"/>.
        /// </summary>
        /// <typeparam name="T1">The first parameter of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <typeparam name="T2">The second parameter of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <typeparam name="T3">The third parameter of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <typeparam name="T4">The fourth parameter of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <param name="name">Name of the function on the window object.</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync{T1, T2, T3, T4, TResult}(string, Func{T1, T2, T3, T4, TResult})"/> survive navigations.
        /// </remarks>
        /// <returns>Task.</returns>
        Task ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> puppeteerFunction);

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves to the return value of <paramref name="puppeteerFunction"/>.
        /// </summary>
        /// <typeparam name="T1">The first parameter of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <typeparam name="T2">The second parameter of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <typeparam name="T3">The third parameter of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <param name="name">Name of the function on the window object.</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync{T1, T2, T3, TResult}(string, Func{T1, T2, T3, TResult})"/> survive navigations.
        /// </remarks>
        /// <returns>Task.</returns>
        Task ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> puppeteerFunction);

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves to the return value of <paramref name="puppeteerFunction"/>.
        /// </summary>
        /// <typeparam name="T1">The first parameter of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <typeparam name="T2">The second parameter of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <param name="name">Name of the function on the window object.</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync{T1, T2, TResult}(string, Func{T1, T2, TResult})"/> survive navigations.
        /// </remarks>
        /// <returns>Task.</returns>
        Task ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> puppeteerFunction);

        /// <summary>
        /// Adds a function called <c>name</c> on the page's <c>window</c> object.
        /// When called, the function executes <paramref name="puppeteerFunction"/> in C# and returns a <see cref="Task"/> which resolves to the return value of <paramref name="puppeteerFunction"/>.
        /// </summary>
        /// <typeparam name="TResult">The result of <paramref name="puppeteerFunction"/>.</typeparam>
        /// <param name="name">Name of the function on the window object.</param>
        /// <param name="puppeteerFunction">Callback function which will be called in Puppeteer's context.</param>
        /// <remarks>
        /// If the <paramref name="puppeteerFunction"/> returns a <see cref="Task"/>, it will be awaited.
        /// Functions installed via <see cref="ExposeFunctionAsync{TResult}(string, Func{TResult})"/> survive navigations.
        /// </remarks>
        /// <returns>Task.</returns>
        Task ExposeFunctionAsync<TResult>(string name, Func<TResult> puppeteerFunction);

        /// <summary>
        /// Fetches an element with <paramref name="selector"/> and focuses it.
        /// </summary>
        /// <param name="selector">A selector to search for element to focus. If there are multiple elements satisfying the selector, the first will be focused.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/>.</exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully focused.</returns>
        Task FocusAsync(string selector);

        /// <summary>
        /// Gets the full HTML contents of the page, including the doctype.
        /// </summary>
        /// <returns>Task which resolves to the HTML content.</returns>
        /// <seealso cref="IFrame.GetContentAsync"/>
        Task<string> GetContentAsync();

        /// <summary>
        /// Returns the page's cookies.
        /// </summary>
        /// <param name="urls">Url's to return cookies for.</param>
        /// <returns>Array of cookies.</returns>
        /// <remarks>
        /// If no URLs are specified, this method returns cookies for the current page URL.
        /// If URLs are specified, only cookies for those URLs are returned.
        /// </remarks>
        Task<CookieParam[]> GetCookiesAsync(params string[] urls);

        /// <summary>
        /// Returns page's title.
        /// </summary>
        /// <returns>page's title.</returns>
        /// <see cref="IFrame.GetTitleAsync"/>
        Task<string> GetTitleAsync();

        /// <summary>
        /// Navigate to the previous page in history.
        /// </summary>
        /// <returns>Task that resolves to the main resource response. In case of multiple redirects,
        /// the navigation will resolve with the response of the last redirect. If can not go back, resolves to null.</returns>
        /// <param name="options">Navigation parameters.</param>
        Task<IResponse> GoBackAsync(NavigationOptions options = null);

        /// <summary>
        /// Navigate to the next page in history.
        /// </summary>
        /// <returns>Task that resolves to the main resource response. In case of multiple redirects,
        /// the navigation will resolve with the response of the last redirect. If can not go forward, resolves to null.</returns>
        /// <param name="options">Navigation parameters.</param>
        Task<IResponse> GoForwardAsync(NavigationOptions options = null);

        /// <summary>
        /// Navigates to an url.
        /// </summary>
        /// <param name="url">URL to navigate page to. The url should include scheme, e.g. https://.</param>
        /// <param name="timeout">Maximum navigation time in milliseconds, defaults to 30 seconds, pass <c>0</c> to disable timeout. </param>
        /// <param name="waitUntil">When to consider navigation succeeded, defaults to <see cref="WaitUntilNavigation.Load"/>. Given an array of <see cref="WaitUntilNavigation"/>, navigation is considered to be successful after all events have been fired.</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.</returns>
        /// <seealso cref="GoToAsync(string, NavigationOptions)"/>
        Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null);

        /// <summary>
        /// Navigates to an url.
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
        /// including 404 "Not Found" and 500 "Internal Server Error".  The status code for such responses can be retrieved by calling <see cref="PuppeteerSharp.IResponse.Status"/>
        ///
        /// > **NOTE** <see cref="GoToAsync(string, int?, WaitUntilNavigation[])"/> either throws an error or returns a main resource response.
        /// The only exceptions are navigation to `about:blank` or navigation to the same URL with a different hash, which would succeed and return `null`.
        ///
        /// > **NOTE** Headless mode doesn't support navigation to a PDF document. See the <see fref="https://bugs.chromium.org/p/chromium/issues/detail?id=761295">upstream issue</see>.
        ///
        /// Shortcut for. <seealso cref="IFrame.GoToAsync(string, int?, WaitUntilNavigation[])"/>
        /// </remarks>
        /// <param name="url">URL to navigate page to. The url should include scheme, e.g. https://.</param>
        /// <param name="options">Navigation parameters.</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.</returns>
        /// <seealso cref="GoToAsync(string, int?, WaitUntilNavigation[])"/>
        Task<IResponse> GoToAsync(string url, NavigationOptions options);

        /// <summary>
        /// Navigates to an url.
        /// </summary>
        /// <param name="url">URL to navigate page to. The url should include scheme, e.g. https://.</param>
        /// <param name="waitUntil">When to consider navigation succeeded.</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.</returns>
        /// <seealso cref="GoToAsync(string, NavigationOptions)"/>
        Task<IResponse> GoToAsync(string url, WaitUntilNavigation waitUntil);

        /// <summary>
        /// Fetches an element with <paramref name="selector"/>, scrolls it into view if needed, and then uses <see cref="Mouse"/> to hover over the center of the element.
        /// </summary>
        /// <param name="selector">A selector to search for element to hover. If there are multiple elements satisfying the selector, the first will be hovered.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/>.</exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully hovered.</returns>
        Task HoverAsync(string selector);

        /// <summary>
        /// Returns metrics.
        /// </summary>
        /// <returns>Task which resolves into a list of metrics.</returns>
        /// <remarks>
        /// All timestamps are in monotonic time: monotonically increasing time in seconds since an arbitrary point in the past.
        /// </remarks>
        Task<Dictionary<string, decimal>> MetricsAsync();

        /// <summary>
        /// generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>.
        /// </summary>
        /// <param name="file">The file path to save the PDF to. paths are resolved using <see cref="Path.GetFullPath(string)"/>.</param>
        /// <returns>A Task which resolves after the PDF is generated.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless.
        /// </remarks>
        Task PdfAsync(string file);

        /// <summary>
        ///  generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>.
        /// </summary>
        /// <param name="file">The file path to save the PDF to. paths are resolved using <see cref="Path.GetFullPath(string)"/>.</param>
        /// <param name="options">pdf options.</param>
        /// <returns>A Task which resolves after the PDF is generated.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless.
        /// </remarks>
        Task PdfAsync(string file, PdfOptions options);

        /// <summary>
        /// Generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless.
        /// </remarks>
        Task<byte[]> PdfDataAsync();

        /// <summary>
        /// Generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>.
        /// </summary>
        /// <param name="options">pdf options.</param>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless.
        /// </remarks>
        Task<byte[]> PdfDataAsync(PdfOptions options);

        /// <summary>
        /// generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless.
        /// </remarks>
        Task<Stream> PdfStreamAsync();

        /// <summary>
        /// Generates a pdf of the page with <see cref="MediaType.Print"/> css media. To generate a pdf with <see cref="MediaType.Screen"/> media call <see cref="EmulateMediaTypeAsync(MediaType)"/> with <see cref="MediaType.Screen"/>.
        /// </summary>
        /// <param name="options">pdf options.</param>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the PDF data.</returns>
        /// <remarks>
        /// Generating a pdf is currently only supported in Chrome headless.
        /// </remarks>
        Task<Stream> PdfStreamAsync(PdfOptions options);

        /// <summary>
        /// The method iterates JavaScript heap and finds all the objects with the given prototype.
        /// Shortcut for <c>page.MainFrame.GetExecutionContextAsync().QueryObjectsAsync(prototypeHandle)</c>.
        /// </summary>
        /// <returns>A task which resolves to a handle to an array of objects with this prototype.</returns>
        /// <param name="prototypeHandle">A handle to the object prototype.</param>
        Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle);

        /// <summary>
        /// Runs <c>document.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="selector">A selector to query page for.</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements.</returns>
        /// <seealso cref="IFrame.QuerySelectorAllAsync(string)"/>
        Task<IElementHandle[]> QuerySelectorAllAsync(string selector);

        /// <summary>
        /// A utility function to be used with <see cref="PuppeteerHandleExtensions.EvaluateFunctionAsync{T}(Task{IJSHandle}, string, object[])"/>.
        /// </summary>
        /// <param name="selector">A selector to query page for.</param>
        /// <returns>Task which resolves to a <see cref="IJSHandle"/> of <c>document.querySelectorAll</c> result.</returns>
        Task<IJSHandle> QuerySelectorAllHandleAsync(string selector);

        /// <summary>
        /// The method runs <c>document.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">A selector to query page for.</param>
        /// <returns>Task which resolves to <see cref="IElementHandle"/> pointing to the frame element.</returns>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.QuerySelectorAsync(selector)</c>.
        /// </remarks>
        /// <seealso cref="IFrame.QuerySelectorAsync(string)"/>
        Task<IElementHandle> QuerySelectorAsync(string selector);

        /// <summary>
        /// Reloads the page.
        /// </summary>
        /// <param name="timeout">Maximum navigation time in milliseconds, defaults to 30 seconds, pass <c>0</c> to disable timeout. </param>
        /// <param name="waitUntil">When to consider navigation succeeded, defaults to <see cref="WaitUntilNavigation.Load"/>. Given an array of <see cref="WaitUntilNavigation"/>, navigation is considered to be successful after all events have been fired.</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.</returns>
        /// <seealso cref="ReloadAsync(NavigationOptions)"/>
        Task<IResponse> ReloadAsync(int? timeout = null, WaitUntilNavigation[] waitUntil = null);

        /// <summary>
        /// Reloads the page.
        /// </summary>
        /// <param name="options">Navigation options.</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.</returns>
        /// <seealso cref="ReloadAsync(int?, WaitUntilNavigation[])"/>
        Task<IResponse> ReloadAsync(NavigationOptions options);

        /// <summary>
        /// Captures a screenshot of this <see cref="IPage"/>.
        /// </summary>
        /// <returns>The screenshot task.</returns>
        /// <param name="file">The file path to save the image to. The screenshot type will be inferred from file extension.
        /// If path is a relative path, then it is resolved relative to current working directory. If no path is provided,
        /// the image won't be saved to the disk.</param>
        Task ScreenshotAsync(string file);

        /// <summary>
        /// Takes a screenshot of the page.
        /// </summary>
        /// <returns>The screenshot task.</returns>
        /// <param name="file">The file path to save the image to. The screenshot type will be inferred from file extension.
        /// If path is a relative path, then it is resolved relative to current working directory. If no path is provided,
        /// the image won't be saved to the disk.</param>
        /// <param name="options">Screenshot options.</param>
        Task ScreenshotAsync(string file, ScreenshotOptions options);

        /// <summary>
        /// Takes a screenshot of the page.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="string"/> containing the image data as base64.</returns>
        Task<string> ScreenshotBase64Async();

        /// <summary>
        /// Takes a screenshot of the page.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="string"/> containing the image data as base64.</returns>
        /// <param name="options">Screenshot options.</param>
        Task<string> ScreenshotBase64Async(ScreenshotOptions options);

        /// <summary>
        /// Takes a screenshot of the page.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the image data.</returns>
        Task<byte[]> ScreenshotDataAsync();

        /// <summary>
        /// Takes a screenshot of the page.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the image data.</returns>
        /// <param name="options">Screenshot options.</param>
        Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options);

        /// <summary>
        /// Takes a screenshot of the page.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the image data.</returns>
        Task<Stream> ScreenshotStreamAsync();

        /// <summary>
        /// Takes a screenshot of the page.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the image data.</returns>
        /// <param name="options">Screenshot options.</param>
        Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options);

        /// <summary>
        /// Triggers a change and input event once all the provided options have been selected.
        /// If there's no <![CDATA[<select>]]> element matching selector, the method throws an error.
        /// </summary>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/>.</exception>
        /// <param name="selector">A selector to query page for.</param>
        /// <param name="values">Values of options to select. If the <![CDATA[<select>]]> has the multiple attribute,
        /// all values are considered, otherwise only the first one is taken into account.</param>
        /// <returns>Returns an array of option values that have been successfully selected.</returns>
        /// <seealso cref="IFrame.SelectAsync(string, string[])"/>
        Task<string[]> SelectAsync(string selector, params string[] values);

        /// <summary>
        /// Resets the background color and Viewport after taking Screenshots using BurstMode.
        /// </summary>
        /// <returns>The burst mode off.</returns>
        Task SetBurstModeOffAsync();

        /// <summary>
        /// Toggles bypassing page's Content-Security-Policy.
        /// </summary>
        /// <param name="enabled">sets bypassing of page's Content-Security-Policy.</param>
        /// <returns>A Task which resolves after the message is sent to the browser.</returns>
        /// <remarks>
        /// CSP bypassing happens at the moment of CSP initialization rather then evaluation.
        /// Usually this means that <see cref="SetBypassCSPAsync(bool)"/> should be called before navigating to the domain.
        /// </remarks>
        Task SetBypassCSPAsync(bool enabled);

        /// <summary>
        /// Toggles ignoring cache for each request based on the enabled state. By default, caching is enabled.
        /// </summary>
        /// <param name="enabled">sets the <c>enabled</c> state of the cache.</param>
        /// <returns>Task.</returns>
        Task SetCacheEnabledAsync(bool enabled = true);

        /// <summary>
        /// Sets the HTML markup to the page.
        /// </summary>
        /// <param name="html">HTML markup to assign to the page.</param>
        /// <param name="options">The navigations options.</param>
        /// <returns>Task.</returns>
        /// <seealso cref="IFrame.SetContentAsync(string, NavigationOptions)"/>
        Task SetContentAsync(string html, NavigationOptions options = null);

        /// <summary>
        /// Clears all of the current cookies and then sets the cookies for the page.
        /// </summary>
        /// <param name="cookies">Cookies to set.</param>
        /// <returns>Task.</returns>
        Task SetCookieAsync(params CookieParam[] cookies);

        /// <summary>
        /// Whether to enable drag interception.
        /// </summary>
        /// <remarks>
        /// Activating drag interception enables the `Input.drag`,
        /// methods This provides the capability to capture drag events emitted
        /// on the page, which can then be used to simulate drag-and-drop.
        /// </remarks>
        /// <param name="enabled">Interception enabled.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        [Obsolete("We no longer support intercepting drag payloads. Use the new drag APIs found on ElementHandle to drag (or just use the Page.Mouse)")]
        Task SetDragInterceptionAsync(bool enabled);

        /// <summary>
        /// Sets extra HTTP headers that will be sent with every request the page initiates.
        /// </summary>
        /// <param name="headers">Additional http headers to be sent with every request.</param>
        /// <returns>Task.</returns>
        Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers);

        /// <summary>
        /// Sets the page's geolocation.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="options">Geolocation options.</param>
        /// <remarks>
        /// Consider using <seealso cref="IBrowserContext.OverridePermissionsAsync(string, IEnumerable{OverridePermission})"/> to grant permissions for the page to read its geolocation.
        /// </remarks>
        Task SetGeolocationAsync(GeolocationOption options);

        /// <summary>
        /// Enables/Disables Javascript on the page.
        /// </summary>
        /// <returns>Task.</returns>
        /// <param name="enabled">Whether or not to enable JavaScript on the page.</param>
        Task SetJavaScriptEnabledAsync(bool enabled);

        /// <summary>
        /// Set offline mode for the page.
        /// </summary>
        /// <returns>Result task.</returns>
        /// <param name="value">When <c>true</c> enables offline mode for the page.</param>
        Task SetOfflineModeAsync(bool value);

        /// <summary>
        /// Activating request interception enables <see cref="PuppeteerSharp.IRequest.AbortAsync(RequestAbortErrorCode, int?)">request.AbortAsync</see>,
        /// <see cref="PuppeteerSharp.IRequest.ContinueAsync(Payload, int?)">request.ContinueAsync</see> and <see cref="PuppeteerSharp.IRequest.RespondAsync(ResponseData, int?)">request.RespondAsync</see> methods.
        /// </summary>
        /// <returns>The request interception task.</returns>
        /// <param name="value">Whether to enable request interception..</param>
        Task SetRequestInterceptionAsync(bool value);

        /// <summary>
        /// Sets the user agent to be used in this page.
        /// </summary>
        /// <param name="userAgent">Specific user agent to use in this page.</param>
        /// <param name="userAgentData">Specific user agent client hint data to use in this page.</param>
        /// <returns>Task.</returns>
        Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentData = null);

        /// <summary>
        /// Sets the viewport.
        /// In the case of multiple pages in a single browser, each page can have its own viewport size.
        /// <see cref="SetViewportAsync(ViewPortOptions)"/> will resize the page. A lot of websites don't expect phones to change size, so you should set the viewport before navigating to the page.
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// using(var page = await browser.NewPageAsync())
        /// {
        ///     await page.SetViewPortAsync(new ViewPortOptions
        ///     {
        ///         Width = 640,
        ///         Height = 480,
        ///         DeviceScaleFactor = 1
        ///     });
        ///     await page.goto('https://www.example.com');
        /// }
        /// ]]>
        /// </example>
        /// <returns>The viewport task.</returns>
        /// <param name="viewport">Viewport options.</param>
        Task SetViewportAsync(ViewPortOptions viewport);

        /// <summary>
        /// Fetches an element with <paramref name="selector"/>, scrolls it into view if needed, and then uses <see cref="Touchscreen"/> to tap in the center of the element.
        /// </summary>
        /// <param name="selector">A selector to search for element to tap. If there are multiple elements satisfying the selector, the first will be clicked.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/>.</exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully tapped.</returns>
        Task TapAsync(string selector);

        /// <summary>
        /// Sends a <c>keydown</c>, <c>keypress</c>/<c>input</c>, and <c>keyup</c> event for each character in the text.
        /// </summary>
        /// <param name="selector">A selector of an element to type into. If there are multiple elements satisfying the selector, the first will be used.</param>
        /// <param name="text">A text to type into a focused element.</param>
        /// <param name="options">The options to apply to the type operation.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/>.</exception>
        /// <remarks>
        /// To press a special key, like <c>Control</c> or <c>ArrowDown</c> use <see cref="IKeyboard.PressAsync(string, PressOptions)"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// await page.TypeAsync("#mytextarea", "Hello"); // Types instantly
        /// await page.TypeAsync("#mytextarea", "World", new TypeOptions { Delay = 100 }); // Types slower, like a user
        /// </code>
        /// </example>
        /// <returns>Task.</returns>
        Task TypeAsync(string selector, string text, TypeOptions options = null);

        /// <summary>
        /// Waits for an expression to be evaluated to a truthy value.
        /// </summary>
        /// <param name="script">Expression to be evaluated in browser context.</param>
        /// <param name="options">Optional waiting parameters.</param>
        /// <returns>A task that resolves when the <c>script</c> returns a truthy value.</returns>
        /// <seealso cref="IFrame.WaitForExpressionAsync(string, WaitForFunctionOptions)"/>
        Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options = null);

        /// <summary>
        /// Waits for a frame.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var frame = await page.WaitForFrameAsync(frame => frame.Name == "Test");
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>A task which resolves when a matching frame was attached to the page.</returns>
        /// <param name="url">Frame url.</param>
        /// <param name="options">Options.</param>
        public Task<IFrame> WaitForFrameAsync(string url, WaitForOptions options = null);

        /// <summary>
        /// Waits for a frame.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var frame = await page.WaitForFrameAsync(frame => frame.Name == "Test");
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>A task which resolves when a matching frame was attached to the page.</returns>
        /// <param name="predicate">Function which looks for a matching frame.</param>
        /// <param name="options">Options.</param>
        public Task<IFrame> WaitForFrameAsync(Func<IFrame, bool> predicate, WaitForOptions options = null);

        /// <summary>
        /// Waits for a page to open a file picker.
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
        /// var waitTask = page.WaitForFileChooserAsync();
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
        Task<FileChooser> WaitForFileChooserAsync(WaitForOptions options = null);

        /// <summary>
        /// Waits for a function to be evaluated to a truthy value.
        /// </summary>
        /// <param name="script">Function to be evaluated in browser context.</param>
        /// <param name="options">Optional waiting parameters.</param>
        /// <param name="args">Arguments to pass to <c>script</c>.</param>
        /// <returns>A task that resolves when the <c>script</c> returns a truthy value.</returns>
        /// <seealso cref="IFrame.WaitForFunctionAsync(string, WaitForFunctionOptions, object[])"/>
        Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args);

        /// <summary>
        /// Waits for a function to be evaluated to a truthy value.
        /// </summary>
        /// <param name="script">Function to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to <c>script</c>.</param>
        /// <returns>A task that resolves when the <c>script</c> returns a truthy value.</returns>
        Task<IJSHandle> WaitForFunctionAsync(string script, params object[] args);

        /// <summary>
        /// This resolves when the page navigates to a new URL or reloads.
        /// It is useful for when you run code which will indirectly cause the page to navigate.
        /// </summary>
        /// <param name="options">navigation options.</param>
        /// <returns>Task which resolves to the main resource response.
        /// In case of multiple redirects, the navigation will resolve with the response of the last redirect.
        /// In case of navigation to a different anchor or navigation due to History API usage, the navigation will resolve with `null`.
        /// </returns>
        /// <remarks>
        /// Usage of the <c>History API</c> <see href="https://developer.mozilla.org/en-US/docs/Web/API/History_API"/> to change the URL is considered a navigation.
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
        Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null);

        /// <summary>
        /// Waits for Network Idle.
        /// </summary>
        /// <param name="options">Optional waiting parameters.</param>
        /// <returns>returns Task which resolves when network is idle.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// page.EvaluateFunctionAsync("() => fetch('some-url')");
        /// await page.WaitForNetworkIdle(); // The Task resolves after fetch above finishes
        /// ]]>
        /// </code>
        /// </example>
        Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions options = null);

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
        Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions options = null);

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
        Task<IRequest> WaitForRequestAsync(string url, WaitForOptions options = null);

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
        Task<IResponse> WaitForResponseAsync(Func<IResponse, bool> predicate, WaitForOptions options = null);

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
        Task<IResponse> WaitForResponseAsync(Func<IResponse, Task<bool>> predicate, WaitForOptions options = null);

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
        Task<IResponse> WaitForResponseAsync(string url, WaitForOptions options = null);

        /// <summary>
        /// Waits for a selector to be added to the DOM.
        /// </summary>
        /// <param name="selector">A selector of an element to wait for.</param>
        /// <param name="options">Optional waiting parameters.</param>
        /// <returns>A task that resolves when element specified by selector string is added to DOM.
        /// Resolves to `null` if waiting for `hidden: true` and selector is not found in DOM.</returns>
        /// <seealso cref="IFrame.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
        Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null);

        /// <summary>
        /// Waits for a xpath selector to be added to the DOM.
        /// </summary>
        /// <param name="xpath">A xpath selector of an element to wait for.</param>
        /// <param name="options">Optional waiting parameters.</param>
        /// <returns>A task which resolves when element specified by xpath string is added to DOM.
        /// Resolves to `null` if waiting for `hidden: true` and xpath is not found in DOM.</returns>
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
        [Obsolete("Use " + nameof(WaitForSelectorAsync) + " instead")]
        Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null);

        /// <summary>
        /// Evaluates the XPath expression.
        /// </summary>
        /// <param name="expression">Expression to evaluate <see href="https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate"/>.</param>
        /// <returns>Task which resolves to an array of <see cref="IElementHandle"/>.</returns>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.XPathAsync(expression)</c>.
        /// </remarks>
        [Obsolete("Use " + nameof(QuerySelectorAsync) + " instead")]
        Task<IElementHandle[]> XPathAsync(string expression);

        /// <summary>
        /// This method is typically coupled with an action that triggers a device
        /// request from an api such as WebBluetooth.
        ///
        /// Caution.
        ///
        /// This must be called before the device request is made. It will not return a
        /// currently active device prompt.
        /// </summary>
        /// <example>
        /// <code source="../PuppeteerSharp.Tests/DeviceRequestPromptTests/WaitForDevicePromptTests.cs" region="IPageWaitForDevicePromptAsyncUsage" lang="csharp"/>
        /// </example>
        /// <param name="options">Optional waiting parameters.</param>
        /// <returns>A task that resolves after the page gets the prompt.</returns>
        Task<DeviceRequestPrompt> WaitForDevicePromptAsync(WaitForOptions options = null);

        /// <summary>
        /// <see cref="IRequest.RespondAsync"/>, <see cref="IRequest.AbortAsync"/>, and <see cref="IRequest.ContinueAsync"/> can accept an optional `priority` to activate Cooperative Intercept Mode.
        /// In Cooperative Mode, all interception tasks are guaranteed to run and all async handlers are awaited.
        /// The interception is resolved to the highest-priority resolution.
        /// </summary>
        /// <param name="interceptionTask">Interception task.</param>
        void AddRequestInterceptor(Func<IRequest, Task> interceptionTask);

        /// <summary>
        /// Removes a previously added request interceptor.
        /// </summary>
        /// <param name="interceptionTask">Interception task.</param>
        void RemoveRequestInterceptor(Func<IRequest, Task> interceptionTask);

        /// <summary>
        /// Creates a Chrome Devtools Protocol session attached to the page.
        /// </summary>
        /// <returns>A task that returns a <see cref="ICDPSession"/>.</returns>
        Task<ICDPSession> CreateCDPSessionAsync();

        /// <summary>
        /// Removes script that injected into page by <see cref="IPage.EvaluateExpressionOnNewDocumentAsync(string)"/> or <see cref="IPage.EvaluateExpressionOnNewDocumentAsync(string)"/>.
        /// </summary>
        /// <param name="identifier">Function identifier.</param>
        /// <returns>A task that resolves when the script is removed.</returns>
        Task RemoveScriptToEvaluateOnNewDocumentAsync(string identifier);

        /// <summary>
        /// Toggles ignoring of service worker for each request.
        /// </summary>
        /// <param name="bypass">When <c>true</c> bypasses service worker.</param>
        /// <returns>A task that resolves when the message is sent to the browser.</returns>
        Task SetBypassServiceWorkerAsync(bool bypass);
    }
}
