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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using Request = PuppeteerSharp.Bidi.Core.Request;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiFrame : Frame
{
    private readonly ConcurrentDictionary<BrowsingContext, BidiFrame> _frames = new();
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
    public override IReadOnlyCollection<IFrame> ChildFrames => _frames.Values.Cast<IFrame>().ToList();

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
    public override Task<IElementHandle> AddStyleTagAsync(AddTagOptions options) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task<IElementHandle> AddScriptTagAsync(AddTagOptions options) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override async Task SetContentAsync(string html, NavigationOptions options = null)
    {
        var timeout = options?.Timeout ?? TimeoutSettings.NavigationTimeout;

        // Wait for load and network idle events
        var waitForLoadTask = WaitForLoadAsync(options);
        var waitForNetworkIdleTask = WaitForNetworkIdleAsync(options);

        // Set the frame content using JavaScript, similar to CDP implementation
        await EvaluateFunctionAsync(
            @"html => {
                document.open();
                document.write(html);
                document.close();
            }",
            html).ConfigureAwait(false);

        await Task.WhenAll(waitForLoadTask, waitForNetworkIdleTask).WithTimeout(timeout).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<IResponse> GoToAsync(string url, NavigationOptions options)
    {
        var waitForNavigationTask = WaitForNavigationAsync(options);
        var navigationTask = NavigateAsync(url);

        try
        {
            await Task.WhenAll(waitForNavigationTask, navigationTask).ConfigureAwait(false);
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

        return waitForNavigationTask.Result;
    }

    /// <inheritdoc />
    public override async Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
    {
        var timeout = options?.Timeout ?? TimeoutSettings.NavigationTimeout;

        // Setup frame detachment handler at the outer level to race with the entire navigation
        var frameDetachedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnFrameDetached(object sender, ClosedEventArgs args)
        {
            frameDetachedTcs.TrySetException(new PuppeteerException("Navigating frame was detached"));
        }

        BrowsingContext.Closed += OnFrameDetached;

        try
        {
            async Task<Navigation> WaitForEventNavigationAsync()
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

                try
                {
                    // Wait for either event
                    var completedTask = await Task.WhenAny(navigationTcs.Task, historyUpdatedTcs.Task).ConfigureAwait(false);

                    if (completedTask == historyUpdatedTcs.Task)
                    {
                        var delay = Task.Delay(100);
                        await Task.WhenAny(navigationTcs.Task, delay).ConfigureAwait(false);

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

                    // Wait for load events first
                    var waitForLoadTask = WaitForLoadAsync(options);

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
                    var waitForLoadAndChildFramesTask = Task.Run(async () =>
                    {
                        await waitForLoadTask.ConfigureAwait(false);

                        if (childFrameDetachedTasks.Count > 0)
                        {
                            await Task.WhenAll(childFrameDetachedTasks).ConfigureAwait(false);
                        }
                    });

                    // Race between (load+childFrames) and fragment/failed/aborted
                    // Any of these events can complete the navigation
                    await Task.WhenAny(
                        waitForLoadAndChildFramesTask,
                        waitForFragmentTask,
                        waitForFailedTask,
                        waitForAbortedTask).ConfigureAwait(false);

                    // Wait for request to be created if we don't have one yet
                    if (navigation.Request == null)
                    {
                        var requestCreatedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                        void OnRequestCreated(object sender, Core.RequestEventArgs args) => requestCreatedTcs.TrySetResult(true);

                        navigation.RequestCreated += OnRequestCreated;

                        // Wait up to 1 second for request to be created
                        // If it doesn't happen, it means this is a cached/history navigation
                        var requestCreatedTask = requestCreatedTcs.Task;
                        var delay = Task.Delay(1000);
                        await Task.WhenAny(requestCreatedTask, delay).ConfigureAwait(false);

                        navigation.RequestCreated -= OnRequestCreated;
                    }

                    // Wait for request completion if navigation has a request
                    if (navigation.Request != null)
                    {
                        await WaitForRequestFinishedAsync(navigation.Request).ConfigureAwait(false);
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

            var waitForEventNavigationTask = WaitForEventNavigationAsync();
            var waitForNetworkIdleTask = WaitForNetworkIdleAsync(options);

            var waitForResponse = new Func<Task<IResponse>>(async () =>
            {
                await Task.WhenAll(waitForEventNavigationTask, waitForNetworkIdleTask).ConfigureAwait(false);
                var navigation = waitForEventNavigationTask.Result;

                // Navigation might be null
                if (navigation == null)
                {
                    return null;
                }

                // If there's no request associated with this navigation after waiting,
                // it means this is either:
                // 1. A special URL like about:blank (no network request) - return null
                // 2. A cached history navigation (GoBack/GoForward) - create synthetic response
                // See: https://github.com/w3c/webdriver-bidi/issues/502
                var request = navigation.Request;

                if (request == null)
                {
                    var url = BrowsingContext.Url;

                    // Special URLs like about:blank don't have network requests
                    if (url.StartsWith("about:", StringComparison.OrdinalIgnoreCase) ||
                        url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                    // For cached history navigations, create a synthetic response
                    return BidiHttpResponse.FromCachedNavigation(url);
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

            // Handle cancellation token if provided
            var tasksToRace = new List<Task> { waitForResponseTask, frameDetachedTcs.Task };

            if (options?.CancellationToken.HasValue == true)
            {
                var cancellationToken = options.CancellationToken.Value;
                var cancellationTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                cancellationToken.Register(() => cancellationTcs.TrySetCanceled(cancellationToken));
                tasksToRace.Add(cancellationTcs.Task);
            }

            var completedTask = await Task.WhenAny(tasksToRace).WithTimeout(timeout).ConfigureAwait(false);

            // Check if frame was detached - re-throw the exception if so
            if (frameDetachedTcs.Task.IsCompleted)
            {
                await frameDetachedTcs.Task.ConfigureAwait(false);
            }

            var result = await waitForResponseTask.ConfigureAwait(false);
            return result;
        }
        finally
        {
            BrowsingContext.Closed -= OnFrameDetached;
        }
    }

    internal static BidiFrame From(BidiPage parentPage, BidiFrame parentFrame, BrowsingContext browsingContext)
    {
        parentFrame = new BidiFrame(parentPage, parentFrame, browsingContext);
        parentFrame.Initialize();
        return parentFrame;
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

    private PuppeteerException RewriteNavigationError(Exception ex, string url, int timeoutSettingsNavigationTimeout)
    {
        return ex is TimeoutException
            ? new NavigationException($"Navigation timeout of {timeoutSettingsNavigationTimeout} ms exceeded", url, ex)
            : new PuppeteerException("Navigation failed: " + ex.Message, ex);
    }

    private Task WaitForLoadAsync(NavigationOptions options)
    {
        var waitUntil = options?.WaitUntil ?? new[] { WaitUntilNavigation.Load };
        var timeout = options?.Timeout ?? TimeoutSettings.NavigationTimeout;

        List<Task> tasks = new();

        if (waitUntil.Contains(WaitUntilNavigation.Load))
        {
            var loadTcs = new TaskCompletionSource<bool>();
            BrowsingContext.Load += (sender, args) => loadTcs.TrySetResult(true);
            tasks.Add(loadTcs.Task);
        }

        if (waitUntil.Contains(WaitUntilNavigation.DOMContentLoaded))
        {
            var domContentLoadedTcs = new TaskCompletionSource<bool>();
            BrowsingContext.DomContentLoaded += (sender, args) => domContentLoadedTcs.TrySetResult(true);
            tasks.Add(domContentLoadedTcs.Task);
        }

        // TODO: Check frame detached event.
        return Task.WhenAll(tasks).WithTimeout(timeout);
    }

    private Task WaitForNetworkIdleAsync(NavigationOptions options)
    {
        // TODO: Complete this method.
        return Task.CompletedTask;
    }

    private async Task WaitForRequestFinishedAsync(Request request)
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
            await WaitForRequestFinishedAsync(request.LastRedirect).ConfigureAwait(false);
            return;
        }

        // Wait for success, error, or redirect events
        var successTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var errorTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var redirectTcs = new TaskCompletionSource<Request>(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnSuccess(object sender, ResponseEventArgs args) => successTcs.TrySetResult(true);
        void OnError(object sender, Core.ErrorEventArgs args) => errorTcs.TrySetResult(true);
        void OnRedirect(object sender, Core.RequestEventArgs args) => redirectTcs.TrySetResult(args.Request);

        request.Success += OnSuccess;
        request.Error += OnError;
        request.Redirect += OnRedirect;

        try
        {
            var completedTask = await Task.WhenAny(successTcs.Task, errorTcs.Task, redirectTcs.Task).ConfigureAwait(false);

            // If we got a redirect, recursively wait for it to finish
            if (completedTask == redirectTcs.Task)
            {
                var redirectRequest = await redirectTcs.Task.ConfigureAwait(false);
                await WaitForRequestFinishedAsync(redirectRequest).ConfigureAwait(false);
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

            throw new NavigationException($"Failed to navigate to {url}. {ex.Message}", url, ex);
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

            ((BidiPage)Page).OnFrameDetached(new FrameEventArgs(this));
        };

        BrowsingContext.Request += (sender, args) =>
        {
            var httpRequest = BidiHttpRequest.From(args.Request, this);

            args.Request.Success += (o, eventArgs) =>
            {
                ((BidiPage)Page).OnRequestFinished(new RequestEventArgs(httpRequest));
            };

            args.Request.Error += (o, eventArgs) =>
            {
                ((BidiPage)Page).OnRequestFailed(new RequestEventArgs(httpRequest));
            };

            _ = httpRequest.FinalizeInterceptionsAsync();
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

    private class Realms(BidiFrameRealm defaultRealm, BidiFrameRealm internalRealm)
    {
        public BidiFrameRealm Default { get; } = defaultRealm;

        public BidiFrameRealm Internal { get; } = internalRealm;
    }
}

