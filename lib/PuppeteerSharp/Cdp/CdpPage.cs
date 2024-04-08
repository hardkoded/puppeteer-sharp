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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Cdp.Messaging.Protocol.Network;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Media;
using PuppeteerSharp.PageAccessibility;
using PuppeteerSharp.PageCoverage;
using StackTrace = PuppeteerSharp.Cdp.Messaging.StackTrace;
using Timer = System.Timers.Timer;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc />
public class CdpPage : Page
{
    private static readonly Dictionary<string, decimal> _unitToPixels = new()
    {
        ["px"] = 1,
        ["in"] = 96,
        ["cm"] = 37.8m,
        ["mm"] = 3.78m,
    };

    private readonly ConcurrentDictionary<string, CdpWebWorker> _workers = new();
    private readonly ITargetManager _targetManager;
    private readonly EmulationManager _emulationManager;
    private readonly ILogger _logger;
    private readonly Task _closedFinishedTask;
    private readonly ConcurrentDictionary<string, Binding> _bindings = new();
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<FileChooser>> _fileChooserInterceptors = new();
    private readonly ConcurrentDictionary<string, string> _exposedFunctions = new();
    private TaskCompletionSource<bool> _sessionClosedTcs;

    private CdpPage(
        CdpCDPSession client,
        CdpTarget target,
        TaskQueue screenshotTaskQueue,
        bool ignoreHTTPSErrors) : base(screenshotTaskQueue)
    {
        PrimaryTargetClient = client;
        TabTargetClient = (CdpCDPSession)client.ParentSession;
        TabTarget = (CdpTarget)TabTargetClient.Target;
        PrimaryTarget = target;
        _targetManager = target.TargetManager;
        Keyboard = new CdpKeyboard(client);
        Mouse = new CdpMouse(client, Keyboard);
        Touchscreen = new CdpTouchscreen(client, Keyboard);
        Tracing = new Tracing(client);
        Coverage = new Coverage(client);

        _emulationManager = new EmulationManager(client);
        _logger = Client.Connection.LoggerFactory.CreateLogger<Page>();
        FrameManager = new FrameManager(client, this, ignoreHTTPSErrors, TimeoutSettings);
        Accessibility = new Accessibility(client);

        FrameManager.FrameAttached += (_, e) => OnFrameAttached(e);
        FrameManager.FrameDetached += (_, e) => OnFrameDetached(e);
        FrameManager.FrameNavigated += (_, e) => OnFrameNavigated(e);

        FrameManager.NetworkManager.Request += (_, e) => OnRequest(e.Request);
        FrameManager.NetworkManager.RequestFailed += (_, e) => OnRequestFailed(e);
        FrameManager.NetworkManager.Response += (_, e) => OnResponse(e);
        FrameManager.NetworkManager.RequestFinished += (_, e) => OnRequestFinished(e);
        FrameManager.NetworkManager.RequestServedFromCache += (_, e) => OnRequestServedFromCache(e);

        TabTargetClient.Swapped += (sender, args) => _ = OnActivationAsync(args.Session as CdpCDPSession);
        TabTargetClient.Ready += (sender, args) => _ = OnSecondaryTargetAsync(args.Session as CdpCDPSession);
        _targetManager.TargetGone += OnDetachedFromTarget;

        _closedFinishedTask = TabTarget.CloseTask.ContinueWith(
            _ =>
            {
                try
                {
                    TabTarget.TargetManager.TargetGone -= OnDetachedFromTarget;
                    OnClose();
                }
                finally
                {
                    IsClosed = true;
                }
            },
            TaskScheduler.Default);

        SetupPrimaryTargetListeners();
    }

    /// <inheritdoc cref="CDPSession"/>
    public override CDPSession Client => PrimaryTargetClient;

    /// <inheritdoc cref="CDPSession"/>
    public override Target Target => PrimaryTarget;

    /// <inheritdoc/>
    public override IFrame MainFrame => FrameManager.MainFrame;

    /// <inheritdoc/>
    public override IFrame[] Frames => FrameManager.GetFrames();

    /// <inheritdoc/>
    public override WebWorker[] Workers => _workers.Values.ToArray();

    /// <inheritdoc/>
    public override IBrowserContext BrowserContext => PrimaryTarget.BrowserContext;

    /// <inheritdoc/>
    public override bool IsJavaScriptEnabled => _emulationManager.JavascriptEnabled;

    /// <inheritdoc />
    protected override Browser Browser => PrimaryTarget.Browser;

    private CdpCDPSession PrimaryTargetClient { get; set; }

    private CdpTarget PrimaryTarget { get; set; }

    private CdpCDPSession TabTargetClient { get; }

    private CdpTarget TabTarget { get; }

    private Task SessionClosedTask
    {
        get
        {
            if (_sessionClosedTcs == null)
            {
                _sessionClosedTcs =
                    new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                Client.Disconnected += ClientDisconnected;

                void ClientDisconnected(object sender, EventArgs e)
                {
                    _sessionClosedTcs.TrySetException(new TargetClosedException("Target closed", "Session closed"));
                    Client.Disconnected -= ClientDisconnected;
                }
            }

            return _sessionClosedTcs.Task;
        }
    }

    private FrameManager FrameManager { get; set; }

    /// <inheritdoc/>
    public override Task SetGeolocationAsync(GeolocationOption options)
        => _emulationManager.SetGeolocationAsync(options);

    /// <inheritdoc/>
    public override Task SetDragInterceptionAsync(bool enabled)
    {
        IsDragInterceptionEnabled = enabled;
        return PrimaryTargetClient.SendAsync(
            "Input.setInterceptDrags",
            new InputSetInterceptDragsRequest { Enabled = enabled });
    }

    /// <inheritdoc/>
    public override async Task<CookieParam[]> GetCookiesAsync(params string[] urls)
    {
        if (urls == null)
        {
            throw new ArgumentNullException(nameof(urls));
        }

        return (await PrimaryTargetClient.SendAsync<NetworkGetCookiesResponse>(
                "Network.getCookies",
                new NetworkGetCookiesRequest { Urls = urls.Length > 0 ? urls : [Url], })
            .ConfigureAwait(false)).Cookies;
    }

    /// <inheritdoc/>
    public override async Task SetCookieAsync(params CookieParam[] cookies)
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
            await PrimaryTargetClient
                .SendAsync("Network.setCookies", new NetworkSetCookiesRequest { Cookies = cookies, })
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override async Task RemoveExposedFunctionAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (!_exposedFunctions.TryRemove(name, out var exposedFun) && !_bindings.TryRemove(name, out _))
        {
            throw new PuppeteerException(
                $"Failed to remove page binding with name {name}: window['{name}'] does not exists!");
        }

        await Client.SendAsync("Runtime.removeBinding", new RuntimeRemoveBindingRequest { Name = name, })
            .ConfigureAwait(false);

        await RemoveScriptToEvaluateOnNewDocumentAsync(exposedFun).ConfigureAwait(false);

        await Task.WhenAll(
            Frames.Select(frame =>
                {
                    // If a frame has not started loading, it might never start. Rely on
                    // addScriptToEvaluateOnNewDocument in that case.
                    if (frame != MainFrame && !((Frame)frame).HasStartedLoading)
                    {
                        return Task.CompletedTask;
                    }

                    return frame.EvaluateFunctionAsync("name => globalThis[name] = undefined", name);
                })
                .ToArray()).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task DeleteCookieAsync(params CookieParam[] cookies)
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

            await PrimaryTargetClient.SendAsync("Network.deleteCookies", cookie).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override async Task<Dictionary<string, decimal>> MetricsAsync()
    {
        var response = await Client.SendAsync<PerformanceGetMetricsResponse>("Performance.getMetrics")
            .ConfigureAwait(false);
        return BuildMetricsObject(response.Metrics);
    }

    /// <inheritdoc/>
    public override async Task<NewDocumentScriptEvaluation> EvaluateFunctionOnNewDocumentAsync(
        string pageFunction,
        params object[] args)
    {
        var source = BindingUtils.EvaluationString(pageFunction, args);
        var documentIdentifier = await Client
            .SendAsync<PageAddScriptToEvaluateOnNewDocumentResponse>(
                "Page.addScriptToEvaluateOnNewDocument",
                new PageAddScriptToEvaluateOnNewDocumentRequest { Source = source, }).ConfigureAwait(false);

        return new NewDocumentScriptEvaluation(documentIdentifier.Identifier);
    }

    /// <inheritdoc/>
    public override Task RemoveScriptToEvaluateOnNewDocumentAsync(string identifier)
        => Client.SendAsync("Page.removeScriptToEvaluateOnNewDocument", new PageRemoveScriptToEvaluateOnNewDocumentRequest
        {
            Identifier = identifier,
        });

    /// <inheritdoc />
    public override Task SetBypassServiceWorkerAsync(bool bypass)
    {
        IsServiceWorkerBypassed = bypass;
        return Client.SendAsync("Network.setBypassServiceWorker", new SetBypassServiceWorkerRequest
        {
            Bypass = bypass,
        });
    }

    /// <inheritdoc/>
    public override async Task<NewDocumentScriptEvaluation> EvaluateExpressionOnNewDocumentAsync(string expression)
    {
        var documentIdentifier = await
            Client.SendAsync<PageAddScriptToEvaluateOnNewDocumentResponse>(
                "Page.addScriptToEvaluateOnNewDocument",
                new PageAddScriptToEvaluateOnNewDocumentRequest { Source = expression, }).ConfigureAwait(false);

        return new NewDocumentScriptEvaluation(documentIdentifier.Identifier);
    }

    /// <inheritdoc/>
    public override async Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle)
    {
        if (prototypeHandle == null)
        {
            throw new ArgumentNullException(nameof(prototypeHandle));
        }

        if (prototypeHandle.Disposed)
        {
            throw new PuppeteerException("Prototype JSHandle is disposed!");
        }

        if (prototypeHandle.RemoteObject.ObjectId == null)
        {
            throw new PuppeteerException("Prototype JSHandle must not be referencing primitive value");
        }

        var response = await Client.SendAsync<RuntimeQueryObjectsResponse>(
                "Runtime.queryObjects",
                new RuntimeQueryObjectsRequest { PrototypeObjectId = prototypeHandle.RemoteObject.ObjectId, })
            .ConfigureAwait(false);

        var context = await FrameManager.MainFrame.MainWorld.GetExecutionContextAsync().ConfigureAwait(false);
        return context.CreateJSHandle(response.Objects);
    }

    /// <inheritdoc/>
    public override Task SetRequestInterceptionAsync(bool value)
        => FrameManager.NetworkManager.SetRequestInterceptionAsync(value);

    /// <inheritdoc/>
    public override Task SetOfflineModeAsync(bool value) => FrameManager.NetworkManager.SetOfflineModeAsync(value);

    /// <inheritdoc/>
    public override Task SetJavaScriptEnabledAsync(bool enabled)
        => _emulationManager.SetJavaScriptEnabledAsync(enabled);

    /// <inheritdoc/>
    public override Task SetBypassCSPAsync(bool enabled) => PrimaryTargetClient.SendAsync(
        "Page.setBypassCSP",
        new PageSetBypassCSPRequest { Enabled = enabled, });

    /// <inheritdoc/>
    public override Task EmulateMediaTypeAsync(MediaType type)
        => _emulationManager.EmulateMediaTypeAsync(type);

    /// <inheritdoc/>
    public override Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features)
        => _emulationManager.EmulateMediaFeaturesAsync(features);

    /// <inheritdoc/>
    public override async Task SetViewportAsync(ViewPortOptions viewport)
    {
        if (viewport == null)
        {
            throw new ArgumentNullException(nameof(viewport));
        }

        var needsReload = await _emulationManager.EmulateViewportAsync(viewport).ConfigureAwait(false);
        Viewport = viewport;

        if (needsReload)
        {
            await ReloadAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions)
        => FrameManager.NetworkManager.EmulateNetworkConditionsAsync(networkConditions);

    /// <inheritdoc/>
    public override Task SetCacheEnabledAsync(bool enabled = true)
        => FrameManager.NetworkManager.SetCacheEnabledAsync(enabled);

    /// <inheritdoc/>
    public override Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentData = null)
        => FrameManager.NetworkManager.SetUserAgentAsync(userAgent, userAgentData);

    /// <inheritdoc/>
    public override async Task<IResponse> ReloadAsync(NavigationOptions options)
    {
        var navigationTask = WaitForNavigationAsync(options);

        await Task.WhenAll(
                navigationTask,
                PrimaryTargetClient.SendAsync("Page.reload", new PageReloadRequest { FrameId = MainFrame.Id }))
            .ConfigureAwait(false);

        return navigationTask.Result;
    }

    /// <inheritdoc/>
    public override async Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions options = null)
    {
        var timeout = options?.Timeout ?? DefaultTimeout;
        var idleTime = options?.IdleTime ?? 500;

        var networkIdleTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var idleTimer = new Timer { Interval = idleTime, };

        idleTimer.Elapsed += (_, _) => { networkIdleTcs.TrySetResult(true); };

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

        await Task.WhenAny(networkIdleTcs.Task, SessionClosedTask).WithTimeout(timeout, t =>
        {
            Cleanup();

            return new TimeoutException($"Timeout of {t.TotalMilliseconds} ms exceeded");
        }).ConfigureAwait(false);

        Cleanup();

        if (SessionClosedTask.IsFaulted)
        {
            await SessionClosedTask.ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override async Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions options = null)
    {
        var timeout = options?.Timeout ?? DefaultTimeout;
        var requestTcs = new TaskCompletionSource<IRequest>(TaskCreationOptions.RunContinuationsAsynchronously);

        void RequestEventListener(object sender, RequestEventArgs e)
        {
            if (predicate(e.Request))
            {
                requestTcs.TrySetResult(e.Request);
                FrameManager.NetworkManager.Request -= RequestEventListener;
            }
        }

        FrameManager.NetworkManager.Request += RequestEventListener;

        await Task.WhenAny(requestTcs.Task, SessionClosedTask).WithTimeout(timeout, t =>
        {
            FrameManager.NetworkManager.Request -= RequestEventListener;
            return new TimeoutException($"Timeout of {t.TotalMilliseconds} ms exceeded");
        }).ConfigureAwait(false);

        if (SessionClosedTask.IsFaulted)
        {
            await SessionClosedTask.ConfigureAwait(false);
        }

        return await requestTcs.Task.ConfigureAwait(false);
    }

    /// <inheritdoc/>
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
                FrameManager.FrameNavigated -= FrameNavigatedEventListener;
            }
        }

        void FrameAttachedEventListener(object sender, FrameEventArgs e)
        {
            if (predicate(e.Frame))
            {
                frameTcs.TrySetResult(e.Frame);
                FrameManager.FrameAttached -= FrameAttachedEventListener;
            }
        }

        FrameManager.FrameAttached += FrameAttachedEventListener;
        FrameManager.FrameNavigated += FrameNavigatedEventListener;

        var eventRace = Task.WhenAny(frameTcs.Task, SessionClosedTask).WithTimeout(timeout, t =>
        {
            FrameManager.FrameAttached -= FrameAttachedEventListener;
            FrameManager.FrameNavigated -= FrameNavigatedEventListener;
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

        if (SessionClosedTask.IsFaulted)
        {
            await SessionClosedTask.ConfigureAwait(false);
        }

        return await frameTcs.Task.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override Task BringToFrontAsync() => PrimaryTargetClient.SendAsync("Page.bringToFront");

    /// <inheritdoc/>
    public override Task EmulateVisionDeficiencyAsync(VisionDeficiency type)
        => _emulationManager.EmulateVisionDeficiencyAsync(type);

    /// <inheritdoc/>
    public override Task EmulateTimezoneAsync(string timezoneId)
        => _emulationManager.EmulateTimezoneAsync(timezoneId);

    /// <inheritdoc/>
    public override Task EmulateIdleStateAsync(EmulateIdleOverrides overrides = null)
        => _emulationManager.EmulateIdleStateAsync(overrides);

    /// <inheritdoc/>
    public override Task EmulateCPUThrottlingAsync(decimal? factor = null)
        => _emulationManager.EmulateCPUThrottlingAsync(factor);

    /// <inheritdoc/>
    public override Task<IResponse> GoBackAsync(NavigationOptions options = null) => GoAsync(-1, options);

    /// <inheritdoc/>
    public override Task<IResponse> GoForwardAsync(NavigationOptions options = null) => GoAsync(1, options);

    /// <inheritdoc/>
    public override async Task<IResponse> WaitForResponseAsync(
        Func<IResponse, Task<bool>> predicate,
        WaitForOptions options = null)
    {
        var timeout = options?.Timeout ?? DefaultTimeout;
        var responseTcs = new TaskCompletionSource<IResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

        async void ResponseEventListener(object sender, ResponseCreatedEventArgs e)
        {
            try
            {
                if (await predicate(e.Response).ConfigureAwait(false))
                {
                    responseTcs.TrySetResult(e.Response);
                    FrameManager.NetworkManager.Response -= ResponseEventListener;
                }
            }
            catch (Exception ex)
            {
                responseTcs.TrySetException(new Exception("Predicated failed", ex));
            }
        }

        FrameManager.NetworkManager.Response += ResponseEventListener;

        await Task.WhenAny(responseTcs.Task, SessionClosedTask).WithTimeout(timeout).ConfigureAwait(false);

        if (SessionClosedTask.IsFaulted)
        {
            await SessionClosedTask.ConfigureAwait(false);
        }

        return await responseTcs.Task.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<FileChooser> WaitForFileChooserAsync(WaitForOptions options = null)
    {
        if (_fileChooserInterceptors.IsEmpty)
        {
            await PrimaryTargetClient.SendAsync(
                "Page.setInterceptFileChooserDialog",
                new PageSetInterceptFileChooserDialog { Enabled = true, }).ConfigureAwait(false);
        }

        var timeout = options?.Timeout ?? TimeoutSettings.Timeout;
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

    /// <inheritdoc/>
    public override Task SetBurstModeOffAsync()
    {
        ScreenshotBurstModeOn = false;
        if (ScreenshotBurstModeOptions != null)
        {
            return ResetBackgroundColorAndViewportAsync(ScreenshotBurstModeOptions);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
    {
        if (headers == null)
        {
            throw new ArgumentNullException(nameof(headers));
        }

        return FrameManager.NetworkManager.SetExtraHTTPHeadersAsync(headers);
    }

    /// <inheritdoc/>
    public override Task AuthenticateAsync(Credentials credentials) =>
        FrameManager.NetworkManager.AuthenticateAsync(credentials);

    /// <inheritdoc/>
    public override async Task CloseAsync(PageCloseOptions options = null)
    {
        if (Client?.Connection?.IsClosed ?? true)
        {
            _logger.LogWarning("Protocol error: Connection closed. Most likely the page has been closed.");
            return;
        }

        var runBeforeUnload = options?.RunBeforeUnload ?? false;

        if (runBeforeUnload)
        {
            await PrimaryTargetClient.SendAsync("Page.close").ConfigureAwait(false);
        }
        else
        {
            await PrimaryTargetClient.Connection
                .SendAsync("Target.closeTarget", new TargetCloseTargetRequest { TargetId = Target.TargetId, })
                .ConfigureAwait(false);

            // Puppeteer waits for Target.CloseTask. But I found some race condition where IsClose didn't get set to true.
            // So I'm waiting for the task that set IsClose to true.
            await _closedFinishedTask.ConfigureAwait(false);
        }
    }

    internal static async Task<Page> CreateAsync(
        CdpCDPSession client,
        CdpTarget target,
        bool ignoreHTTPSErrors,
        ViewPortOptions defaultViewPort,
        TaskQueue screenshotTaskQueue)
    {
        var page = new CdpPage(client, target, screenshotTaskQueue, ignoreHTTPSErrors);

        try
        {
            await page.InitializeAsync().ConfigureAwait(false);

            if (defaultViewPort != null)
            {
                await page.SetViewportAsync(defaultViewPort).ConfigureAwait(false);
            }

            return page;
        }
        catch
        {
            await page.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    /// <inheritdoc />
    protected override async Task ExposeFunctionAsync(string name, Delegate puppeteerFunction)
    {
        if (!_bindings.TryAdd(name, new Binding(name, puppeteerFunction)))
        {
            throw new PuppeteerException(
                $"Failed to add page binding with name {name}: window['{name}'] already exists!");
        }

        var expression = BindingUtils.PageBindingInitString("exposedFun", name);
        await PrimaryTargetClient.SendAsync("Runtime.addBinding", new RuntimeAddBindingRequest { Name = name })
            .ConfigureAwait(false);
        var functionInfo = await PrimaryTargetClient
            .SendAsync<PageAddScriptToEvaluateOnNewDocumentResponse>(
                "Page.addScriptToEvaluateOnNewDocument",
                new PageAddScriptToEvaluateOnNewDocumentRequest { Source = expression, }).ConfigureAwait(false);

        _exposedFunctions.TryAdd(name, functionInfo.Identifier);

        await Task.WhenAll(Frames.Select(
                frame =>
                {
                    // If a frame has not started loading, it might never start. Rely on
                    // addScriptToEvaluateOnNewDocument in that case.
                    if (frame != MainFrame && !((Frame)frame).HasStartedLoading)
                    {
                        return Task.CompletedTask;
                    }

                    return frame
                        .EvaluateExpressionAsync(expression)
                        .ContinueWith(
                            task =>
                            {
                                if (task.IsFaulted && task.Exception != null)
                                {
                                    _logger.LogError(task.Exception.ToString());
                                }
                            },
                            TaskScheduler.Default);
                }))
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<byte[]> PdfInternalAsync(string file, PdfOptions options)
    {
        var paperWidth = PaperFormat.Letter.Width;
        var paperHeight = PaperFormat.Letter.Height;

        Debug.Assert(options != null, nameof(options) + " != null");

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

        if (options.Outline)
        {
            options.Tagged = true;
        }

        if (options.OmitBackground)
        {
            await _emulationManager.SetTransparentBackgroundColorAsync().ConfigureAwait(false);
        }

        var result = await PrimaryTargetClient.SendAsync<PagePrintToPDFResponse>(
            "Page.printToPDF",
            new PagePrintToPDFRequest
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
                PreferCSSPageSize = options.PreferCSSPageSize,
                GenerateTaggedPDF = options.Tagged,
                GenerateDocumentOutline = options.Outline,
            }).ConfigureAwait(false);

        if (options.OmitBackground)
        {
            await _emulationManager.ResetDefaultBackgroundColorAsync().ConfigureAwait(false);
        }

        return await ProtocolStreamReader.ReadProtocolStreamByteAsync(Client, result.Stream, file)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<string> PerformScreenshotAsync(ScreenshotType type, ScreenshotOptions options)
    {
        var stack = new DisposableTasksStack();
        await using (stack.ConfigureAwait(false))
        {
            Debug.Assert(options != null, nameof(options) + " != null");

            var clip = options.Clip;
            var captureBeyondViewport = options.CaptureBeyondViewport;

            if (Browser.BrowserType != SupportedBrowser.Firefox &&
                options.OmitBackground &&
                (type == ScreenshotType.Png || type == ScreenshotType.Webp))
            {
                await _emulationManager.SetTransparentBackgroundColorAsync().ConfigureAwait(false);
                stack.Defer(() => _emulationManager.ResetDefaultBackgroundColorAsync());
            }

            if (clip != null && !captureBeyondViewport)
            {
                var viewport = await FrameManager.MainFrame.IsolatedRealm.EvaluateFunctionAsync<BoundingBox>(
                    @"() => {
                        const {
                            height,
                            pageLeft: x,
                            pageTop: y,
                            width,
                        } = window.visualViewport;
                        return {x, y, height, width};
                    }").ConfigureAwait(false);

                clip = GetIntersectionRect(clip, viewport);
            }

            var screenMessage = new PageCaptureScreenshotRequest
            {
                Format = type.ToString().ToLower(CultureInfo.CurrentCulture),
                CaptureBeyondViewport = captureBeyondViewport,
                FromSurface = options.FromSurface,
                OptimizeForSpeed = options.OptimizeForSpeed,
            };

            if (options.Quality.HasValue)
            {
                screenMessage.Quality = options.Quality.Value;
            }

            if (clip != null)
            {
                screenMessage.Clip = clip;
            }

            var result = await PrimaryTargetClient
                .SendAsync<PageCaptureScreenshotResponse>("Page.captureScreenshot", screenMessage)
                .ConfigureAwait(false);

            return result.Data;
        }
    }

    private void SetupPrimaryTargetListeners()
    {
        PrimaryTargetClient.Ready += OnAttachedToTarget;
        PrimaryTargetClient.MessageReceived += Client_MessageReceived;
    }

    private void OnAttachedToTarget(object sender, SessionEventArgs e)
    {
        var session = e.Session as CDPSession;
        Debug.Assert(session != null, nameof(session) + " != null");
        FrameManager.OnAttachedToTarget(new TargetChangedArgs { Target = session.Target });

        if (session.Target.Type == TargetType.Worker)
        {
            var worker = new CdpWebWorker(
                session,
                session.Target.Url,
                session.Target.TargetId,
                session.Target.TargetInfo.Type,
                AddConsoleMessageAsync,
                HandleException);
            _workers[session.Id] = worker;
            OnWorkerCreated(worker);
        }

        session.Ready += OnAttachedToTarget;
    }

    private async void Client_MessageReceived(object sender, MessageEventArgs e)
    {
        try
        {
            switch (e.MessageID)
            {
                case "Page.domContentEventFired":
                    OnDOMContentLoaded();
                    break;
                case "Page.loadEventFired":
                    OnLoad();
                    break;
                case "Runtime.consoleAPICalled":
                    await OnConsoleAPIAsync(e.MessageData.ToObject<PageConsoleResponse>(true))
                        .ConfigureAwait(false);
                    break;
                case "Page.javascriptDialogOpening":
                    OnDialog(e.MessageData.ToObject<PageJavascriptDialogOpeningResponse>(true));
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
                    await OnLogEntryAddedAsync(e.MessageData.ToObject<LogEntryAddedResponse>(true))
                        .ConfigureAwait(false);
                    break;
                case "Runtime.bindingCalled":
                    await OnBindingCalledAsync(e.MessageData.ToObject<BindingCalledResponse>(true))
                        .ConfigureAwait(false);
                    break;
                case "Page.fileChooserOpened":
                    await OnFileChooserAsync(e.MessageData.ToObject<PageFileChooserOpenedResponse>(true))
                        .ConfigureAwait(false);
                    break;
            }
        }
        catch (Exception ex)
        {
            var message = $"Page failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
            _logger.LogError(ex, message);
            PrimaryTargetClient.Close(message);
        }
    }

    private async Task OnActivationAsync(CdpCDPSession newSession)
    {
        try
        {
            PrimaryTargetClient = newSession;
            PrimaryTarget = (CdpTarget)PrimaryTargetClient.Target;
            ((CdpKeyboard)Keyboard).UpdateClient(Client);
            ((CdpMouse)Mouse).UpdateClient(Client);
            ((CdpTouchscreen)Touchscreen).UpdateClient(Client);
            Accessibility.UpdateClient(Client);
            _emulationManager.UpdateClient(Client);
            Tracing.UpdateClient(Client);
            Coverage.UpdateClient(Client);
            await FrameManager.SwapFrameTreeAsync(Client).ConfigureAwait(false);
            SetupPrimaryTargetListeners();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to activate primary target");
        }
    }

    private async Task OnSecondaryTargetAsync(CDPSession session)
    {
        if (session.Target.TargetInfo.Subtype != "prerender")
        {
            return;
        }

        try
        {
            await FrameManager.RegisterSpeculativeSessionAsync(session).ConfigureAwait(false);
            await _emulationManager.RegisterSpeculativeSessionAsync(session).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register speculative session");
        }
    }

    private void OnDetachedFromTarget(object sender, TargetChangedArgs e)
    {
        var sessionId = e.Target.Session?.Id;
        if (sessionId != null && _workers.TryRemove(sessionId, out var worker))
        {
            OnWorkerDestroyed(worker);
        }
    }

    private Task OnConsoleAPIAsync(PageConsoleResponse message)
    {
        if (message.ExecutionContextId == 0)
        {
            return Task.CompletedTask;
        }

        var ctx = FrameManager.ExecutionContextById(message.ExecutionContextId, Client);

        if (ctx == null)
        {
            _logger.LogError($"ExecutionContext not found from message.");
            return Task.CompletedTask;
        }

        var values = message.Args.Select(ctx.CreateJSHandle).ToArray();

        return AddConsoleMessageAsync(message.Type, values, message.StackTrace);
    }

    private async Task AddConsoleMessageAsync(ConsoleType type, IJSHandle[] values, StackTrace stackTrace)
    {
        if (HasConsoleEventListeners)
        {
            await Task.WhenAll(values.Select(v =>
                RemoteObjectHelper.ReleaseObjectAsync(Client, v.RemoteObject, _logger))).ConfigureAwait(false);
            return;
        }

        var tokens = values.Select(i =>
            i.RemoteObject.ObjectId != null || i.RemoteObject.Type == RemoteObjectType.Object
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
        OnConsole(new ConsoleEventArgs(consoleMessage));
    }

    private void EmitMetrics(PerformanceMetricsResponse metrics)
        => OnMetrics(new MetricEventArgs(metrics.Title, BuildMetricsObject(metrics.Metrics)));

    private void HandleException(EvaluateExceptionResponseDetails exceptionDetails)
        => OnPageError(new PageErrorEventArgs(GetExceptionMessage(exceptionDetails)));

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

    private string GetExceptionMessage(EvaluateExceptionResponseDetails exceptionDetails)
    {
        if (exceptionDetails.Exception != null)
        {
            return exceptionDetails.Exception.Description;
        }

        var message = exceptionDetails.Text;
        if (exceptionDetails.StackTrace == null)
        {
            return message;
        }

        foreach (var callFrame in exceptionDetails.StackTrace.CallFrames)
        {
            var location = $"{callFrame.Url}:{callFrame.LineNumber}:{callFrame.ColumnNumber}";
            var functionName = callFrame.FunctionName ?? "<anonymous>";
            message += $"\n at {functionName} ({location})";
        }

        return message;
    }

    private void OnTargetCrashed()
    {
        if (!HasErrorEventListeners)
        {
            throw new TargetCrashedException();
        }

        OnError(new ErrorEventArgs("Page crashed!"));
    }

    private async Task OnLogEntryAddedAsync(LogEntryAddedResponse e)
    {
        if (e.Entry.Args != null)
        {
            foreach (var arg in e.Entry.Args)
            {
                await RemoteObjectHelper.ReleaseObjectAsync(PrimaryTargetClient, arg, _logger)
                    .ConfigureAwait(false);
            }
        }

        if (e.Entry.Source != TargetType.Worker)
        {
            OnConsole(new ConsoleEventArgs(new ConsoleMessage(
                e.Entry.Level,
                e.Entry.Text,
                null,
                new ConsoleMessageLocation { URL = e.Entry.URL, LineNumber = e.Entry.LineNumber, })));
        }
    }

    private async Task OnBindingCalledAsync(BindingCalledResponse e)
    {
        if (e.BindingPayload.Type != "exposedFun" || !_bindings.ContainsKey(e.BindingPayload.Name))
        {
            return;
        }

        var context = FrameManager.GetExecutionContextById(e.ExecutionContextId, Client);

        await BindingUtils.ExecuteBindingAsync(context, e, _bindings).ConfigureAwait(false);
    }

    private async Task OnFileChooserAsync(PageFileChooserOpenedResponse e)
    {
        if (_fileChooserInterceptors.IsEmpty)
        {
            try
            {
                await PrimaryTargetClient.SendAsync(
                        "Page.handleFileChooser",
                        new PageHandleFileChooserRequest { Action = FileChooserAction.Fallback, })
                    .ConfigureAwait(false);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
            }
        }

        var frame = await FrameManager.FrameTree.GetFrameAsync(e.FrameId).ConfigureAwait(false);
        var element = await frame.MainWorld.AdoptBackendNodeAsync(e.BackendNodeId).ConfigureAwait(false);
        var fileChooser = new FileChooser(element, e);
        while (!_fileChooserInterceptors.IsEmpty)
        {
            var key = _fileChooserInterceptors.FirstOrDefault().Key;

            if (_fileChooserInterceptors.TryRemove(key, out var tcs))
            {
                tcs.TrySetResult(fileChooser);
            }
        }
    }

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

    private Clip GetIntersectionRect(Clip clip, BoundingBox viewport)
    {
        var x = Math.Max(clip.X, viewport.X);
        var y = Math.Max(clip.Y, viewport.Y);

        return new Clip()
        {
            X = x,
            Y = y,
            Width = Math.Min(clip.X + clip.Width, viewport.X + viewport.Width) - x,
            Height = Math.Min(clip.Y + clip.Height, viewport.Y + viewport.Height) - y,
        };
    }

    private async Task InitializeAsync()
    {
        await FrameManager.InitializeAsync(PrimaryTargetClient).ConfigureAwait(false);

        await Task.WhenAll(
            PrimaryTargetClient.SendAsync("Performance.enable"),
            PrimaryTargetClient.SendAsync("Log.enable")).ConfigureAwait(false);
    }

    private async Task<IResponse> GoAsync(int delta, NavigationOptions options)
    {
        var history = await PrimaryTargetClient
            .SendAsync<PageGetNavigationHistoryResponse>("Page.getNavigationHistory").ConfigureAwait(false);

        if (history.Entries.Count <= history.CurrentIndex + delta || history.CurrentIndex + delta < 0)
        {
            return null;
        }

        var entry = history.Entries[history.CurrentIndex + delta];
        var waitTask = WaitForNavigationAsync(options);

        await Task.WhenAll(
            waitTask,
            PrimaryTargetClient.SendAsync(
                "Page.navigateToHistoryEntry",
                new PageNavigateToHistoryEntryRequest { EntryId = entry.Id, })).ConfigureAwait(false);

        return waitTask.Result;
    }

    private Task ResetBackgroundColorAndViewportAsync(ScreenshotOptions options)
    {
        var omitBackgroundTask = options is { OmitBackground: true, Type: ScreenshotType.Png }
            ? _emulationManager.ResetDefaultBackgroundColorAsync()
            : Task.CompletedTask;
        var setViewPortTask = (options?.FullPage == true && Viewport != null)
            ? SetViewportAsync(Viewport)
            : Task.CompletedTask;
        return Task.WhenAll(omitBackgroundTask, setViewPortTask);
    }
}
