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
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Media;
using WebDriverBiDi.BrowsingContext;
using WebDriverBiDi.Input;
using WebDriverBiDi.Network;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiPage : Page
{
    private readonly ConcurrentDictionary<string, BidiWebWorker> _workers = new();
    private readonly CdpEmulationManager _cdpEmulationManager;
    private readonly TaskQueue _requestInterceptionQueue = new();

    // Track URLs that have fired the RequestServedFromCache event to deduplicate Firefox's duplicate events.
    // Cleared on navigation start.
    private readonly ConcurrentDictionary<string, bool> _cacheEventsFired = new();

    // Track URLs that have fired the RequestFailed event to deduplicate Firefox's duplicate events.
    private readonly ConcurrentDictionary<string, bool> _failedEventsFired = new();

    // Track URLs that have fired the Response event to deduplicate Firefox's duplicate events.
    private readonly ConcurrentDictionary<string, bool> _responseEventsFired = new();

    // Track URLs that have fired the Request event to deduplicate Firefox's duplicate events.
    // Firefox BiDi sends duplicate BeforeRequestSent events with different request IDs for speculative/parallel loading.
    private readonly ConcurrentDictionary<string, bool> _requestEventsFired = new();

    // Flag to track if we're currently navigating. Only deduplicate requests during navigation.
    private volatile bool _isNavigating;
    private InternalNetworkConditions _emulatedNetworkConditions;
    private TaskCompletionSource<bool> _closedTcs;
    private string _requestInterception;
    private string _extraHeadersInterception;
    private string _authInterception;
    private bool _isJavaScriptEnabled = true;

    private BidiPage(BidiBrowserContext browserContext, BrowsingContext browsingContext) : base(browserContext.ScreenshotTaskQueue)
    {
        BrowserContext = browserContext;
        Browser = browserContext.Browser;
        BidiMainFrame = BidiFrame.From(this, null, browsingContext);
        _cdpEmulationManager = new CdpEmulationManager(BidiMainFrame.Client);
        Mouse = new BidiMouse(this);
        Keyboard = new BidiKeyboard(this);
    }

    /// <inheritdoc />
    public override IBrowserContext BrowserContext { get; }

    /// <inheritdoc />
    public override CDPSession Client { get; }

    /// <inheritdoc />
    public override Target Target { get; }

    /// <inheritdoc />
    public override IFrame[] Frames
    {
        get
        {
            var frames = new List<IFrame> { BidiMainFrame };
            for (var i = 0; i < frames.Count; i++)
            {
                frames.AddRange(frames[i].ChildFrames);
            }

            return [.. frames];
        }
    }

    /// <inheritdoc />
    public override WebWorker[] Workers => _workers.Values.ToArray();

    /// <inheritdoc />
    public override bool IsJavaScriptEnabled => _isJavaScriptEnabled;

    /// <inheritdoc/>
    public override IFrame MainFrame => BidiMainFrame;

    internal BidiBrowser BidiBrowser => (BidiBrowser)BrowserContext.Browser;

    internal BidiFrame BidiMainFrame { get; set; }

    internal ConcurrentDictionary<string, string> ExtraHttpHeaders { get; set; } = new();

    internal ConcurrentDictionary<string, string> UserAgentHeaders { get; set; } = new();

    internal bool IsNetworkInterceptionEnabled =>
        _requestInterception != null || _extraHeadersInterception != null || _authInterception != null;

    internal Credentials Credentials { get; private set; }

    /// <summary>
    /// Gets the task queue used to serialize request interception handling.
    /// This ensures that user-provided request handlers run sequentially across
    /// concurrent requests, matching JavaScript's single-threaded behavior.
    /// </summary>
    internal TaskQueue RequestInterceptionQueue => _requestInterceptionQueue;

    /// <inheritdoc />
    protected override Browser Browser { get; }

    private Task ClosedTask
    {
        get
        {
            if (_closedTcs == null)
            {
                _closedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                BidiMainFrame.BrowsingContext.Closed += ContextClosed;

                void ContextClosed(object sender, ClosedEventArgs e)
                {
                    _closedTcs.TrySetException(new TargetClosedException("Target closed", "Browsing context closed"));
                    BidiMainFrame.BrowsingContext.Closed -= ContextClosed;
                }
            }

            return _closedTcs.Task;
        }
    }

    /// <inheritdoc />
    public override async Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
    {
        // Clear existing extra headers
        ExtraHttpHeaders.Clear();

        // Add the new headers (normalized to lowercase keys as per upstream)
        if (headers != null)
        {
            foreach (var kvp in headers)
            {
                ExtraHttpHeaders[kvp.Key.ToLowerInvariant()] = kvp.Value;
            }
        }

        // Toggle network interception for BeforeRequestSent phase
        _extraHeadersInterception = await ToggleInterceptionAsync(
            [InterceptPhase.BeforeRequestSent],
            _extraHeadersInterception,
            !ExtraHttpHeaders.IsEmpty).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task AuthenticateAsync(Credentials credentials)
    {
        _authInterception = await ToggleInterceptionAsync(
            [InterceptPhase.AuthRequired],
            _authInterception,
            credentials != null).ConfigureAwait(false);

        Credentials = credentials;
    }

    /// <inheritdoc />
    public override async Task BringToFrontAsync()
        => await BidiMainFrame.BrowsingContext.ActivateAsync().ConfigureAwait(false);

    /// <inheritdoc />
    public override Task EmulateVisionDeficiencyAsync(VisionDeficiency type)
        => _cdpEmulationManager.EmulateVisionDeficiencyAsync(type);

    /// <inheritdoc />
    public override async Task EmulateTimezoneAsync(string timezoneId)
    {
        // CDP requires `GMT` prefix before timezone offset, while BiDi does not. Remove the
        // `GMT` for interop between CDP and BiDi.
        if (timezoneId?.StartsWith("GMT", StringComparison.Ordinal) == true)
        {
            timezoneId = timezoneId.Substring(3);
        }

        try
        {
            var commandParameters = new WebDriverBiDi.Emulation.SetTimeZoneOverrideCommandParameters()
            {
                TimeZone = timezoneId,
                Contexts = [BidiMainFrame.BrowsingContext.Id],
            };

            await BidiMainFrame.BrowsingContext.Session.Driver.Emulation.SetTimeZoneOverrideAsync(commandParameters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // BiDi throws an error if the timezone ID is invalid
            if (ex.Message.Contains("invalid argument") || ex.Message.Contains("timezone"))
            {
                throw new PuppeteerException($"Invalid timezone ID: {timezoneId}", ex);
            }

            throw;
        }
    }

    /// <inheritdoc />
    public override Task ResizeAsync(int contentWidth, int contentHeight)
        => throw new NotSupportedException("Resize is not yet supported in WebDriver BiDi.");

    /// <inheritdoc />
    public override Task<string> WindowIdAsync()
        => Task.FromResult(BidiMainFrame.BrowsingContext.WindowId);

    /// <inheritdoc />
    public override Task EmulateIdleStateAsync(EmulateIdleOverrides idleOverrides = null)
        => _cdpEmulationManager.EmulateIdleStateAsync(idleOverrides);

    /// <inheritdoc />
    public override Task EmulateCPUThrottlingAsync(decimal? factor = null)
        => _cdpEmulationManager.EmulateCPUThrottlingAsync(factor);

    /// <inheritdoc />
    public override async Task<IResponse> ReloadAsync(NavigationOptions options)
    {
        // Clear the events trackers on navigation to allow fresh events
        ClearEventTrackers();

        var navOptions = options == null
            ? new NavigationOptions { IgnoreSameDocumentNavigation = true }
            : options with { IgnoreSameDocumentNavigation = true };
        var waitForNavigationTask = WaitForNavigationAsync(navOptions);
        var navigationTask = BidiMainFrame.BrowsingContext.ReloadAsync();

        try
        {
            // Use WhenAllFailFast to fail immediately if any task fails (like Promise.all in JS)
            // This ensures timeout exceptions propagate immediately instead of waiting for all tasks
            await new[] { waitForNavigationTask, navigationTask }.WhenAllFailFast().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("no such history entry"))
            {
                return null;
            }

            throw new NavigationException(ex.Message, ex);
        }

        return waitForNavigationTask.Result;
    }

    /// <inheritdoc />
    public override async Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions options = null)
    {
        var timeout = options?.Timeout ?? DefaultTimeout;
        var idleTime = options?.IdleTime ?? 500;
        var concurrency = options?.Concurrency ?? 0;

        var networkIdleTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var idleTimer = new System.Timers.Timer { Interval = idleTime, AutoReset = false };

        idleTimer.Elapsed += (_, _) => { networkIdleTcs.TrySetResult(true); };

        var inflightRequests = 0;
        var requestLock = new object();

        void Evaluate()
        {
            idleTimer.Stop();

            lock (requestLock)
            {
                if (inflightRequests <= concurrency)
                {
                    idleTimer.Start();
                }
            }
        }

        void RequestEventListener(object sender, RequestEventArgs e)
        {
            lock (requestLock)
            {
                inflightRequests++;
            }

            Evaluate();
        }

        void RequestFinishedEventListener(object sender, RequestEventArgs e)
        {
            lock (requestLock)
            {
                inflightRequests = Math.Max(0, inflightRequests - 1);
            }

            Evaluate();
        }

        void ResponseEventListener(object sender, ResponseCreatedEventArgs e)
        {
            lock (requestLock)
            {
                inflightRequests = Math.Max(0, inflightRequests - 1);
            }

            Evaluate();
        }

        void Cleanup()
        {
            idleTimer.Stop();
            idleTimer.Dispose();

            Request -= RequestEventListener;
            RequestFinished -= RequestFinishedEventListener;
            RequestFailed -= RequestFinishedEventListener;
            Response -= ResponseEventListener;
        }

        Request += RequestEventListener;
        RequestFinished += RequestFinishedEventListener;
        RequestFailed += RequestFinishedEventListener;
        Response += ResponseEventListener;

        Evaluate();

        await Task.WhenAny(networkIdleTcs.Task, ClosedTask).WithTimeout(timeout, t =>
        {
            Cleanup();

            return new TimeoutException($"Timeout of {t.TotalMilliseconds} ms exceeded");
        }).ConfigureAwait(false);

        Cleanup();

        if (ClosedTask.IsFaulted)
        {
            await ClosedTask.ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override async Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions options = null)
    {
        var timeout = options?.Timeout ?? DefaultTimeout;
        var requestTcs = new TaskCompletionSource<IRequest>(TaskCreationOptions.RunContinuationsAsynchronously);

        void RequestEventListener(object sender, RequestEventArgs e)
        {
            if (predicate(e.Request))
            {
                requestTcs.TrySetResult(e.Request);
                Request -= RequestEventListener;
            }
        }

        Request += RequestEventListener;

        try
        {
            await Task.WhenAny(requestTcs.Task, ClosedTask).WithTimeout(timeout, t =>
                new TimeoutException($"Timeout of {t.TotalMilliseconds} ms exceeded")).ConfigureAwait(false);

            if (ClosedTask.IsFaulted)
            {
                await ClosedTask.ConfigureAwait(false);
            }

            return await requestTcs.Task.ConfigureAwait(false);
        }
        finally
        {
            Request -= RequestEventListener;
        }
    }

    /// <inheritdoc />
    public override async Task<IResponse> WaitForResponseAsync(Func<IResponse, Task<bool>> predicate, WaitForOptions options = null)
    {
        var timeout = options?.Timeout ?? DefaultTimeout;
        var responseTcs = new TaskCompletionSource<IResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

        async void ResponseHandler(object sender, ResponseCreatedEventArgs e)
        {
            try
            {
                if (await predicate(e.Response).ConfigureAwait(false))
                {
                    responseTcs.TrySetResult(e.Response);
                }
            }
            catch (Exception ex)
            {
                responseTcs.TrySetException(ex);
            }
        }

        Response += ResponseHandler;

        try
        {
            await Task.WhenAny(responseTcs.Task, ClosedTask).WithTimeout(timeout, t =>
                new TimeoutException($"Timeout of {t.TotalMilliseconds} ms exceeded")).ConfigureAwait(false);

            if (ClosedTask.IsFaulted)
            {
                await ClosedTask.ConfigureAwait(false);
            }

            return await responseTcs.Task.ConfigureAwait(false);
        }
        finally
        {
            Response -= ResponseHandler;
        }
    }

    /// <inheritdoc />
    public override async Task<FileChooser> WaitForFileChooserAsync(WaitForOptions options = null)
    {
        var timeout = options?.Timeout ?? DefaultTimeout;
        var fileChooserTcs = new TaskCompletionSource<FileChooser>(TaskCreationOptions.RunContinuationsAsynchronously);

        void FileDialogOpenedHandler(object sender, FileDialogOpenedEventArgs e)
        {
            if (e.Element == null)
            {
                return;
            }

            var element = CreateElementHandleFromSharedReference(e.Element);
            var chooser = new FileChooser(element, e.IsMultiple);
            fileChooserTcs.TrySetResult(chooser);
        }

        BidiMainFrame.BrowsingContext.FileDialogOpened += FileDialogOpenedHandler;

        try
        {
            await Task.WhenAny(fileChooserTcs.Task, ClosedTask).WithTimeout(timeout, t =>
                new TimeoutException($"Waiting for `FileChooser` failed: {t.TotalMilliseconds}ms exceeded")).ConfigureAwait(false);

            if (ClosedTask.IsFaulted)
            {
                await ClosedTask.ConfigureAwait(false);
            }

            return await fileChooserTcs.Task.ConfigureAwait(false);
        }
        finally
        {
            BidiMainFrame.BrowsingContext.FileDialogOpened -= FileDialogOpenedHandler;
        }
    }

    /// <inheritdoc />
    public override Task<IResponse> GoBackAsync(NavigationOptions options = null) => GoAsync(-1, options);

    /// <inheritdoc />
    public override Task<IResponse> GoForwardAsync(NavigationOptions options = null) => GoAsync(1, options);

    /// <inheritdoc />
    public override Task SetBurstModeOffAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override async Task<IFrame> WaitForFrameAsync(Func<IFrame, bool> predicate, WaitForOptions options = null)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        var timeout = options?.Timeout ?? DefaultTimeout;
        var frameTcs = new TaskCompletionSource<IFrame>(TaskCreationOptions.RunContinuationsAsynchronously);

        void FrameNavigatedEventListener(object sender, FrameNavigatedEventArgs e)
        {
            if (predicate(e.Frame))
            {
                frameTcs.TrySetResult(e.Frame);
                FrameNavigated -= FrameNavigatedEventListener;
            }
        }

        void FrameAttachedEventListener(object sender, FrameEventArgs e)
        {
            if (predicate(e.Frame))
            {
                frameTcs.TrySetResult(e.Frame);
                FrameAttached -= FrameAttachedEventListener;
            }
        }

        FrameAttached += FrameAttachedEventListener;
        FrameNavigated += FrameNavigatedEventListener;

        var eventRace = Task.WhenAny(frameTcs.Task, ClosedTask).WithTimeout(timeout, t =>
        {
            FrameAttached -= FrameAttachedEventListener;
            FrameNavigated -= FrameNavigatedEventListener;
            return new TimeoutException($"Timeout of {t.TotalMilliseconds} ms exceeded");
        });

        foreach (var frame in Frames)
        {
            if (predicate(frame))
            {
                return frame;
            }
        }

        await eventRace.ConfigureAwait(false);

        if (ClosedTask.IsFaulted)
        {
            await ClosedTask.ConfigureAwait(false);
        }

        return await frameTcs.Task.ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task SetGeolocationAsync(GeolocationOption options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var longitude = options.Longitude;
        var latitude = options.Latitude;
        var accuracy = options.Accuracy;

        if (longitude < -180 || longitude > 180)
        {
            throw new ArgumentException($"Invalid longitude '{longitude}': precondition -180 <= LONGITUDE <= 180 failed.");
        }

        if (latitude < -90 || latitude > 90)
        {
            throw new ArgumentException($"Invalid latitude '{latitude}': precondition -90 <= LATITUDE <= 90 failed.");
        }

        if (accuracy < 0)
        {
            throw new ArgumentException($"Invalid accuracy '{accuracy}': precondition 0 <= ACCURACY failed.");
        }

        var coordinates = new WebDriverBiDi.Emulation.GeolocationCoordinates((double)longitude, (double)latitude)
        {
            Accuracy = (double?)accuracy,
        };

        var commandParameters = new WebDriverBiDi.Emulation.SetGeolocationOverrideCoordinatesCommandParameters
        {
            Coordinates = coordinates,
            Contexts = [BidiMainFrame.BrowsingContext.Id],
        };

        await BidiMainFrame.BrowsingContext.Session.Driver.Emulation.SetGeolocationOverrideAsync(commandParameters).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task SetJavaScriptEnabledAsync(bool enabled)
    {
        var commandParameters = new WebDriverBiDi.Emulation.SetScriptingEnabledCommandParameters(enabled)
        {
            Contexts = [BidiMainFrame.BrowsingContext.Id],
        };

        await BidiMainFrame.BrowsingContext.Session.Driver.Emulation.SetScriptingEnabledAsync(commandParameters).ConfigureAwait(false);
        _isJavaScriptEnabled = enabled;
    }

    /// <inheritdoc />
    public override async Task SetBypassCSPAsync(bool enabled)
    {
        // TODO: handle CDP-specific cases such as MPArch.
        await BidiMainFrame.Client.SendAsync(
            "Page.setBypassCSP",
            new Cdp.Messaging.PageSetBypassCSPRequest { Enabled = enabled }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task SetCacheEnabledAsync(bool enabled = true)
    {
        var commandParameters = new SetCacheBehaviorCommandParameters
        {
            CacheBehavior = enabled ? CacheBehavior.Default : CacheBehavior.Bypass,
            Contexts = [BidiMainFrame.BrowsingContext.Id],
        };

        await BidiMainFrame.BrowsingContext.Session.Driver.Network.SetCacheBehaviorAsync(commandParameters).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task EmulateMediaTypeAsync(MediaType type) => _cdpEmulationManager.EmulateMediaTypeAsync(type);

    /// <inheritdoc />
    public override Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features)
        => _cdpEmulationManager.EmulateMediaFeaturesAsync(features);

    /// <inheritdoc />
    public override async Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentData = null)
    {
        if (!BidiBrowser.CdpSupported && userAgentData != null)
        {
            throw new PuppeteerException("Current Browser does not support `userAgentMetadata`");
        }

        var enable = !string.IsNullOrEmpty(userAgent);

        await BidiMainFrame.BrowsingContext.SetUserAgentAsync(enable ? userAgent : null).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task SetViewportAsync(ViewPortOptions viewport)
    {
        if (viewport == null)
        {
            throw new ArgumentNullException(nameof(viewport));
        }

        if (!BidiBrowser.CdpSupported)
        {
            // Set the screen orientation based on IsLandscape
            var screenOrientation = viewport.IsLandscape
                ? new WebDriverBiDi.Emulation.ScreenOrientation(
                    WebDriverBiDi.Emulation.ScreenOrientationNatural.Landscape,
                    WebDriverBiDi.Emulation.ScreenOrientationType.LandscapePrimary)
                : new WebDriverBiDi.Emulation.ScreenOrientation(
                    WebDriverBiDi.Emulation.ScreenOrientationNatural.Portrait,
                    WebDriverBiDi.Emulation.ScreenOrientationType.PortraitPrimary);

            await Task.WhenAll(
                BidiMainFrame.BrowsingContext.SetViewportAsync(
                    new SetViewportOptions()
                    {
                        Viewport = viewport is { Width: > 0, Height: > 0 }
                            ? new Viewport()
                            {
                                Width = (ulong)viewport.Width,
                                Height = (ulong)viewport.Height,
                            }
                            : null,
                        DevicePixelRatio = viewport.DeviceScaleFactor,
                    }),
                BidiMainFrame.BrowsingContext.SetScreenOrientationOverrideAsync(screenOrientation)).ConfigureAwait(false);
            Viewport = viewport;
            return;
        }

        var needsReload = await _cdpEmulationManager.EmulateViewportAsync(viewport).ConfigureAwait(false);
        Viewport = viewport;

        if (needsReload)
        {
            await ReloadAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override async Task SetCookieAsync(params CookieParam[] cookies)
    {
        if (cookies == null)
        {
            throw new ArgumentNullException(nameof(cookies));
        }

        var pageUrl = Url;
        var pageUrlStartsWithHttp = pageUrl.StartsWith("http", StringComparison.Ordinal);

        foreach (var cookie in cookies)
        {
            var cookieUrl = cookie.Url ?? string.Empty;
            if (string.IsNullOrEmpty(cookieUrl) && pageUrlStartsWithHttp)
            {
                cookieUrl = pageUrl;
            }

            if (cookieUrl == "about:blank")
            {
                throw new PuppeteerException($"Blank page can not have cookie \"{cookie.Name}\"");
            }

            if (cookieUrl.StartsWith("data:", StringComparison.Ordinal))
            {
                throw new PuppeteerException($"Data URL page can not have cookie \"{cookie.Name}\"");
            }

            Uri normalizedUrl = null;
            if (Uri.TryCreate(cookieUrl, UriKind.Absolute, out var parsedUrl))
            {
                normalizedUrl = parsedUrl;
            }

            var domain = cookie.Domain ?? normalizedUrl?.Host;
            if (string.IsNullOrEmpty(domain))
            {
                throw new MessageException("At least one of the url and domain needs to be specified");
            }

            // Automatically set secure flag for HTTPS URLs if not explicitly provided
            var cookieToUse = cookie;
            if (!cookie.Secure.HasValue && normalizedUrl?.Scheme == "https")
            {
                // Create a copy to avoid mutating the input parameter
                cookieToUse = new CookieParam
                {
                    Name = cookie.Name,
                    Value = cookie.Value,
                    Url = cookie.Url,
                    Domain = cookie.Domain,
                    Path = cookie.Path,
                    Secure = true,
                    HttpOnly = cookie.HttpOnly,
                    Expires = cookie.Expires,
                    SameSite = cookie.SameSite,
                    PartitionKey = cookie.PartitionKey,
                };
            }

            var bidiCookie = BidiCookieHelper.PuppeteerToBidiCookie(cookieToUse, domain);

            if (!string.IsNullOrEmpty(cookie.PartitionKey))
            {
                var userContext = ((BidiBrowserContext)BrowserContext).UserContext;
                await BidiBrowser.Driver.Storage.SetCookieAsync(new WebDriverBiDi.Storage.SetCookieCommandParameters(bidiCookie)
                {
                    Partition = new WebDriverBiDi.Storage.StorageKeyPartitionDescriptor
                    {
                        SourceOrigin = cookie.PartitionKey,
                        UserContextId = userContext.Id,
                    },
                }).ConfigureAwait(false);
            }
            else
            {
                await BidiBrowser.Driver.Storage.SetCookieAsync(new WebDriverBiDi.Storage.SetCookieCommandParameters(bidiCookie)
                {
                    Partition = new WebDriverBiDi.Storage.BrowsingContextPartitionDescriptor(BidiMainFrame.BrowsingContext.Id),
                }).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc />
    public override async Task CloseAsync(PageCloseOptions options = null)
    {
        try
        {
            await BidiMainFrame.BrowsingContext.CloseAsync(options?.RunBeforeUnload).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Swallow.
        }
    }

    /// <inheritdoc />
    public override async Task DeleteCookieAsync(params CookieParam[] cookies)
    {
        await Task.WhenAll(cookies.Select(async cookie =>
        {
            var cookieUrl = cookie.Url ?? Url;
            Uri normalizedUrl = null;
            if (Uri.TryCreate(cookieUrl, UriKind.Absolute, out var parsedUrl))
            {
                normalizedUrl = parsedUrl;
            }

            var domain = cookie.Domain ?? normalizedUrl?.Host;
            if (string.IsNullOrEmpty(domain))
            {
                throw new MessageException("At least one of the url and domain needs to be specified");
            }

            var filter = new WebDriverBiDi.Storage.CookieFilter
            {
                Domain = domain,
                Name = cookie.Name,
            };

            if (!string.IsNullOrEmpty(cookie.Path))
            {
                filter.Path = cookie.Path;
            }

            await BidiBrowser.Driver.Storage.DeleteCookiesAsync(new WebDriverBiDi.Storage.DeleteCookiesCommandParameters
            {
                Filter = filter,
                Partition = new WebDriverBiDi.Storage.BrowsingContextPartitionDescriptor(BidiMainFrame.BrowsingContext.Id),
            }).ConfigureAwait(false);
        })).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task SetDragInterceptionAsync(bool enabled) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<Dictionary<string, decimal>> MetricsAsync()
        => throw new NotSupportedException("Metrics is not supported in BiDi protocol");

    /// <inheritdoc />
    public override Task CaptureHeapSnapshotAsync(HeapSnapshotOptions options)
        => throw new NotSupportedException("CaptureHeapSnapshot is not supported in BiDi protocol");

    /// <inheritdoc />
    public override async Task<NewDocumentScriptEvaluation> EvaluateFunctionOnNewDocumentAsync(string pageFunction, params object[] args)
    {
        var expression = EvaluationExpression(pageFunction, args);
        var commandParameters = new WebDriverBiDi.Script.AddPreloadScriptCommandParameters(expression)
        {
            Contexts = [BidiMainFrame.BrowsingContext.Id],
        };

        var result = await BidiMainFrame.BrowsingContext.Session.Driver.Script.AddPreloadScriptAsync(commandParameters).ConfigureAwait(false);
        return new NewDocumentScriptEvaluation(result.PreloadScriptId);
    }

    /// <inheritdoc />
    public override Task RemoveExposedFunctionAsync(string name)
        => BidiMainFrame.RemoveExposedFunctionAsync(name);

    /// <inheritdoc />
    public override async Task RemoveScriptToEvaluateOnNewDocumentAsync(string identifier)
    {
        var commandParameters = new WebDriverBiDi.Script.RemovePreloadScriptCommandParameters(identifier);
        await BidiMainFrame.BrowsingContext.Session.Driver.Script.RemovePreloadScriptAsync(commandParameters).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task SetBypassServiceWorkerAsync(bool bypass) => throw new NotImplementedException();

    /// <inheritdoc />
    public override async Task<NewDocumentScriptEvaluation> EvaluateExpressionOnNewDocumentAsync(string expression)
    {
        // Wrap the expression in a function so it can be used as a preload script
        var functionExpression = $"() => {{{expression}}}";
        var commandParameters = new WebDriverBiDi.Script.AddPreloadScriptCommandParameters(functionExpression)
        {
            Contexts = [BidiMainFrame.BrowsingContext.Id],
        };

        var result = await BidiMainFrame.BrowsingContext.Session.Driver.Script.AddPreloadScriptAsync(commandParameters).ConfigureAwait(false);
        return new NewDocumentScriptEvaluation(result.PreloadScriptId);
    }

    /// <inheritdoc />
    public override Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle) => throw new NotImplementedException();

    /// <inheritdoc />
    public override async Task SetRequestInterceptionAsync(bool value)
    {
        _requestInterception = await ToggleInterceptionAsync(
            [InterceptPhase.BeforeRequestSent],
            _requestInterception,
            value).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task SetOfflineModeAsync(bool value)
    {
        if (!BidiBrowser.CdpSupported)
        {
            throw new NotSupportedException();
        }

        _emulatedNetworkConditions ??= new InternalNetworkConditions
        {
            Offline = false,
            Upload = -1,
            Download = -1,
            Latency = 0,
        };

        _emulatedNetworkConditions.Offline = value;
        await ApplyNetworkConditionsAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions)
    {
        if (!BidiBrowser.CdpSupported)
        {
            // WebDriver BiDi supports only offline mode
            if (networkConditions != null &&
                !networkConditions.Offline &&
                (networkConditions.Upload >= 0 ||
                 networkConditions.Download >= 0 ||
                 networkConditions.Latency > 0))
            {
                throw new PuppeteerException("Network throttling is not supported in this browser with WebDriver BiDi");
            }

            await BidiMainFrame.BrowsingContext.SetOfflineModeAsync(networkConditions?.Offline ?? false).ConfigureAwait(false);
            return;
        }

        _emulatedNetworkConditions ??= new InternalNetworkConditions
        {
            Offline = networkConditions?.Offline ?? false,
            Upload = -1,
            Download = -1,
            Latency = 0,
        };

        _emulatedNetworkConditions.Upload = networkConditions?.Upload ?? -1;
        _emulatedNetworkConditions.Download = networkConditions?.Download ?? -1;
        _emulatedNetworkConditions.Latency = networkConditions?.Latency ?? 0;
        _emulatedNetworkConditions.Offline = networkConditions?.Offline ?? false;
        await ApplyNetworkConditionsAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<CookieParam[]> GetCookiesAsync(params string[] urls)
    {
        if (urls == null)
        {
            throw new ArgumentNullException(nameof(urls));
        }

        var normalizedUrls = (urls.Length > 0 ? urls : [Url]).Select(url => new Uri(url)).ToArray();

        var result = await BidiBrowser.Driver.Storage.GetCookiesAsync(new WebDriverBiDi.Storage.GetCookiesCommandParameters
        {
            Partition = new WebDriverBiDi.Storage.BrowsingContextPartitionDescriptor(BidiMainFrame.BrowsingContext.Id),
        }).ConfigureAwait(false);

        return result.Cookies
            .Select(BidiCookieHelper.BidiToPuppeteerCookie)
            .Where(cookie => normalizedUrls.Any(url => BidiCookieHelper.TestUrlMatchCookie(cookie, url)))
            .ToArray();
    }

    internal static BidiPage From(BidiBrowserContext browserContext, BrowsingContext browsingContext)
    {
        var page = new BidiPage(browserContext, browsingContext);
        page.Initialize();
        return page;
    }

    internal new void OnPageError(PageErrorEventArgs e) => base.OnPageError(e);

    internal new void OnConsole(ConsoleEventArgs e) => base.OnConsole(e);

    internal void OnWorkerCreated(BidiWebWorker worker)
    {
        _workers[worker.RealmId] = worker;
        base.OnWorkerCreated(worker);
    }

    internal void OnWorkerDestroyed(BidiWebWorker worker)
    {
        _workers.TryRemove(worker.RealmId, out var _);
        base.OnWorkerDestroyed(worker);
    }

    /// <summary>
    /// Attempts to mark a URL as having fired the RequestServedFromCache event.
    /// Returns true if this is the first time marking the URL, false if already marked.
    /// This is used to deduplicate Firefox BiDi's duplicate BeforeRequestSent events.
    /// </summary>
    internal bool TryMarkCacheEventFired(string url) => _cacheEventsFired.TryAdd(url, true);

    /// <summary>
    /// Attempts to mark a URL as having fired the RequestFailed event.
    /// Returns true if this is the first time marking the URL, false if already marked.
    /// This is used to deduplicate Firefox BiDi's duplicate BeforeRequestSent events.
    /// </summary>
    internal bool TryMarkFailedEventFired(string url) => _failedEventsFired.TryAdd(url, true);

    /// <summary>
    /// Attempts to mark a URL as having fired the Response event.
    /// Returns true if this is the first time marking the URL, false if already marked.
    /// This is used to deduplicate Firefox BiDi's duplicate BeforeRequestSent events.
    /// Only deduplicates during navigation.
    /// </summary>
    internal bool TryMarkResponseEventFired(string url)
    {
        // Only deduplicate during navigation. After navigation, allow all responses.
        if (!_isNavigating)
        {
            return true;
        }

        return _responseEventsFired.TryAdd(url, true);
    }

    /// <summary>
    /// Attempts to mark a URL as having fired the Request event.
    /// Returns true if this is the first time marking the URL, false if already marked.
    /// This is used to deduplicate Firefox BiDi's duplicate BeforeRequestSent events.
    /// Only deduplicates during navigation. After navigation completes, all requests are allowed.
    /// </summary>
    internal bool TryMarkRequestEventFired(string url)
    {
        // Only deduplicate during navigation. After navigation, allow all requests.
        if (!_isNavigating)
        {
            return true;
        }

        return _requestEventsFired.TryAdd(url, true);
    }

    /// <summary>
    /// Clears the event deduplication trackers and starts navigation mode.
    /// Called on navigation start to allow fresh events with deduplication.
    /// </summary>
    internal void ClearEventTrackers()
    {
        _cacheEventsFired.Clear();
        _failedEventsFired.Clear();
        _responseEventsFired.Clear();
        _requestEventsFired.Clear();
        _isNavigating = true;
    }

    /// <summary>
    /// Ends navigation mode and clears the request event tracker.
    /// Called after navigation completes to allow subsequent fetch/XHR calls
    /// to the same URLs without being filtered as duplicates.
    /// </summary>
    internal void EndNavigationAndClearTracker()
    {
        _isNavigating = false;
        _requestEventsFired.Clear();
    }

    /// <inheritdoc />
    protected override async Task<byte[]> PdfInternalAsync(string file, PdfOptions options)
    {
        Debug.Assert(options != null, nameof(options) + " != null");

        // BiDi uses centimeters, PaperFormat is in inches
        // 1 inch = 2.54 cm
        const double inchesToCm = 2.54;

        // Default to Letter size (8.5 x 11 inches = 21.59 x 27.94 cm)
        var paperWidth = (double)PaperFormat.Letter.Width * inchesToCm;
        var paperHeight = (double)PaperFormat.Letter.Height * inchesToCm;

        if (options.Format != null)
        {
            paperWidth = (double)options.Format.Width * inchesToCm;
            paperHeight = (double)options.Format.Height * inchesToCm;
        }
        else
        {
            if (options.Width != null)
            {
                paperWidth = ConvertPrintParameterToCentimeters(options.Width);
            }

            if (options.Height != null)
            {
                paperHeight = ConvertPrintParameterToCentimeters(options.Height);
            }
        }

        var marginTop = ConvertPrintParameterToCentimeters(options.MarginOptions.Top);
        var marginLeft = ConvertPrintParameterToCentimeters(options.MarginOptions.Left);
        var marginBottom = ConvertPrintParameterToCentimeters(options.MarginOptions.Bottom);
        var marginRight = ConvertPrintParameterToCentimeters(options.MarginOptions.Right);

        // Wait for fonts to be ready
        await BidiMainFrame.IsolatedRealm.EvaluateExpressionAsync("document.fonts.ready")
            .WithTimeout(TimeoutSettings.Timeout).ConfigureAwait(false);

        var pageRanges = new List<object>();
        if (!string.IsNullOrEmpty(options.PageRanges))
        {
            foreach (var range in options.PageRanges.Split(','))
            {
                pageRanges.Add(range.Trim());
            }
        }

        var printOptions = new PrintCommandParameters(BidiMainFrame.BrowsingContext.Id)
        {
            Background = options.PrintBackground,
            Orientation = options.Landscape ? PrintOrientation.Landscape : PrintOrientation.Portrait,
            Page = new PrintPageParameters
            {
                Width = paperWidth,
                Height = paperHeight,
            },
            Margins = new PrintMarginParameters
            {
                Top = marginTop,
                Bottom = marginBottom,
                Left = marginLeft,
                Right = marginRight,
            },
            Scale = (double)options.Scale,
            ShrinkToFit = !options.PreferCSSPageSize,
        };

        if (pageRanges.Count > 0)
        {
            printOptions.PageRanges.AddRange(pageRanges);
        }

        var base64Data = await BidiMainFrame.BrowsingContext.PrintAsync(printOptions).ConfigureAwait(false);
        var data = Convert.FromBase64String(base64Data);

        if (!string.IsNullOrEmpty(file))
        {
            using var fs = AsyncFileHelper.CreateStream(file, System.IO.FileMode.Create);
            await fs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
        }

        return data;
    }

    /// <inheritdoc />
    protected override async Task<string> PerformScreenshotAsync(ScreenshotType type, ScreenshotOptions options)
    {
        Debug.Assert(options != null, nameof(options) + " != null");

        if (options.OmitBackground)
        {
            throw new PuppeteerException("BiDi does not support 'omitBackground'.");
        }

        if (options.OptimizeForSpeed == true)
        {
            throw new PuppeteerException("BiDi does not support 'optimizeForSpeed'.");
        }

        if (options.FromSurface == true)
        {
            throw new PuppeteerException("BiDi does not support 'fromSurface'.");
        }

        if (options.Clip != null && options.Clip.Scale != 1)
        {
            throw new PuppeteerException("BiDi does not support 'scale' in 'clip'.");
        }

        BoundingBox box = null;
        if (options.Clip != null)
        {
            if (options.CaptureBeyondViewport)
            {
                // Create a new box to avoid reference issues
                box = new Clip
                {
                    X = options.Clip.X,
                    Y = options.Clip.Y,
                    Width = options.Clip.Width,
                    Height = options.Clip.Height,
                };
            }
            else
            {
                // The clip is always with respect to the document coordinates, so we
                // need to convert this to viewport coordinates when we aren't capturing
                // beyond the viewport.
                var points = await EvaluateFunctionAsync<decimal[]>(@"() => {
                    if (!window.visualViewport) {
                        throw new Error('window.visualViewport is not supported.');
                    }
                    return [
                        window.visualViewport.pageLeft,
                        window.visualViewport.pageTop,
                    ];
                }").ConfigureAwait(false);

                box = new Clip
                {
                    X = options.Clip.X - points[0],
                    Y = options.Clip.Y - points[1],
                    Width = options.Clip.Width,
                    Height = options.Clip.Height,
                };
            }
        }

        var fileType = options.Type switch
        {
            ScreenshotType.Jpeg => "jpg",
            ScreenshotType.Png => "png",
            ScreenshotType.Webp => "webp",
            _ => "png",
        };

        var parameters = new ScreenshotParameters()
        {
            Origin = options.CaptureBeyondViewport ? ScreenshotOrigin.Document : ScreenshotOrigin.Viewport,
            Format = new ImageFormat()
            {
                Type = $"image/{fileType}",
                Quality = options.Quality / 100,
            },
        };

        if (box != null)
        {
            parameters.Clip = new BoxClipRectangle
            {
                X = (double)box.X,
                Y = (double)box.Y,
                Width = (double)box.Width,
                Height = (double)box.Height,
            };
        }

        var data = await BidiMainFrame.BrowsingContext.CaptureScreenshotAsync(parameters).ConfigureAwait(false);

        return data;
    }

    /// <inheritdoc />
    protected override Task ExposeFunctionAsync(string name, Delegate puppeteerFunction)
        => BidiMainFrame.ExposeFunctionAsync(name, puppeteerFunction);

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _requestInterceptionQueue.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Creates an evaluation expression for preload scripts.
    /// Similar to upstream's evaluationExpression function which wraps the function
    /// so it can be executed as a preload script.
    /// </summary>
    private static double ConvertPrintParameterToCentimeters(object parameter)
    {
        if (parameter == null)
        {
            return 0;
        }

        // Conversion factors to pixels
        const double pxPerInch = 96;
        const double pxPerCm = 37.8;
        const double pxPerMm = 3.78;

        double pixels;
        if (parameter is decimal or int or double or float)
        {
            // Treat numbers as pixel values
            pixels = Convert.ToDouble(parameter, System.Globalization.CultureInfo.InvariantCulture);
        }
        else
        {
            var text = parameter.ToString();
            var unit = text.Length > 2 ? text.Substring(text.Length - 2).ToLowerInvariant() : string.Empty;
            string valueText;

            double? pxPerUnit = unit switch
            {
                "px" => 1,
                "in" => pxPerInch,
                "cm" => pxPerCm,
                "mm" => pxPerMm,
                _ => null,
            };

            if (pxPerUnit.HasValue)
            {
                valueText = text.Substring(0, text.Length - 2);
            }
            else
            {
                // Unknown unit, treat as pixels
                pxPerUnit = 1;
                valueText = text;
            }

            if (double.TryParse(valueText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var number))
            {
                pixels = number * pxPerUnit.Value;
            }
            else
            {
                throw new ArgumentException($"Failed to parse parameter value: '{text}'", nameof(parameter));
            }
        }

        // Convert pixels to centimeters
        return pixels / pxPerCm;
    }

    private static string EvaluationExpression(string fun, params object[] args)
    {
        return $"() => {{{BindingUtils.EvaluationString(fun, args)}}}";
    }

    private async Task<IResponse> GoAsync(int delta, NavigationOptions options)
    {
        var waitForNavigationTask = WaitForNavigationAsync(options);
        var navigationTask = BidiMainFrame.BrowsingContext.TraverseHistoryAsync(delta);

        try
        {
            await Task.WhenAll(waitForNavigationTask, navigationTask).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("no such history entry"))
            {
                return null;
            }

            throw new NavigationException(ex.Message, ex);
        }

        return waitForNavigationTask.Result;
    }

    private async Task ApplyNetworkConditionsAsync()
    {
        if (_emulatedNetworkConditions == null)
        {
            return;
        }

        await BidiMainFrame.Client.SendAsync(
            "Network.emulateNetworkConditions",
            new Cdp.Messaging.NetworkEmulateNetworkConditionsRequest
            {
                Offline = _emulatedNetworkConditions.Offline,
                Latency = _emulatedNetworkConditions.Latency,
                UploadThroughput = _emulatedNetworkConditions.Upload,
                DownloadThroughput = _emulatedNetworkConditions.Download,
            }).ConfigureAwait(false);
    }

    private async Task<string> ToggleInterceptionAsync(
        InterceptPhase[] phases,
        string interception,
        bool expected)
    {
        if (expected && interception == null)
        {
            var options = new AddInterceptCommandParameters();
            foreach (var phase in phases)
            {
                options.Phases.Add(phase);
            }

            return await BidiMainFrame.BrowsingContext.AddInterceptAsync(options).ConfigureAwait(false);
        }
        else if (!expected && interception != null)
        {
            await BidiMainFrame.BrowsingContext.UserContext.Browser.RemoveInterceptAsync(interception).ConfigureAwait(false);
            return null;
        }

        return interception;
    }

    private BidiElementHandle CreateElementHandleFromSharedReference(SharedReference sharedReference)
    {
        // Create a RemoteValue from the SharedReference using JSON deserialization
        var json = JsonSerializer.Serialize(new
        {
            type = "node",
            sharedId = sharedReference.SharedId,
            handle = sharedReference.Handle,
        });

        var remoteValue = JsonSerializer.Deserialize<RemoteValue>(json);
        return new BidiElementHandle(remoteValue, (BidiFrameRealm)BidiMainFrame.MainRealm);
    }

    private void Initialize()
    {
        BidiMainFrame.BrowsingContext.Closed += (_, _) =>
        {
            OnClose();
            IsClosed = true;
        };

        // Track workers
        WorkerCreated += (_, e) =>
        {
            if (e.Worker is BidiWebWorker worker)
            {
                _workers[worker.RealmId] = worker;
            }
        };

        WorkerDestroyed += (_, e) =>
        {
            if (e.Worker is BidiWebWorker worker)
            {
                _workers.TryRemove(worker.RealmId, out var _);
            }
        };
    }
}

#endif
