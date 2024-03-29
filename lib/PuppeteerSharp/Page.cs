using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Input;
using PuppeteerSharp.Media;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.PageAccessibility;
using PuppeteerSharp.PageCoverage;
using Timer = System.Timers.Timer;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    [DebuggerDisplay("Page {Url}")]
    public class Page : IPage
    {
        /// <summary>
        /// List of supported metrics.
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
            "JSHeapTotalSize",
        };

        private static readonly Dictionary<string, decimal> _unitToPixels = new()
        {
            ["px"] = 1,
            ["in"] = 96,
            ["cm"] = 37.8m,
            ["mm"] = 3.78m,
        };

        private readonly TaskQueue _screenshotTaskQueue;
        private readonly EmulationManager _emulationManager;
        private readonly ConcurrentDictionary<string, Binding> _bindings = new();
        private readonly ConcurrentDictionary<string, WebWorker> _workers = new();
        private readonly ILogger _logger;
        private readonly TimeoutSettings _timeoutSettings = new();
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<FileChooser>> _fileChooserInterceptors = new();
        private readonly ConcurrentDictionary<string, string> _exposedFunctions = new();
        private readonly ConcurrentSet<Func<IRequest, Task>> _requestInterceptionTask = [];
        private readonly ITargetManager _targetManager;
        private readonly Task _closedFinishedTask;
        private bool _screenshotBurstModeOn;
        private ScreenshotOptions _screenshotBurstModeOptions;
        private TaskCompletionSource<bool> _sessionClosedTcs;

        private Page(
            CDPSession client,
            Target target,
            TaskQueue screenshotTaskQueue,
            bool ignoreHTTPSErrors)
        {
            PrimaryTargetClient = client;
            TabTargetClient = client.ParentSession;
            TabTarget = TabTargetClient.Target;
            PrimaryTarget = target;
            _targetManager = target.TargetManager;
            Keyboard = new Keyboard(client);
            Mouse = new Mouse(client, Keyboard);
            Touchscreen = new Touchscreen(client, Keyboard);
            Tracing = new Tracing(client);
            Coverage = new Coverage(client);

            _emulationManager = new EmulationManager(client);
            _logger = Client.Connection.LoggerFactory.CreateLogger<Page>();
            FrameManager = new FrameManager(client, this, ignoreHTTPSErrors, _timeoutSettings);
            Accessibility = new Accessibility(client);

            _screenshotTaskQueue = screenshotTaskQueue;

            FrameManager.FrameAttached += (_, e) => FrameAttached?.Invoke(this, e);
            FrameManager.FrameDetached += (_, e) => FrameDetached?.Invoke(this, e);
            FrameManager.FrameNavigated += (_, e) => FrameNavigated?.Invoke(this, e);

            FrameManager.NetworkManager.Request += (_, e) => OnRequest(e.Request);
            FrameManager.NetworkManager.RequestFailed += (_, e) => RequestFailed?.Invoke(this, e);
            FrameManager.NetworkManager.Response += (_, e) => Response?.Invoke(this, e);
            FrameManager.NetworkManager.RequestFinished += (_, e) => RequestFinished?.Invoke(this, e);
            FrameManager.NetworkManager.RequestServedFromCache += (_, e) => RequestServedFromCache?.Invoke(this, e);

            TabTargetClient.Swapped += (sender, args) => _ = OnActivationAsync(args.Session as CDPSession);
            TabTargetClient.Ready += (sender, args) => _ = OnSecondaryTargetAsync(args.Session as CDPSession);
            _targetManager.TargetGone += OnDetachedFromTarget;

            _closedFinishedTask = TabTarget.CloseTask.ContinueWith(
                _ =>
                {
                    try
                    {
                        TabTarget.TargetManager.TargetGone -= OnDetachedFromTarget;
                        Close?.Invoke(this, EventArgs.Empty);
                    }
                    finally
                    {
                        IsClosed = true;
                    }
                },
                TaskScheduler.Default);

            SetupPrimaryTargetListeners();
        }

        /// <inheritdoc/>
        public event EventHandler Load;

        /// <inheritdoc/>
        public event EventHandler<ErrorEventArgs> Error;

        /// <inheritdoc/>
        public event EventHandler<MetricEventArgs> Metrics;

        /// <inheritdoc/>
        public event EventHandler<DialogEventArgs> Dialog;

        /// <inheritdoc/>
        public event EventHandler DOMContentLoaded;

        /// <inheritdoc/>
        public event EventHandler<ConsoleEventArgs> Console;

        /// <inheritdoc/>
        public event EventHandler<FrameEventArgs> FrameAttached;

        /// <inheritdoc/>
        public event EventHandler<FrameEventArgs> FrameDetached;

        /// <inheritdoc/>
        public event EventHandler<FrameEventArgs> FrameNavigated;

        /// <inheritdoc/>
        public event EventHandler<ResponseCreatedEventArgs> Response;

        /// <inheritdoc/>
        public event EventHandler<RequestEventArgs> Request;

        /// <inheritdoc/>
        public event EventHandler<RequestEventArgs> RequestFinished;

        /// <inheritdoc/>
        public event EventHandler<RequestEventArgs> RequestFailed;

        /// <inheritdoc/>
        public event EventHandler<RequestEventArgs> RequestServedFromCache;

        /// <inheritdoc/>
        public event EventHandler<PageErrorEventArgs> PageError;

        /// <inheritdoc/>
        public event EventHandler<WorkerEventArgs> WorkerCreated;

        /// <inheritdoc/>
        public event EventHandler<WorkerEventArgs> WorkerDestroyed;

        /// <inheritdoc/>
        public event EventHandler Close;

        /// <inheritdoc/>
        public event EventHandler<PopupEventArgs> Popup;

        /// <inheritdoc cref="CDPSession"/>
        public CDPSession Client => PrimaryTargetClient;

        /// <inheritdoc cref="CDPSession"/>
        public Target Target => PrimaryTarget;

        /// <inheritdoc cref="ICDPSession"/>
        ICDPSession IPage.Client => Client;

        /// <inheritdoc/>
        public int DefaultNavigationTimeout
        {
            get => _timeoutSettings.NavigationTimeout;
            set => _timeoutSettings.NavigationTimeout = value;
        }

        /// <inheritdoc/>
        public int DefaultTimeout
        {
            get => _timeoutSettings.Timeout;
            set => _timeoutSettings.Timeout = value;
        }

        /// <inheritdoc/>
        public IFrame MainFrame => FrameManager.MainFrame;

        /// <inheritdoc/>
        public IFrame[] Frames => FrameManager.GetFrames();

        /// <inheritdoc/>
        public WebWorker[] Workers => _workers.Values.ToArray();

        /// <inheritdoc/>
        public string Url => MainFrame.Url;

        /// <inheritdoc/>
        ITarget IPage.Target => Target;

        /// <inheritdoc/>
        IKeyboard IPage.Keyboard => Keyboard;

        /// <inheritdoc/>
        ITouchscreen IPage.Touchscreen => Touchscreen;

        /// <inheritdoc/>
        ICoverage IPage.Coverage => Coverage;

        /// <inheritdoc/>
        ITracing IPage.Tracing => Tracing;

        /// <inheritdoc/>
        IMouse IPage.Mouse => Mouse;

        /// <inheritdoc/>
        public ViewPortOptions Viewport { get; private set; }

        /// <inheritdoc/>
        IBrowser IPage.Browser => Browser;

        /// <inheritdoc/>
        public IBrowserContext BrowserContext => PrimaryTarget.BrowserContext;

        /// <summary>
        /// Get an indication that the page has been closed.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the accessibility.
        /// </summary>
        IAccessibility IPage.Accessibility => Accessibility;

        /// <inheritdoc/>
        public bool IsJavaScriptEnabled => _emulationManager.JavascriptEnabled;

        /// <inheritdoc/>
        public bool IsDragInterceptionEnabled { get; private set; }

        internal Accessibility Accessibility { get; }

        internal Keyboard Keyboard { get; }

        internal Touchscreen Touchscreen { get; }

        internal Coverage Coverage { get; }

        internal Tracing Tracing { get; }

        internal Mouse Mouse { get; }

        internal CDPSession PrimaryTargetClient { get; private set; }

        internal Target PrimaryTarget { get; private set; }

        internal CDPSession TabTargetClient { get; }

        internal Target TabTarget { get; }

        internal bool IsDragging { get; set; }

        internal bool HasPopupEventListeners => Popup?.GetInvocationList().Any() == true;

        private Browser Browser => PrimaryTarget.Browser;

        private FrameManager FrameManager { get; set; }

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

        /// <inheritdoc/>
        public Task SetGeolocationAsync(GeolocationOption options)
            => _emulationManager.SetGeolocationAsync(options);

        /// <inheritdoc/>
        public Task SetDragInterceptionAsync(bool enabled)
        {
            IsDragInterceptionEnabled = enabled;
            return PrimaryTargetClient.SendAsync(
                "Input.setInterceptDrags",
                new InputSetInterceptDragsRequest { Enabled = enabled });
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, decimal>> MetricsAsync()
        {
            var response = await Client.SendAsync<PerformanceGetMetricsResponse>("Performance.getMetrics")
                .ConfigureAwait(false);
            return BuildMetricsObject(response.Metrics);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public Task<IElementHandle> QuerySelectorAsync(string selector)
            => MainFrame.QuerySelectorAsync(selector);

        /// <inheritdoc/>
        public Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
            => MainFrame.QuerySelectorAllAsync(selector);

        /// <inheritdoc/>
        public Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
            => MainFrame.QuerySelectorAllHandleAsync(selector);

        /// <inheritdoc/>
#pragma warning disable CS0618 // Using obsolete
        public Task<IElementHandle[]> XPathAsync(string expression) => MainFrame.XPathAsync(expression);
#pragma warning restore CS0618

        /// <inheritdoc/>
        public Task<DeviceRequestPrompt> WaitForDevicePromptAsync(
            WaitForOptions options = default(WaitForOptions))
            => MainFrame.WaitForDevicePromptAsync(options);

        /// <inheritdoc/>
        public Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
            => MainFrame.EvaluateExpressionHandleAsync(script);

        /// <inheritdoc/>
        public Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
            => MainFrame.EvaluateFunctionHandleAsync(pageFunction, args);

        /// <inheritdoc/>
        public async Task<NewDocumentScriptEvaluation> EvaluateFunctionOnNewDocumentAsync(
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
        public Task RemoveScriptToEvaluateOnNewDocumentAsync(string identifier)
            => Client.SendAsync("Page.removeScriptToEvaluateOnNewDocument", new PageRemoveScriptToEvaluateOnNewDocumentRequest
            {
                Identifier = identifier,
            });

        /// <inheritdoc/>
        public async Task<NewDocumentScriptEvaluation> EvaluateExpressionOnNewDocumentAsync(string expression)
        {
            var documentIdentifier = await
                Client.SendAsync<PageAddScriptToEvaluateOnNewDocumentResponse>(
                    "Page.addScriptToEvaluateOnNewDocument",
                    new PageAddScriptToEvaluateOnNewDocumentRequest { Source = expression, }).ConfigureAwait(false);

            return new NewDocumentScriptEvaluation(documentIdentifier.Identifier);
        }

        /// <inheritdoc/>
        public async Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle)
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
        public Task SetRequestInterceptionAsync(bool value)
            => FrameManager.NetworkManager.SetRequestInterceptionAsync(value);

        /// <inheritdoc/>
        public Task SetOfflineModeAsync(bool value) => FrameManager.NetworkManager.SetOfflineModeAsync(value);

        /// <inheritdoc/>
        public Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions) =>
            FrameManager.NetworkManager.EmulateNetworkConditionsAsync(networkConditions);

        /// <inheritdoc/>
        public async Task<CookieParam[]> GetCookiesAsync(params string[] urls)
        {
            if (urls == null)
            {
                throw new ArgumentNullException(nameof(urls));
            }

            return (await PrimaryTargetClient.SendAsync<NetworkGetCookiesResponse>(
                    "Network.getCookies",
                    new NetworkGetCookiesRequest { Urls = urls.Length > 0 ? urls : new[] { Url }, })
                .ConfigureAwait(false)).Cookies;
        }

        /// <inheritdoc/>
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
                await PrimaryTargetClient
                    .SendAsync("Network.setCookies", new NetworkSetCookiesRequest { Cookies = cookies, })
                    .ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
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

                await PrimaryTargetClient.SendAsync("Network.deleteCookies", cookie).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public Task<IElementHandle> AddScriptTagAsync(AddTagOptions options) => MainFrame.AddScriptTagAsync(options);

        /// <inheritdoc/>
        public Task<IElementHandle> AddScriptTagAsync(string url) => AddScriptTagAsync(new AddTagOptions { Url = url });

        /// <inheritdoc/>
        public Task<IElementHandle> AddStyleTagAsync(AddTagOptions options) => MainFrame.AddStyleTagAsync(options);

        /// <inheritdoc/>
        public Task<IElementHandle> AddStyleTagAsync(string url) => AddStyleTagAsync(new AddTagOptions { Url = url });

        /// <inheritdoc/>
        public Task ExposeFunctionAsync(string name, Action puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public Task ExposeFunctionAsync<TResult>(string name, Func<TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public Task ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public Task ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public Task ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public Task ExposeFunctionAsync<T1, T2, T3, T4, TResult>(
            string name,
            Func<T1, T2, T3, T4, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public async Task RemoveExposedFunctionAsync(string name)
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
        public Task<string> GetContentAsync() => FrameManager.MainFrame.GetContentAsync();

        /// <inheritdoc/>
        public Task SetContentAsync(string html, NavigationOptions options = null) =>
            FrameManager.MainFrame.SetContentAsync(html, options);

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, NavigationOptions options) =>
            FrameManager.MainFrame.GoToAsync(url, options);

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null)
            => GoToAsync(url, new NavigationOptions { Timeout = timeout, WaitUntil = waitUntil });

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, WaitUntilNavigation waitUntil)
            => GoToAsync(url, new NavigationOptions { WaitUntil = new[] { waitUntil } });

        /// <inheritdoc/>
        public Task PdfAsync(string file) => PdfAsync(file, new PdfOptions());

        /// <inheritdoc/>
        public async Task PdfAsync(string file, PdfOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            await PdfInternalAsync(file, options).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<Stream> PdfStreamAsync() => PdfStreamAsync(new PdfOptions());

        /// <inheritdoc/>
        public async Task<Stream> PdfStreamAsync(PdfOptions options)
            => new MemoryStream(await PdfDataAsync(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<byte[]> PdfDataAsync() => PdfDataAsync(new PdfOptions());

        /// <inheritdoc/>
        public Task<byte[]> PdfDataAsync(PdfOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return PdfInternalAsync(null, options);
        }

        /// <inheritdoc/>
        public Task SetJavaScriptEnabledAsync(bool enabled)
            => _emulationManager.SetJavaScriptEnabledAsync(enabled);

        /// <inheritdoc/>
        public Task SetBypassCSPAsync(bool enabled) => PrimaryTargetClient.SendAsync(
            "Page.setBypassCSP",
            new PageSetBypassCSPRequest { Enabled = enabled, });

        /// <inheritdoc/>
        public Task EmulateMediaTypeAsync(MediaType type)
            => _emulationManager.EmulateMediaTypeAsync(type);

        /// <inheritdoc/>
        public Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features)
            => _emulationManager.EmulateMediaFeaturesAsync(features);

        /// <inheritdoc/>
        public async Task SetViewportAsync(ViewPortOptions viewport)
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

        /// <inheritdoc/>
        public Task ScreenshotAsync(string file) => ScreenshotAsync(file, new ScreenshotOptions());

        /// <inheritdoc/>
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

            using var fs = AsyncFileHelper.CreateStream(file, FileMode.Create);
            await fs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<Stream> ScreenshotStreamAsync() => ScreenshotStreamAsync(new ScreenshotOptions());

        /// <inheritdoc/>
        public async Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options)
            => new MemoryStream(await ScreenshotDataAsync(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<string> ScreenshotBase64Async() => ScreenshotBase64Async(new ScreenshotOptions());

        /// <inheritdoc/>
        public Task<string> ScreenshotBase64Async(ScreenshotOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var screenshotType = options.Type ?? ScreenshotType.Png;

            if (options.Quality.HasValue)
            {
                if (screenshotType != ScreenshotType.Jpeg)
                {
                    throw new ArgumentException($"options.Quality is unsupported for the {screenshotType} screenshots");
                }

                if (options.Quality < 0 || options.Quality > 100)
                {
                    throw new ArgumentException(
                        $"Expected options.quality to be between 0 and 100 (inclusive), got {options.Quality}");
                }
            }

            if (options.Clip?.Width == 0)
            {
                throw new PuppeteerException("Expected options.Clip.Width not to be 0.");
            }

            if (options.Clip?.Height == 0)
            {
                throw new PuppeteerException("Expected options.Clip.Height not to be 0.");
            }

            if (options.Clip != null && options.FullPage)
            {
                throw new ArgumentException("options.clip and options.fullPage are exclusive");
            }

            return _screenshotTaskQueue.Enqueue(() => PerformScreenshot(screenshotType, options));
        }

        /// <inheritdoc/>
        public Task<byte[]> ScreenshotDataAsync() => ScreenshotDataAsync(new ScreenshotOptions());

        /// <inheritdoc/>
        public async Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options)
            => Convert.FromBase64String(await ScreenshotBase64Async(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<string> GetTitleAsync() => MainFrame.GetTitleAsync();

        /// <inheritdoc/>
        public async Task CloseAsync(PageCloseOptions options = null)
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

        /// <inheritdoc/>
        public Task SetCacheEnabledAsync(bool enabled = true)
            => FrameManager.NetworkManager.SetCacheEnabledAsync(enabled);

        /// <inheritdoc/>
        public Task ClickAsync(string selector, ClickOptions options = null) =>
            FrameManager.MainFrame.ClickAsync(selector, options);

        /// <inheritdoc/>
        public Task HoverAsync(string selector) => FrameManager.MainFrame.HoverAsync(selector);

        /// <inheritdoc/>
        public Task FocusAsync(string selector) => FrameManager.MainFrame.FocusAsync(selector);

        /// <inheritdoc/>
        public Task TypeAsync(string selector, string text, TypeOptions options = null)
            => FrameManager.MainFrame.TypeAsync(selector, text, options);

        /// <inheritdoc/>
        public Task<JToken> EvaluateExpressionAsync(string script)
            => FrameManager.MainFrame.EvaluateExpressionAsync<JToken>(script);

        /// <inheritdoc/>
        public Task<T> EvaluateExpressionAsync<T>(string script)
            => FrameManager.MainFrame.EvaluateExpressionAsync<T>(script);

        /// <inheritdoc/>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
            => FrameManager.MainFrame.EvaluateFunctionAsync<JToken>(script, args);

        /// <inheritdoc/>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
            => FrameManager.MainFrame.EvaluateFunctionAsync<T>(script, args);

        /// <inheritdoc/>
        public Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentData = null)
            => FrameManager.NetworkManager.SetUserAgentAsync(userAgent, userAgentData);

        /// <inheritdoc/>
        public Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            return FrameManager.NetworkManager.SetExtraHTTPHeadersAsync(headers);
        }

        /// <inheritdoc/>
        public Task AuthenticateAsync(Credentials credentials) =>
            FrameManager.NetworkManager.AuthenticateAsync(credentials);

        /// <inheritdoc/>
        public async Task<IResponse> ReloadAsync(NavigationOptions options)
        {
            var navigationTask = WaitForNavigationAsync(options);

            await Task.WhenAll(
                    navigationTask,
                    PrimaryTargetClient.SendAsync("Page.reload", new PageReloadRequest { FrameId = MainFrame.Id }))
                .ConfigureAwait(false);

            return navigationTask.Result;
        }

        /// <inheritdoc/>
        public Task<IResponse> ReloadAsync(int? timeout = null, WaitUntilNavigation[] waitUntil = null)
            => ReloadAsync(new NavigationOptions { Timeout = timeout, WaitUntil = waitUntil });

        /// <inheritdoc/>
        public Task<string[]> SelectAsync(string selector, params string[] values)
            => MainFrame.SelectAsync(selector, values);

        /// <inheritdoc/>
        public Task WaitForTimeoutAsync(int milliseconds)
            => MainFrame.WaitForTimeoutAsync(milliseconds);

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args)
            => MainFrame.WaitForFunctionAsync(script, options ?? new WaitForFunctionOptions(), args);

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForFunctionAsync(string script, params object[] args) =>
            WaitForFunctionAsync(script, null, args);

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options = null)
            => MainFrame.WaitForExpressionAsync(script, options ?? new WaitForFunctionOptions());

        /// <inheritdoc/>
        public Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
            => MainFrame.WaitForSelectorAsync(selector, options ?? new WaitForSelectorOptions());

        /// <inheritdoc/>
#pragma warning disable CS0618 // WaitForXPathAsync is obsolete
        public Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
            => MainFrame.WaitForXPathAsync(xpath, options ?? new WaitForSelectorOptions());
#pragma warning restore CS0618

        /// <inheritdoc/>
        public Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
            => MainFrame.WaitForNavigationAsync(options);

        /// <inheritdoc/>
        public async Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions options = null)
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
        public Task<IRequest> WaitForRequestAsync(string url, WaitForOptions options = null)
            => WaitForRequestAsync(request => request.Url == url, options);

        /// <inheritdoc/>
        public async Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions options = null)
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
        public Task<IFrame> WaitForFrameAsync(string url, WaitForOptions options = null)
            => WaitForFrameAsync((frame) => frame.Url == url, options);

        /// <inheritdoc/>
        public async Task<IFrame> WaitForFrameAsync(Func<IFrame, bool> predicate, WaitForOptions options = null)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var timeout = options?.Timeout ?? DefaultTimeout;
            var frameTcs = new TaskCompletionSource<IFrame>(TaskCreationOptions.RunContinuationsAsynchronously);

            void FrameEventListener(object sender, FrameEventArgs e)
            {
                if (predicate(e.Frame))
                {
                    frameTcs.TrySetResult(e.Frame);
                    FrameManager.FrameAttached -= FrameEventListener;
                    FrameManager.FrameNavigated -= FrameEventListener;
                }
            }

            FrameManager.FrameAttached += FrameEventListener;
            FrameManager.FrameNavigated += FrameEventListener;

            var eventRace = Task.WhenAny(frameTcs.Task, SessionClosedTask).WithTimeout(timeout, t =>
            {
                FrameManager.FrameAttached -= FrameEventListener;
                FrameManager.FrameNavigated -= FrameEventListener;
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
        public Task<IResponse> WaitForResponseAsync(string url, WaitForOptions options = null)
            => WaitForResponseAsync(response => response.Url == url, options);

        /// <inheritdoc/>
        public Task<IResponse> WaitForResponseAsync(Func<IResponse, bool> predicate, WaitForOptions options = null)
            => WaitForResponseAsync((response) => Task.FromResult(predicate(response)), options);

        /// <inheritdoc/>
        public async Task<IResponse> WaitForResponseAsync(
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
        public async Task<FileChooser> WaitForFileChooserAsync(WaitForOptions options = null)
        {
            if (_fileChooserInterceptors.IsEmpty)
            {
                await PrimaryTargetClient.SendAsync(
                    "Page.setInterceptFileChooserDialog",
                    new PageSetInterceptFileChooserDialog { Enabled = true, }).ConfigureAwait(false);
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

        /// <inheritdoc/>
        public Task<IResponse> GoBackAsync(NavigationOptions options = null) => GoAsync(-1, options);

        /// <inheritdoc/>
        public Task<IResponse> GoForwardAsync(NavigationOptions options = null) => GoAsync(1, options);

        /// <inheritdoc/>
        public Task SetBurstModeOffAsync()
        {
            _screenshotBurstModeOn = false;
            if (_screenshotBurstModeOptions != null)
            {
                return ResetBackgroundColorAndViewportAsync(_screenshotBurstModeOptions);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task BringToFrontAsync() => PrimaryTargetClient.SendAsync("Page.bringToFront");

        /// <inheritdoc/>
        public Task EmulateVisionDeficiencyAsync(VisionDeficiency type)
            => _emulationManager.EmulateVisionDeficiencyAsync(type);

        /// <inheritdoc/>
        public Task EmulateTimezoneAsync(string timezoneId)
            => _emulationManager.EmulateTimezoneAsync(timezoneId);

        /// <inheritdoc/>
        public Task EmulateIdleStateAsync(EmulateIdleOverrides overrides = null)
            => _emulationManager.EmulateIdleStateAsync(overrides);

        /// <inheritdoc/>
        public Task EmulateCPUThrottlingAsync(decimal? factor = null)
            => _emulationManager.EmulateCPUThrottlingAsync(factor);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await CloseAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void AddRequestInterceptor(Func<IRequest, Task> interceptionTask)
            => _requestInterceptionTask.Add(interceptionTask);

        /// <inheritdoc />
        public void RemoveRequestInterceptor(Func<IRequest, Task> interceptionTask)
            => _requestInterceptionTask.Remove(interceptionTask);

        /// <inheritdoc />
        public Task<ICDPSession> CreateCDPSessionAsync() => Target.CreateCDPSessionAsync();

        internal static async Task<Page> CreateAsync(
            CDPSession client,
            Target target,
            bool ignoreHTTPSErrors,
            ViewPortOptions defaultViewPort,
            TaskQueue screenshotTaskQueue)
        {
            var page = new Page(client, target, screenshotTaskQueue, ignoreHTTPSErrors);

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

        internal void OnPopup(IPage popupPage) => Popup?.Invoke(this, new PopupEventArgs { PopupPage = popupPage });

        /// <summary>
        /// Dispose resources.
        /// </summary>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing)
        {
            Mouse.Dispose();
            _ = DisposeAsync();
        }

        private async Task<byte[]> PdfInternalAsync(string file, PdfOptions options)
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

        private async Task InitializeAsync()
        {
            await FrameManager.InitializeAsync(PrimaryTargetClient).ConfigureAwait(false);

            await Task.WhenAll(
                PrimaryTargetClient.SendAsync("Performance.enable"),
                PrimaryTargetClient.SendAsync("Log.enable")).ConfigureAwait(false);
        }

        private void OnRequest(IRequest request)
        {
            if (request == null)
            {
                return;
            }

            // Run tasks one after the other
            foreach (var subscriber in _requestInterceptionTask)
            {
                (request as CdpHttpRequest)?.EnqueueInterceptionAction(subscriber);
            }

            Request?.Invoke(this, new RequestEventArgs(request));
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
            var stack = new DisposableTasksStack();
            await using (stack.ConfigureAwait(false))
            {
                if (!_screenshotBurstModeOn)
                {
                    await BringToFrontAsync().ConfigureAwait(false);
                }

                // FromSurface is not supported on Firefox.
                // It seems that Puppeteer solved this just by ignoring screenshot tests in firefox.
                if (Browser.BrowserType == SupportedBrowser.Firefox)
                {
                    if (options.FromSurface != null)
                    {
                        throw new ArgumentException(
                            "Screenshots from surface are not supported on Firefox.",
                            nameof(options.FromSurface));
                    }
                }
                else
                {
                    options.FromSurface ??= true;
                }

                if (options.Clip != null && options.FullPage)
                {
                    throw new ArgumentException("Clip and FullPage are exclusive");
                }

                var clip = options.Clip != null ? RoundRectangle(NormalizeRectangle(options.Clip)) : null;
                var captureBeyondViewport = options.CaptureBeyondViewport;

                if (!_screenshotBurstModeOn)
                {
                    if (options.Clip == null)
                    {
                        if (options.FullPage)
                        {
                            // Overwrite clip for full page at all times.
                            clip = null;

                            if (!captureBeyondViewport)
                            {
                                var scrollDimensions = await FrameManager.MainFrame.IsolatedRealm
                                    .EvaluateFunctionAsync<BoundingBox>(@"() => {
                                        const element = document.documentElement;
                                        return {
                                            width: element.scrollWidth,
                                            height: element.scrollHeight,
                                        };
                                    }").ConfigureAwait(false);

                                var viewport = Viewport with { };

                                await SetViewportAsync(viewport with
                                {
                                    Width = Convert.ToInt32(scrollDimensions.Width),
                                    Height = Convert.ToInt32(scrollDimensions.Height),
                                }).ConfigureAwait(false);

                                stack.Defer(() => SetViewportAsync(viewport));
                            }
                        }
                        else
                        {
                            captureBeyondViewport = false;
                        }
                    }

                    if (Browser.BrowserType != SupportedBrowser.Firefox &&
                        options.OmitBackground &&
                        (type == ScreenshotType.Png || type == ScreenshotType.Webp))
                    {
                        await _emulationManager.SetTransparentBackgroundColorAsync().ConfigureAwait(false);
                        stack.Defer(() => _emulationManager.ResetDefaultBackgroundColorAsync());
                    }
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

                if (options.BurstMode)
                {
                    _screenshotBurstModeOptions = options;
                    _screenshotBurstModeOn = true;
                }

                return result.Data;
            }
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

        private Clip RoundRectangle(Clip clip)
        {
            var x = Math.Round(clip.X);
            var y = Math.Round(clip.Y);

            return new Clip
            {
                X = x,
                Y = y,
                Width = Math.Round(clip.Width + clip.X - x, MidpointRounding.AwayFromZero),
                Height = Math.Round(clip.Height + clip.Y - y, MidpointRounding.AwayFromZero),
                Scale = clip.Scale,
            };
        }

        private Clip NormalizeRectangle(Clip clip)
        {
            return new Clip()
            {
                Scale = clip.Scale,
                X = clip.Width < 0 ? clip.X + clip.Width : clip.X,
                Y = clip.Height < 0 ? clip.Y + clip.Height : clip.Y,
                Width = Math.Abs(clip.Width),
                Height = Math.Abs(clip.Height),
            };
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

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
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

        private async Task OnBindingCalledAsync(BindingCalledResponse e)
        {
            if (e.BindingPayload.Type != "exposedFun" || !_bindings.ContainsKey(e.BindingPayload.Name))
            {
                return;
            }

            var context = FrameManager.GetExecutionContextById(e.ExecutionContextId, Client);

            await BindingUtils.ExecuteBindingAsync(context, e, _bindings).ConfigureAwait(false);
        }

        private void OnDetachedFromTarget(object sender, TargetChangedArgs e)
        {
            var sessionId = e.Target.Session?.Id;
            if (sessionId != null && _workers.TryRemove(sessionId, out var worker))
            {
                WorkerDestroyed?.Invoke(this, new WorkerEventArgs(worker));
            }
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
                    session.TargetType,
                    AddConsoleMessageAsync,
                    HandleException);
                _workers[session.Id] = worker;
                WorkerCreated?.Invoke(this, new WorkerEventArgs(worker));
            }

            session.Ready += OnAttachedToTarget;
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
                Console?.Invoke(this, new ConsoleEventArgs(new ConsoleMessage(
                    e.Entry.Level,
                    e.Entry.Text,
                    null,
                    new ConsoleMessageLocation { URL = e.Entry.URL, LineNumber = e.Entry.LineNumber, })));
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

        private void OnDialog(PageJavascriptDialogOpeningResponse message)
        {
            var dialog = new CdpDialog(Client, message.Type, message.Message, message.DefaultPrompt);
            Dialog?.Invoke(this, new DialogEventArgs(dialog));
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

        private async Task AddConsoleMessageAsync(ConsoleType type, IJSHandle[] values, Messaging.StackTrace stackTrace)
        {
            if (Console?.GetInvocationList().Length == 0)
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
            Console?.Invoke(this, new ConsoleEventArgs(consoleMessage));
        }

        private async Task ExposeFunctionAsync(string name, Delegate puppeteerFunction)
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

        private void SetupPrimaryTargetListeners()
        {
            PrimaryTargetClient.Ready += OnAttachedToTarget;
            PrimaryTargetClient.MessageReceived += Client_MessageReceived;
        }

        private async Task OnActivationAsync(CDPSession newSession)
        {
            try
            {
                PrimaryTargetClient = newSession;
                PrimaryTarget = PrimaryTargetClient.Target;
                Keyboard.UpdateClient(Client);
                Mouse.UpdateClient(Client);
                Touchscreen.UpdateClient(Client);
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
    }
}
