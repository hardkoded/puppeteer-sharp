// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

#if !CDP_ONLY

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using WebDriverBiDi.Script;
using BidiLocator = WebDriverBiDi.BrowsingContext.Locator;
using Request = PuppeteerSharp.Bidi.Core.Request;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiFrame : Frame
{
    private readonly ConcurrentDictionary<BrowsingContext, BidiFrame> _frames = new();
    private readonly ConcurrentDictionary<string, ExposableFunction> _exposedFunctions = new();
    private readonly Realms _realms;

    private BidiFrame(BidiPage parentPage, BidiFrame parentFrame, BrowsingContext browsingContext)
    {
        Client = new BidiCdpSession(this, parentPage?.BidiBrowser?.LoggerFactory ?? parentFrame?.BidiPage?.BidiBrowser?.LoggerFactory);
        ParentPage = parentPage;
        ParentFrame = parentFrame;
        BrowsingContext = browsingContext;
        Id = browsingContext.Id;
        _realms = new Realms(
#pragma warning disable CA2000
            BidiFrameRealm.From(browsingContext.DefaultRealm, this),
            BidiFrameRealm.From(browsingContext.CreateWindowRealm($"__puppeteer_internal_{new Random().Next(0, 10000)}"), this));
#pragma warning restore CA2000
    }

    /// <inheritdoc />
    public override IReadOnlyCollection<IFrame> ChildFrames =>
        BrowsingContext.Children
            .Select(child => _frames.TryGetValue(child, out var frame) ? frame : null)
            .Where(frame => frame is not null)
            .Cast<IFrame>()
            .ToList();

    /// <inheritdoc/>
    public override string Url => BrowsingContext.Url;

    /// <inheritdoc />
    public override bool Detached => BrowsingContext.IsClosed;

    /// <inheritdoc />
    public override IPage Page => BidiPage;

    /// <inheritdoc />
    public override ICDPSession Client { get; protected set; }

    /// <inheritdoc />
    internal override Realm MainRealm => _realms.Default;

    internal override Realm IsolatedRealm => _realms.Internal;

    internal BidiPage BidiPage
    {
        get
        {
            var frame = this;
            while (frame?.ParentPage == null)
            {
                frame = frame.ParentFrame as BidiFrame;
            }

            return frame.ParentPage;
        }
    }

    internal BrowsingContext BrowsingContext { get; }

    internal BidiPage ParentPage { get; }

    internal override Frame ParentFrame { get; }

    internal TimeoutSettings TimeoutSettings => BidiPage.TimeoutSettings;

    /// <inheritdoc />
    public override async Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (string.IsNullOrEmpty(options.Url) && string.IsNullOrEmpty(options.Path) &&
            string.IsNullOrEmpty(options.Content))
        {
            throw new ArgumentException("Provide options with a `Url`, `Path` or `Content` property");
        }

        var content = options.Content;

        if (!string.IsNullOrEmpty(options.Path))
        {
            content = await AsyncFileHelper.ReadAllText(options.Path).ConfigureAwait(false);
            content += "//# sourceURL=" + options.Path.Replace("\n", string.Empty);
        }

        var handle = await IsolatedRealm.EvaluateFunctionHandleAsync(
            @"async (puppeteerUtil, url, id, type, content) => {
                  const createDeferredPromise = puppeteerUtil.createDeferredPromise;
                  const promise = createDeferredPromise();
                  let element;
                  if (!url) {
                    element = document.createElement('style');
                    element.appendChild(document.createTextNode(content));
                  } else {
                    const link = document.createElement('link');
                    link.rel = 'stylesheet';
                    link.href = url;
                    element = link;
                  }
                  element.addEventListener(
                    'load',
                    () => {
                      promise.resolve();
                    },
                    {once: true}
                  );
                  element.addEventListener(
                    'error',
                    event => {
                      promise.reject(
                        new Error(
                          event.message ?? 'Could not load style'
                        )
                      );
                    },
                    {once: true}
                  );
                  document.head.appendChild(element);
                  await promise;
                  return element;
                }",
            new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
            options.Url,
            options.Id,
            options.Type,
            content).ConfigureAwait(false);

        return (await MainRealm.TransferHandleAsync(handle).ConfigureAwait(false)) as IElementHandle;
    }

    /// <inheritdoc />
    public override async Task SetContentAsync(string html, NavigationOptions options = null)
    {
        var timeout = options?.Timeout ?? TimeoutSettings.NavigationTimeout;

        // Setup load event listeners before setting content to avoid race conditions
        var (waitForLoadTask, cleanupLoadListeners) = SetupLoadEventListeners(options);
        var waitForNetworkIdleTask = WaitForNetworkIdleAsync(options);

        try
        {
            // Set the frame content using JavaScript, similar to CDP implementation
            await EvaluateFunctionAsync(
                @"html => {
                    document.open();
                    document.write(html);
                    document.close();
                }",
                html).ConfigureAwait(false);

            await Task.WhenAll(waitForLoadTask, waitForNetworkIdleTask).WithTimeout(
                timeout,
                t => new TimeoutException($"Navigation timeout of {t.TotalMilliseconds} ms exceeded")).ConfigureAwait(false);
        }
        finally
        {
            cleanupLoadListeners();
        }
    }

    /// <inheritdoc />
    public override async Task<IResponse> GoToAsync(string url, NavigationOptions options)
    {
        // Clear the events trackers on navigation start to allow fresh events
        BidiPage.ClearEventTrackers();

        // Check if this is a same-document (fragment) navigation.
        // Firefox BiDi doesn't reliably fire navigation/historyUpdated events for
        // fragment navigations in OOPIFs, so we handle them specially.
        // See: https://bugzilla.mozilla.org/show_bug.cgi?id=1862018
        if (IsSameDocumentNavigation(Url, url))
        {
            try
            {
                await NavigateAsync(url).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw RewriteNavigationError(ex, url, options?.Timeout ?? TimeoutSettings.NavigationTimeout);
            }

            // For same-document navigations, there's no response
            BidiPage.EndNavigationAndClearTracker();
            return null;
        }

        var waitForNavigationTask = WaitForNavigationAsync(options);
        var navigationTask = NavigateAsync(url);

        try
        {
            // - If any task fails/is canceled, immediately throw (don't wait for other tasks)
            // - If all tasks succeed, return when all have completed
            await new[] { waitForNavigationTask, navigationTask }.WhenAllFailFast().ConfigureAwait(false);
        }
        catch (NavigationException)
        {
            throw;
        }
        catch (PuppeteerException ex) when (ex.Message == "Navigating frame was detached")
        {
            // Convert frame detachment to NavigationException for GoToAsync
            throw new NavigationException(ex.Message, url, ex);
        }
        catch (Exception ex)
        {
            throw RewriteNavigationError(ex, url, options?.Timeout ?? TimeoutSettings.NavigationTimeout);
        }

        // End navigation mode and clear the request events tracker after navigation completes.
        // This ensures duplicate sub-resource requests during navigation are filtered,
        // while subsequent fetch/XHR calls are not deduplicated.
        BidiPage.EndNavigationAndClearTracker();

        return waitForNavigationTask.Result;
    }

    /// <inheritdoc />
    public override async Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
    {
        var timeout = options?.Timeout ?? TimeoutSettings.NavigationTimeout;

        // Create a cancellation token that will trigger timeout for the ENTIRE operation
        // This ensures all nested waits (including the initial navigation/historyUpdated wait) are bounded
        // A timeout of 0 means "no timeout" (infinite wait), so we don't set a timeout in that case
        using var timeoutCts = timeout > 0
            ? new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout))
            : new CancellationTokenSource();
        var timeoutToken = timeoutCts.Token;

        // Setup frame detachment handler at the outer level to race with the entire navigation
        var frameDetachedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnFrameDetached(object sender, ClosedEventArgs args)
        {
            frameDetachedTcs.TrySetException(new PuppeteerException("Navigating frame was detached"));
        }

        BrowsingContext.Closed += OnFrameDetached;

        // Register timeout to also complete the frame detached task to unblock any waits
        // Only register if we have a timeout (timeout > 0)
        CancellationTokenRegistration? timeoutRegistration = timeout > 0
            ? timeoutToken.Register(() =>
                frameDetachedTcs.TrySetException(new TimeoutException($"Navigation timeout of {timeout}ms exceeded")))
            : null;

        // Setup load event listeners BEFORE waiting for navigation to avoid race conditions
        // This ensures we capture Load/DOMContentLoaded events even if they fire quickly after navigation starts
        var (waitForLoadTask, cleanupLoadListeners) = SetupLoadEventListeners(options);

        try
        {
            async Task<Navigation> WaitForEventNavigationAsync(CancellationToken cancellationToken)
            {
                // Wait for navigation or history updated event
                var navigationTcs = new TaskCompletionSource<Navigation>(TaskCreationOptions.RunContinuationsAsynchronously);
                var historyUpdatedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                void OnNavigation(object sender, BrowserContextNavigationEventArgs args)
                {
                    navigationTcs.TrySetResult(args.Navigation);
                }

                void OnHistoryUpdated(object sender, EventArgs args)
                {
                    historyUpdatedTcs.TrySetResult(true);
                }

                BrowsingContext.Navigation += OnNavigation;
                BrowsingContext.HistoryUpdated += OnHistoryUpdated;

                // Create a task that completes when cancellation is requested (timeout)
                var cancellationTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                using var cancellationRegistration = cancellationToken.Register(() => cancellationTcs.TrySetCanceled(cancellationToken));

                try
                {
                    // Race between navigation, historyUpdated, and timeout
                    var completedTask = await Task.WhenAny(navigationTcs.Task, historyUpdatedTcs.Task, cancellationTcs.Task).ConfigureAwait(false);

                    // Check if we timed out
                    cancellationToken.ThrowIfCancellationRequested();

                    if (completedTask == historyUpdatedTcs.Task)
                    {
                        var delay = Task.Delay(100, cancellationToken);
                        await Task.WhenAny(navigationTcs.Task, delay).ConfigureAwait(false);

                        // Check again after the delay
                        cancellationToken.ThrowIfCancellationRequested();

                        if (!navigationTcs.Task.IsCompleted)
                        {
                            return null;
                        }
                    }

                    var navigation = navigationTcs.Task.Result;

                    // Collect child frames for detachment tracking
                    var childFrames = ChildFrames.Cast<BidiFrame>().ToList();
                    var childFrameDetachedTasks = childFrames.Select(frame =>
                    {
                        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                        void OnClosed(object sender, ClosedEventArgs args) => tcs.TrySetResult(true);
                        frame.BrowsingContext.Closed += OnClosed;

                        // Clean up handler when task completes
                        tcs.Task.ContinueWith(_ => frame.BrowsingContext.Closed -= OnClosed, TaskScheduler.Default);
                        return tcs.Task;
                    }).ToList();

                    // Setup fragment, failed, and aborted event handlers
                    Task<bool> waitForFragmentTask;
                    if (navigation.FragmentReceived)
                    {
                        waitForFragmentTask = Task.FromResult(true);
                    }
                    else
                    {
                        var fragmentTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                        void OnFragment(object sender, NavigationEventArgs args) => fragmentTcs.TrySetResult(true);
                        navigation.Fragment += OnFragment;
                        waitForFragmentTask = fragmentTcs.Task;
                        _ = waitForFragmentTask.ContinueWith(_ => navigation.Fragment -= OnFragment, TaskScheduler.Default);
                    }

                    var failedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    navigation.Failed += OnFailed;
                    var waitForFailedTask = failedTcs.Task;
                    _ = waitForFailedTask.ContinueWith(_ => navigation.Failed -= OnFailed, TaskScheduler.Default);

                    var abortedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    navigation.Aborted += OnAborted;
                    var waitForAbortedTask = abortedTcs.Task;
                    _ = waitForAbortedTask.ContinueWith(_ => navigation.Aborted -= OnAborted, TaskScheduler.Default);

                    // Create a task that waits for load, then child frames to detach
                    var waitForLoadAndChildFramesTask = Task.Run(
                        async () =>
                        {
                            await waitForLoadTask.ConfigureAwait(false);

                            if (childFrameDetachedTasks.Count > 0)
                            {
                                await Task.WhenAll(childFrameDetachedTasks).ConfigureAwait(false);
                            }
                        },
                        cancellationToken);

                    // Race between (load+childFrames) and fragment/failed/aborted and timeout
                    // Any of these events can complete the navigation
                    var navCompletedTask = await Task.WhenAny(
                        waitForLoadAndChildFramesTask,
                        waitForFragmentTask,
                        waitForFailedTask,
                        waitForAbortedTask,
                        cancellationTcs.Task).ConfigureAwait(false);

                    cancellationToken.ThrowIfCancellationRequested();

                    // If navigation failed or was aborted, don't wait for request - navigation is done
                    var navigationEnded = navCompletedTask == waitForFailedTask || navCompletedTask == waitForAbortedTask;

                    // Wait for request to be created if we don't have one yet and navigation hasn't ended
                    if (navigation.Request == null && !navigationEnded)
                    {
                        var requestCreatedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                        void OnRequestCreated(object sender, Core.RequestEventArgs args) => requestCreatedTcs.TrySetResult(true);

                        navigation.RequestCreated += OnRequestCreated;

                        // Wait up to 1 second for request to be created
                        // If it doesn't happen, it means this is a cached/history navigation
                        var requestCreatedTask = requestCreatedTcs.Task;
                        var delay = Task.Delay(1000, cancellationToken);
                        await Task.WhenAny(requestCreatedTask, delay, cancellationTcs.Task).ConfigureAwait(false);

                        navigation.RequestCreated -= OnRequestCreated;
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    // Wait for request completion if navigation has a request and navigation hasn't ended
                    if (navigation.Request != null && !navigationEnded)
                    {
                        await WaitForRequestFinishedAsync(navigation.Request, cancellationToken).ConfigureAwait(false);
                    }

                    return navigation;

                    void OnAborted(object sender, NavigationEventArgs args) => abortedTcs.TrySetResult(true);

                    void OnFailed(object sender, NavigationEventArgs args) => failedTcs.TrySetResult(true);
                }
                finally
                {
                    BrowsingContext.Navigation -= OnNavigation;
                    BrowsingContext.HistoryUpdated -= OnHistoryUpdated;
                }
            }

            var waitForEventNavigationTask = WaitForEventNavigationAsync(timeoutToken);
            var waitForNetworkIdleTask = WaitForNetworkIdleAsync(options, timeoutToken);

            var waitForResponse = new Func<Task<IResponse>>(async () =>
            {
                // Use WhenAllFailFast to fail immediately if any task fails (like Promise.all in JS)
                await new[] { waitForEventNavigationTask, waitForNetworkIdleTask }.WhenAllFailFast().ConfigureAwait(false);
                var navigation = waitForEventNavigationTask.Result;

                // Navigation might be null
                if (navigation == null)
                {
                    return null;
                }

                var request = navigation.Request;

                if (request == null)
                {
                    return null;
                }

                var lastRequest = request.LastRedirect ?? request;

                // Try to get the BidiHttpRequest wrapper
                if (BidiHttpRequest.Requests.TryGetValue(lastRequest, out var httpRequest))
                {
                    return httpRequest?.Response;
                }

                // If not found (e.g., history navigation), check if the Core.Request has a response
                // and create a BidiHttpResponse from it
                if (lastRequest.Response != null)
                {
                    // For history navigations, create the BidiHttpRequest wrapper
                    // It might already have a response or the Success event might have already fired
                    var bidiRequest = BidiHttpRequest.From(lastRequest, this, null);

                    // If the response isn't set yet (because Success event already fired before handler was attached),
                    // we need to trigger the response creation manually
                    if (bidiRequest.Response == null)
                    {
                        // Create the response directly since the Success event was missed
                        var response = BidiHttpResponse.From(lastRequest.Response, bidiRequest, BidiPage.BidiBrowser.CdpSupported);
                        return response;
                    }

                    return bidiRequest.Response;
                }

                return null;
            });

            var waitForResponseTask = waitForResponse();

            // Handle user-provided cancellation token if provided
            var tasksToRace = new List<Task> { waitForResponseTask, frameDetachedTcs.Task };

            if (options?.CancellationToken.HasValue == true)
            {
                var userCancellationToken = options.CancellationToken.Value;
                var userCancellationTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                userCancellationToken.Register(() => userCancellationTcs.TrySetCanceled(userCancellationToken));
                tasksToRace.Add(userCancellationTcs.Task);
            }

            await Task.WhenAny(tasksToRace).ConfigureAwait(false);

            // Check if frame was detached or timeout occurred - re-throw the exception if so
            if (frameDetachedTcs.Task.IsCompleted)
            {
                await frameDetachedTcs.Task.ConfigureAwait(false);
            }

            var result = await waitForResponseTask.ConfigureAwait(false);
            return result;
        }
        finally
        {
            timeoutRegistration?.Dispose();
            BrowsingContext.Closed -= OnFrameDetached;
            cleanupLoadListeners();
        }
    }

    /// <inheritdoc />
    public override async Task<ElementHandle> FrameElementAsync()
    {
        var parentFrame = ParentFrame as BidiFrame;
        if (parentFrame == null)
        {
            return null;
        }

        var nodes = await parentFrame.BrowsingContext.LocateNodesAsync(
            new WebDriverBiDi.BrowsingContext.ContextLocator(Id)).ConfigureAwait(false);

        var node = nodes.FirstOrDefault();
        if (node == null)
        {
            return null;
        }

        return BidiElementHandle.From(node, (BidiRealm)parentFrame.MainRealm) as ElementHandle;
    }

    internal static BidiFrame From(BidiPage parentPage, BidiFrame parentFrame, BrowsingContext browsingContext)
    {
        parentFrame = new BidiFrame(parentPage, parentFrame, browsingContext);
        parentFrame.Initialize();
        return parentFrame;
    }

    /// <summary>
    /// Exposes a function to the page's JavaScript context.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="puppeteerFunction">The function to expose.</param>
    /// <returns>A task that resolves when the function has been exposed.</returns>
    internal async Task ExposeFunctionAsync(string name, Delegate puppeteerFunction)
    {
        if (_exposedFunctions.ContainsKey(name))
        {
            throw new PuppeteerException($"Failed to add page binding with name {name}: globalThis['{name}'] already exists!");
        }

        var exposable = await ExposableFunction.FromAsync(this, name, puppeteerFunction).ConfigureAwait(false);
        _exposedFunctions[name] = exposable;
    }

    /// <summary>
    /// Removes an exposed function from the page.
    /// </summary>
    /// <param name="name">The name of the function to remove.</param>
    /// <returns>A task that resolves when the function has been removed.</returns>
    internal async Task RemoveExposedFunctionAsync(string name)
    {
        if (!_exposedFunctions.TryRemove(name, out var exposedFunction))
        {
            throw new PuppeteerException($"Failed to remove page binding with name {name}: globalThis['{name}'] does not exist!");
        }

        await exposedFunction.DisposeAsync().ConfigureAwait(false);
    }

    internal async Task SetFilesAsync(BidiElementHandle element, string[] files)
    {
        await BrowsingContext.SetFilesAsync(
            element.Value.ToSharedReference(),
            files).ConfigureAwait(false);
    }

    internal async Task<IList<RemoteValue>> LocateNodesAsync(BidiElementHandle element, BidiLocator locator)
    {
        return await BrowsingContext.LocateNodesAsync(
            locator,
            [element.Value.ToSharedReference()]).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected internal override DeviceRequestPromptManager GetDeviceRequestPromptManager() => throw new System.NotImplementedException();

    private static ConsoleType ConvertConsoleMessageLevel(string method)
    {
        return method switch
        {
            "group" => ConsoleType.StartGroup,
            "groupCollapsed" => ConsoleType.StartGroupCollapsed,
            "groupEnd" => ConsoleType.EndGroup,
            "log" => ConsoleType.Log,
            "debug" => ConsoleType.Debug,
            "info" => ConsoleType.Info,
            "error" => ConsoleType.Error,
            "warn" => ConsoleType.Warning,
            "dir" => ConsoleType.Dir,
            "dirxml" => ConsoleType.Dirxml,
            "table" => ConsoleType.Table,
            "trace" => ConsoleType.Trace,
            "clear" => ConsoleType.Clear,
            "assert" => ConsoleType.Assert,
            "profile" => ConsoleType.Profile,
            "profileEnd" => ConsoleType.ProfileEnd,
            "count" => ConsoleType.Count,
            "timeEnd" => ConsoleType.TimeEnd,
            "verbose" => ConsoleType.Verbose,
            "timeStamp" => ConsoleType.Timestamp,
            _ => ConsoleType.Log,
        };
    }

    private static ConsoleMessageLocation GetStackTraceLocation(WebDriverBiDi.Script.StackTrace stackTrace)
    {
        if (stackTrace?.CallFrames?.Count > 0)
        {
            var callFrame = stackTrace.CallFrames[0];
            return new ConsoleMessageLocation
            {
                URL = callFrame.Url,
                LineNumber = (int)callFrame.LineNumber,
                ColumnNumber = (int)callFrame.ColumnNumber,
            };
        }

        return null;
    }

    private static IList<ConsoleMessageLocation> GetStackTrace(WebDriverBiDi.Script.StackTrace stackTrace)
    {
        if (stackTrace?.CallFrames?.Count > 0)
        {
            return stackTrace.CallFrames.Select(callFrame => new ConsoleMessageLocation
            {
                URL = callFrame.Url,
                LineNumber = (int)callFrame.LineNumber,
                ColumnNumber = (int)callFrame.ColumnNumber,
            }).ToList();
        }

        return [];
    }

    /// <summary>
    /// Determines if navigating from currentUrl to targetUrl is a same-document navigation.
    /// Same-document navigations occur when only the fragment (hash) portion of the URL changes.
    /// Firefox BiDi doesn't reliably fire navigation events for these in OOPIFs.
    /// </summary>
    private static bool IsSameDocumentNavigation(string currentUrl, string targetUrl)
    {
        if (string.IsNullOrEmpty(currentUrl) || string.IsNullOrEmpty(targetUrl))
        {
            return false;
        }

        // Parse both URLs to compare their non-fragment parts
        if (!Uri.TryCreate(currentUrl, UriKind.Absolute, out var currentUri) ||
            !Uri.TryCreate(targetUrl, UriKind.Absolute, out var targetUri))
        {
            return false;
        }

        // Compare everything except the fragment
        // GetLeftPart(UriPartial.Query) returns scheme + authority + path + query (everything except fragment)
        var currentWithoutFragment = currentUri.GetLeftPart(UriPartial.Query);
        var targetWithoutFragment = targetUri.GetLeftPart(UriPartial.Query);

        // It's a same-document navigation if:
        // 1. The non-fragment parts are identical
        // 2. The target URL has a fragment (otherwise it would be a regular navigation)
        return string.Equals(currentWithoutFragment, targetWithoutFragment, StringComparison.Ordinal)
            && !string.IsNullOrEmpty(targetUri.Fragment);
    }

    private PuppeteerException RewriteNavigationError(Exception ex, string url, int timeoutSettingsNavigationTimeout)
    {
        return ex is TimeoutException
            ? new NavigationException($"Navigation timeout of {timeoutSettingsNavigationTimeout} ms exceeded", url, ex)
            : new PuppeteerException("Navigation failed: " + ex.Message, ex);
    }

    /// <summary>
    /// Sets up load event listeners early to avoid race conditions.
    /// Returns a task that completes when all load events fire, and a cleanup action.
    /// </summary>
    private (Task WaitTask, Action Cleanup) SetupLoadEventListeners(NavigationOptions options)
    {
        var waitUntil = options?.WaitUntil ?? new[] { WaitUntilNavigation.Load };
        var timeout = options?.Timeout ?? TimeoutSettings.NavigationTimeout;

        List<Task> tasks = new();
        var cleanupActions = new List<Action>();

        if (waitUntil.Contains(WaitUntilNavigation.Load))
        {
            var loadTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            void OnLoad(object sender, EventArgs args) => loadTcs.TrySetResult(true);
            BrowsingContext.Load += OnLoad;
            tasks.Add(loadTcs.Task);
            cleanupActions.Add(() => BrowsingContext.Load -= OnLoad);
        }

        if (waitUntil.Contains(WaitUntilNavigation.DOMContentLoaded))
        {
            var domContentLoadedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            void OnDomContentLoaded(object sender, EventArgs args) => domContentLoadedTcs.TrySetResult(true);
            BrowsingContext.DomContentLoaded += OnDomContentLoaded;
            tasks.Add(domContentLoadedTcs.Task);
            cleanupActions.Add(() => BrowsingContext.DomContentLoaded -= OnDomContentLoaded);
        }

        var waitTask = tasks.Count > 0
            ? Task.WhenAll(tasks).WithTimeout(
                timeout,
                t => new TimeoutException($"Navigation timeout of {t.TotalMilliseconds} ms exceeded"))
            : Task.CompletedTask;

        void Cleanup()
        {
            foreach (var action in cleanupActions)
            {
                action();
            }
        }

        return (waitTask, Cleanup);
    }

    private Task WaitForNetworkIdleAsync(NavigationOptions options) =>
        WaitForNetworkIdleAsync(options, CancellationToken.None);

    private async Task WaitForNetworkIdleAsync(NavigationOptions options, CancellationToken cancellationToken)
    {
        var waitUntil = options?.WaitUntil ?? [WaitUntilNavigation.Load];

        // Determine concurrency based on WaitUntil navigation options
        // networkidle0 = wait until there are 0 active connections
        // networkidle2 = wait until there are at most 2 active connections
        int? concurrency = null;
        foreach (var condition in waitUntil)
        {
            switch (condition)
            {
                case WaitUntilNavigation.Networkidle0:
                    concurrency = concurrency.HasValue ? Math.Min(0, concurrency.Value) : 0;
                    break;
                case WaitUntilNavigation.Networkidle2:
                    concurrency = concurrency.HasValue ? Math.Min(2, concurrency.Value) : 2;
                    break;
            }
        }

        // If no network idle condition was specified, return completed task
        if (!concurrency.HasValue)
        {
            return;
        }

        var networkIdleTask = BidiPage.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions
        {
            IdleTime = 500,
            Timeout = options?.Timeout ?? TimeoutSettings.Timeout,
            Concurrency = concurrency.Value,
        });

        // Race with cancellation token
        var cancellationTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cancellationToken.Register(() => cancellationTcs.TrySetCanceled(cancellationToken));

        await Task.WhenAny(networkIdleTask, cancellationTcs.Task).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
    }

    private async Task WaitForRequestFinishedAsync(Request request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return;
        }

        // Reduces flakiness if the response events arrive after the load event.
        // Usually, the response or error is already there at this point.
        if (request.Response != null || request.HasError)
        {
            return;
        }

        // If this is a redirect, recursively wait for the redirect to complete
        if (request.LastRedirect != null)
        {
            await WaitForRequestFinishedAsync(request.LastRedirect, cancellationToken).ConfigureAwait(false);
            return;
        }

        // Wait for success, error, or redirect events
        var successTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var errorTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var redirectTcs = new TaskCompletionSource<Request>(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancellationTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnSuccess(object sender, ResponseEventArgs args) => successTcs.TrySetResult(true);
        void OnError(object sender, Core.ErrorEventArgs args) => errorTcs.TrySetResult(true);
        void OnRedirect(object sender, Core.RequestEventArgs args) => redirectTcs.TrySetResult(args.Request);

        request.Success += OnSuccess;
        request.Error += OnError;
        request.Redirect += OnRedirect;

        using var registration = cancellationToken.Register(() => cancellationTcs.TrySetCanceled(cancellationToken));

        try
        {
            var completedTask = await Task.WhenAny(successTcs.Task, errorTcs.Task, redirectTcs.Task, cancellationTcs.Task).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            // If we got a redirect, recursively wait for it to finish
            if (completedTask == redirectTcs.Task)
            {
                var redirectRequest = await redirectTcs.Task.ConfigureAwait(false);
                await WaitForRequestFinishedAsync(redirectRequest, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            request.Success -= OnSuccess;
            request.Error -= OnError;
            request.Redirect -= OnRedirect;
        }
    }

    private async Task NavigateAsync(string url)
    {
        // Some implementations currently only report errors when the
        // readiness=interactive.
        //
        // Related: https://bugzilla.mozilla.org/show_bug.cgi?id=1846601
        try
        {
            await BrowsingContext.NavigateAsync(url, WebDriverBiDi.BrowsingContext.ReadinessState.Interactive).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("net::ERR_HTTP_RESPONSE_CODE_FAILURE"))
            {
                return;
            }

            if (ex.Message.Contains("navigation canceled"))
            {
                return;
            }

            if (ex.Message.Contains("Navigation was aborted by another navigation"))
            {
                return;
            }

            // The browser error message is passed through as-is (e.g., "NS_ERROR_ABORT" for Firefox).
            throw new NavigationException($"{ex.Message} at {url}", url, ex);
        }
    }

    private void Initialize()
    {
        foreach (var browsingContext in BrowsingContext.Children)
        {
            CreateFrameTarget(browsingContext);
        }

        BrowsingContext.BrowsingContextCreated += (sender, args) =>
        {
            CreateFrameTarget(args.BrowsingContext);
        };

        BrowsingContext.Closed += (sender, args) =>
        {
            foreach (var session in BidiCdpSession.Sessions)
            {
                if (session.Frame == this)
                {
                    session.OnClose();
                }
            }

            // Dispose all exposed functions to prevent memory leaks
            // Use fire-and-forget pattern to avoid async event handler issues
            foreach (var kvp in _exposedFunctions)
            {
                _ = kvp.Value.DisposeAsync().AsTask().ContinueWith(
                    _ => { },
                    TaskScheduler.Default);
            }

            _exposedFunctions.Clear();

            ((BidiPage)Page).OnFrameDetached(new FrameEventArgs(this));
        };

        BrowsingContext.Request += (sender, args) =>
        {
            var httpRequest = BidiHttpRequest.From(args.Request, this);
            SetupRequestHandlers(args.Request, httpRequest);

            // Use the request interception queue to serialize handler execution.
            // This ensures that the first request (with user handlers) is processed
            // before any duplicate requests (e.g., Firefox BiDi speculative loading),
            // preventing duplicates from racing to the server without proper headers.
            _ = BidiPage.RequestInterceptionQueue.Enqueue(
                () => httpRequest.FinalizeInterceptionsAsync());
        };

        BrowsingContext.Navigation += (sender, args) =>
        {
            if (args.Navigation.FragmentReceived)
            {
                ((Page)Page).OnFrameNavigated(new FrameNavigatedEventArgs(this, NavigationType.Navigation));
            }

            args.Navigation.Fragment += (o, eventArgs) =>
            {
                ((Page)Page).OnFrameNavigated(new FrameNavigatedEventArgs(this, NavigationType.Navigation));
            };
        };

        BrowsingContext.Load += (sender, args) =>
        {
            ((Page)Page).OnLoad();
        };

        BrowsingContext.DomContentLoaded += (sender, args) =>
        {
            OnLoadingStarted();
            ((Page)Page).OnDOMContentLoaded();
            ((Page)Page).OnFrameNavigated(new FrameNavigatedEventArgs(this, NavigationType.Navigation));
        };

        BrowsingContext.UserPrompt += (sender, args) =>
        {
            var dialog = BidiDialog.From(args.UserPrompt);
            ((Page)Page).OnDialog(new DialogEventArgs(dialog));
        };

        BrowsingContext.Log += (sender, args) =>
        {
            if (Id != args.Source.Context)
            {
                return;
            }

            if (args.Type == "console")
            {
                var consoleArgs = args.Arguments;
                var handleArgs = consoleArgs?.Select(arg => ((BidiFrameRealm)MainRealm).CreateHandle(arg)).ToArray() ?? [];

                var text = string.Join(
                    " ",
                    handleArgs.Select(arg =>
                    {
                        if (arg is BidiJSHandle { IsPrimitiveValue: true } jsHandle)
                        {
                            return BidiDeserializer.Deserialize(jsHandle.RemoteValue);
                        }

                        return arg.ToString();
                    })).Trim();

                var location = GetStackTraceLocation(args.StackTrace);
                var stackTrace = GetStackTrace(args.StackTrace);

                var consoleMessage = new ConsoleMessage(
                    ConvertConsoleMessageLevel(args.Method),
                    text,
                    handleArgs,
                    location,
                    stackTrace);

                BidiPage.OnConsole(new ConsoleEventArgs(consoleMessage));
            }
            else if (args.Type == "javascript")
            {
                var text = args.Text ?? string.Empty;
                var messageLines = new List<string> { text };

                var stackLines = new List<string>();
                if (args.StackTrace != null)
                {
                    foreach (var frame in args.StackTrace.CallFrames)
                    {
                        stackLines.Add($"    at {(string.IsNullOrEmpty(frame.FunctionName) ? "<anonymous>" : frame.FunctionName)} ({frame.Url}:{frame.LineNumber + 1}:{frame.ColumnNumber + 1})");
                        if (stackLines.Count >= 10)
                        {
                            break;
                        }
                    }
                }

                var fullStack = string.Join("\n", messageLines.Concat(stackLines));
                BidiPage.OnPageError(new PageErrorEventArgs(fullStack));
            }
        };

        // Wire up worker events
        BrowsingContext.Worker += (_, args) =>
        {
            var worker = BidiWebWorker.From(this, args.Realm);
            args.Realm.Destroyed += (_, _) =>
            {
                BidiPage.OnWorkerDestroyed(worker);
            };
            BidiPage.OnWorkerCreated(worker);
        };
    }

    private void CreateFrameTarget(BrowsingContext browsingContext)
    {
        var frame = From(null, this, browsingContext);
        _frames.TryAdd(browsingContext, frame);
        ((BidiPage)Page).OnFrameAttached(new FrameEventArgs(frame));

        browsingContext.Closed += (_, _) =>
        {
            _frames.TryRemove(browsingContext, out var _);
        };
    }

    private void SetupRequestHandlers(Core.Request coreRequest, BidiHttpRequest httpRequest)
    {
        var successFired = false;
        var errorFired = false;

        coreRequest.Success += (o, eventArgs) =>
        {
            // Emulate 'once' behavior - only fire once
            if (successFired)
            {
                return;
            }

            successFired = true;
            ((BidiPage)Page).OnRequestFinished(new RequestEventArgs(httpRequest));
        };

        coreRequest.Error += (o, eventArgs) =>
        {
            // Emulate 'once' behavior - only fire once
            if (errorFired)
            {
                return;
            }

            errorFired = true;

            if (BidiPage.TryMarkFailedEventFired(httpRequest.Url))
            {
                ((BidiPage)Page).OnRequestFailed(new RequestEventArgs(httpRequest));
            }
        };
    }

    private class Realms(BidiFrameRealm defaultRealm, BidiFrameRealm internalRealm)
    {
        public BidiFrameRealm Default { get; } = defaultRealm;

        public BidiFrameRealm Internal { get; } = internalRealm;
    }
}

#endif
