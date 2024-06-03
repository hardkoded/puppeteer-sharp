using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Input;

namespace PuppeteerSharp
{
    /// <summary>
    /// Provides methods to interact with a single page frame in Chromium. One <see cref="IPage"/> instance might have multiple <see cref="IFrame"/> instances.
    /// At every point of time, page exposes its current frame tree via the <see cref="IPage.MainFrame"/> and <see cref="ChildFrames"/> properties.
    ///
    /// <see cref="IFrame"/> object's lifecycle is controlled by three events, dispatched on the page object
    /// - <see cref="IPage.FrameAttached"/> - fires when the frame gets attached to the page. A Frame can be attached to the page only once
    /// - <see cref="IPage.FrameNavigated"/> - fired when the frame commits navigation to a different URL
    /// - <see cref="IPage.FrameDetached"/> - fired when the frame gets detached from the page.  A Frame can be detached from the page only once.
    /// </summary>
    /// <example>
    /// An example of dumping frame tree.
    /// <code>
    /// <![CDATA[
    /// var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
    /// var page = await browser.NewPageAsync();
    /// await page.GoToAsync("https://www.google.com/chrome/browser/canary.html");
    /// dumpFrameTree(page.MainFrame, string.Empty);
    /// await browser.CloseAsync();
    ///
    /// void dumpFrameTree(IFrame frame, string indent)
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
    public interface IFrame
    {
        /// <summary>
        /// Raised when the frame is swapped.
        /// </summary>
        event EventHandler FrameSwappedByActivation;

        /// <summary>
        /// Gets the child frames of the this frame.
        /// </summary>
        IReadOnlyCollection<IFrame> ChildFrames { get; }

        /// <summary>
        /// Gets a value indicating if the frame is detached or not.
        /// </summary>
        bool Detached { get; }

        /// <summary>
        /// Frame Id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the frame's name attribute as specified in the tag
        /// If the name is empty, returns the id attribute instead.
        /// </summary>
        [Obsolete("Use (await frame.FrameElementAsync()).EvaluateFunctionAsync<string>(\"frame => frame.name\") instead.")]
        string Name { get; }

        /// <summary>
        ///  The <see cref="IPage"/> associated with the frame.
        /// </summary>
        IPage Page { get; }

        /// <summary>
        /// Gets the parent frame, if any. Detached frames and main frames return <c>null</c>.
        /// </summary>
        IFrame ParentFrame { get; }

        /// <summary>
        /// `true` if the frame is an OOP frame, or `false` otherwise.
        /// </summary>
        bool IsOopFrame { get; }

        /// <summary>
        /// Gets the frame's url.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Adds a <c><![CDATA[<script>]]></c> tag into the page with the desired url or content.
        /// </summary>
        /// <param name="options">add script tag options.</param>
        /// <returns>Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame.</returns>
        /// <seealso cref="IPage.AddScriptTagAsync(AddTagOptions)"/>
        /// <seealso cref="IPage.AddScriptTagAsync(string)"/>
        Task<IElementHandle> AddScriptTagAsync(AddTagOptions options);

        /// <summary>
        /// Adds a <c><![CDATA[<link rel="stylesheet">]]></c> tag into the page with the desired url or a <c><![CDATA[<link rel="stylesheet">]]></c> tag with the content.
        /// </summary>
        /// <param name="options">add style tag options.</param>
        /// <returns>Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame.</returns>
        /// <seealso cref="IPage.AddStyleTagAsync(AddTagOptions)"/>
        /// <seealso cref="IPage.AddStyleTagAsync(string)"/>
        Task<IElementHandle> AddStyleTagAsync(AddTagOptions options);

        /// <summary>
        /// Fetches an element with <paramref name="selector"/>, scrolls it into view if needed, and then uses <see cref="IPage.Mouse"/> to click in the center of the element.
        /// </summary>
        /// <param name="selector">A selector to search for element to click. If there are multiple elements satisfying the selector, the first will be clicked.</param>
        /// <param name="options">click options.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/>.</exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully clicked.</returns>
        Task ClickAsync(string selector, ClickOptions options = null);

        /// <summary>
        /// Executes a script in browser context.
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <returns>Task which resolves to script return value.</returns>
        /// <seealso cref="EvaluateFunctionAsync{T}(string, object[])"/>
        /// <seealso cref="IPage.EvaluateExpressionAsync{T}(string)"/>
        Task<JToken> EvaluateExpressionAsync(string script);

        /// <summary>
        /// Executes a script in browser context.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <returns>Task which resolves to script return value.</returns>
        /// <seealso cref="EvaluateFunctionAsync{T}(string, object[])"/>
        /// <seealso cref="IPage.EvaluateExpressionAsync{T}(string)"/>
        Task<T> EvaluateExpressionAsync<T>(string script);

        /// <summary>
        /// Passes an expression to the <see cref="ExecutionContext.EvaluateExpressionHandleAsync(string)"/>, returns a <see cref="Task"/>, then <see cref="ExecutionContext.EvaluateExpressionHandleAsync(string)"/> would wait for the <see cref="Task"/> to resolve and return its value.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var handle = await Page.MainFrame.EvaluateExpressionHandleAsync("1 + 2");
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>Resolves to the return value of <paramref name="script"/>.</returns>
        /// <param name="script">Expression to be evaluated in the. <seealso cref="ExecutionContext"/></param>
        Task<IJSHandle> EvaluateExpressionHandleAsync(string script);

        /// <summary>
        /// Executes a function in browser context.
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context.</param>
        /// <param name="args">Arguments to pass to script.</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IJSHandle"/> instances can be passed as arguments.
        /// </remarks>
        /// <returns>Task which resolves to script return value.</returns>
        /// <seealso cref="EvaluateExpressionAsync{T}(string)"/>
        /// <seealso cref="IPage.EvaluateFunctionAsync{T}(string, object[])"/>
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
        /// <returns>Task which resolves to script return value.</returns>
        /// <seealso cref="EvaluateExpressionAsync{T}(string)"/>
        /// <seealso cref="IPage.EvaluateFunctionAsync{T}(string, object[])"/>
        Task<T> EvaluateFunctionAsync<T>(string script, params object[] args);

        /// <summary>
        /// Passes a function to the <see cref="IExecutionContext.EvaluateFunctionAsync(string, object[])"/>, returns a <see cref="Task"/>, then <see cref="ExecutionContext.EvaluateFunctionHandleAsync(string, object[])"/> would wait for the <see cref="Task"/> to resolve and return its value.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var handle = await Page.MainFrame.EvaluateFunctionHandleAsync("() => Promise.resolve(self)");
        /// return handle; // Handle for the global object.
        /// ]]>
        /// </code>
        /// <see cref="IJSHandle"/> instances can be passed as arguments to the <see cref="IExecutionContext.EvaluateFunctionAsync(string, object[])"/>:
        /// <code>
        /// <![CDATA[
        /// var handle = await Page.MainFrame.EvaluateExpressionHandleAsync("document.body");
        /// var resultHandle = await Page.MainFrame.EvaluateFunctionHandleAsync("body => body.innerHTML", handle);
        /// return await resultHandle.JsonValueAsync(); // prints body's innerHTML
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>Resolves to the return value of <paramref name="func"/>.</returns>
        /// <param name="func">Function to be evaluated in the <see cref="ExecutionContext"/>.</param>
        /// <param name="args">Arguments to pass to <paramref name="func"/>.</param>
        Task<IJSHandle> EvaluateFunctionHandleAsync(string func, params object[] args);

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
        /// <seealso cref="IPage.GetContentAsync"/>
        Task<string> GetContentAsync();

        /// <summary>
        /// Returns page's title.
        /// </summary>
        /// <returns>page's title.</returns>
        /// <seealso cref="IPage.GetTitleAsync"/>
        Task<string> GetTitleAsync();

        /// <summary>
        /// Navigates to an url.
        /// </summary>
        /// <param name="url">URL to navigate page to. The url should include scheme, e.g. https://.</param>
        /// <param name="timeout">maximum navigation time in milliseconds. Defaults to 30 seconds. Pass 0
        /// to disable timeout. The default value can be changed by using the <see cref="IPage.DefaultNavigationTimeout"/>
        /// property.</param>
        /// <param name="waitUntil">When to consider navigation succeeded, defaults to <see cref="WaitUntilNavigation.Load"/>. Given an array of <see cref="WaitUntilNavigation"/>, navigation is considered to be successful after all events have been fired.</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.</returns>
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
        /// including 404 "Not Found" and 500 "Internal Server Error".  The status code for such responses can be retrieved by calling <see cref="IResponse.Status"/>
        ///
        /// > **NOTE** <see cref="GoToAsync(string, int?, WaitUntilNavigation[])"/> either throws an error or returns a main resource response.
        /// The only exceptions are navigation to `about:blank` or navigation to the same URL with a different hash, which would succeed and return `null`.
        ///
        /// > **NOTE** Headless mode doesn't support navigation to a PDF document. See the <see fref="https://bugs.chromium.org/p/chromium/issues/detail?id=761295">upstream issue</see>.
        /// </remarks>
        /// <param name="url">URL to navigate page to. The url should include scheme, e.g. https://.</param>
        /// <param name="options">Navigation parameters.</param>
        /// <returns>Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.</returns>
        /// <seealso cref="GoToAsync(string, int?, WaitUntilNavigation[])"/>
        Task<IResponse> GoToAsync(string url, NavigationOptions options);

        /// <summary>
        /// Fetches an element with <paramref name="selector"/>, scrolls it into view if needed, and then uses <see cref="IPage.Mouse"/> to hover over the center of the element.
        /// </summary>
        /// <param name="selector">A selector to search for element to hover. If there are multiple elements satisfying the selector, the first will be hovered.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/>.</exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully hovered.</returns>
        Task HoverAsync(string selector);

        /// <summary>
        /// Queries frame for the selector. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="selector">A selector to query frame for.</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements.</returns>
        /// <seealso cref="IPage.QuerySelectorAllAsync(string)"/>
        Task<IElementHandle[]> QuerySelectorAllAsync(string selector);

        /// <summary>
        /// A utility function to be used with <see cref="PuppeteerHandleExtensions.EvaluateFunctionAsync{T}(Task{IJSHandle}, string, object[])"/>.
        /// </summary>
        /// <param name="selector">A selector to query page for.</param>
        /// <returns>Task which resolves to a <see cref="IJSHandle"/> of <c>document.querySelectorAll</c> result.</returns>
        Task<IJSHandle> QuerySelectorAllHandleAsync(string selector);

        /// <summary>
        /// Queries frame for the selector. If there's no such element within the frame, the method will resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">Selector to query frame for.</param>
        /// <returns>Task which resolves to <see cref="IElementHandle"/> pointing to the frame element.</returns>
        /// <seealso cref="IPage.QuerySelectorAsync(string)"/>
        Task<IElementHandle> QuerySelectorAsync(string selector);

        /// <summary>
        /// Triggers a change and input event once all the provided options have been selected.
        /// If there's no <![CDATA[<select>]]> element matching selector, the method throws an error.
        /// </summary>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/>.</exception>
        /// <param name="selector">A selector to query page for.</param>
        /// <param name="values">Values of options to select. If the <![CDATA[<select>]]> has the multiple attribute,
        /// all values are considered, otherwise only the first one is taken into account.</param>
        /// <returns>Returns an array of option values that have been successfully selected.</returns>
        /// <seealso cref="IPage.SelectAsync(string, string[])"/>
        Task<string[]> SelectAsync(string selector, params string[] values);

        /// <summary>
        /// Sets the HTML markup to the page.
        /// </summary>
        /// <param name="html">HTML markup to assign to the page.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task.</returns>
        /// <seealso cref="IPage.SetContentAsync(string, NavigationOptions)"/>
        Task SetContentAsync(string html, NavigationOptions options = null);

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
        /// <![CDATA[
        /// await frame.TypeAsync("#mytextarea", "Hello"); // Types instantly
        /// await frame.TypeAsync("#mytextarea", "World", new TypeOptions { Delay = 100 }); // Types slower, like a user
        /// ]]>
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
        /// <seealso cref="IPage.WaitForExpressionAsync(string, WaitForFunctionOptions)"/>
        /// <exception cref="WaitTaskTimeoutException">If timeout occurred.</exception>
        Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options);

        /// <summary>
        /// Waits for a function to be evaluated to a truthy value.
        /// </summary>
        /// <param name="script">Function to be evaluated in browser context.</param>
        /// <param name="options">Optional waiting parameters.</param>
        /// <param name="args">Arguments to pass to <c>script</c>.</param>
        /// <returns>A task that resolves when the <c>script</c> returns a truthy value.</returns>
        /// <seealso cref="IPage.WaitForFunctionAsync(string, WaitForFunctionOptions, object[])"/>
        /// <exception cref="WaitTaskTimeoutException">If timeout occurred.</exception>
        Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args);

        /// <summary>
        /// This resolves when the frame navigates to a new URL or reloads.
        /// It is useful for when you run code which will indirectly cause the frame to navigate.
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
        /// var navigationTask = Page.WaitForNavigationAsync();
        /// await Page.MainFrame.ClickAsync("a.my-link");
        /// await navigationTask;
        /// ]]>
        /// </code>
        /// </example>
        Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null);

        /// <summary>
        /// Waits for a selector to be added to the DOM.
        /// </summary>
        /// <param name="selector">A selector of an element to wait for.</param>
        /// <param name="options">Optional waiting parameters.</param>
        /// <returns>A task that resolves when element specified by selector string is added to DOM.
        /// Resolves to `null` if waiting for `hidden: true` and selector is not found in DOM.</returns>
        /// <seealso cref="IPage.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
        /// <exception cref="WaitTaskTimeoutException">If timeout occurred.</exception>
        Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null);

        /// <summary>
        /// Waits for a selector to be added to the DOM.
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
        /// <exception cref="WaitTaskTimeoutException">If timeout occurred.</exception>
        [Obsolete("Use " + nameof(WaitForSelectorAsync) + " instead")]
        Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null);

        /// <summary>
        /// Evaluates the XPath expression.
        /// </summary>
        /// <param name="expression">Expression to evaluate <see href="https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate"/>.</param>
        /// <returns>Task which resolves to an array of <see cref="IElementHandle"/>.</returns>
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
        /// <code source="../PuppeteerSharp.Tests/DeviceRequestPromptTests/WaitForDevicePromptTests.cs" region="IFrameWaitForDevicePromptAsyncUsage" lang="csharp"/>
        /// </example>
        /// <param name="options">Optional waiting parameters.</param>
        /// <returns>A task that resolves after the page gets the prompt.</returns>
        Task<DeviceRequestPrompt> WaitForDevicePromptAsync(WaitForOptions options = null);

        /// <summary>
        /// Fetches an element with <paramref name="selector"/>, scrolls it into view if needed, and then uses <see cref="Touchscreen"/> to tap in the center of the element.
        /// </summary>
        /// <param name="selector">A selector to search for element to tap. If there are multiple elements satisfying the selector, the first will be clicked.</param>
        /// <exception cref="SelectorException">If there's no element matching <paramref name="selector"/>.</exception>
        /// <returns>Task which resolves when the element matching <paramref name="selector"/> is successfully tapped.</returns>
        Task TapAsync(string selector);

        /// <summary>
        /// The frame element associated with this frame (if any).
        /// </summary>
        /// <returns>Task which resolves to the frame element.</returns>
        Task<ElementHandle> FrameElementAsync();
    }
}
